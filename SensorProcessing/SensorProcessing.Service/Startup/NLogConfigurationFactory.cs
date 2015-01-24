using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SensorProcessing.Service.Startup
{
     public static class NLogConfigurationFactory
    {
        public static LoggingConfiguration CreateNLogConfiguration()
        {
            var result = new LoggingConfiguration();
            
            string loggingDirectory = ConfigurationManager.AppSettings["loggingDirectory"] ?? Path.Combine(".","log");
            string logFileName = Path.Combine(loggingDirectory, "sensorProcessing.log");
            
            AddFileTarget(logFileName, LogLevel.Info, result);
            AddConsoleTarget(LogLevel.Trace, result);
            return result;
        }

        private static void AddConsoleTarget(LogLevel minimumLogLevel, LoggingConfiguration config)
        {
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("file", consoleTarget);
            var consoleRule = new LoggingRule("*", minimumLogLevel, consoleTarget);
            config.LoggingRules.Add(consoleRule);
        }

        private static void AddFileTarget(string filename, LogLevel minimumLogLevel, LoggingConfiguration config)
        {
            string archiveName = GenerateArchiveFilenameWithPlaceholder(filename);

            var fileTarget = new FileTarget
            {
                FileName = filename,
                ArchiveFileName = archiveName,
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.Rolling,
                MaxArchiveFiles = 7,
                ConcurrentWrites = true,
                KeepFileOpen = false
            };
            config.AddTarget("file", fileTarget);

            var fileRule = new LoggingRule("*", minimumLogLevel, fileTarget);
            config.LoggingRules.Add(fileRule);
        }

        private static string GenerateArchiveFilenameWithPlaceholder(string filename)
        {
            string[] splitted = filename.Split(new[] { '.' });
            string archiveName = @"Archived.";

            for (int i = 0; i < splitted.Length - 1; i++)
            {
                archiveName += splitted[i] + ".";
            }
            archiveName += "{#}." + splitted[splitted.Length - 1];
            return archiveName;
        }
    }


    
}
