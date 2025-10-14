# Lesson 5: Multi-Agent Orchestration with OpenTelemetry

## Overview
This lesson combines multi-agent orchestration with comprehensive OpenTelemetry tracing. You'll learn to create specialized agents that work together in a sequential workflow while collecting detailed telemetry data for monitoring and observability. This demonstrates how to build production-ready multi-agent systems with enterprise-grade monitoring capabilities.

## Learning Objectives
- Create multiple specialized agents with distinct roles
- Use AgentWorkflowBuilder for sequential orchestration
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

### 3. Run the Application
```bash
cd Lessons/Lesson5
dotnet run
```

### 4. View Traces in AI Toolkit
1. Run the portfolio analysis application
2. Generate some traces by entering stock symbols (e.g., "MSFT, AAPL, NVDA")
3. In AI Toolkit, click **Refresh** in the Tracing section
4. Select a trace to view detailed execution flow
5. Explore the span tree and AI message flows

## Architecture

The application demonstrates a sequential multi-agent workflow:

```
User Input → Research Agent → Risk Assessment Agent → Investment Advisor Agent → Final Report
     ↓              ↓                    ↓                      ↓              ↓
  Telemetry    OpenTelemetry        OpenTelemetry          OpenTelemetry   Telemetry
    Data         Traces               Traces                 Traces         Export
```

### Agent Specialization
1. **Portfolio Research Agent**: Gathers market data and news for stocks
2. **Risk Assessment Agent**: Analyzes portfolio composition and risk profile  
3. **Investment Advisor Agent**: Provides actionable investment recommendations

### Telemetry Features
- **Console Output**: Immediate trace visibility during development
- **OTLP Export**: Integration with observability platforms
- **AI Toolkit Integration**: Rich trace visualization in VS Code
- **Agent Instrumentation**: Automatic tracing of agent interactions
- **Performance Monitoring**: Execution timing and token usage tracking

## TODO Implementation Steps

### TODO 1: Add OpenTelemetry Packages
Update your `Lesson5.csproj` to include OpenTelemetry packages:

```xml
<ItemGroup>
  <PackageReference Include="OpenTelemetry" />
  <PackageReference Include="OpenTelemetry.Exporter.Console" />
  <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" />
</ItemGroup>
```

### TODO 2: Add OpenTelemetry Imports
Add the necessary using statements:

```csharp
using OpenTelemetry;
using OpenTelemetry.Trace;
```

### TODO 3: Configure TracerProvider
Set up OpenTelemetry with AI Toolkit compatibility:

```csharp
// Create TracerProvider that exports to console and OTLP
// Following Python Agent Framework pattern: uses gRPC on port 4317
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("agent-telemetry-source")
    .AddConsoleExporter()
    .AddOtlpExporter(options =>
    {
        // Primary: gRPC on 4317 (matches Python Agent Framework and AI Toolkit)
        options.Endpoint = new Uri("http://localhost:4317");
        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
    })
    .Build();
```

### TODO 4: Instrument Agents with OpenTelemetry
Update each agent to use OpenTelemetry instrumentation:

```csharp
// Portfolio Research Agent with OpenTelemetry
ChatClientAgent portfolioResearchAgent = chatClient
    .AsBuilder()
    .UseOpenTelemetry()
    .Build();

// Create the agent with instructions and tools
portfolioResearchAgent = new ChatClientAgent(
    portfolioResearchAgent,
    instructions: researchAgentInstructions,
    name: "PortfolioResearchAgent",
    description: "Gathers market data and news for portfolio stocks",
    tools: [stockPriceTool, webSearchTool, timeTool]
);
```

**Note**: The instrumentation is applied to the base chat client, then wrapped with agent-specific configuration.

### TODO 5: Create Sequential Workflow
Create the workflow using SequentialAgentWorkflow:

```csharp
// Create sequential workflow with all three agents
var workflow = SequentialAgentWorkflow.Create(
    portfolioResearchAgent,
    riskAssessmentAgent,
    investmentAdvisorAgent
);
```

**Note**: `SequentialAgentWorkflow.Create()` automatically handles the orchestration and maintains telemetry context across agents.

### TODO 6: Execute Workflow with Performance Monitoring
Execute the workflow with timing:

```csharp
// Start timing for performance measurement
var stopwatch = System.Diagnostics.Stopwatch.StartNew();

// Execute workflow with appropriate prompt
var prompt = $"Analyze portfolio for stocks: {string.Join(", ", symbols)}. Provide comprehensive research, risk assessment, and investment recommendations.";

// Execute workflow and get response
var response = await workflow.InvokeAsync(prompt);

stopwatch.Stop();

// Display results with timing information
Console.WriteLine($"\n✅ Analysis completed in {stopwatch.ElapsedMilliseconds}ms");
Console.WriteLine($"📊 Final Investment Analysis:\n{response}");
Console.WriteLine("🔍 Check console output above for OpenTelemetry traces");
Console.WriteLine("💡 Open AI Toolkit in VS Code to visualize traces");
```

