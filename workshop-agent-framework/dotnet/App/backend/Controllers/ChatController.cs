using Core.Utilities.Models;
using Core.Utilities.Config;
using Core.Utilities.Extensions;
using Core.Utilities.Plugins;
using Core.Utilities.Services;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.AspNetCore.Mvc;
using Azure.Identity;
using Azure.AI.Agents.Persistent;
using Azure.AI.OpenAI;

namespace Controllers;

[ApiController]
[Route("chat")]
public class ChatController : ControllerBase 
{
    private readonly AIAgent _financialAnalysisAgent;
    private readonly AgentThread _thread;
    private readonly ChatClientAgentRunOptions _agentOptions;
    
    public ChatController()
    {
        var applicationSettings = AISettingsProvider.GetSettings();
        
        // Set Azure AI and Authentication environment variables (required for Azure AI Foundry agent)
        Environment.SetEnvironmentVariable("AZURE_FOUNDRY_PROJECT_ENDPOINT", applicationSettings.AIFoundryProject.Endpoint);
        Environment.SetEnvironmentVariable("AZURE_FOUNDRY_PROJECT_DEPLOYMENT_NAME", applicationSettings.AIFoundryProject.DeploymentName);
        Environment.SetEnvironmentVariable("BING_CONNECTION_ID", applicationSettings.AIFoundryProject.GroundingWithBingConnectionId);

        // Create PersistentAgentsClient for Azure AI Foundry
        var persistentAgentsClient = new PersistentAgentsClient(
            applicationSettings.AIFoundryProject.ConnectionString,
            new DefaultAzureCredential());

        // Initialize plugins  
        TimeInformationPlugin timePlugin = new();
        HttpClient httpClient = new();
        StockDataPlugin stockDataPlugin = new(new StocksService(httpClient));
        HostedWebSearchTool webSearchTool = new();

        // Create AI Functions from plugins
        var timeTool = AIFunctionFactory.Create(timePlugin.GetCurrentUtcTime);
        var stockPriceTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPrice);
        var stockPriceDateTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPriceForDate);

        // Stock Sentiment Agent system instructions
        string stockSentimentAgentInstructions = """
            You are a Financial Analysis Agent with web search capabilities. Provide direct, comprehensive financial analysis and insights based on user questions.

            CAPABILITIES:
            - Analyze individual stocks, market sectors, or broader financial topics
            - Extract stock symbols from user queries when relevant (e.g., "What do you think about Microsoft?" -> analyze MSFT)
            - Handle free-form questions about market trends, economic conditions, investment strategies
            - Use stock sentiment scale from 1 to 10 where sentiment is 1 for sell and 10 for buy (when analyzing specific stocks)
            - Provide ratings, recommendations (buy/hold/sell), and detailed reasoning for stock-specific queries

            CRITICAL RULES:
            - Provide your complete analysis in a SINGLE response - do not say you're "gathering data" or "working on it"
            - For stock-specific questions: Use web search to gather current market news, analyst opinions, and sentiment data
            - For general financial questions: Use web search to find relevant financial news, economic data, and expert analysis
            - Combine web search results with available stock price data when analyzing specific companies
            - ALWAYS include a dedicated "Sources" section at the end of your response listing all the specific sources you found through web search
            - For each source, include the title, URL (if available), and a brief description of what information it provided
            - Focus on recent news, market trends, and expert analysis
            - Be transparent about which information came from which sources
            - If a user asks about a specific company without mentioning the stock symbol, try to identify the relevant ticker symbol
            - Answer immediately with your full analysis - do not provide status updates or say you're collecting information
            """;

        // Create Financial Analysis Agent in Azure AI Foundry with Bing grounding tool
        var agentTask = persistentAgentsClient.CreateAIAgentAsync(
            applicationSettings.AIFoundryProject.DeploymentName,
            instructions: stockSentimentAgentInstructions,
            tools: [ 
                new BingGroundingToolDefinition(
                    new BingGroundingSearchToolParameters(
                        new[] { 
                            new BingGroundingSearchConfiguration(
                                applicationSettings.AIFoundryProject.GroundingWithBingConnectionId
                            ) 
                        }
                    )
                ) 
            ]
        );
        _financialAnalysisAgent = agentTask.GetAwaiter().GetResult();

        // Create a thread for conversation
        _thread = _financialAnalysisAgent.GetNewThread();

        // Create run options with local function tools
        _agentOptions = new ChatClientAgentRunOptions(new() { 
            Tools = [
                timeTool,
                stockPriceTool,
                stockPriceDateTool,
                webSearchTool
            ] 
        });
    }

    [HttpPost("/chat")]
    public async Task<Core.Utilities.Models.ChatResponse> ReplyAsync([FromBody]ChatRequest request)
    {
        var responseChatHistory = new List<Core.Utilities.Models.ChatMessage>();
        
        // Convert existing history to response format
        foreach (var msg in request.MessageHistory)
        {
            responseChatHistory.Add(msg);
        }

        string fullMessage = "";
        
        if (!string.IsNullOrEmpty(request.InputMessage))
        {
            // Add user message to response history
            responseChatHistory.Add(new Core.Utilities.Models.ChatMessage(request.InputMessage, Core.Utilities.Models.Role.User));
            
            try
            {
                // Use the Azure AI Foundry agent with Bing grounding and local tools
                var response = await _financialAnalysisAgent.RunAsync(request.InputMessage, _thread, _agentOptions);
                fullMessage = response?.ToString() ?? "";
                
                // Add assistant response to history
                responseChatHistory.Add(new Core.Utilities.Models.ChatMessage(fullMessage, Core.Utilities.Models.Role.Assistant));
            }
            catch (Exception ex)
            {
                fullMessage = $"Error processing request: {ex.Message}";
                if (ex.InnerException != null)
                {
                    fullMessage += $"\nInner exception: {ex.InnerException.Message}";
                }
                responseChatHistory.Add(new Core.Utilities.Models.ChatMessage(fullMessage, Core.Utilities.Models.Role.Assistant));
            }
        }
        
        var chatResponse = new Core.Utilities.Models.ChatResponse(fullMessage, responseChatHistory);    
        return chatResponse;
    }
}