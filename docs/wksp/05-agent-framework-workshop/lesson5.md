# Lesson 5: Sequential Workflows with Multiple Agents

This lesson demonstrates sequential orchestration where multiple specialized agents work together in a defined sequence to analyze a complete investment portfolio.

1. Switch to Lesson 5 directory:

    ```bash
    cd workshop-agent-framework/dotnet/Lessons/Lesson5
    ```

1. Run the application to see it works:

    ```bash
    dotnet run
    ```

1. Open `Program.cs` and replace the single agent approach with a sequential workflow of three specialized agents:

    1. **TODO: Step 1** - Add the workflows namespace:

        ```csharp
        using Microsoft.Agents.AI.Workflows;
        ```

    1. **TODO: Step 2** - Replace the single financial agent with the Portfolio Research Agent. Remove the entire section from `// Financial Analysis Agent system instructions...` through the `financialAnalysisAgent` creation, and replace with:

        ```csharp
        // Portfolio Research Agent - Gathers data on all stocks
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

    1. **TODO: Step 3** - Create the Risk Assessment Agent:

        ```csharp
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

    1. **TODO: Step 4** - Create the Investment Advisor Agent:

        ```csharp
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

    1. **TODO: Step 4** - Build the sequential workflow:

        ```csharp
        // Build the workflow and convert it to an agent
        AIAgent workflowAgent = await AgentWorkflowBuilder.BuildSequential([
            researchAgent,
            riskAgent,
            advisorAgent
        ]).AsAgentAsync();
        ```

    1. **TODO: Step 5** - Replace the conversation loop (from `// Create a thread for conversation` onwards) with a direct portfolio analysis test:

        ```csharp
        // Test the portfolio analysis workflow
        string portfolioQuery = "Analyze this portfolio: MSFT, AAPL, GOOGL, TSLA, NVDA. " +
                               "Get current prices, assess risks, and provide recommendations.";

        Console.WriteLine("🔍 Starting Portfolio Analysis...\n");

        try
        {
            string? lastAgentName = null;
            await foreach (var update in workflowAgent.RunStreamingAsync(portfolioQuery))
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
            Console.WriteLine($"❌ Error in portfolio analysis: {ex.Message}");
        }
        ```

1. Test the sequential workflow with portfolio analysis:

    ```bash
    dotnet run
    ```

    Expected behavior:
    1. **Portfolio Research Agent** gathers current prices and market news for each stock (MSFT, AAPL, GOOGL, TSLA, NVDA)
    2. **Risk Assessment Agent** analyzes portfolio balance, diversification, and assigns a risk score
    3. **Investment Advisor Agent** provides buy/hold/sell recommendations and rebalancing suggestions

    The output will show clear sections for each agent with streaming text as they work sequentially.

This lesson demonstrates how the Agent Framework enables sophisticated multi-agent workflows where each agent specializes in a specific task, and their outputs flow sequentially to create comprehensive analysis.