using System.Text.Json.Serialization;

namespace DatadogLogsExporter.models
{

    public class DatadogSearchQueryFilter
    {
        [JsonPropertyName("from")]
        public string From { get; set; }
        [JsonPropertyName("to")]
        public string To { get; set; }
        [JsonPropertyName("query")]
        public string? Query { get; set; }
    }

    public class DatadogSearchQueryPage
    {
        [JsonPropertyName("cursor")]
        public string? Cursor { get; set; }
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
    }



    public class DatadogSearchQuery
    {
        [JsonPropertyName("filter")]
        public DatadogSearchQueryFilter Filter { get; set; }

        [JsonPropertyName("page")]
        public DatadogSearchQueryPage Page { get; set; }
    }
}
