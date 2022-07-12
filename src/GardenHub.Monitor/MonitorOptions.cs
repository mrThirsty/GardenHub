using CommandLine;

namespace GardenHub.Monitor;

public class MonitorOptions
{
    [Option('c', "configure", Required = false, Default = false,
        HelpText = "Use this to run the configuration wizard.")]
    public bool Configure { get; set; }
}