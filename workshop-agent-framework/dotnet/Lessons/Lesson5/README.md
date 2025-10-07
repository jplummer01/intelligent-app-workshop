# Lesson 6: Agent Orchestration with Sequential Workflows

## Overview
This lesson introduces agent orchestration using Microsoft Agent Framework workflows. You'll learn to create multiple specialized agents and coordinate them in a sequential workflow to perform complex analysis tasks. This demonstrates how to build sophisticated multi-agent systems for enterprise applications.

## Learning Objectives
- Understand agent orchestration and workflow concepts
- Create multiple specialized agents with distinct roles
- Use AgentWorkflowBuilder for sequential orchestration
- Handle multi-agent streaming responses
- Build production-ready multi-agent systems

## Key Concepts
- **Agent Orchestration**: Coordinating multiple agents to work together
- **Sequential Workflows**: Agents process information in a specific order
- **Specialized Agents**: Each agent has a focused, specific responsibility
- **Workflow-as-Agent**: Converting workflows into single callable agents
- **Multi-Agent Streaming**: Managing real-time responses from multiple agents

## Architecture

### Three-Agent Portfolio Analysis System

1. **Portfolio Research Agent**
   - Gathers market data for all stocks
   - Retrieves current prices and web sentiment
   - Provides comprehensive research reports

2. **Risk Assessment Agent**
   - Analyzes portfolio composition
   - Calculates risk scores and diversification metrics
   - Identifies concentration concerns

3. **Investment Advisor Agent**
   - Synthesizes research and risk analysis
   - Provides actionable recommendations
   - Suggests specific buy/hold/sell actions

## Steps to Complete

### Step 1: Initialize Chat Client
Same as previous lessons.

### Step 2: Initialize Plugins
Set up TimeInformationPlugin, StockDataPlugin, and HostedWebSearchTool.

### Step 3: Create AI Functions
Convert plugin methods to AIFunction objects.

### Step 4: Create Portfolio Research Agent
Design instructions for comprehensive market data gathering:
- Stock price collection
- Web sentiment analysis
- Structured research reporting

### Step 5: Create Risk Assessment Agent
Design instructions for portfolio risk analysis:
- Sector concentration analysis
- Risk scoring (1-10 scale)
- Diversification assessment

### Step 6: Create Investment Advisor Agent
Design instructions for recommendation synthesis:
- Portfolio health scoring
- Buy/hold/sell recommendations
- Rebalancing suggestions

### Step 7: Build Sequential Workflow
Use `AgentWorkflowBuilder.BuildSequential()` to create an orchestrated workflow and convert it to a single callable agent.

### Step 8: Stream Multi-Agent Responses
Implement streaming that shows which agent is responding and displays their output in real-time.

## Workflow Pattern

```csharp
// Create sequential workflow
AIAgent workflowAgent = await AgentWorkflowBuilder.BuildSequential([
    researchAgent,
    riskAgent, 
    advisorAgent
]).AsAgentAsync();

// Stream responses with agent identification
await foreach (var update in workflowAgent.RunStreamingAsync(input))
{
    // Track which agent is responding
    // Display formatted output
}
```

## Testing the Multi-Agent System

### Portfolio Examples
- "MSFT, AAPL, TSLA, NVDA" (tech-heavy portfolio)
- "MSFT, JPM, PG, XOM, VZ" (diversified portfolio)
- "TSLA, RIVN, NIO, LCID" (EV-focused portfolio)

### Expected Flow
1. **Research Agent**: Gathers data on each stock
2. **Risk Agent**: Analyzes the portfolio composition
3. **Advisor Agent**: Provides final recommendations

## Advanced Features
- **Agent Identification**: Track which agent is responding
- **Formatted Output**: Professional presentation of results
- **Error Handling**: Robust operation across multiple agents
- **Streaming Coordination**: Real-time multi-agent responses

## Expected Behavior
You should see a clear progression through three distinct analysis phases, with each agent building upon the previous agent's work to provide comprehensive portfolio analysis. The system demonstrates how complex tasks can be broken down into specialized components that work together seamlessly.