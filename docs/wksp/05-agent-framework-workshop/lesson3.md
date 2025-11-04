# Lesson 3: Specialized Agent with Stock Sentiment Analysis

This lesson creates a specialized agent focused on stock sentiment analysis using the Agent Framework with specific system instructions and function calling capabilities.

1. Switch to Lesson 3 directory:

    ```bash
    cd workshop/dotnet/Lessons/Lesson3
    ```

1. Copy the configuration file from the Solutions directory:

    ```bash
    cp ../../Solutions/Lesson3/appsettings.json .
    ```

1. Run the application to see it works:

    ```bash
    dotnet run
    ```

1. Open `Program.cs` and build a specialized stock sentiment agent:

    1. **TODO: Step 1** - Initialize the chat client and plugins:

        ```csharp
        IChatClient chatClient = AgentFrameworkProvider.CreateChatClientWithApiKey();

        // Initialize plugins
        TimeInformationPlugin timePlugin = new();
        HttpClient httpClient = new();
        StockDataPlugin stockDataPlugin = new(new StocksService(httpClient));
        ```

    1. **TODO: Step 2** - Create AI Functions from plugins:

        ```csharp
        var timeTool = AIFunctionFactory.Create(timePlugin.GetCurrentUtcTime);
        var stockPriceTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPrice);
        var stockPriceDateTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPriceForDate);
        ```

    1. **TODO: Step 3** - Define specialized system instructions for stock sentiment analysis:

        ```csharp
        string stockSentimentAgentInstructions = """
            You are a Stock Sentiment Agent. Your responsibility is to find the stock sentiment for a given Stock.

            RULES:
            - Use stock sentiment scale from 1 to 10 where stock sentiment is 1 for sell and 10 for buy.
            - Provide the rating in your response and a recommendation to buy, hold or sell.
            - Include the reasoning behind your recommendation.
            - Include the source of the sentiment in your response.
            - Focus on technical analysis based on stock price data and general market knowledge.
            """;
        ```

    1. **TODO: Step 4** - Create the specialized Stock Sentiment Agent:

        ```csharp
        ChatClientAgent stockSentimentAgent = new(
            chatClient,
            instructions: stockSentimentAgentInstructions,
            name: "StockSentimentAgent",
            description: "An intelligent agent that analyzes stock sentiment using market data",
            tools: [
                timeTool,
                stockPriceTool, 
                stockPriceDateTool
            ]
        );
        ```

    1. **TODO: Step 5** - Create thread and process user requests:

        ```csharp
        AgentThread thread = stockSentimentAgent.GetNewThread();
        
        var response = await stockSentimentAgent.RunAsync(userInput, thread);
        
        if (response?.Messages?.Any() == true)
        {
            var lastMessage = response.Messages.Last();
            Console.WriteLine(lastMessage.Text ?? "No response generated.");
        }
        ```

1. Test the specialized agent with stock sentiment queries:

    ```bash
    dotnet run
    ```

    Example questions to test:
    ```
    User > What is your sentiment on MSFT?
    Assistant > (should analyze Microsoft stock and provide sentiment rating 1-10 with buy/hold/sell recommendation)
    
    User > Analyze AAPL sentiment
    Assistant > (should provide Apple stock sentiment analysis with reasoning)
    
    User > Should I buy TSLA?
    Assistant > (should analyze Tesla and provide specific buy/hold/sell recommendation)
    ```

This lesson demonstrates how to create specialized agents with focused system instructions and domain-specific capabilities using the Agent Framework.