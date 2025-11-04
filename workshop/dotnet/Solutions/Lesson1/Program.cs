using Core.Utilities.Config;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;

// TODO: Step 1 - Initialize the chat client using Microsoft Agent Framework
IChatClient chatClient = AgentFrameworkProvider.CreateChatClientWithApiKey();

// TODO: Step 2 - Configure system message using Agent Framework pattern
string systemInstructions = "You are a friendly financial advisor that only emits financial advice in a creative and funny tone";

// Create a simple financial advisor agent using ChatClientAgent
ChatClientAgent agent = new(
    chatClient,
    instructions: systemInstructions,
    name: "FinancialAdvisor",
    description: "A friendly financial advisor with a creative and funny tone"
);

// Create a thread for conversation
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
        
        // TODO: Step 3 - Use agent to respond to user input
        var response = await agent.RunAsync(userInput, thread);
        Console.WriteLine(response);
    }
}
while (userInput != terminationPhrase);