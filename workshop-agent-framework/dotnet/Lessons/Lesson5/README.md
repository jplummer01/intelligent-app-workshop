# Lesson 5: Multi-Agent Orchestration with OpenTelemetry

## Overview
This lesson combines multi-agent orchestration with comprehensive OpenTelemetry tracing. You'll learn to create specialized agents that work together in a sequential workflow while collecting detailed telemetry data for monitoring and observability. This demonstrates how to build production-ready multi-agent systems with enterprise-grade monitoring capabilities.

## Learning Objectives
- Create multiple specialized agents with distinct roles
- Use Sequential Workflows for agent orchestration
- Implement OpenTelemetry tracing with Microsoft Agent Framework
- Configure AI Toolkit for VS Code tracing visualization
- Handle multi-agent streaming responses with telemetry
- Build observable multi-agent systems for production

## Key Concepts
- **Agent Orchestration**: Coordinating multiple agents to work together
- **Sequential Workflows**: Agents process information in a specific order
- **OpenTelemetry Integration**: Built-in telemetry for AI agent applications
- **Specialized Agents**: Each agent has a focused, specific responsibility
- **Workflow-as-Agent**: Converting workflows into single callable agents
- **AI Toolkit Tracing**: VS Code extension for trace visualization

## Prerequisites
- Completed Lesson 4 (Basic Agent Framework Usage)
- Understanding of observability concepts (traces, metrics, logs)
- Visual Studio Code with AI Toolkit extension installed

## Setup Instructions

### 1. Install AI Toolkit for VS Code
1. Open Visual Studio Code
2. Go to Extensions (Ctrl+Shift+X)
3. Search for "AI Toolkit"
4. Install the **AI Toolkit** extension by Microsoft
5. Restart VS Code if needed

### 2. Configure AI Toolkit Tracing
1. Open the **AI Toolkit** panel in VS Code
2. Navigate to the **Tracing** section
3. Click **Start Collector** to start the local OTLP trace collector
4. The collector will start listening on `http://localhost:4317` (gRPC) and `http://localhost:4318` (HTTP)

## Architecture

### Three-Agent Portfolio Analysis System with OpenTelemetry

1. **Portfolio Research Agent** (with OpenTelemetry instrumentation)
   - Gathers market data for all stocks
   - Retrieves current prices and web sentiment
   - Provides comprehensive research reports
   - Traces all data collection operations

2. **Risk Assessment Agent** (with OpenTelemetry instrumentation)
   - Analyzes portfolio composition
   - Calculates risk scores and diversification metrics
   - Identifies concentration concerns
   - Traces risk calculation processes

3. **Investment Advisor Agent** (with OpenTelemetry instrumentation)
   - Synthesizes research and risk analysis
   - Provides actionable recommendations
   - Suggests specific buy/hold/sell actions
   - Traces recommendation generation

## Steps to Complete

### Step 1: Configure OpenTelemetry TracerProvider
Set up OpenTelemetry with console and OTLP exporters:
```csharp
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("agent-telemetry-source")
    .AddConsoleExporter()
    .AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://localhost:4317");
        options.Protocol = OtlpExportProtocol.Grpc;
    })
    .Build();
```

### Step 2: Initialize Chat Client with OpenTelemetry
Create the chat client and instrument it for telemetry.

### Step 3: Initialize Plugins
Set up TimeInformationPlugin, StockDataPlugin, and HostedWebSearchTool.

### Step 4: Create AI Functions
Convert plugin methods to AIFunction objects.

### Step 5: Create Portfolio Research Agent with OpenTelemetry
Design instructions for comprehensive market data gathering with telemetry:
- Stock price collection (instrumented)
- Web sentiment analysis (traced)
- Structured research reporting
- Use `.AsBuilder().UseOpenTelemetry().Build()` for instrumentation

### Step 6: Create Risk Assessment Agent with OpenTelemetry
Design instructions for portfolio risk analysis with telemetry:
- Sector concentration analysis (traced)
- Risk scoring (1-10 scale) with performance metrics
- Diversification assessment
- Use `.AsBuilder().UseOpenTelemetry().Build()` for instrumentation

### Step 7: Create Investment Advisor Agent with OpenTelemetry
Design instructions for recommendation synthesis with telemetry:
- Portfolio health scoring (traced)
- Buy/hold/sell recommendations with timing
- Rebalancing suggestions
- Use `.AsBuilder().UseOpenTelemetry().Build()` for instrumentation

### Step 8: Build Sequential Workflow with Telemetry
Use `SequentialAgentWorkflow.Create()` to create an orchestrated workflow with full tracing.

### Step 9: Execute Workflow with Performance Monitoring
Implement execution with stopwatch timing and comprehensive telemetry output.

## Workflow Pattern with OpenTelemetry

```csharp
// Configure OpenTelemetry TracerProvider
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("agent-telemetry-source")
    .AddConsoleExporter()
    .AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://localhost:4317");
        options.Protocol = OtlpExportProtocol.Grpc;
    })
    .Build();

// Create instrumented agents
var researchAgent = chatClient.AsBuilder().UseOpenTelemetry().Build();
var riskAgent = chatClient.AsBuilder().UseOpenTelemetry().Build();
var advisorAgent = chatClient.AsBuilder().UseOpenTelemetry().Build();

// Create sequential workflow
var workflow = SequentialAgentWorkflow.Create(researchAgent, riskAgent, advisorAgent);

// Execute with timing
var stopwatch = Stopwatch.StartNew();
var response = await workflow.InvokeAsync(prompt);
stopwatch.Stop();
```

## Testing the Multi-Agent System with OpenTelemetry

### Portfolio Examples
- "MSFT, AAPL, TSLA, NVDA" (tech-heavy portfolio)
- "MSFT, JPM, PG, XOM, VZ" (diversified portfolio)
- "TSLA, RIVN, NIO, LCID" (EV-focused portfolio)

### Expected Flow with Telemetry
1. **Research Agent**: Gathers data on each stock (traced operations)
2. **Risk Agent**: Analyzes the portfolio composition (performance metrics)
3. **Advisor Agent**: Provides final recommendations (timing data)

### Viewing Telemetry Data
- **Console Output**: See traces directly in terminal
- **AI Toolkit**: Open tracing view in VS Code
- **Performance Metrics**: Execution timing displayed
- **OTLP Export**: Data sent to localhost:4317 for external tools

## Advanced Features
- **OpenTelemetry Integration**: Full distributed tracing across agents
- **AI Toolkit Visualization**: Built-in VS Code trace viewing
- **Performance Monitoring**: Real-time execution timing
- **Agent Identification**: Track which agent is responding in traces
- **Formatted Output**: Professional presentation of results
- **Error Handling**: Robust operation across multiple agents with error tracing
- **OTLP Export**: Compatible with external observability tools

## Expected Behavior
You should see a clear progression through three distinct analysis phases, with each agent building upon the previous agent's work to provide comprehensive portfolio analysis. Additionally, you'll observe detailed telemetry data showing:
- Agent execution timing
- Request/response traces
- Performance metrics
- Error tracking (if any)
- Complete workflow orchestration visibility

The system demonstrates how complex tasks can be broken down into specialized components that work together seamlessly while maintaining full observability for production monitoring.