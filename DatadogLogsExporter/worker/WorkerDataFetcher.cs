
using DatadogLogsExporter.helpers;
using DatadogLogsExporter.models;
using DatadogLogsExporter.service;
using System.Diagnostics;

namespace DatadogLogsExporter.worker
{
    public class WorkerDataFetcher
    {

        private FileWriter _logWriter;
        private HttpDatadogClient _client;
        private DatadogSearchQuery _payload;
        private string _url;
        private int _maxIterations;
        private bool _verbose;
        private int _cooldown;


        public WorkerDataFetcher(Config config, FileWriter logWriter, HttpDatadogClient datadogClient)
        {
            var opts = config.GetConfig();

            _verbose = opts.Verbose;
            _logWriter = logWriter;
            _client = datadogClient;
            _url = $"https://api.datadoghq.{opts.Domain}/api/v2/logs/events/search";
            _maxIterations = opts.MaxIterations;
            _payload = new DatadogSearchQuery
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

        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            if (_verbose) Console.WriteLine($"{DateTime.Now} Starting Worker");

            var isRunning = true;
            var iterations = 0;
            var counter = 0;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var stopWatchTotal = new Stopwatch();
            stopWatchTotal.Start();

            while (isRunning)
            {

                if (cancellationToken.IsCancellationRequested)
                {
                    if (_verbose) Console.WriteLine($"{DateTime.Now} Cancellation token is requested. Exit");
                    _logWriter.Dispose();
                    Environment.Exit(0);
                }

                if (_cooldown > 0) await Task.Delay(_cooldown);

                iterations++;
                stopWatch.Restart();

                if (_verbose) Console.WriteLine($"{DateTime.Now} Start iteration {iterations}");

                try
                {
                    var response = await _client.GetSearchResult<DatadogResponse>(cancellationToken, _url, _payload);

                    if (_verbose) Console.WriteLine($"{DateTime.Now} HTTP response with items {response.Data.Count}");

                    foreach (var item in response.Data)
                    {
                        counter++;
                        _logWriter.Write(item);
                    }

                    var timeMs = stopWatch.ElapsedMilliseconds;
                    if (_maxIterations == 0) Console.WriteLine($"{DateTime.Now} Loaded data iteration and written output {iterations}, it took {timeMs}ms");
                    else Console.WriteLine($"{DateTime.Now} Loaded data iteration and written output {iterations} of {_maxIterations}, it took {timeMs}ms");

                    if (response.Meta == null || response.Meta.Page == null || response.Meta.Page.After == null)
                    {
                        Console.WriteLine($"{DateTime.Now} No more data to collect");
                        isRunning = false;
                    }
                    else if (_maxIterations > 0 && iterations >= _maxIterations)
                    {
                        Console.WriteLine($"{DateTime.Now} Max iteration limit reached");
                        isRunning = false;
                    }
                    else _payload.Page.Cursor = response.Meta.Page.After;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} Error occured: {ex.Message}");
                    _logWriter.Dispose();
                    Environment.Exit(0);
                }
            }


            Console.WriteLine("");
            Console.WriteLine($"{DateTime.Now} Completed");
            Console.WriteLine($"   Iterations: {iterations}");
            Console.WriteLine($"   Loglines: {counter}");
            Console.WriteLine($"   Elapsed time: {(stopWatchTotal.ElapsedMilliseconds / 1000)}s");
        }

    }
}
