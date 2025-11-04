# Creating the Backend API

These changes are already available in the repository. These instructions walk
you through the process followed to create the backend API from the Console application:

1. Start by creating a new directory:

    ```bash
    mkdir -p workshop/dotnet/App/backend
    ```

1. Next create a new SDK .NET project:

    ```bash
    cd workshop/dotnet/App/
    dotnet new webapi -n backend --no-openapi --force
    cd backend
    ```

1. Build project to confirm it is successful:

    ```txt
    dotnet build

    Build succeeded.
        0 Warning(s)
        0 Error(s)
    ```

1. Add the following nuget packages:

    ```bash
    dotnet add package Microsoft.AspNetCore.Mvc
    dotnet add package Swashbuckle.AspNetCore
    ```

1. Replace the contents of `Program.cs` in the project directory with the following code. This file initializes and loads the required services and configuration for the API, namely configuring CORS protection, enabling controllers for the API and exposing Swagger document:

    ```csharp
    using Microsoft.AspNetCore.Antiforgery;
    using Extensions;
    using System.Text.Json.Serialization;

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    // See: https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    // Required to generate enumeration values in Swagger doc
    builder.Services.AddControllersWithViews().AddJsonOptions(options => 
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    builder.Services.AddOutputCache();
    builder.Services.AddAntiforgery(options => { 
        options.HeaderName = "X-CSRF-TOKEN-HEADER"; 
        options.FormFieldName = "X-CSRF-TOKEN-FORM"; });
    builder.Services.AddHttpClient();
    builder.Services.AddDistributedMemoryCache();

    // Load user secrets
    builder.Configuration.AddUserSecrets<Program>();

    var app = builder.Build();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseOutputCache();
    app.UseRouting();
    app.UseCors();
    app.UseAntiforgery();
    app.MapControllers();

    app.Use(next => context =>
    {
        var antiforgery = app.Services.GetRequiredService<IAntiforgery>();
        var tokens = antiforgery.GetAndStoreTokens(context);
        context.Response.Cookies.Append("XSRF-TOKEN", tokens?.RequestToken ?? string.Empty, new CookieOptions() { HttpOnly = false });
        return next(context);
    });

    app.Map("/", () => Results.Redirect("/swagger"));

    app.MapControllerRoute(
        "default",
        "{controller=ChatController}");

    app.Run();
    ```

1. Next we need to create a `Controllers` directory to add REST API controller classes:

    ```bash
    cd ..
    mkdir Controllers
    cd Controllers
    ```

