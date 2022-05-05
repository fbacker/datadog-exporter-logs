using DatadogLogsExporter.models;
using System.Net.Http.Json;
using System.Text.Json;

namespace DatadogLogsExporter.service
{
    public class HttpDatadogClient
    {
        private readonly HttpClient httpClient;

        public HttpDatadogClient(HttpClient httpClient) => this.httpClient = httpClient;


        public async Task<DatadogResponse> GetSearchResult<DatadogResponse>(CancellationToken cancellationToken, string path, DatadogSearchQuery search)
        {
            var clientRequest = await this.httpClient.PostAsJsonAsync<DatadogSearchQuery>(path, search, cancellationToken);
            clientRequest.EnsureSuccessStatusCode();

            var data = await JsonSerializer.DeserializeAsync<DatadogResponse>(clientRequest.Content.ReadAsStream());
            return data;
        }
    }
}
