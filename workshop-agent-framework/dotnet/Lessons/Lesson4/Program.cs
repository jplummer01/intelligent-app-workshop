using Core.Utilities.Config;
using Core.Utilities.Plugins;
using Core.Utilities.Services;
using Core.Utilities.Extensions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// TODO: Step 1 - Initialize the chat client with Agent Framework  


// TODO: Step 2 - Initialize plugins
// Hint: Same as previous lessons - TimeInformationPlugin, StockDataPlugin


// TODO: Step 3 - Create AI Functions from plugins
// Hint: Use AIFunctionFactory.Create() for each plugin method


// TODO: Step 4 - Create Stock Sentiment Agent with specialized instructions
// Define system instructions for a stock sentiment agent that:
// - Uses stock sentiment scale from 1 to 10 (1=sell, 10=buy)
// - Provides rating, recommendation (buy/hold/sell), and reasoning
// - Includes sources in responses
// - Focuses on technical analysis


// TODO: Step 5 - Create the Stock Sentiment Agent using ChatClientAgent
// Hint: Use the specialized instructions and include the tools


// TODO: Step 6 - Create a thread for conversation


// Execute program
const string terminationPhrase = "quit";
string? userInput;

Console.WriteLine("=== Stock Sentiment Agent with Microsoft Agent Framework ===");
Console.WriteLine("This agent analyzes stock sentiment using current market data and technical analysis.");
Console.WriteLine("Enter a stock symbol (e.g., 'MSFT', 'AAPL') or ask questions about stocks.");
Console.WriteLine("Type 'quit' to exit.");
Console.WriteLine("===============================================================");
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
            // TODO: Step 7 - Use the agent to process the user message
            // Hint: Use agent.RunAsync() with error handling
            // var response = await stockSentimentAgent.RunAsync(userInput, thread);
            // Console.WriteLine(response);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        
        Console.WriteLine();
    }
}
while (userInput != terminationPhrase);

Console.WriteLine("Thank you for using the Stock Sentiment Agent!");