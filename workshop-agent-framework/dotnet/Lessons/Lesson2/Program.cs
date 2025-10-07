using Core.Utilities.Config;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;

// TODO: Step 1 - Initialize the chat client using Microsoft Agent Framework
// (This should be the same as Lesson 1)


// TODO: Step 2 - Create financial advisor agent that maintains conversation context
// Hint: Use the same ChatClientAgent setup as Lesson 1, but note that Agent Framework
//       automatically maintains conversation history through the AgentThread


// TODO: Step 3 - Create a thread for conversation with history
// Hint: Use agent.GetNewThread() - this thread will automatically maintain conversation context


// Execute program with conversation history demonstration
const string terminationPhrase = "quit";
string? userInput;

Console.WriteLine("=== Financial Advisor with Conversation Memory ===");
Console.WriteLine("This lesson demonstrates how Agent Framework automatically maintains conversation history.");
Console.WriteLine("Try asking follow-up questions to see the agent remember previous context!");
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
        
        // TODO: Step 4 - Use agent to respond with conversation history maintained automatically
        // Hint: Same as Lesson 1, but notice how the agent remembers previous messages
        //       The AgentThread automatically maintains this context for you
        // var response = await agent.RunAsync(userInput, thread);
        // Console.WriteLine(response);

    }
}
while (userInput != terminationPhrase);

Console.WriteLine("Notice how the agent remembered our conversation context! This is automatic with Agent Framework.");