1. Within the `Controllers` directory create a `ChatController.cs` file which implements a sequential orchestration workflow with three specialized agents (following the Lesson 5 pattern):

    ```csharp
    using Core.Utilities.Models;
    using Core.Utilities.Config;
    using Core.Utilities.Extensions;
    using Core.Utilities.Plugins;
    using Core.Utilities.Services;
    using Microsoft.Agents.AI;
    using Microsoft.Agents.AI.Workflows;
    using Microsoft.Extensions.AI;
    using Microsoft.AspNetCore.Mvc;

    namespace Controllers;

    [ApiController]
    [Route("chat")]
    public class ChatController : ControllerBase 
    {
        private readonly AIAgent _workflowAgent;
        private readonly IChatClient _chatClient;
        
        public ChatController()
        {
            // Initialize the chat client with Agent Framework
            _chatClient = AgentFrameworkProvider.CreateChatClientWithApiKey();

            // Initialize plugins  
            TimeInformationPlugin timePlugin = new();
            HttpClient httpClient = new();
            StockDataPlugin stockDataPlugin = new(new StocksService(httpClient));
            HostedWebSearchTool webSearchTool = new();

            // Create AI Functions from plugins
            var timeTool = AIFunctionFactory.Create(timePlugin.GetCurrentUtcTime);
            var stockPriceTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPrice);
            var stockPriceDateTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPriceForDate);

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

            AIAgent researchAgent = new ChatClientAgent(
                _chatClient,
                instructions: researchAgentInstructions,
                name: "PortfolioResearchAgent",
                description: "Gathers market data and news for portfolio stocks",
                tools: [stockPriceTool, webSearchTool, stockPriceDateTool, timeTool]
            );

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

            AIAgent riskAgent = new ChatClientAgent(
                _chatClient,
                instructions: riskAgentInstructions,
                name: "RiskAssessmentAgent",
                description: "Analyzes portfolio risk and diversification"
            );

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

            AIAgent advisorAgent = new ChatClientAgent(
                _chatClient,
                instructions: advisorAgentInstructions,
                name: "InvestmentAdvisorAgent",
                description: "Provides investment recommendations based on research and risk analysis"
            );

            // Build the sequential workflow as an agent
            var workflowTask = AgentWorkflowBuilder.BuildSequential([
                researchAgent,
                riskAgent,
                advisorAgent
            ]).AsAgentAsync();
            
            _workflowAgent = workflowTask.GetAwaiter().GetResult();
        }

        [HttpPost("/chat")]
        public async Task<Core.Utilities.Models.ChatResponse> ReplyAsync([FromBody]ChatRequest request)
        {
            var responseChatHistory = new List<Core.Utilities.Models.ChatMessage>();
            
            // Convert existing history to response format
            foreach (var msg in request.MessageHistory)
            {
                responseChatHistory.Add(msg);
            }

            string fullMessage = "";
            
            if (!string.IsNullOrEmpty(request.InputMessage))
            {
                // Add user message to response history
                responseChatHistory.Add(new Core.Utilities.Models.ChatMessage(request.InputMessage, Core.Utilities.Models.Role.User));
                
                try
                {
                    // Run the sequential workflow (Research → Risk → Advisor)
                    var response = await _workflowAgent.RunAsync(request.InputMessage);
                    fullMessage = response?.ToString() ?? "";
                    
                    // Add assistant response to history
                    responseChatHistory.Add(new Core.Utilities.Models.ChatMessage(fullMessage, Core.Utilities.Models.Role.Assistant));
                }
                catch (Exception ex)
                {
                    fullMessage = $"Error processing request: {ex.Message}";
                    if (ex.InnerException != null)
                    {
                        fullMessage += $"\nInner exception: {ex.InnerException.Message}";
                    }
                    responseChatHistory.Add(new Core.Utilities.Models.ChatMessage(fullMessage, Core.Utilities.Models.Role.Assistant));
                }
            }
            
            var chatResponse = new Core.Utilities.Models.ChatResponse(fullMessage, responseChatHistory);    
            return chatResponse;
        }
    }
    ```

    This implementation uses the **Microsoft Agent Framework** with sequential orchestration (Lesson 5 pattern):
    
    - **Three specialized agents** work in sequence:
      1. **PortfolioResearchAgent** - Gathers market data using stock price tools and web search
      2. **RiskAssessmentAgent** - Analyzes portfolio composition and calculates risk scores
      3. **InvestmentAdvisorAgent** - Synthesizes findings into actionable recommendations
    
    - **Sequential workflow** ensures each agent builds upon the previous agent's output
    - **Stateless design** - Each request is independent, no thread management needed
    - Uses `AgentFrameworkProvider` which reads configuration from `appsettings.json`

## Running the Backend API locally

1. To run API locally first copy valid `appsettings.json` from completed `Solutions/Lesson5` into `backend` directory:

    ```bash
    #cd into backend directory
    cd ../
    cp ../../Solutions/Lesson5/appsettings.json .
    ```

1. Next run using `dotnet run`:

    ```bash
    dotnet run
    ...
    info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
    ```

1. Application will start on specified port (port may be different). Navigate to <http://localhost:5000> or corresponding [forwarded address](https://docs.github.com/en/codespaces/developing-in-a-codespace/forwarding-ports-in-your-codespace) (if using Github CodeSpace) and it should redirect you to the swagger UI page.

1. You can test the `/chat` endpoint using the "Try it out" feature from within Swagger UI, or via command line using `curl` command with stock symbols:

    ```bash
    curl -X 'POST' \
    'http://localhost:5000/chat' \
    -H 'accept: text/plain' \
    -H 'Content-Type: application/json' \
    -d '{
    "inputMessage": "MSFT, AAPL, NVDA",
    "messageHistory": []
    }'
    ```

    The sequential workflow will execute:
    1. **Research Agent** - Gathers current prices and market news for each stock
    2. **Risk Agent** - Analyzes portfolio composition and diversification
    3. **Advisor Agent** - Provides buy/hold/sell recommendations and key takeaways
