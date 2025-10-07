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

// Initialize web search tool for sentiment analysis
HostedWebSearchTool webSearchTool = new();

// Create AI Functions from plugins
var timeTool = AIFunctionFactory.Create(timePlugin.GetCurrentUtcTime);
var stockPriceTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPrice);
var stockPriceDateTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPriceForDate);

// Stock Sentiment Agent system instructions - defines the agent's behavior and rules
string stockSentimentAgentInstructions = """
    You are a Stock Sentiment Agent. Your responsibility is to find the stock sentiment for a given Stock.

    RULES:
    - Use stock sentiment scale from 1 to 10 where stock sentiment is 1 for sell and 10 for buy.
    - Provide the rating in your response and a recommendation to buy, hold or sell.
    - Include the reasoning behind your recommendation.
    - Include the source of the sentiment in your response.
    - Focus on technical analysis based on stock price data and general market knowledge.
    """;

// TODO: Step 1 - Enhance the system instructions for comprehensive financial analysis

// Create the Stock Sentiment Agent using ChatClientAgent
ChatClientAgent stockSentimentAgent = new(
    chatClient,
    instructions: stockSentimentAgentInstructions,
    name: "StockSentimentAgent",
    description: "An intelligent agent that analyzes stock sentiment using market data",
    tools: [
        timeTool,
        stockPriceTool, 
        stockPriceDateTool,
        webSearchTool
    ]
);

// TODO: Step 2 - Update agent name and description to reflect enhanced capabilities

// Create a thread for conversation
AgentThread thread = stockSentimentAgent.GetNewThread();

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
        
        // Use agent with automatic function calling
        var response = await stockSentimentAgent.RunAsync(userInput, thread);
        Console.WriteLine(response);
    }
}
while (userInput != terminationPhrase);