## Testing Your Implementation

### 1. Start AI Toolkit Collector
- Open AI Toolkit in VS Code
- Navigate to Tracing section
- Click "Start Collector"
- Verify collector is running on localhost:4317

### 2. Run the Application
```bash
dotnet run
```

### 3. Generate Traces
- Enter stock symbols: `MSFT, AAPL, NVDA`
- Observe console output with embedded trace data
- Watch for OpenTelemetry activity information

### 4. View in AI Toolkit
- In AI Toolkit, click "Refresh" in Tracing section
- Select a trace from the list
- Explore the span tree showing all three agents
- Check Input + Output tab for AI message flows
- Review Metadata tab for detailed trace information

### 5. Analyze Trace Data
Look for these trace elements:
- **invoke_agent spans**: One for each agent (Research, Risk, Advisor)
- **Parent-child relationships**: Workflow → Individual agents
- **Timing information**: Duration of each agent's execution
- **AI metadata**: Model calls, token usage, response IDs
- **Custom tags**: Portfolio symbols, agent names, operation types

## Key Features Demonstrated

### Multi-Agent Coordination
- **Sequential Processing**: Each agent builds on the previous agent's output
- **Specialized Roles**: Clear separation of concerns and responsibilities
- **Streaming Responses**: Real-time output from multi-agent workflows
- **Error Handling**: Graceful handling of agent failures

### OpenTelemetry Integration
- **Automatic Instrumentation**: Built-in tracing for Microsoft Agent Framework
- **Rich Metadata**: AI-specific trace data including token usage and model information
- **Multiple Exporters**: Console for development, OTLP for production
- **Correlation**: End-to-end tracing across the entire workflow

### Production Readiness
- **Observability**: Comprehensive monitoring and debugging capabilities
- **Performance Tracking**: Detailed timing and resource usage information
- **Scalability**: Patterns that work for enterprise applications
- **Integration**: Works with any OTLP-compatible observability platform

## Expected Output

The application will show:

1. **Console Traces**: Detailed OpenTelemetry activity information for each agent
2. **Agent Responses**: Structured output from each specialized agent
3. **Performance Metrics**: Execution timing and completion status
4. **AI Toolkit Visualization**: Rich trace exploration in VS Code

Example trace structure:
```
📊 Portfolio Analysis Workflow
├── 🔍 PortfolioResearchAgent (8.2s)
│   ├── gen_ai.operation.name: chat
│   ├── gen_ai.usage.input_tokens: 404
│   └── gen_ai.usage.output_tokens: 656
├── ⚖️ RiskAssessmentAgent (5.0s)
│   ├── gen_ai.operation.name: chat
│   └── gen_ai.usage.output_tokens: 485
└── 💼 InvestmentAdvisorAgent (4.1s)
    ├── gen_ai.operation.name: chat
    └── gen_ai.usage.output_tokens: 382
```

## Troubleshooting

### AI Toolkit Issues
- **Collector not starting**: Restart VS Code and try again
- **Port conflicts**: Check if ports 4317/4318 are already in use
- **Extension not visible**: Ensure AI Toolkit extension is installed and enabled

### Trace Export Issues
- **No traces in AI Toolkit**: Verify collector is running and refresh the view
- **Console traces only**: Check OTLP endpoint configuration and network connectivity
- **Performance degradation**: Adjust trace sampling if needed for high-volume applications

### Application Issues
- **Build errors**: Ensure all OpenTelemetry packages are restored (`dotnet restore`)
- **Agent failures**: Check API keys and network connectivity
- **Streaming issues**: Verify agent configuration and error handling

## Next Steps

- Experiment with different agent configurations and instructions
- Try connecting to other observability platforms (Jaeger, DataDog, Azure Monitor)
- Add custom metrics and logs to complement tracing data
- Explore advanced workflow patterns (parallel, conditional, etc.)
- Implement error recovery and retry logic with telemetry

## Additional Resources

- [AI Toolkit VS Code Extension](https://marketplace.visualstudio.com/items?itemName=ms-ai-toolkit.vscode-ai-toolkit)
- [OpenTelemetry .NET Documentation](https://opentelemetry.io/docs/languages/net/)
- [Microsoft Agent Framework Documentation](https://learn.microsoft.com/en-us/agent-framework/)
- [VS Code AI Toolkit Tracing Guide](https://code.visualstudio.com/docs/intelligentapps/tracing)