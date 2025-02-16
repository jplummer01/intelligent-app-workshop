using Core.Utilities.Config;
// Add import for Plugins
using Core.Utilities.Plugins;
// Add import required for StockService
using Core.Utilities.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
// Step 1 - Add imports for Bing Search plugin
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
// Add ChatCompletion import
using Microsoft.SemanticKernel.ChatCompletion;
// Temporarily added to enable Semantic Kernel tracing
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


// Initialize the kernel with chat completion
IKernelBuilder builder = KernelBuilderProvider.CreateKernelWithChatCompletion();
// Enable tracing
//builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));
Kernel kernel = builder.Build();

// Initialize Time plugin and registration in the kernel
kernel.Plugins.AddFromObject(new TimeInformationPlugin());

// Initialize Stock Data Plugin and register it in the kernel
HttpClient httpClient = new();
StockDataPlugin stockDataPlugin = new(new StocksService(httpClient));
kernel.Plugins.AddFromObject(stockDataPlugin);

// Step 2 - Initialize Bing Search plugin
var bingApiKey = AISettingsProvider.GetSettings().BingSearchService.ApiKey;
if (!string.IsNullOrEmpty(bingApiKey))
{
    var bingConnector = new BingConnector(bingApiKey);
    var bing = new WebSearchEnginePlugin(bingConnector);
    kernel.ImportPluginFromObject(bing, "bing");
}

// Get chatCompletionService and initialize chatHistory with system prompt
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
ChatHistory chatHistory = new("You are a friendly financial advisor that only emits financial advice in a creative and funny tone");
// Remove the promptExecutionSettings and kernelArgs initialization code
// Add system prompt
OpenAIPromptExecutionSettings promptExecutionSettings = new()
{
    // Add Auto invoke kernel functions as the tool call behavior
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

// Initialize kernel arguments
KernelArguments kernelArgs = new(promptExecutionSettings);

// Execute program.
const string terminationPhrase = "quit";
string? userInput;
do
{
    Console.Write("User > ");
    userInput = Console.ReadLine();

    if (userInput != null && userInput != terminationPhrase)
    {
        Console.Write("Assistant > ");
        // Initialize fullMessage variable and add user input to chat history
        string fullMessage = "";
        chatHistory.AddUserMessage(userInput);

        // Provide promptExecutionSettings and kernel arguments
        await foreach (var chatUpdate in chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory, promptExecutionSettings, kernel))
        {
            Console.Write(chatUpdate.Content);
            fullMessage += chatUpdate.Content ?? "";
        }
        chatHistory.AddAssistantMessage(fullMessage);

        Console.WriteLine();
    }
}
while (userInput != terminationPhrase);