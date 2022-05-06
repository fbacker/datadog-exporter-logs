using CommandLine;
using DatadogLogsExporter.helpers;
using DatadogLogsExporter.models;
using DatadogLogsExporter.Policies;
using DatadogLogsExporter.service;
using DatadogLogsExporter.worker;
using Microsoft.Extensions.DependencyInjection;

CommandLineArguments opts = null;
Parser.Default.ParseArguments<CommandLineArguments>(args)
    .WithParsed(o => opts = o)
    .WithNotParsed(errs => Environment.Exit(0));

if (opts.Count < 0 || opts.Count > 1000)
{
    Console.WriteLine("Count is out of bounds. Must be between 1-1000");
    Environment.Exit(0);
}


Console.WriteLine("Datadog Logs Exporter");

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine("Canceling...");
    cts.Cancel();
    e.Cancel = true;
};


var services = new ServiceCollection();
services.AddOptions();
services.AddSingleton<WorkerDataFetcher>();
services.AddSingleton<HTTPClientPolicy>();
services.AddSingleton<FileWriter>();
services.AddSingleton<HttpDatadogClient>();
services.AddSingleton<HTTPClientPolicy>();
services.AddSingleton<Config>(new Config(opts));

var servicesProvider = services.BuildServiceProvider();

var worker = servicesProvider.GetRequiredService<WorkerDataFetcher>();
await worker.DoWork(cts.Token);
