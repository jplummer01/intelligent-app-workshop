# Lesson 4: Azure AI Foundry Agent with Web Search

In this lesson we will upgrade from a basic Agent Framework agent to an Azure AI Foundry agent with web search capabilities using Bing grounding. This provides real-time information access for comprehensive financial analysis.

1. Ensure all [pre-requisites](pre-reqs.md) are met and installed.

1. Switch to Lesson 4 directory:

    ```bash
    cd ../Lesson4
    ```

1. Start by copying `appsettings.json` from Lesson 1:

    ```bash
    cp ../Lesson1/appsettings.json .
    ```

1. Run program to validate the starter code is functional:

    ```bash
    dotnet run
    ```

1. Locate **TODO: Step 1 - Add Azure imports for AI Foundry integration** in `Program.cs` and add the three using statements:

    ```csharp
    using Azure.Identity;
    using Azure.AI.Agents.Persistent;
    using Azure.AI.OpenAI;
    ```

1. Locate **TODO: Step 2 - Set up Azure AI Foundry client and environment** and replace the IChatClient initialization with:

    ```csharp
    var applicationSettings = AISettingsProvider.GetSettings();
    Environment.SetEnvironmentVariable("AZURE_FOUNDRY_PROJECT_ENDPOINT", applicationSettings.AIFoundryProject.Endpoint);
    Environment.SetEnvironmentVariable("AZURE_FOUNDRY_PROJECT_DEPLOYMENT_NAME", applicationSettings.AIFoundryProject.DeploymentName);
    Environment.SetEnvironmentVariable("BING_CONNECTION_ID", applicationSettings.AIFoundryProject.GroundingWithBingConnectionId);
    var persistentAgentsClient = new PersistentAgentsClient(applicationSettings.AIFoundryProject.ConnectionString, new DefaultAzureCredential());
    ```

1. Locate **TODO: Step 3 - Add web search tool for Bing grounding** and add:

    ```csharp
    HostedWebSearchTool webSearchTool = new();
    ```

1. Locate **TODO: Step 4 - Create Azure AI Foundry agent with Bing grounding and local tools** and replace the entire ChatClientAgent creation and thread setup with:

    ```csharp
    var financialAnalysisAgent = await persistentAgentsClient.CreateAIAgentAsync(
        applicationSettings.AIFoundryProject.DeploymentName,
        instructions: stockSentimentAgentInstructions,
        tools: [ 
            new BingGroundingToolDefinition(
                new BingGroundingSearchToolParameters(
                    new[] { 
                        new BingGroundingSearchConfiguration(
                            applicationSettings.AIFoundryProject.GroundingWithBingConnectionId
                        ) 
                    }
                )
            ) 
        ]
    );

    var thread = financialAnalysisAgent.GetNewThread();

    var agentOptions = new ChatClientAgentRunOptions(new() { 
        Tools = [
            timeTool,
            stockPriceTool,
            stockPriceDateTool,
            webSearchTool
        ] 
    });
    ```

1. Locate **TODO: Step 5 - Update agent execution for Azure AI Foundry** and replace the agent execution and response handling with:

    ```csharp
    var response = await financialAnalysisAgent.RunAsync(userInput, thread, agentOptions);
    Console.WriteLine(response);
    ```

    Remove the existing response handling code (the if/else blocks for extracting messages).

1. Run the program to test the Azure AI Foundry agent:

    ```bash
    dotnet run
    ```

1. Test the agent with questions that require web search:
   - "What do you think about Microsoft stock?"
   - "How is the renewable energy sector performing?"
   - "Should I invest in AI stocks right now?"

The agent should now provide comprehensive analysis using both real-time web search data and local stock price functions.

## Troubleshooting

If you encounter issues:

1. **Build Errors**: Ensure all required packages are installed by running:
   ```bash
   dotnet restore
   ```

2. **Authentication Issues**: Verify your Azure credentials are configured:
   ```bash
   az login
   ```

3. **Missing Environment Variables**: Check that your `appsettings.json` contains all required Azure AI Foundry configuration:
   - AIFoundryProject.Endpoint
   - AIFoundryProject.DeploymentName
   - AIFoundryProject.ConnectionString
   - AIFoundryProject.GroundingWithBingConnectionId

4. **Runtime Errors**: Ensure you've completed all TODO steps in order and that the Azure AI Foundry project is properly configured in the Azure portal.

## Verification

To verify your implementation is working correctly:

1. The application should start without errors
2. You should see the updated console banner mentioning "Financial Analysis Agent"
3. Questions about stocks should return responses that include web search results
4. The agent should provide sources and comprehensive analysis
5. Both local stock price data and web search information should be included in responses

## Key Concepts Learned

- **Azure AI Foundry Integration**: Using PersistentAgentsClient for cloud-based agent hosting
- **Bing Grounding**: Real-time web search capabilities for current information
- **Hybrid Tool Architecture**: Combining foundry tools (web search) with local functions (stock data)
- **Environment Configuration**: Setting up Azure AI Foundry connection parameters
- **ChatClientAgentRunOptions Pattern**: Passing local tools at runtime following GitHub example patterns

## Architecture

This lesson demonstrates the foundry agent pattern where:
1. Agent is created in Azure AI Foundry with foundry-specific tools (Bing grounding)
2. Local function tools are passed at runtime via ChatClientAgentRunOptions
3. Agent orchestrates between cloud and local capabilities for comprehensive responses

## Expected Output

When you successfully complete the lesson, you should see output similar to this when asking "What do you think about Microsoft stock?":

```
=== Financial Analysis Agent with Microsoft Agent Framework ===
This agent provides comprehensive financial analysis using web search and market data.
Ask any financial question - about specific stocks, market trends, sectors, or investment strategies.
Examples: 'What do you think about Microsoft?', 'How is the tech sector performing?', 'Should I invest in renewable energy stocks?'
Type 'quit' to exit.
==================================================================================

User > What do you think about Microsoft stock?
Assistant > Based on my analysis of Microsoft Corporation (MSFT), here's a comprehensive assessment:

**Current Stock Analysis:**
- Current stock price: $[current_price] 
- Market sentiment: [sentiment_analysis]

**Recent Market News:**
[Real-time news and analysis from web search]

**Technical Analysis:**
[Analysis based on stock price data and market trends]

**Investment Recommendation:**
Rating: [1-10 scale]
Recommendation: [Buy/Hold/Sell]

**Sources:**
- [List of web sources used for analysis]
- Stock price data from financial APIs

User > quit
Thank you for using the Financial Analysis Agent!
```

## Next Steps

In [Lesson 5](lesson5.md), you'll learn how to extend this foundation with additional advanced agent capabilities and multi-agent scenarios.
