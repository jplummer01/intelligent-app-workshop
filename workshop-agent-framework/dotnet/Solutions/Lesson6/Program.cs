using Core.Utilities.Config;
using Core.Utilities.Plugins;
using Core.Utilities.Services;
using Core.Utilities.Extensions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// Initialize the chat client with Agent Framework  
IChatClient chatClient = AgentFrameworkProvider.CreateChatClientWithApiKey();

// Initialize plugins
TimeInformationPlugin timePlugin = new();
HttpClient httpClient = new();
StockDataPlugin stockDataPlugin = new(new StocksService(httpClient));

// Create web search tool for enhanced sentiment analysis
HostedWebSearchTool webSearchTool = new();

// Create AI Functions from plugins
var timeTool = AIFunctionFactory.Create(timePlugin.GetCurrentUtcTime);
var stockPriceTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPrice);
var stockPriceDateTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPriceForDate);

// Stock Sentiment Agent system instructions - defines the agent's behavior and rules
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

// Create the Financial Analysis Agent using ChatClientAgent
ChatClientAgent financialAnalysisAgent = new(
    chatClient,
    instructions: stockSentimentAgentInstructions,
    name: "FinancialAnalysisAgent",
    description: "An intelligent agent that provides comprehensive financial analysis using web search and market data",
    tools: [
        timeTool,
        stockPriceTool, 
        stockPriceDateTool,
        webSearchTool
    ]
);

// Create a thread for conversation
AgentThread thread = financialAnalysisAgent.GetNewThread();

// Execute program
const string terminationPhrase = "quit";
string? userInput;

Console.WriteLine("=== Financial Analysis Agent with Microsoft Agent Framework ===");
Console.WriteLine("This agent provides comprehensive financial analysis using web search and market data.");
Console.WriteLine("Ask any financial question - about specific stocks, market trends, sectors, or investment strategies.");
Console.WriteLine("Examples: 'What do you think about Microsoft?', 'How is the tech sector performing?', 'Should I invest in renewable energy stocks?'");
Console.WriteLine("Type 'quit' to exit.");
Console.WriteLine("==================================================================================");
Console.WriteLine();

do
{
    Console.Write("User > ");
    userInput = Console.ReadLine();

    if (userInput is not null and not terminationPhrase)
    {
        Console.Write("Assistant > ");
        
        try
        {
            // Use the agent to process the user message with web search capabilities
            var response = await financialAnalysisAgent.RunAsync(userInput, thread);
            
            // Extract and display the response content
            if (response?.Messages?.Any() == true)
            {
                var lastMessage = response.Messages.Last();
                Console.WriteLine(lastMessage.Text ?? "No response generated.");
            }
            else
            {
                Console.WriteLine("No response generated from the agent.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing request: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
        }
        
        Console.WriteLine();
    }
}
while (userInput != terminationPhrase);

Console.WriteLine("Thank you for using the Financial Analysis Agent!");


