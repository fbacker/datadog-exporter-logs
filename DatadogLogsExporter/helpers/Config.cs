using DatadogLogsExporter.models;

namespace DatadogLogsExporter.helpers
{
    public class Config
    {
        private CommandLineArguments _args;

        public Config(CommandLineArguments args)
        {
            _args = args;
        }

        public CommandLineArguments GetConfig()
        {
            return _args;
        }
    }
}
