using Core.Utilities.Models;
using Core.Utilities.Config;
using Core.Utilities.Extensions;
using Core.Utilities.Plugins;
using Core.Utilities.Services;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.AspNetCore.Mvc;

namespace Controllers;

[ApiController]
[Route("chat")]
public class ChatController : ControllerBase 
{
    private readonly AIAgent _workflowAgent;
    private readonly IChatClient _chatClient;
    
    public ChatController()
    {
        // Initialize the chat client with Agent Framework
        // Uses managed identity when deployed to Azure Container Apps, API key for local development
        _chatClient = AgentFrameworkProvider.CreateChatClient();

        // Initialize plugins  
        TimeInformationPlugin timePlugin = new();
        HttpClient httpClient = new();
        StockDataPlugin stockDataPlugin = new(new StocksService(httpClient));
        HostedWebSearchTool webSearchTool = new();

        // Create AI Functions from plugins
        var timeTool = AIFunctionFactory.Create(timePlugin.GetCurrentUtcTime);
        var stockPriceTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPrice);
        var stockPriceDateTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPriceForDate);

        // Agent 1: Portfolio Research Agent - Gathers data on all stocks
        string researchAgentInstructions = """
            You are a Portfolio Research Agent. Your job is to gather comprehensive market data for stocks.
            
            For each stock symbol provided:
            - Get the current stock price
            - Search the web for recent news and market sentiment
            - Provide a brief summary of each stock's current situation
            
            Provide your complete research in a SINGLE response with clear sections for each stock.
            Format your response as a research report with stock symbols as headers.
            """;

        AIAgent researchAgent = new ChatClientAgent(
            _chatClient,
            instructions: researchAgentInstructions,
            name: "PortfolioResearchAgent",
            description: "Gathers market data and news for portfolio stocks",
            tools: [stockPriceTool, webSearchTool, timeTool, stockPriceDateTool]
        );

        // Agent 2: Risk Assessment Agent - Analyzes portfolio risk
        string riskAgentInstructions = """
            You are a Risk Assessment Agent. Analyze the portfolio composition and risk profile.
            
            Based on the research provided:
            - Identify sector concentration (tech-heavy, diversified, etc.)
            - Assess portfolio balance and diversification
            - Calculate a risk score from 1-10 (1=very safe, 10=very risky)
            - Highlight any concerns about over-concentration
            
            Provide your complete analysis in a SINGLE response.
            Be concise and actionable.
            """;

        AIAgent riskAgent = new ChatClientAgent(
            _chatClient,
            instructions: riskAgentInstructions,
            name: "RiskAssessmentAgent",
            description: "Analyzes portfolio risk and diversification"
        );

        // Agent 3: Investment Advisor Agent - Provides recommendations
        string advisorAgentInstructions = """
            You are an Investment Advisor Agent. Synthesize research and risk analysis into actionable recommendations.
            
            Based on the research and risk assessment:
            - Provide an overall portfolio health score (1-10)
            - Give specific buy/hold/sell recommendations for each stock
            - Suggest rebalancing actions if needed
            - Provide 2-3 key takeaways
            
            Provide your complete recommendations in a SINGLE response.
            Be clear, concise, and actionable.
            """;

        AIAgent advisorAgent = new ChatClientAgent(
            _chatClient,
            instructions: advisorAgentInstructions,
            name: "InvestmentAdvisorAgent",
            description: "Provides investment recommendations based on research and risk analysis"
        );

        // Build the sequential workflow as an agent
        var workflowTask = AgentWorkflowBuilder.BuildSequential([
            researchAgent,
            riskAgent,
            advisorAgent
        ]).AsAgentAsync();
        
        _workflowAgent = workflowTask.GetAwaiter().GetResult();
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
                // Run the sequential workflow (Research → Risk → Advisor)
                var response = await _workflowAgent.RunAsync(request.InputMessage);
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