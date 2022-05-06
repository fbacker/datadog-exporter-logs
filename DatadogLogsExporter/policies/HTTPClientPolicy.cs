using Polly;
using Polly.Retry;

namespace DatadogLogsExporter.Policies
{
    public class HTTPClientPolicy
    {
        public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; }

        public HTTPClientPolicy()
        {
            ImmediateHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
                                    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                                    .WaitAndRetryAsync(3, 
                                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt)),
                                        (exception, retryCount) =>
                                        {
                                            Console.WriteLine($"{DateTime.Now} Failed http call retry delay: {retryCount}, response code: {exception.Result.StatusCode}");
                                        });
        }
    }
}
