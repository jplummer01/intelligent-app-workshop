# Lesson 1: Getting Started with Microsoft Agent Framework

## Overview
This lesson introduces you to the Microsoft Agent Framework, showing you how to create a simple AI agent that can have conversations with users. You'll build a financial advisor agent that provides advice in a creative and funny tone.

## Learning Objectives
- Initialize a chat client using the Agent Framework
- Create and configure a ChatClientAgent
- Handle conversation threads
- Process user input and generate responses

## Key Concepts
- **IChatClient**: The core interface for chat interactions
- **ChatClientAgent**: A wrapper that adds agent behavior around a chat client
- **AgentThread**: Manages conversation context and history
- **AgentFrameworkProvider**: Utility for creating configured chat clients

## Steps to Complete

### Step 1: Initialize the Chat Client
Create an `IChatClient` using the `AgentFrameworkProvider.CreateChatClientWithApiKey()` method.

### Step 2: Configure System Instructions
Define a string with instructions that tell the agent how to behave. Make it a friendly financial advisor with a creative and funny tone.

### Step 3: Create the ChatClientAgent
Instantiate a `ChatClientAgent` with:
- The chat client
- System instructions
- A name for the agent
- A description of what the agent does

### Step 4: Create a Conversation Thread
Use the agent's `GetNewThread()` method to create a new conversation context.

### Step 5: Process User Input
In the main loop, use `agent.RunAsync(userInput, thread)` to get the agent's response and display it to the user.

## Running the Lesson
1. Copy `appsettings.json.example` to `appsettings.json` and configure your API settings
2. Build and run the project
3. Type messages to chat with your financial advisor
4. Type "quit" to exit

## Expected Behavior
The agent should respond to financial questions with helpful advice delivered in a creative and humorous way, maintaining conversation context across multiple exchanges.