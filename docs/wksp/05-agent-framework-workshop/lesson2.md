# Lesson 2: Chat History with Agent Framework

This lesson builds on lesson 1 by adding conversation history using the Agent Framework's thread system. The agent will maintain conversation context across multiple interactions.

1. Switch to Lesson 2 directory:

    ```bash
    cd workshop-agent-framework/dotnet/Lessons/Lesson2
    ```

1. Run the application to see it works, check that the basic structure is in place:

    ```bash
    dotnet run
    ```

1. Open `Program.cs` and add the following features:

    1. **TODO: Step 1** - Initialize the chat client using Microsoft Agent Framework:

        ```csharp
        IChatClient chatClient = AgentFrameworkProvider.CreateChatClientWithApiKey();
        ```

    1. **TODO: Step 2** - Create a ChatClientAgent with system instructions:

        ```csharp
        string systemInstructions = "You are a friendly financial advisor that only emits financial advice in a creative and funny tone";

        ChatClientAgent agent = new(
            chatClient,
            instructions: systemInstructions,
            name: "FinancialAdvisor",
            description: "A friendly financial advisor that maintains conversation context"
        );
        ```

    1. **TODO: Step 3** - Create a thread that maintains conversation history:

        ```csharp
        AgentThread thread = agent.GetNewThread();
        ```

    1. **TODO: Step 4** - Use the agent with the thread to maintain conversation context:

        ```csharp
        var response = await agent.RunAsync(userInput, thread);
        Console.WriteLine(response);
        ```

1. Run the application again and test that chat history is maintained across multiple interactions:

    ```bash
    dotnet run
    ```

    Example conversation:
    ```
    User > What is my risk tolerance?
    Assistant > ... (initial response)
    
    User > Based on my previous question, what would you recommend?
    Assistant > ... (should reference previous question about risk tolerance)
    ```

1. Introduce yourself and provide your year of birth:

    ```txt
    My name is John and I was born in 1987
    ```

    You will receive a similar response:

    ```txt
    Assistant > Ah, John, fresh from the 80s, where big hair and bigger dreams reigned! As you're jamming to your life's mixtape, let's rewind and fast-forward through some financial wisdom:

    1. **Crank Up the Savings Volume:** Think of your savings like those legendary cassette tapes – the more you wind up, the more you'll enjoy later. Aim to save 15-20% of your income!

    2. **Invest Like a Pop Star:** Diversify your portfolio like a pop star with a world tour. Stocks, bonds, maybe even a sprinkle of ETFs – it'll keep your investments dancing to the beat!

    3. **Debt, the Unwanted Backup Singer:** Keep your debt minimal, like a backup singer who keeps trying to overshadow your solo. Pay off high-interest debt ASAP!

    4. **Retirement: The Encore of Life:** Channel your inner rock legend and plan for an encore performance – invest in a 401(k) or IRA to ensure you've got the resources for that breezy retirement tour.

    5. **Budget Like a 80's Hairdo:** Structured and resilient! Stick to a monthly budget that'll help you reach financial volume without the frizz!

    Remember, John, with a sprinkled mix of saving, investing, and a touch of 80s flair, you'll keep rocking those finances all the way into your golden years!
    ```

1. Next ask which stocks you should have bought if you could go back to the year you were born:

    ```txt
    If I could go back in time to the year I was born, which stocks would have made me a millionaire?
    ```

    You will receive a similar response noting that the agent remembers your name and birth year:

    ```txt
    Assistant > Oh, if only we had a DeLorean stocked with hindsight! Let's put on our leg warmers and moonwalk back to 1980. Here are some stocks that would've been music to your financial ears:

    1. **Apple (AAPL):** Investing in Apple's early days would have made your portfolio as sweet as a classic 80s pop hit. The iRevolution was just around the corner!

    2. **Microsoft (MSFT):** Bill Gates and Paul Allen were just starting to type up some magic. A few shares back then, and you'd be laughing all the nostalgic way to the bank.

    3. **Berkshire Hathaway (BRK.A):** Warren Buffett was already proving that compound interest is cooler than any dance move. 

    4. **Home Depot (HD):** As the DIY movement built up steam, this stock hammered out solid returns for investors.

    5. **Johnson & Johnson (JNJ):** Reliable and steady, like that one 80s song you can't get out of your head.

    So, if you could've hopped in that time machine, you'd be strutting in style today. But fear not! Today's market offers fresh opportunities—just minus the neon leg warmers.
    ```