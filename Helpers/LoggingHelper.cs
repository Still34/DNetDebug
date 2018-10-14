using System;
using System.Threading.Tasks;
using Discord;
using Serilog;

namespace DNetDebug.Helpers
{
    public static class LoggingHelper
    {
        public static Task LogAsync(LogMessage logMessage)
        {
            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                    Log.Fatal(logMessage.Exception, $"{logMessage.Source}: {logMessage.Message}");
                    break;
                case LogSeverity.Error:
                    Log.Error(logMessage.Exception, $"{logMessage.Source}: {logMessage.Message}");
                    break;
                case LogSeverity.Warning:
                    Log.Warning(logMessage.Exception, $"{logMessage.Source}: {logMessage.Message}");
                    break;
                case LogSeverity.Info:
                    Log.Information(logMessage.Exception, $"{logMessage.Source}: {logMessage.Message}");
                    break;
                case LogSeverity.Verbose:
                    Log.Verbose(logMessage.Exception, $"{logMessage.Source}: {logMessage.Message}");
                    break;
                case LogSeverity.Debug:
                    Log.Debug(logMessage.Exception, $"{logMessage.Source}: {logMessage.Message}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Task.CompletedTask;
        }
    }
}