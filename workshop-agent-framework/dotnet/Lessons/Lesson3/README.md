# Lesson 4: Specialized Agent - Stock Sentiment Analysis

## Overview
This lesson demonstrates how to create a specialized AI agent with domain-specific instructions and behavior. You'll build a Stock Sentiment Agent that analyzes stocks using a standardized scoring system and provides structured investment recommendations.

## Learning Objectives
- Create specialized agents with domain-specific instructions
- Implement structured response formats and scoring systems
- Combine function calling with specialized analysis
- Design professional-grade AI agent behavior

## Key Concepts
- **Specialized Instructions**: Detailed system prompts that define agent behavior
- **Structured Responses**: Consistent output formats for professional applications
- **Domain Expertise**: Agent designed for specific use cases
- **Scoring Systems**: Standardized metrics for analysis (1-10 sentiment scale)

## Agent Specifications

### Stock Sentiment Agent Rules
- Uses stock sentiment scale from 1 to 10 (1=sell, 10=buy)
- Provides rating, recommendation (buy/hold/sell), and reasoning
- Includes sources for sentiment analysis
- Focuses on technical analysis based on stock price data
- Maintains professional, analytical tone

### Expected Response Format
```
Stock: [SYMBOL]
Sentiment Score: [1-10]
Recommendation: [BUY/HOLD/SELL]
Reasoning: [Technical analysis explanation]
Source: [Market data, technical indicators, etc.]
```

## Steps to Complete

### Step 1: Initialize Chat Client
Same as previous lessons.

### Step 2: Initialize Plugins
Set up TimeInformationPlugin and StockDataPlugin for real-time data access.

### Step 3: Create AI Functions
Convert plugin methods to AIFunction objects using AIFunctionFactory.Create().

### Step 4: Create Specialized System Instructions
Write detailed instructions that define:
- The agent's role as a stock sentiment analyzer
- Specific rules for analysis and scoring
- Required response format and structure
- Professional behavior expectations

### Step 5: Create Stock Sentiment Agent
Use ChatClientAgent with the specialized instructions and tools.

### Step 6: Create Thread
Same as previous lessons.

### Step 7: Process User Messages
Use agent.RunAsync() with proper error handling for robust operation.

## Testing the Agent

### Stock Analysis Queries
- "MSFT" - Get sentiment analysis for Microsoft
- "Analyze AAPL" - Apple stock sentiment
- "What's your sentiment on TSLA?" - Tesla analysis

### Comparison Queries
- "Compare MSFT and GOOGL sentiment"
- "Which is better: AAPL or AMZN?"

### Market Analysis
- "What stocks should I buy today?"
- "Give me sentiment analysis for tech stocks"

## Expected Behavior
The agent should provide consistent, structured analysis with:
- Clear numerical sentiment scores (1-10)
- Specific buy/hold/sell recommendations
- Technical reasoning based on current market data
- Professional, analytical responses
- Source attribution for analysis

This specialized agent demonstrates how Agent Framework can be used to create professional-grade AI applications with domain-specific expertise and structured outputs.