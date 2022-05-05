using CommandLine;
using DatadogLogsExporter.models;
using DatadogLogsExporter.service;
using System.Diagnostics;

CommandLineArguments opts = null;
Parser.Default.ParseArguments<CommandLineArguments>(args)
    .WithParsed(o => opts = o)
    .WithNotParsed(errs => Environment.Exit(0));

if(opts.Count<0 || opts.Count > 1000)
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

var http = new HttpClient();
http.DefaultRequestHeaders.Clear();
http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
http.DefaultRequestHeaders.Add("DD-API-KEY", opts.DDKey);
http.DefaultRequestHeaders.Add("DD-APPLICATION-KEY", opts.DDApp);

var client = new HttpDatadogClient(http);
var url = $"https://api.datadoghq.{opts.Domain}/api/v2/logs/events/search";

var payload = new DatadogSearchQuery
{
    Filter = new DatadogSearchQueryFilter
    {
        From = opts.From,
        To = opts.To,
        Query = opts.Query
    },
    Page = new DatadogSearchQueryPage
    {
        Limit = opts.Count
    }
};


// Docker container local path
var docPath = "/files";

//@TODO, set max log size from options, but in mb, max limit?
double maxLogSize = 5e+8;
var logWriter = new FileWriter(docPath, maxLogSize);

var isRunning = true;
var iterations = 0;
var counter = 0;

var stopWatch = new Stopwatch();
stopWatch.Start();

var stopWatchTotal = new Stopwatch();
stopWatchTotal.Start();

while (isRunning)
{
 
    if (cts.IsCancellationRequested)
    {
        logWriter.Dispose();
        Environment.Exit(0);
    }

    iterations++;
    stopWatch.Restart();
        
    try
    {
        var response = await client.GetSearchResult<DatadogResponse>(cts.Token, url, payload);
            
        foreach (var item in response.Data)
        {
            counter++;
            logWriter.Write(item);
        }

        var timeMs = stopWatch.ElapsedMilliseconds;
        if (opts.MaxIterations == 0) Console.WriteLine($"Loaded data iteration and written output {iterations}, it took {timeMs}ms");
        else Console.WriteLine($"Loaded data iteration and written output {iterations} of {opts.MaxIterations}, it took {timeMs}ms");

        if (response.Meta == null || response.Meta.Page == null || response.Meta.Page.After == null)
        {
            Console.WriteLine("No more data to collect");
            isRunning = false;
        }
        else if (opts.MaxIterations > 0 && iterations >= opts.MaxIterations)
        {
            Console.WriteLine("Max iteration limit reached");
            isRunning = false;
        }
        else payload.Page.Cursor = response.Meta.Page.After;
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error occured: "+ ex.Message);
        logWriter.Dispose();
        Environment.Exit(0);
    }
}


Console.WriteLine("");
Console.WriteLine("Completed");
Console.WriteLine($"   Iterations: {iterations}");
Console.WriteLine($"   Loglines: {counter}");
Console.WriteLine($"   Elapsed time: {(stopWatchTotal.ElapsedMilliseconds/1000)}s");