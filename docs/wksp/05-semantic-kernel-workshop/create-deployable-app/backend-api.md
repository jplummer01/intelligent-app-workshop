# Creating the Backend API

These changes are already available in the repository. These instructions walk
you through the process followed to create the backend API from the Console application using **Microsoft Agent Framework**:

1. Start by creating a new directory:

    ```bash
    mkdir -p workshop-agent-framework/dotnet/App/backend
    ```

1. Next create a new SDK .NET project:

    ```bash
    cd workshop-agent-framework/dotnet/App/
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

1. Add the following nuget packages for Microsoft Agent Framework:

    ```bash
    dotnet add package Microsoft.AspNetCore.Mvc
    dotnet add package Swashbuckle.AspNetCore
    dotnet add package Azure.Identity
    dotnet add package Microsoft.Agents.AI
    dotnet add package Microsoft.Extensions.AI
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
    // Add service extensions (kept for backward compatibility)
    builder.Services.AddSkServices();

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

1. Within the `Controllers` directory create a `ChatController.cs` file which exposes a reply method mapped to the `chat` path and the `HTTP POST` method using **Microsoft Agent Framework** with sequential workflow orchestration:

    ```csharp
    using Core.Utilities.Models;
    using Core.Utilities.Config;
    using Core.Utilities.Extensions;
    using Core.Utilities.Plugins;
    using Core.Utilities.Services;
    // Microsoft Agent Framework imports
    using Microsoft.Agents.AI;
    using Microsoft.Agents.AI.Workflows;
    using Microsoft.Extensions.AI;
    using Azure.Identity;
    using Microsoft.AspNetCore.Mvc;

    namespace Controllers;

    [ApiController]
    [Route("sk")]
    public class ChatController : ControllerBase {

        private readonly IChatClient _chatClient;
        private readonly AIAgent _researchAgent;
        private readonly AIAgent _riskAgent;
        private readonly AIAgent _advisorAgent;
        private readonly AIAgent _workflowAgent;
        
        public ChatController()
        {
            // Initialize chat client with Agent Framework
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

            // Agent 1: Portfolio Research Agent
            string researchAgentInstructions = """
                You are a Portfolio Research Agent. Your job is to gather comprehensive market data for stocks.
                
                For each stock symbol provided:
                - Get the current stock price
                - Search the web for recent news and market sentiment
                - Provide a brief summary of each stock's current situation
                
                Provide your complete research in a SINGLE response with clear sections for each stock.
                Format your response as a research report with stock symbols as headers.
                """;

            _researchAgent = new ChatClientAgent(
                _chatClient,
                instructions: researchAgentInstructions,
                name: "PortfolioResearchAgent",
                description: "Gathers market data and news for portfolio stocks",
                tools: [stockPriceTool, webSearchTool, timeTool]
            ).AsBuilder().Build();

            // Agent 2: Risk Assessment Agent
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

            _riskAgent = new ChatClientAgent(
                _chatClient,
                instructions: riskAgentInstructions,
                name: "RiskAssessmentAgent",
                description: "Analyzes portfolio risk and diversification"
            ).AsBuilder().Build();

            // Agent 3: Investment Advisor Agent
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

            _advisorAgent = new ChatClientAgent(
                _chatClient,
                instructions: advisorAgentInstructions,
                name: "InvestmentAdvisorAgent",
                description: "Provides investment recommendations based on research and risk analysis"
            ).AsBuilder().Build();

            // Build sequential workflow
            _workflowAgent = AgentWorkflowBuilder.BuildSequential([
                _researchAgent,
                _riskAgent,
                _advisorAgent
            ]).AsAgentAsync().Result;
        }

        [HttpPost("/chat")]
        public async Task<ChatResponse> ReplyAsync([FromBody]ChatRequest request)
        {
            var messageHistory = new List<Message>();
            
            // Initialize system message if no history
            if (request.MessageHistory.Count == 0) 
            { 
                messageHistory.Add(new Message 
                { 
                    Message1 = "You are a friendly financial advisor who provides portfolio analysis and investment recommendations.", 
                    Role = "system" 
                });
            }
            else 
            {
                messageHistory = request.MessageHistory;
            }

            // Build response message
            string fullMessage = "";
            
            if (request.InputMessage != null)
            {
                // Add user message to history
                messageHistory.Add(new Message { Message1 = request.InputMessage, Role = "user" });

                try
                {
                    // Run the workflow agent with streaming
                    await foreach (var update in _workflowAgent.RunStreamingAsync($"Analyze this portfolio of stocks: {request.InputMessage}"))
                    {
                        fullMessage += update.Text;
                    }

                    // Add assistant response to history
                    messageHistory.Add(new Message { Message1 = fullMessage, Role = "assistant" });
                }
                catch (Exception ex)
                {
                    fullMessage = $"Error analyzing portfolio: {ex.Message}";
                    messageHistory.Add(new Message { Message1 = fullMessage, Role = "assistant" });
                }
            }
                
            var chatResponse = new ChatResponse(fullMessage, messageHistory);    
            return chatResponse;
        }
    }
    ```

1. Create a `PluginInfoController.cs` file in the `Controllers` directory:

    ```csharp
    using Microsoft.AspNetCore.Mvc;
    using Core.Utilities.Models;

    namespace Controllers;

    [ApiController]
    [Route("sk")]
    public class PluginInfoController : ControllerBase {

        public PluginInfoController()
        {
        }

        /// <summary>
        /// Get the metadata for all the plugins and functions.
        /// Note: This endpoint is deprecated for Microsoft Agent Framework.
        /// MAF uses AIFunctionFactory and tools are defined per-agent.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/pluginInfo/metadata")]
        public async Task<IList<PluginFunctionMetadata>> GetPluginInfoMetadata()
        {
            // Return empty list - MAF doesn't have a global plugin registry like SK
            return new List<PluginFunctionMetadata>();
        }
    }
    ```

## Running the Backend API locally

1. To run API locally first copy valid `appsettings.json` from completed `Lessons/Lesson3` into `backend` directory:

    ```bash
    #cd into backend directory
    cd ../
    cp ../../Lessons/Lesson3/appsettings.json .
    ```

1. Next run using `dotnet run`:

    ```bash
    dotnet run
    ...
    info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
    ```

1. Application will start on specified port (port may be different). Navigate to <http://localhost:5000> or corresponding [forwarded address](https://docs.github.com/en/codespaces/developing-in-a-codespace/forwarding-ports-in-your-codespace) (if using Github CodeSpace) and it should redirect you to the swagger UI page.

1. You can test the `chat` endpoint using the "Try it out" feature from within Swagger UI, or via command line using `curl` command. Note: The API now uses Microsoft Agent Framework with sequential workflow orchestration (Research → Risk Assessment → Investment Advisor):

    ```bash
    curl -X 'POST' \
    'http://localhost:5000/chat' \
    -H 'accept: text/plain' \
    -H 'Content-Type: application/json' \
    -d '{
    "inputMessage": "MSFT, AAPL, TSLA",
    "messageHistory": [
    ]
    }'
    ```