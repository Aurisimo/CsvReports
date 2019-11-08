using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using CsvReports.Models;

namespace CsvReports {
    public class CsvLoader {
        private static string SEARCH_PATTERN = "*.csv";
        
        private string _dataDirectory;

        public CsvLoader(string dataDirectory)
        {
            if (!Directory.Exists(dataDirectory)) throw new DirectoryNotFoundException($"Data folder {dataDirectory} does not exist");

            _dataDirectory = dataDirectory;
        }
        
        public IEnumerable<Row> Load() {
            var bag = new ConcurrentBag<Row>();

            Parallel.ForEach (Directory.GetFiles(_dataDirectory, SEARCH_PATTERN, SearchOption.AllDirectories), (file) => {
                try {
                    using (var fs = File.OpenRead(file))
                    using (var sr = new StreamReader(fs)) {
                       var parser = new CsvParser(sr, new RowParser());
                       foreach (var row in parser.Parse()) {
                           bag.Add(row);
                       }
                    }
                } catch (Exception e) {
                    Console.WriteLine($"Failed to load a file {file}: {e.Message}");
                }
            });

            return bag;
        }
    }
}
