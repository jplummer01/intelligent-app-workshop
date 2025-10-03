# Lesson 6: Bing Grounding with Agent Framework ✅

## Overview
This lesson demonstrates how to use **HostedWebSearchTool** (Bing grounding) with Microsoft Agent Framework to create an intelligent stock sentiment agent that can search the web for real-time information.

## Key Concepts

### HostedWebSearchTool
The `HostedWebSearchTool` class from the `Microsoft.Agents.AI` namespace provides built-in web search capabilities powered by Bing:

```csharp
using Microsoft.Agents.AI;

new HostedWebSearchTool()  // Bing grounding for web search
```

### ChatClientAgent with Web Search
Combine AI agents with web search and custom functions:

```csharp
var agent = new ChatClientAgent(chatClient, new ChatClientAgentOptions(
    instructions: "You are a stock analyst with web search capabilities...",
    name: "StockSentimentAgent",
    description: "Analyzes stock sentiment using web search and stock price data.")
{
    ChatOptions = new ChatOptions
    {
        Tools = [
            AIFunctionFactory.Create(timePlugin.GetCurrentUtcTime),
            AIFunctionFactory.Create(stockDataPlugin.GetStockPrice),
            new HostedWebSearchTool()  // Web search capability
        ]
    }
});
```

## Migration from Semantic Kernel

### Semantic Kernel Approach
```csharp
// SK uses Azure AI Agent Service with Azure.AI.Agents.Persistent
var agentMetadata = await assistantsClient.Administration.CreateAgentAsync(
    model: model,
    name: "StockSentimentAgent",
    instructions: instructions,
    tools: [new BingGroundingToolDefinition()]);
```

### Agent Framework Approach
```csharp
// Agent Framework uses HostedWebSearchTool from Microsoft.Agents.AI
var agent = new ChatClientAgent(chatClient, new ChatClientAgentOptions(
    instructions: instructions,
    name: "StockSentimentAgent")
{
    ChatOptions = new ChatOptions
    {
        Tools = [
            new HostedWebSearchTool()  // Direct web search integration
        ]
    }
});
```

## Features Demonstrated

1. **Web Search Integration**: Real-time web search using Bing
2. **Multi-Tool Orchestration**: Combines web search with custom functions
3. **Agent Threads**: Maintains conversation context across interactions
4. **Sentiment Analysis**: Analyzes stock sentiment based on web research

## Technical Details

### Hosted Tools in Agent Framework
The Agent Framework provides several hosted tools:
- `HostedWebSearchTool`: Bing-powered web search
- `HostedCodeInterpreterTool`: Code execution
- `HostedFileSearchTool`: Document search
- `HostedMCPTool`: Model Context Protocol servers

### Usage Pattern
```csharp
// Add to tools collection
new HostedWebSearchTool()
```

The tool automatically integrates with the chat client's function calling mechanism to provide web search results when needed.

## Running the Sample

```bash
cd Solutions/Lesson6
dotnet run
```

Example interaction:
```
User > Analyze MSFT stock
Assistant > [Searches web for MSFT news/analysis]
Based on current market data showing MSFT at $515.74 and recent news 
from [source], the sentiment rating is 8/10 - BUY. 
Reasoning: Strong Q4 earnings, positive cloud growth trends...
```

## Configuration Requirements

### Azure OpenAI Setup
Ensure your `appsettings.json` has valid Azure OpenAI credentials:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "DeploymentName": "gpt-4o-mini",
    "ApiKey": "your-key"
  }
}
```

### Bing Connection (Optional)
For enhanced Bing grounding with connection-specific configuration, you may need to set up a Bing resource in Azure AI Foundry. The basic `HostedWebSearchTool()` works with default settings.

## Discovery Process

The implementation of `HostedWebSearchTool` was discovered through:
1. Blog post confirming Bing grounding availability in Agent Framework
2. Python examples showing `HostedWebSearchTool` usage pattern
3. GitHub repository search finding .NET example in [05_MultiModelService/Program.cs](https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/GettingStarted/Workflows/_Foundational/05_MultiModelService/Program.cs)

### Key Finding
Line 47 of the GitHub example shows:
```csharp
AIAgent factChecker = new ChatClientAgent(openai,
    instructions: "Fact-checks reliable sources and flags inaccuracies.",
    name: "fact_checker",
    description: "...",
    [new HostedWebSearchTool()]);  // ✅ WORKS IN .NET!
```

## Learning Outcomes
- Understand how to integrate web search into Agent Framework agents
- Learn the pattern for using hosted tools (`new HostedWebSearchTool()`)
- See how to combine multiple tool types (custom functions + hosted tools)
- Experience multi-agent orchestration with web-grounded responses

## Resources

- [Microsoft Agent Framework Blog](https://devblogs.microsoft.com/dotnet/introducing-microsoft-agent-framework-preview/)
- [Agent Framework GitHub](https://github.com/microsoft/agent-framework)
- [05_MultiModelService Example](https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/GettingStarted/Workflows/_Foundational/05_MultiModelService/Program.cs) - Source of HostedWebSearchTool discovery
- [Microsoft.Extensions.AI Docs](https://learn.microsoft.com/dotnet/ai/)
