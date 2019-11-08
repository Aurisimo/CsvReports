using System;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace CsvReports
{
    class Program
    {
        private static string DATA_DIRECTORY_KEY = "dataDirectory";
        private static string REPORT_DIRECTORY_KEY = "reportDirectory";
        private static string APP_SETTINGS_KEY = "appsettings.json";
        
        static void Main(string[] args)
        {
            try {            
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(APP_SETTINGS_KEY);
                var config = builder.Build();
                var dataDirectory = config.GetSection(DATA_DIRECTORY_KEY).Get<string>();
                var reportDirectory = config.GetSection(REPORT_DIRECTORY_KEY).Get<string>();
    
                var loader = new CsvLoader(dataDirectory);
                var data = loader.Load();

                var reportGenerator = new ReportGenerator(reportDirectory, data);
                reportGenerator.Generate();
            } catch (Exception e) {
                System.Console.WriteLine($"Failed to generate reports: {e.Message}");
            }
        }
    }
}
