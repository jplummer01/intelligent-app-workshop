# Lesson 6 - Agent Framework with Bing Grounding (Work in Progress)

## Original Semantic Kernel Lesson 6
The original lesson demonstrates:
- Azure AI Agent Service integration via `Microsoft.SemanticKernel.Agents.AzureAI`
- Bing grounding for real-time web search
- Agent threads for conversation management
- Access to current news and analyst ratings

## Microsoft Agent Framework and Bing Grounding

According to the [Microsoft Agent Framework blog post](https://devblogs.microsoft.com/dotnet/introducing-microsoft-agent-framework-preview/):

> "Because Microsoft Agent Framework builds on Microsoft.Extensions.AI, your agents can use more robust tools including:
> - Model Context Protocol (MCP) servers
> - **Hosted tools – Access server-side tools like Code Interpreter, Bing Grounding, and many more**"

### Current Status

The `Microsoft.Agents.AI` package (preview) **DOES support Bing grounding**, but:

1. **Documentation Gap**: The blog post mentions Bing grounding as a hosted tool but doesn't provide specific code examples
2. **Package Conflicts**: `Microsoft.Agents.AI` brings dependencies that conflict with the current `Microsoft.Extensions.AI` setup
3. **Azure Foundry Integration**: Bing grounding appears to require Azure AI Foundry agent hosting, not just local chat completion

### What We Know

- ✅ Bing grounding IS available in Microsoft Agent Framework
- ✅ It works as a "hosted tool" (server-side capability)
- ❓ Exact API usage not yet documented in preview
- ❓ May require Azure AI Foundry deployment (not localhost)

## Current Implementation

This version uses the standard Agent Framework pattern WITHOUT Bing grounding:
- ✅ Chat completion with `Microsoft.Extensions.AI`
- ✅ Function calling with stock price and time plugins
- ✅ System prompt for stock sentiment agent behavior
- ✅ Technical analysis based on stock price data only
- ❌ No Bing search / web grounding (yet)

## Next Steps

To add Bing grounding to this lesson, we need:

1. **Find working examples** of Bing grounding with `Microsoft.Agents.AI`
2. **Resolve package conflicts** between Agent Framework components
3. **Understand deployment model** - does it require Azure AI Foundry hosting?
4. **Document the API** for creating Bing grounding tools

## Recommendation

**For Now:**
- Use **Semantic Kernel Lesson 6** for production Bing-grounded agents
- Use **Agent Framework Lessons 1-5** for learning new patterns
- Watch for **Microsoft Agent Framework documentation updates** on hosted tools

**Future:**
- Once Bing grounding API is documented, update this lesson
- Microsoft Agent Framework preview is evolving rapidly
- Check [Agent Framework GitHub](https://github.com/microsoft/agent-framework) for latest examples

## Resources

- [Microsoft Agent Framework Blog](https://devblogs.microsoft.com/dotnet/introducing-microsoft-agent-framework-preview/)
- [Agent Framework GitHub](https://github.com/microsoft/agent-framework)
- [Microsoft.Extensions.AI Docs](https://learn.microsoft.com/dotnet/ai/)
- [Semantic Kernel Agents](https://learn.microsoft.com/semantic-kernel/frameworks/agent/)
