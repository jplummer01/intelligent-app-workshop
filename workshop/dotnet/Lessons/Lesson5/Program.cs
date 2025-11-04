using Core.Utilities.Config;
using Core.Utilities.Plugins;
using Core.Utilities.Services;
using Core.Utilities.Extensions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
// TODO: Step 1 - Add OpenTelemetry imports for distributed tracing
// using OpenTelemetry;
// using OpenTelemetry.Trace;
// TODO: Step 1 - Add workflows import for multi-agent orchestration  
// using Microsoft.Agents.AI.Workflows;

// TODO: Step 2 - Create TracerProvider for OpenTelemetry observability
// This will export traces to console and OTLP (for AI Toolkit integration)
// Hint: Use Sdk.CreateTracerProviderBuilder() with "agent-telemetry-source"

// Initialize the chat client with Agent Framework  
IChatClient chatClient = AgentFrameworkProvider.CreateChatClientWithApiKey();

// Initialize plugins  
TimeInformationPlugin timePlugin = new();
HttpClient httpClient = new();
StockDataPlugin stockDataPlugin = new(new StocksService(httpClient));
HostedWebSearchTool webSearchTool = new();

// Create AI Functions from plugins
var timeTool = AIFunctionFactory.Create(timePlugin.GetCurrentUtcTime);
var stockPriceTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPrice);
var stockPriceDateTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPriceForDate);

// TODO: Step 3 - Create Portfolio Research Agent with OpenTelemetry instrumentation
// This agent will gather market data and news for portfolio stocks
string researchAgentInstructions = """
    You are a Portfolio Research Agent. Your job is to gather comprehensive market data for stocks.
    
    For each stock symbol provided:
    - Get the current stock price
    - Search the web for recent news and market sentiment
    - Provide a brief summary of each stock's current situation
    
    Provide your complete research in a SINGLE response with clear sections for each stock.
    Format your response as a research report with stock symbols as headers.
    """;

// Create research agent (for now, basic ChatClientAgent - will be enhanced with OpenTelemetry)
ChatClientAgent portfolioResearchAgent = new(
    chatClient,
    instructions: researchAgentInstructions,
    name: "PortfolioResearchAgent",
    description: "Gathers market data and news for portfolio stocks",
    tools: [stockPriceTool, webSearchTool, timeTool]
);

// TODO: Step 4 - Create Risk Assessment Agent with OpenTelemetry instrumentation
// This agent will analyze portfolio composition and risk profile
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

// Create risk assessment agent (for now, basic ChatClientAgent - will be enhanced with OpenTelemetry)
ChatClientAgent riskAssessmentAgent = new(
    chatClient,
    instructions: riskAgentInstructions,
    name: "RiskAssessmentAgent",
    description: "Analyzes portfolio risk and diversification"
);

// TODO: Step 5 - Create Investment Advisor Agent with OpenTelemetry instrumentation
// This agent will provide final recommendations based on research and risk analysis
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

// Create investment advisor agent (for now, basic ChatClientAgent - will be enhanced with OpenTelemetry)
ChatClientAgent investmentAdvisorAgent = new(
    chatClient,
    instructions: advisorAgentInstructions,
    name: "InvestmentAdvisorAgent",
    description: "Provides investment recommendations based on research and risk analysis"
);

Console.WriteLine("=== Investment Portfolio Analyzer with Sequential Orchestration ===");
Console.WriteLine("This demonstrates multi-agent orchestration for comprehensive portfolio analysis.");
Console.WriteLine("Enter stock symbols separated by commas (e.g., 'MSFT, AAPL, TSLA, NVDA')");
Console.WriteLine("Type 'quit' to exit.");
Console.WriteLine("====================================================================");
Console.WriteLine();

const string terminationPhrase = "quit";
string? userInput;

do
{
    Console.Write("🎯 Enter stock symbols (comma-separated): ");
    userInput = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(userInput) || userInput.ToLower() == terminationPhrase)
    {
        Console.WriteLine("Goodbye! 👋");
        break;
    }

    try
    {
        // Parse stock symbols
        var symbols = userInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(s => s.Trim().ToUpperInvariant())
                              .ToList();

        if (!symbols.Any())
        {
            Console.WriteLine("❌ Please enter valid stock symbols.");
            continue;
        }

        Console.WriteLine($"\n🚀 Starting analysis for: {string.Join(", ", symbols)}");
        
        // TODO: Step 6 - Create Sequential Workflow with OpenTelemetry
        // Replace the individual agent calls below with SequentialAgentWorkflow.Create()
        // This will orchestrate all three agents in sequence with full telemetry
        
        // TODO: Step 7 - Add performance monitoring with Stopwatch
        // Track total execution time for the workflow
        
        // For now, manually call each agent in sequence (will be replaced with workflow)
        var prompt = $"Analyze portfolio for stocks: {string.Join(", ", symbols)}. Provide comprehensive research, risk assessment, and investment recommendations.";
        
        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine("PORTFOLIO ANALYSIS - SEQUENTIAL ORCHESTRATION");
        Console.WriteLine(new string('=', 70) + "\n");
        
        // Step 1: Portfolio Research
        Console.WriteLine("[PortfolioResearchAgent]");
        Console.WriteLine(new string('-', 70));
        var researchResponse = await portfolioResearchAgent.RunAsync(prompt);
        Console.WriteLine(researchResponse);
        Console.WriteLine(new string('-', 70) + "\n");
        
        // Step 2: Risk Assessment  
        Console.WriteLine("[RiskAssessmentAgent]");
        Console.WriteLine(new string('-', 70));
        var riskResponse = await riskAssessmentAgent.RunAsync($"Based on this research: {researchResponse}\n\nAnalyze the portfolio risk.");
        Console.WriteLine(riskResponse);
        Console.WriteLine(new string('-', 70) + "\n");
        
        // Step 3: Investment Recommendations
        Console.WriteLine("[InvestmentAdvisorAgent]");
        Console.WriteLine(new string('-', 70));
        var advisorResponse = await investmentAdvisorAgent.RunAsync($"Based on this research: {researchResponse}\n\nAnd this risk assessment: {riskResponse}\n\nProvide investment recommendations.");
        Console.WriteLine(advisorResponse);
        Console.WriteLine(new string('-', 70) + "\n");
        
        Console.WriteLine("✓ ANALYSIS COMPLETE");
        Console.WriteLine(new string('=', 70));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error during analysis: {ex.Message}");
        Console.WriteLine("Please try again with different stock symbols.");
    }
    
    Console.WriteLine();
}
while (userInput != terminationPhrase);

Console.WriteLine("Thank you for using the Portfolio Analyzer!");
