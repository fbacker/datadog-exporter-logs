using DatadogLogsExporter.helpers;
using DatadogLogsExporter.models;
using DatadogLogsExporter.Policies;
using System.Net.Http.Json;
using System.Text.Json;

namespace DatadogLogsExporter.service
{
    public class HttpDatadogClient
    {
        private readonly HttpClient _httpClient;
        private readonly HTTPClientPolicy _clientPolicy;
        private bool _verbose;

        public HttpDatadogClient(Config config, HTTPClientPolicy clientPolicy)
        {
            _clientPolicy = clientPolicy;

            var opts = config.GetConfig();

            _verbose = opts.Verbose;

            var http = new HttpClient();
            http.DefaultRequestHeaders.Clear();
            http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            http.DefaultRequestHeaders.Add("DD-API-KEY", opts.DDKey);
            http.DefaultRequestHeaders.Add("DD-APPLICATION-KEY", opts.DDApp);
            _httpClient = http;
        }


        public async Task<DatadogResponse> GetSearchResult<DatadogResponse>(CancellationToken cancellationToken, string path, DatadogSearchQuery search)
        {
            if (_verbose) Console.WriteLine($"{DateTime.Now} Start HTTP Request");
            //var clientRequest = await _httpClient.PostAsJsonAsync<DatadogSearchQuery>(path, search, cancellationToken);
            var clientRequest = await _clientPolicy.ImmediateHttpRetry.ExecuteAsync(
                async () => await _httpClient.PostAsJsonAsync<DatadogSearchQuery>(path, search, cancellationToken)
            );

            if (_verbose) Console.WriteLine($"{DateTime.Now} End HTTP Request");

            clientRequest.EnsureSuccessStatusCode();

            return await JsonSerializer.DeserializeAsync<DatadogResponse>(clientRequest.Content.ReadAsStream());
        }
    }
}
