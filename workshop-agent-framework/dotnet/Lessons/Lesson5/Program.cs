using Core.Utilities.Config;
using Core.Utilities.Plugins;
using Core.Utilities.Services;
using Core.Utilities.Extensions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// TODO: Step 1 - Initialize the chat client with Agent Framework  


// TODO: Step 2 - Initialize plugins including web search
// Hint: Add HostedWebSearchTool for enhanced analysis capabilities
// TimeInformationPlugin timePlugin = new();
// HttpClient httpClient = new();
// StockDataPlugin stockDataPlugin = new(new StocksService(httpClient));
// HostedWebSearchTool webSearchTool = new();


// TODO: Step 3 - Create AI Functions from plugins including web search
// Hint: Convert all plugin methods including the web search tool


// TODO: Step 4 - Create Financial Analysis Agent with comprehensive instructions
// Create system instructions that define an agent capable of:
// - Analyzing stocks, sectors, and financial topics
// - Using web search for current market news and sentiment
// - Extracting stock symbols from natural language queries
// - Using 1-10 sentiment scale for stock analysis
// - Providing sources for all analysis


// TODO: Step 5 - Create the Financial Analysis Agent using ChatClientAgent
// Hint: Include all tools (time, stock data, and web search)


// TODO: Step 6 - Create a thread for conversation


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
    
    // Handle null input (e.g., from piped input or EOF)
    if (userInput == null)
    {
        Console.WriteLine("Input ended. Exiting...");
        break;
    }

    if (userInput is not terminationPhrase)
    {
        Console.Write("Assistant > ");
        
        try
        {
            // TODO: Step 7 - Use the agent to process complex financial queries
            // Hint: The agent will automatically use web search and stock data as needed
            // var response = await financialAnalysisAgent.RunAsync(userInput, thread);
            // Console.WriteLine(response);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
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