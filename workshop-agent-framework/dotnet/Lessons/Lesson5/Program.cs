using Core.Utilities.Config;
using Core.Utilities.Plugins;
using Core.Utilities.Services;
using Core.Utilities.Extensions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
// TODO: Step 1 - Add the Workflows namespace for sequential orchestration

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

// Financial Analysis Agent system instructions with comprehensive capabilities
string financialAnalysisInstructions = """
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
    instructions: financialAnalysisInstructions,
    name: "FinancialAnalysisAgent",
    description: "An intelligent agent that provides comprehensive financial analysis using web search and market data",
    tools: [
        timeTool,
        stockPriceTool, 
        stockPriceDateTool,
        webSearchTool
    ]
);

// TODO: Step 2 - Create three specialized agents for portfolio analysis

// TODO: Step 3 - Create Risk Assessment Agent  

// TODO: Step 4 - Create Investment Advisor Agent

// Create a thread for conversation
AgentThread thread = financialAnalysisAgent.GetNewThread();

// Execute program
const string terminationPhrase = "quit";
string? userInput;

do
{
    Console.Write("User > ");
    userInput = Console.ReadLine();
    
    // Handle null input (e.g., from piped input or EOF)
    if (userInput == null)
    {
        Console.WriteLine("Input ended. Exiting...");
        break;
    }

    if (userInput is not terminationPhrase)
    {
        Console.Write("Assistant > ");
        
        // Use single agent for comprehensive financial analysis
        var response = await financialAnalysisAgent.RunAsync(userInput, thread);
        Console.WriteLine(response);
        
        // TODO: Step 5 - Replace single agent with sequential workflow
    }
}
while (userInput != terminationPhrase);