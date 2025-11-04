using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI;

namespace Core.Utilities.Config;

public static class AgentFrameworkProvider
{
    public static IChatClient CreateChatClient()
    {
        var applicationSettings = AISettingsProvider.GetSettings();
        
        // Create Azure OpenAI client using Managed Identity
        var azureOpenAIClient = new AzureOpenAIClient(
            new Uri(applicationSettings.AIFoundryProject.Endpoint),
            new DefaultAzureCredential());
        
        // Get chat client directly from Azure OpenAI client
        var chatClient = azureOpenAIClient.GetChatClient(applicationSettings.AIFoundryProject.DeploymentName);
        return chatClient.AsIChatClient();
    }
    
    public static IChatClient CreateChatClientWithApiKey()
    {
        var applicationSettings = AISettingsProvider.GetSettings();
        
        // Create Azure OpenAI client with API key
        var azureOpenAIClient = new AzureOpenAIClient(
            new Uri(applicationSettings.AIFoundryProject.Endpoint),
            new Azure.AzureKeyCredential(applicationSettings.AIFoundryProject.ApiKey));
        
        // Get chat client directly from Azure OpenAI client 
        var chatClient = azureOpenAIClient.GetChatClient(applicationSettings.AIFoundryProject.DeploymentName);
        return chatClient.AsIChatClient();
    }
}

