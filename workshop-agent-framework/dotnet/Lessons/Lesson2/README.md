# Lesson 2: Conversation Memory with Agent Framework

## Overview
This lesson demonstrates how the Microsoft Agent Framework automatically handles conversation memory and context. Unlike traditional chat implementations where you manually manage chat history, Agent Framework's AgentThread automatically maintains conversation context for you.

## Learning Objectives
- Understand how AgentThread maintains conversation history automatically
- Experience the difference between stateless and stateful conversations
- Learn about the built-in conversation management in Agent Framework

## Key Concepts
- **Automatic Context Management**: Agent Framework handles conversation history behind the scenes
- **AgentThread Persistence**: The thread object maintains state across multiple interactions
- **Stateful vs Stateless**: Comparison with manual chat history management approaches

## What's Different from Lesson 1?
While the code looks very similar to Lesson 1, the key difference is understanding that:
- The `AgentThread` automatically stores all conversation history
- Each call to `agent.RunAsync()` has access to the full conversation context
- No manual management of `ChatHistory` is required

## Steps to Complete

### Step 1: Initialize the Chat Client
Same as Lesson 1 - create an `IChatClient` using `AgentFrameworkProvider.CreateChatClientWithApiKey()`.

### Step 2: Create the ChatClientAgent
Same setup as Lesson 1, but with awareness that this agent will maintain conversation context.

### Step 3: Create the AgentThread
Use `agent.GetNewThread()` - this thread will automatically track all conversation history.

### Step 4: Process Conversations with Memory
Use `agent.RunAsync(userInput, thread)` and observe how the agent remembers previous exchanges.

## Testing Conversation Memory
Try these conversation patterns to see memory in action:

1. **Reference Test**: 
   - "What's a good investment strategy?"
   - "Why did you recommend that?" (tests if agent remembers the previous recommendation)

2. **Context Building**:
   - "I'm 25 years old"
   - "I have $10,000 to invest"
   - "What should I do?" (tests if agent remembers your age and amount)

3. **Follow-up Questions**:
   - Ask about stocks
   - "What about the risks?" (tests contextual understanding)

## Expected Behavior
The agent should demonstrate clear memory of previous exchanges, referencing earlier parts of the conversation and building upon established context. This happens automatically without any additional code for history management.