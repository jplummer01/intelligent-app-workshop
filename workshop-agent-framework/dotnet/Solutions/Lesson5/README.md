# Lesson 7: Multi-Agent Sequential Orchestration - Portfolio Analyzer ✅

## Overview
This lesson demonstrates **sequential multi-agent orchestration** with Microsoft Agent Framework using the built-in `AgentWorkflowBuilder.BuildSequential()` API. Three specialized agents work together in a pipeline to analyze an investment portfolio.

## The Orchestration Pattern

### Three Specialized Agents

1. **Portfolio Research Agent**
   - Gathers market data and news for each stock
   - Uses web search and stock price tools
   - Outputs: Research report with current prices and sentiment

2. **Risk Assessment Agent**
   - Analyzes portfolio composition and diversification
   - Identifies sector concentration and risk factors
   - Outputs: Risk score (1-10) and concern areas

3. **Investment Advisor Agent**
   - Synthesizes research and risk analysis
   - Provides actionable recommendations
   - Outputs: Buy/hold/sell recommendations and rebalancing suggestions

### Orchestration Flow

```
User Input: "MSFT, AAPL, TSLA, NVDA"
         ↓
    Step 1: Research Agent
         ↓ (passes research data)
    Step 2: Risk Agent
         ↓ (passes risk analysis)
    Step 3: Advisor Agent
         ↓
    Final Output: Complete portfolio analysis
```

## Key Implementation Concepts

### Built-in Sequential Orchestration

```csharp
using Microsoft.Agents.AI.Workflows;

// Build the sequential workflow - agents execute in order
var workflow = AgentWorkflowBuilder.BuildSequential([
    researchAgent,
    riskAgent,
    advisorAgent
]);

// Run the workflow
var messages = new List<ChatMessage> { 
    new(ChatRole.User, "Analyze this portfolio: MSFT, AAPL") 
};
StreamingRun run = await InProcessExecution.StreamAsync(workflow, messages);
await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

// Process events
await foreach (WorkflowEvent evt in run.WatchStreamAsync())
{
    if (evt is AgentRunUpdateEvent updateEvent)
    {
        Console.WriteLine($"[{updateEvent.ExecutorId}]: {updateEvent.Data}");
    }
    else if (evt is WorkflowCompletedEvent completed)
    {
        var result = (List<ChatMessage>)completed.Data!;
        // Display final conversation
    }
}
```

### Agent Specialization

Each agent has:
- **Specific instructions** defining its role
- **Relevant tools** for its task (or none if it synthesizes)
- **Clear input/output contract**

### Data Flow

```csharp
// Research Agent → produces research data
string researchResults = researchResponse?.Messages?.Last()?.Text;

// Risk Agent → consumes research, produces risk analysis
var riskResponse = await riskAgent.RunAsync(
    $"Portfolio: {portfolioStocks}\nResearch:\n{researchResults}"
);

// Advisor Agent → consumes both, produces recommendations
var advisorResponse = await advisorAgent.RunAsync(
    $"Research:\n{researchResults}\nRisk:\n{riskAnalysis}"
);
```

## Why This Orchestration Pattern?

✅ **Separation of Concerns**: Each agent has one clear responsibility  
✅ **Modularity**: Easy to add/remove/replace agents  
✅ **Transparency**: User sees each agent's contribution  
✅ **Composability**: Output of one agent feeds into the next  
✅ **Testability**: Each agent can be tested independently  

## Running the Sample

```bash
cd Solutions/Lesson7
dotnet run
```

Example interaction:
```
Enter portfolio > MSFT, AAPL, TSLA, NVDA

[ORCHESTRATION EVENTS]
[PortfolioResearchAgent]: Researching stocks...
[RiskAssessmentAgent]: Analyzing risk profile...
[InvestmentAdvisorAgent]: Generating recommendations...

[FINAL CONVERSATION]
User: Analyze this portfolio of stocks: MSFT, AAPL, TSLA, NVDA
PortfolioResearchAgent: [Research results with prices and sentiment]
RiskAssessmentAgent: [Risk analysis with concentration score]
InvestmentAdvisorAgent: [Investment recommendations]
```

## Advanced Orchestration Patterns

This lesson uses **sequential orchestration** (Agent 1 → Agent 2 → Agent 3).

Other patterns you could explore:
- **Parallel Orchestration**: Multiple agents work simultaneously
- **Conditional Orchestration**: Route to different agents based on results
- **Iterative Orchestration**: Agents refine each other's work in loops
- **Hierarchical Orchestration**: Manager agent delegates to worker agents

## Learning Outcomes

- Understand how to coordinate multiple agents in a workflow
- Learn to pass data between agents effectively
- See the value of agent specialization vs. single generalist agent
- Experience building complex AI workflows with simple patterns
- Understand when to use orchestration vs. single-agent approaches

## When to Use Multi-Agent Orchestration

**Use orchestration when:**
- Tasks have distinct, separable concerns
- Different agents need different tools/capabilities
- You want transparency into each step
- Results build upon previous analysis

**Use single agent when:**
- Task is straightforward and unified
- All tools can be provided to one agent
- Simplicity is more important than modularity

## Comparison to Lesson 6

| Aspect | Lesson 6 | Lesson 7 |
|--------|----------|----------|
| Agents | 1 (Financial Analysis) | 3 (Research, Risk, Advisor) |
| Pattern | Single agent with all tools | Sequential orchestration |
| Input | Free-form question | Portfolio list |
| Output | Direct answer | Multi-step analysis |
| Best For | General queries | Structured workflows |

## Resources

- [Microsoft Agent Framework Docs](https://github.com/microsoft/agent-framework)
- [Multi-Agent Patterns](https://learn.microsoft.com/azure/ai-studio/concepts/agent-patterns)
- [Agent Orchestration Best Practices](https://devblogs.microsoft.com/dotnet/introducing-microsoft-agent-framework-preview/)
