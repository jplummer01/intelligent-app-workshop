using Core.Utilities.Config;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;

// TODO: Step 1 - Initialize the chat client using Microsoft Agent Framework
// Hint: Use AgentFrameworkProvider.CreateChatClientWithApiKey()


// TODO: Step 2 - Configure system message using Agent Framework pattern
// Hint: Create a string with instructions for a friendly financial advisor


// TODO: Step 3 - Create a ChatClientAgent with the chat client and system instructions
// Hint: new ChatClientAgent(chatClient, instructions: systemInstructions, name: "FinancialAdvisor", description: "...")


// TODO: Step 4 - Create a thread for conversation
// Hint: Use agent.GetNewThread()


// Execute program.
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
        
        // TODO: Step 5 - Use agent to respond to user input
        // Hint: Use await agent.RunAsync(userInput, thread) and output the response

    }
}
while (userInput != terminationPhrase);

Console.WriteLine("Thank you for using the Financial Advisor!");