# Lesson 5: Enhanced Financial Analysis with Web Search

## Overview
This lesson demonstrates advanced agent capabilities by adding web search functionality to create a comprehensive financial analysis agent. You'll build an agent that can analyze stocks, market trends, and financial topics using both real-time market data and current web information.

## Learning Objectives
- Integrate multiple data sources (market data + web search)
- Create sophisticated agent instructions for complex analysis
- Handle natural language queries and extract relevant information
- Provide comprehensive analysis with source attribution
- Build production-ready financial analysis capabilities

## Key Concepts
- **Multi-Modal Analysis**: Combining structured data (stock prices) with unstructured data (web search)
- **Natural Language Processing**: Extracting stock symbols and intent from user queries
- **Source Attribution**: Tracking and reporting information sources
- **Comprehensive Analysis**: Providing complete responses with multiple data points

## New Components

### HostedWebSearchTool
Provides access to current web information for market sentiment, news, and analysis.

### Enhanced Agent Instructions
- Analyze various financial topics (stocks, sectors, trends)
- Extract stock symbols from natural language
- Use web search for current market sentiment
- Provide comprehensive single responses
- Include detailed source attribution

## Steps to Complete

### Step 1: Initialize Chat Client
Same as previous lessons.

### Step 2: Initialize Plugins with Web Search
Add the new HostedWebSearchTool alongside existing plugins:
- TimeInformationPlugin
- StockDataPlugin  
- HostedWebSearchTool (new!)

### Step 3: Create AI Functions
Convert all plugin methods to AIFunction objects, including the web search tool.

### Step 4: Create Comprehensive Agent Instructions
Design system instructions that enable the agent to:
- Analyze stocks, sectors, and general financial topics
- Extract stock symbols from natural language queries
- Use web search for current market news and sentiment
- Apply 1-10 sentiment scoring for stock analysis
- Provide complete analysis in single responses
- Include detailed source attribution

### Step 5: Create Financial Analysis Agent
Use ChatClientAgent with all tools (time, stock data, web search) and comprehensive instructions.

### Step 6: Create Thread
Same as previous lessons.

### Step 7: Process Complex Queries
The agent will automatically orchestrate multiple data sources to provide comprehensive analysis.

## Testing the Enhanced Agent

### Company Analysis (Natural Language)
- "What do you think about Microsoft?" (should extract MSFT)
- "How is Apple performing?" (should extract AAPL)
- "Tell me about Tesla's prospects" (should extract TSLA)

### Sector Analysis
- "How is the tech sector performing?"
- "Should I invest in renewable energy stocks?"
- "What's happening in the banking industry?"

### Market Trends
- "What are the current market trends?"
- "How is the overall economy doing?"
- "What are analysts saying about inflation?"

### Investment Strategy
- "What should I invest in right now?"
- "Is this a good time to buy stocks?"
- "How should I diversify my portfolio?"

## Expected Behavior
The agent should:
- Automatically determine when to use web search vs. stock data
- Extract stock symbols from natural language queries
- Provide comprehensive analysis combining multiple sources
- Include a dedicated "Sources" section with specific references
- Give complete responses without requesting more information
- Apply appropriate analysis frameworks based on query type

This lesson demonstrates how Agent Framework can orchestrate multiple capabilities to create sophisticated, production-ready AI applications.