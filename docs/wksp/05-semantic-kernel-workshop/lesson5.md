# Lesson 5: Multi-Agent Portfolio Analysis with Sequential Orchestration

In this lesson, we'll transform the single financial analysis agent from Lesson 4 into a multi-agent system using Microsoft Agent Framework's Sequential Orchestration. You'll learn how to create specialized agents that work together in a pipeline to provide comprehensive portfolio analysis.

## Learning Objectives

- Convert from Azure AI Foundry single agent to multi-agent Chat Client approach
- Create multiple specialized agents for different analysis tasks
- Use Sequential Orchestration to chain agents together
- Stream responses from a multi-agent workflow

## Prerequisites

1. Complete [Lesson 4](lesson4.md) successfully
2. Ensure all [pre-requisites](pre-reqs.md) are met and installed

## Instructions

### Step 1: Add Additional Imports

1. Navigate to the Lesson 5 directory:
    ```bash
    cd ../Lesson5
    ```

2. Locate **TODO: Step 1 - Add additional imports for workflows and streaming** in `Program.cs` and add:

    ```csharp
    using Microsoft.Agents.AI.Workflows;
    using System.Text;
    ```

### Step 2: Verify Chat Client Setup

1. The Chat Client approach is already set up. Locate **TODO: Step 2 - Initialize the chat client with Agent Framework** to see the initialization that's already done for you.

### Step 3: Create the Portfolio Research Agent

1. Locate **TODO: Step 3 - Create Portfolio Research Agent** and replace it with:

    ```csharp
    // Agent 1: Portfolio Research Agent - Gathers data on all stocks
    string researchAgentInstructions = """
        You are a Portfolio Research Agent. Your job is to gather comprehensive market data for stocks.
        
        For each stock symbol provided:
        - Get the current stock price
        - Search the web for recent news and market sentiment
        - Provide a brief summary of each stock's current situation
        
        Provide your complete research in a SINGLE response with clear sections for each stock.
        Format your response as a research report with stock symbols as headers.
        """;

    ChatClientAgent researchAgent = new(
        chatClient,
        instructions: researchAgentInstructions,
        name: "PortfolioResearchAgent",
        description: "Gathers market data and news for portfolio stocks",
        tools: [stockPriceTool, webSearchTool, timeTool]
    );
    ```

### Step 4: Create the Risk Assessment Agent

1. Locate **TODO: Step 4 - Create Risk Assessment Agent** and replace it with:

    ```csharp
    // Agent 2: Risk Assessment Agent - Analyzes portfolio risk
    string riskAgentInstructions = """
        You are a Risk Assessment Agent. Analyze the portfolio composition and risk profile.
        
        Based on the research provided:
        - Identify sector concentration (tech-heavy, diversified, etc.)
        - Assess portfolio balance and diversification
        - Calculate a risk score from 1-10 (1=very safe, 10=very risky)
        - Highlight any concerns about over-concentration
        
        Provide your complete analysis in a SINGLE response.
        Be concise and actionable.
        """;

    ChatClientAgent riskAgent = new(
        chatClient,
        instructions: riskAgentInstructions,
        name: "RiskAssessmentAgent",
        description: "Analyzes portfolio risk and diversification"
    );
    ```

### Step 5: Create the Investment Advisor Agent

1. Locate **TODO: Step 5 - Create Investment Advisor Agent** and replace it with:

    ```csharp
    // Agent 3: Investment Advisor Agent - Provides recommendations
    string advisorAgentInstructions = """
        You are an Investment Advisor Agent. Synthesize research and risk analysis into actionable recommendations.
        
        Based on the research and risk assessment:
        - Provide an overall portfolio health score (1-10)
        - Give specific buy/hold/sell recommendations for each stock
        - Suggest rebalancing actions if needed
        - Provide 2-3 key takeaways
        
        Provide your complete recommendations in a SINGLE response.
        Be clear, concise, and actionable.
        """;

    ChatClientAgent advisorAgent = new(
        chatClient,
        instructions: advisorAgentInstructions,
        name: "InvestmentAdvisorAgent",
        description: "Provides investment recommendations based on research and risk analysis"
    );
    ```

### Step 6: Update the User Interface and Orchestration

