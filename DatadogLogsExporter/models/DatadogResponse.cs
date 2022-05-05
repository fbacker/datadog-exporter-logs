
using System.Text.Json.Serialization;

namespace DatadogLogsExporter.models
{
    public class DatadogResponseMeta
    {
        [JsonPropertyName("page")]
        public DatadogResponseMetaPage? Page { get; set; }
    }

    public class DatadogResponseMetaPage
    {
        [JsonPropertyName("after")]
        public string? After { get; set; }
    }

    public class DatadogResponse
    {

        [JsonPropertyName("meta")]
        public DatadogResponseMeta Meta { get; set; }

        [JsonPropertyName("data")]
        public List<dynamic> Data { get; set; }
    }
}
