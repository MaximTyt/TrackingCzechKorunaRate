using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Logger
{
    public static class FileLoggerExtensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string filePath)
        {
            builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>(provider => 
            {
                return new FileLoggerProvider(filePath);
            });            
            return builder;
        }
    }
}
