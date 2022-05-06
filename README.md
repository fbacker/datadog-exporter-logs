# datadog-exporter-logs

This container will help to export datadog logs to file on disk. You'll require a datadog api key and application key.

It will split output files when it becomes over 500mb and don't forget to change domain (if you are e.g. in Europe).

To see all available options run:

```docker run fredrickbacker/datadog-exporter-logs:latest```

## Example

We request IIS logs for a service between specific date from datadog EU.
```
docker run \
    -v C:\Users\mystuff\output:/files \
    fredrickbacker/datadog-exporter-logs:latest \
    --datadog-api-key=d146-my-own-key-here-6d144f \
    --datadog-application-key=b214a-my-own-app-key-here-97461e \
    --date-from=2022-05-01T00:00:00Z \
    --date-to=2022-05-01T23:59:59Z \
    --query="source:iis service:website" \
    --domain=eu
```