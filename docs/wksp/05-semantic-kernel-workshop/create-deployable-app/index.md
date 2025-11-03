# Turning the console application into a Deployable application

In this lesson we go over the steps on how to create a deployable version of the
console application using **Microsoft Agent Framework** by creating 3 main components:

1. [Backend API (C#) using ASP .NET Core with Microsoft Agent Framework](backend-api.md)
1. [React JS Web application (with Node Proxy)](web-app.md)
1. [AZD + Bicep templates for Infrastructure as Code deployment](azd-infra.md)

The backend uses **Microsoft Agent Framework** with sequential workflow orchestration, featuring three specialized agents:
- **Portfolio Research Agent** - Gathers market data using stock price and web search tools
- **Risk Assessment Agent** - Analyzes portfolio composition and risk profile
- **Investment Advisor Agent** - Provides actionable investment recommendations

 ![Chatbot Application Diagram](../images/architecture.png)
