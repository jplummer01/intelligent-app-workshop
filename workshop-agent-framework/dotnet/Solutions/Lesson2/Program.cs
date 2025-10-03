using Core.Utilities.Config;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;

// Step 1 - Initialize the chat client using Microsoft Agent Framework
IChatClient chatClient = AgentFrameworkProvider.CreateChatClientWithApiKey();

// Step 2 - Create financial advisor agent with conversation context
string systemInstructions = "You are a friendly financial advisor that only emits financial advice in a creative and funny tone";

ChatClientAgent agent = new(
    chatClient,
    instructions: systemInstructions,
    name: "FinancialAdvisor",
    description: "A friendly financial advisor that maintains conversation context"
);

// Create a thread for conversation with history
AgentThread thread = agent.GetNewThread();

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
        
        // Step 3 & 4 - Use agent to respond with conversation history maintained automatically
        var response = await agent.RunAsync(userInput, thread);
        Console.WriteLine(response);
    }
}
while (userInput != terminationPhrase);