1. Locate **TODO: Step 6 - Update the User Interface and Orchestration** and replace the entire section from the console output lines through the main loop with:

    ```csharp
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
                
                // Build the workflow and convert it to an agent
                AIAgent workflowAgent = await AgentWorkflowBuilder.BuildSequential([
                    researchAgent,
                    riskAgent,
                    advisorAgent
                ]).AsAgentAsync();
                
                // Run the workflow with streaming output
                string? lastAgentName = null;
                await foreach (var update in workflowAgent.RunStreamingAsync($"Analyze this portfolio of stocks: {userInput}"))
                {
                    // Print header when we see a new agent starting
                    if (lastAgentName != update.AuthorName)
                    {
                        if (lastAgentName != null)
                        {
                            Console.WriteLine(); // Add spacing between agents
                            Console.WriteLine(new string('-', 70));
                            Console.WriteLine();
                        }
                        
                        lastAgentName = update.AuthorName;
                        Console.WriteLine($"[{update.AuthorName}]");
                        Console.WriteLine(new string('-', 70));
                    }
                    
                    // Stream the text output in real-time
                    Console.Write(update.Text);
                }
                
                Console.WriteLine("\n" + new string('=', 70));
                Console.WriteLine("✓ ANALYSIS COMPLETE");
                Console.WriteLine(new string('=', 70));
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
    ```

## Testing the Application

1. Build and run the program:
    ```bash
    dotnet run
    ```

2. Try analyzing a portfolio with these examples:
   - "MSFT, AAPL, GOOGL"
   - "TSLA, NVDA, AMD, INTC"
   - "SPY, QQQ, VTI"

## Expected Output

When you successfully complete the lesson, you should see output similar to this when analyzing "MSFT, AAPL, GOOGL":

```
=== Investment Portfolio Analyzer with Sequential Orchestration ===
This demonstrates MAF Sequential Orchestration with three specialized agents:
  1. Portfolio Research Agent (gathers data)
  2. Risk Assessment Agent (analyzes risk)
  3. Investment Advisor Agent (provides recommendations)

Enter stock symbols separated by commas (e.g., 'MSFT, AAPL, TSLA, NVDA')
Type 'quit' to exit.
====================================================================

Enter portfolio > MSFT, AAPL, GOOGL

======================================================================
PORTFOLIO ANALYSIS - SEQUENTIAL ORCHESTRATION
======================================================================

[PortfolioResearchAgent]
----------------------------------------------------------------------
# Portfolio Research Report

## MSFT (Microsoft Corporation)
**Current Price:** $408.43

Recent news indicates strong performance in cloud computing...

## AAPL (Apple Inc.)
**Current Price:** $175.84

Latest market sentiment shows...

## GOOGL (Alphabet Inc.)
**Current Price:** $139.69

Current market analysis reveals...

----------------------------------------------------------------------

[RiskAssessmentAgent]
----------------------------------------------------------------------
# Portfolio Risk Assessment

**Sector Analysis:** This portfolio is heavily concentrated in large-cap technology stocks...

**Risk Score:** 6/10 (Moderate Risk)

**Diversification Concerns:**
- High concentration in tech sector
- All three are mega-cap stocks
- Potential correlation during market downturns

----------------------------------------------------------------------

[InvestmentAdvisorAgent]
----------------------------------------------------------------------
# Investment Recommendations

**Overall Portfolio Health Score:** 7/10

## Individual Stock Recommendations:
- **MSFT:** BUY - Strong cloud growth and AI positioning
- **AAPL:** HOLD - Solid fundamentals but high valuation
- **GOOGL:** BUY - Undervalued relative to growth prospects

## Key Takeaways:
1. Consider adding non-tech exposure for better diversification
2. All three stocks are quality investments for long-term portfolios
3. Monitor for any signs of tech sector rotation

======================================================================
✓ ANALYSIS COMPLETE
======================================================================
```

## Key Concepts Learned

1. **Multi-Agent Architecture**: How to break down complex tasks across specialized agents
2. **Sequential Orchestration**: Using `AgentWorkflowBuilder.BuildSequential()` to chain agents
3. **Streaming Responses**: Real-time output from multi-agent workflows
4. **Agent Specialization**: Creating focused agents for research, analysis, and recommendations

## Troubleshooting

1. **Build Errors**: Ensure you've updated all the using statements and removed the Azure AI Foundry specific code
2. **Runtime Errors**: Verify your API keys are properly configured in `appsettings.json`
3. **No Output**: Check that the agents are properly configured with tools and instructions

## Next Steps

In [Lesson 6](lesson6.md), you'll learn how to implement parallel orchestration and more advanced multi-agent patterns.
