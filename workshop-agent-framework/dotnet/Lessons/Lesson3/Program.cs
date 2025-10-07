using Core.Utilities.Config;
using Core.Utilities.Plugins;
using Core.Utilities.Services;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;

// TODO: Step 1 - Initialize the chat client with Agent Framework  


// TODO: Step 2 - Initialize plugins for function calling
// Hint: Create instances of TimeInformationPlugin, HttpClient, and StockDataPlugin
// Example:
// TimeInformationPlugin timePlugin = new();
// HttpClient httpClient = new();
// StockDataPlugin stockDataPlugin = new(new StocksService(httpClient));


// TODO: Step 3 - Create tools array for the agent
// Hint: Use AIFunctionFactory.Create() to convert plugin methods to AIFunction objects
// You'll need:
// - timePlugin.GetCurrentUtcTime
// - stockDataPlugin.GetStockPrice  
// - stockDataPlugin.GetStockPriceForDate


// TODO: Step 4 - Create financial advisor agent with function calling capabilities
// Hint: Same as previous lessons, but add the tools parameter:
// ChatClientAgent agent = new(chatClient, instructions: systemInstructions, name: "...", description: "...", tools: tools);


// TODO: Step 5 - Create a thread for conversation


// Execute program with function calling demonstration
const string terminationPhrase = "quit";
string? userInput;

Console.WriteLine("=== Financial Advisor with Function Calling ===");
Console.WriteLine("This lesson demonstrates how agents can call functions to get real-time data.");
Console.WriteLine("Try asking about:");
Console.WriteLine("  - Current time: 'What time is it?'");
Console.WriteLine("  - Stock prices: 'What's the current price of MSFT?'");
Console.WriteLine("  - Historical data: 'What was AAPL's price on 2023-01-15?'");
Console.WriteLine("Type 'quit' to exit.");
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
        
        // TODO: Step 6 - Use agent with automatic function calling
        // Hint: Same as previous lessons - agent.RunAsync(userInput, thread)
        // The agent will automatically call functions when needed!
        // var response = await agent.RunAsync(userInput, thread);
        // Console.WriteLine(response);

    }
}
while (userInput != terminationPhrase);

Console.WriteLine("Notice how the agent automatically called functions to get real-time data!");