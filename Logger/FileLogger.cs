using Microsoft.Extensions.Logging;
using System.Text;

namespace Logger
{
    public class FileLogger : ILogger
    {
        private readonly string filePath;
        private readonly object _lock = new object();
        public FileLogger(string path)
        {
            filePath = path;
        }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Information;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {message}{Environment.NewLine}";
            lock (_lock)
            {
                File.AppendAllText(filePath, logMessage, Encoding.UTF8);
                //using (var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                //using (var writer = new StreamWriter(stream, Encoding.UTF8))
                //{
                //    writer.Write(logMessage);
                //}
            }            
        }
    }
}
