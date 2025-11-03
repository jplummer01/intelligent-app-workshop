using Core.Utilities.Config;
using Core.Utilities.Models;
using Core.Utilities.Plugins;
using Core.Utilities.Services;
using Microsoft.Extensions.AI;

namespace Extensions;

public static class ServiceExtensions
{
    public static void AddMafServices(this IServiceCollection services) 
    {
        // MAF uses IChatClient directly from AgentFrameworkProvider
        // No special DI registration needed - agents are created in controllers
        services.AddHttpClient();
    }
}