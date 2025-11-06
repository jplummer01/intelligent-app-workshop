using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using Azure.Identity;
using Azure.Core;
using OpenAI;
using Core.Utilities.Models;

namespace Core.Utilities.Config;

public static class AgentFrameworkProvider
{
    public static IChatClient CreateChatClient()
    {
        var applicationSettings = AISettingsProvider.GetSettings();
        
        // Create Azure OpenAI client using Managed Identity
        TokenCredential credential;
        if (applicationSettings.ManagedIdentity != null && !string.IsNullOrEmpty(applicationSettings.ManagedIdentity.ClientId))
        {
            // Use user-assigned managed identity with specific client ID
            credential = new ManagedIdentityCredential(applicationSettings.ManagedIdentity.ClientId);
        }
        else
        {
            // Fall back to default credential chain
            credential = new DefaultAzureCredential();
        }
        
        var azureOpenAIClient = new AzureOpenAIClient(
            new Uri(applicationSettings.AIFoundryProject.Endpoint),
            credential);
        
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

