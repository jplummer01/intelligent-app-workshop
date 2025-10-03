using System.ComponentModel;

namespace Core.Utilities.Plugins;

public class TimeInformationPlugin
{
    [Description("Retrieves the current time in UTC.")]
    public string GetCurrentUtcTime() => DateTime.UtcNow.ToString("R");
}
