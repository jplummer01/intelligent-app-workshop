using Core.Utilities.Config;
using Core.Utilities.Plugins;
using Core.Utilities.Services;
using Core.Utilities.Extensions;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using System.Text;

// TODO: Step 1 - Initialize the chat client with Agent Framework  


// TODO: Step 2 - Initialize plugins
// Hint: Same as Lesson 5 - TimeInformationPlugin, StockDataPlugin, HostedWebSearchTool


// TODO: Step 3 - Create AI Functions from plugins


// TODO: Step 4 - Create Agent 1: Portfolio Research Agent
// Create instructions for an agent that:
// - Gathers comprehensive market data for stocks
// - Gets current stock prices
// - Searches web for recent news and sentiment
// - Provides research in a single response with clear sections


// TODO: Step 5 - Create Agent 2: Risk Assessment Agent  
// Create instructions for an agent that:
// - Analyzes portfolio composition and risk profile
// - Identifies sector concentration
// - Calculates risk score from 1-10
// - Highlights over-concentration concerns


// TODO: Step 6 - Create Agent 3: Investment Advisor Agent
// Create instructions for an agent that:
// - Synthesizes research and risk analysis
// - Provides portfolio health score (1-10)
// - Gives buy/hold/sell recommendations
// - Suggests rebalancing actions


// Execute program
Console.WriteLine("=== Investment Portfolio Analyzer with Sequential Orchestration ===");
Console.WriteLine("This demonstrates MAF Sequential Orchestration with three specialized agents:");
Console.WriteLine("  1. Portfolio Research Agent (gathers data)");
Console.WriteLine("  2. Risk Assessment Agent (analyzes risk)");
Console.WriteLine("  3. Investment Advisor Agent (provides recommendations)");
Console.WriteLine();
Console.WriteLine("Enter stock symbols separated by commas (e.g., 'MSFT, AAPL, TSLA, NVDA')");
Console.WriteLine("Type 'quit' to exit.");
Console.WriteLine("====================================================================");
Console.WriteLine();

const string terminationPhrase = "quit";
string? userInput;

do
{
    Console.Write("Enter portfolio > ");
    userInput = Console.ReadLine();

    if (userInput is not null and not terminationPhrase)
    {
        try
        {
            Console.WriteLine("\n" + new string('=', 70));
            Console.WriteLine("PORTFOLIO ANALYSIS - SEQUENTIAL ORCHESTRATION");
            Console.WriteLine(new string('=', 70) + "\n");
            
            // TODO: Step 7 - Build the workflow and convert it to an agent
            // Hint: Use AgentWorkflowBuilder.BuildSequential([researchAgent, riskAgent, advisorAgent]).AsAgentAsync()
            // AIAgent workflowAgent = await AgentWorkflowBuilder.BuildSequential([
            //     researchAgent,
            //     riskAgent, 
            //     advisorAgent
            // ]).AsAgentAsync();
            
            // TODO: Step 8 - Run the workflow with streaming output
            // Hint: Use workflowAgent.RunStreamingAsync() and track which agent is responding
            // Display headers when agents change and stream their responses
            // string? lastAgentName = null;
            // await foreach (var update in workflowAgent.RunStreamingAsync($"Analyze this portfolio of stocks: {userInput}"))
            // {
            //     if (lastAgentName != update.AuthorName)
            //     {
            //         if (lastAgentName != null) Console.WriteLine();
            //         lastAgentName = update.AuthorName;
            //         Console.WriteLine($"[{update.AuthorName}]");
            //         Console.WriteLine(new string('-', 70));
            //     }
            //     Console.Write(update.Text);
            // }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error analyzing portfolio: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
        }
        
        Console.WriteLine();
    }
}
while (userInput != terminationPhrase);

Console.WriteLine("Thank you for using the Portfolio Analyzer!");