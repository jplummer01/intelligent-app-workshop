using Core.Utilities.Config;
using Core.Utilities.Plugins;
using Core.Utilities.Services;
using Core.Utilities.Extensions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI.Workflows;

// Initialize the chat client with Agent Framework  
IChatClient chatClient = AgentFrameworkProvider.CreateChatClientWithApiKey();

// Initialize plugins
TimeInformationPlugin timePlugin = new();
HttpClient httpClient = new();
StockDataPlugin stockDataPlugin = new(new StocksService(httpClient));

// Create web search tool for enhanced sentiment analysis
HostedWebSearchTool webSearchTool = new();

// Create AI Functions from plugins
var timeTool = AIFunctionFactory.Create(timePlugin.GetCurrentUtcTime);
var stockPriceTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPrice);
var stockPriceDateTool = AIFunctionFactory.Create(stockDataPlugin.GetStockPriceForDate);

// Financial Analysis Agent system instructions with comprehensive capabilities
string financialAnalysisInstructions = """
    You are a Financial Analysis Agent with web search capabilities. Provide direct, comprehensive financial analysis and insights based on user questions.

    CAPABILITIES:
    - Analyze individual stocks, market sectors, or broader financial topics
    - Extract stock symbols from user queries when relevant (e.g., "What do you think about Microsoft?" -> analyze MSFT)
    - Handle free-form questions about market trends, economic conditions, investment strategies
    - Use stock sentiment scale from 1 to 10 where sentiment is 1 for sell and 10 for buy (when analyzing specific stocks)
    - Provide ratings, recommendations (buy/hold/sell), and detailed reasoning for stock-specific queries

    CRITICAL RULES:
    - Provide your complete analysis in a SINGLE response - do not say you're "gathering data" or "working on it"
    - For stock-specific questions: Use web search to gather current market news, analyst opinions, and sentiment data
    - For general financial questions: Use web search to find relevant financial news, economic data, and expert analysis
    - Combine web search results with available stock price data when analyzing specific companies
    - ALWAYS include a dedicated "Sources" section at the end of your response listing all the specific sources you found through web search
    - For each source, include the title, URL (if available), and a brief description of what information it provided
    - Focus on recent news, market trends, and expert analysis
    - Be transparent about which information came from which sources
    - If a user asks about a specific company without mentioning the stock symbol, try to identify the relevant ticker symbol
    - Answer immediately with your full analysis - do not provide status updates or say you're collecting information
    """;

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

// Risk Assessment Agent - Analyzes portfolio risk
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

// Investment Advisor Agent - Provides recommendations
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

// Build the workflow and convert it to an agent
AIAgent workflowAgent = await AgentWorkflowBuilder.BuildSequential([
    researchAgent,
    riskAgent,
    advisorAgent
]).AsAgentAsync();

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