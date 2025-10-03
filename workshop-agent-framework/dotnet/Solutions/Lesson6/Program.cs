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
    You are a Stock Sentiment Agent with web search capabilities. Your responsibility is to find comprehensive stock sentiment for a given stock.

    RULES:
    - Use stock sentiment scale from 1 to 10 where stock sentiment is 1 for sell and 10 for buy.
    - Provide the rating in your response and a recommendation to buy, hold or sell.
    - Include the reasoning behind your recommendation.
    - Use web search to gather current market news, analyst opinions, and sentiment data about the stock.
    - Combine web search results with current stock price data for comprehensive analysis.
    - ALWAYS include a dedicated "Sources" section at the end of your response listing all the specific sources you found through web search.
    - For each source, include the title, URL (if available), and a brief description of what information it provided.
    - Focus on recent news, market trends, and analyst sentiment.
    - Be transparent about which information came from which sources.
    """;

// Create the Stock Sentiment Agent using ChatClientAgent
ChatClientAgent stockSentimentAgent = new(
    chatClient,
    instructions: stockSentimentAgentInstructions,
    name: "StockSentimentAgent",
    description: "An intelligent agent that analyzes stock sentiment using web search and market data",
    tools: [
        timeTool,
        stockPriceTool, 
        stockPriceDateTool,
        webSearchTool
    ]
);

// Create a thread for conversation
AgentThread thread = stockSentimentAgent.GetNewThread();

// Execute program
const string terminationPhrase = "quit";
string? userInput;

Console.WriteLine("=== Stock Sentiment Agent with Microsoft Agent Framework ===");
Console.WriteLine("This agent combines stock data with web search for comprehensive sentiment analysis.");
Console.WriteLine("Enter a stock symbol (e.g., 'MSFT', 'AAPL') or ask questions about stocks.");
Console.WriteLine("Type 'quit' to exit.");
Console.WriteLine("===============================================================");
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
            var response = await stockSentimentAgent.RunAsync(userInput, thread);
            
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

Console.WriteLine("Thank you for using the Stock Sentiment Agent!");


