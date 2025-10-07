# Lesson 3: Function Calling with Agents

## Overview
This lesson introduces function calling capabilities in Agent Framework. You'll learn how to give your AI agent access to real-time data through functions, enabling it to provide current stock prices, time information, and historical data.

## Learning Objectives
- Understand how to add function calling capabilities to agents
- Learn to use AIFunctionFactory to convert methods into AI functions
- Experience automatic function calling in conversational AI
- Work with real-time data in AI applications

## Key Concepts
- **AIFunction**: Wrapper that makes regular methods callable by AI agents
- **AIFunctionFactory**: Utility for converting methods to AIFunction objects
- **Automatic Function Calling**: Agent Framework automatically determines when to call functions
- **Plugin Architecture**: Organized way to group related functions

## New Components

### Plugins Used
- **TimeInformationPlugin**: Provides current time information
- **StockDataPlugin**: Provides current and historical stock price data
- **StocksService**: Backend service for retrieving stock market data

### Function Conversion
```csharp
AIFunctionFactory.Create(plugin.Method)
```
This converts a regular C# method into an AIFunction that the agent can call automatically.

## Steps to Complete

### Step 1: Initialize Chat Client
Same as previous lessons.

### Step 2: Initialize Plugins
Create instances of the plugins that provide the functionality:
- TimeInformationPlugin for time-related queries
- StockDataPlugin with StocksService for market data

### Step 3: Create Tools Array
Convert plugin methods to AIFunction objects using AIFunctionFactory.Create():
- Time function from timePlugin.GetCurrentUtcTime
- Stock price function from stockDataPlugin.GetStockPrice
- Historical data function from stockDataPlugin.GetStockPriceForDate

### Step 4: Create Agent with Tools
Create ChatClientAgent and pass the tools array to enable function calling.

### Step 5: Create Thread
Same as previous lessons.

### Step 6: Use Agent with Function Calling
The agent will automatically call functions when needed to answer user questions.

## Testing Function Calling

Try these queries to see automatic function calling:

### Time Queries
- "What time is it?"
- "What's the current UTC time?"

### Current Stock Prices
- "What's the current price of MSFT?"
- "How is Apple stock doing today?"
- "Get me Tesla's current stock price"

### Historical Data
- "What was AAPL's price on 2023-01-15?"
- "Show me Microsoft's stock price from last month"

### Combined Queries
- "What time is it and what's Google's current stock price?"
- "Give me a financial report with current time and top tech stock prices"

## Expected Behavior
The agent should automatically determine when function calls are needed and execute them seamlessly within the conversation. You'll see the agent providing real-time, accurate data while maintaining its creative and funny personality.