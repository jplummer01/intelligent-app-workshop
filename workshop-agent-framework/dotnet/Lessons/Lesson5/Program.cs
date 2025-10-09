using Core.Utilities.Config;
using Core.Utilities.Plugins;
using Core.Utilities.Services;
using Core.Utilities.Extensions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// TODO: Step 1 - Add additional imports for workflows and streaming

// TODO: Step 2 - Initialize the chat client with Agent Framework  
IChatClient chatClient = AgentFrameworkProvider.CreateChatClientWithApiKey();

// Initialize plugins
TimeInformationPlugin timePlugin = new();
HttpClient httpClient = new();
StockDataPlugin stockDataPlugin = new(new StocksService(httpClient));
HostedWebSearchTool webSearchTool = new();

// Create AI Functions from plugins
var timeTool = AIFunctionFactory.Create(timePlugin.GetCurrentUtcTime);
var stockPriceTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPrice);
var stockPriceDateTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPriceForDate);

// TODO: Step 3 - Create Portfolio Research Agent

// TODO: Step 4 - Create Risk Assessment Agent

// TODO: Step 5 - Create Investment Advisor Agent

// TODO: Step 6 - Update the User Interface and Orchestration

// Current single agent approach - to be replaced
Console.WriteLine("=== Financial Analysis Agent with Microsoft Agent Framework ===");
Console.WriteLine("This agent provides comprehensive financial analysis using web search and market data.");
Console.WriteLine("Ask any financial question - about specific stocks, market trends, sectors, or investment strategies.");
Console.WriteLine("Examples: 'What do you think about Microsoft?', 'How is the tech sector performing?', 'Should I invest in renewable energy stocks?'");
Console.WriteLine("Type 'quit' to exit.");
Console.WriteLine("==================================================================================");
Console.WriteLine();

const string terminationPhrase = "quit";
string? userInput;

do
{
    Console.Write("User > ");
    userInput = Console.ReadLine();

    if (userInput is not null and not terminationPhrase)
    {
        Console.Write("Assistant > ");
        
        try
        {
            // TODO: Replace this with multi-agent workflow
            Console.WriteLine("Please implement the multi-agent workflow as described in the lesson instructions.");
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
