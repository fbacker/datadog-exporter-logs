using CommandLine;

namespace DatadogLogsExporter.models
{
    public class CommandLineArguments
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('k', "datadog-api-key", Required = true, HelpText = "Datadog API Key.")]
        public string DDKey { get; set; }

        [Option('a', "datadog-application-key", Required = true, HelpText = "Datadog Application Key.")]
        public string DDApp { get; set; }

        [Option('q', "query", Required = false, HelpText = "Datadog query, example: source:iis service:website")]
        public string Query { get; set; }

        [Option('f', "date-from", Required = false, HelpText = "Filter data from timestamp 2022-05-01T00:00:00Z")]
        public string From { get; set; }

        [Option('t', "date-to", Required = false, HelpText = "Filter data to timestamp 2022-05-23:59:59Z")]
        public string To { get; set; }

        [Option('c', "count", Required = false, HelpText = "How many results for each iteration. Max is 1000", Default = 1000)]
        public int Count { get; set; }

        [Option('i', "iterations", Required = false, HelpText = "Max iterations if we want to limit how many requests to do", Default = 0)]
        public int MaxIterations { get; set; }

        [Option('d', "domain", Required = false, HelpText = "What datadog server to use for export? com, eu. https://docs.datadoghq.com/api/latest/logs/#search-logs", Default = "com")]
        public string Domain { get; set; }

        [Option('c', "cooldown", Required = false, HelpText = "Cooldown milliseconds between each http request. Can help with api rate limit", Default = 0)]
        public int Cooldown { get; set; }

    };
}
