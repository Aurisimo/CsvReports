using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using CsvReports.Models;

namespace CsvReports {
    public class ReportGenerator {
        private const string REPORT_PREFIX = "report-";
        private const string REPORT_DATE_FORMAT = "yyy-MM-dd_hhmmss";
        private const string REPORT_EXTENSION = "csv";
        private const string HEADER = "BAND,PCL,Average low TxPower,Average in range TxPower,Average high TxPower,PASS Count,FAIL Count";
        private const string FLOAT_FORMAT = "0.#######";

        private string _reportDirectory;
        private IEnumerable<Row> _data;
        
        public ReportGenerator(string reportDirectory, IEnumerable<Row> data)
        {
            if (string.IsNullOrEmpty(reportDirectory)) throw new ArgumentOutOfRangeException(nameof(reportDirectory));
            if (data == null) throw new ArgumentNullException(nameof(data));

            _reportDirectory = reportDirectory;
            _data = data;
        }

        public void Generate() {
            var reportName = $"{REPORT_PREFIX}{DateTime.Now.ToString(REPORT_DATE_FORMAT)}.{REPORT_EXTENSION}";
            var reportFullPath = Path.Combine(_reportDirectory, reportName);

            Directory.CreateDirectory(_reportDirectory);

            using(var fs = File.Create(reportFullPath))
            using(var sw = new StreamWriter(fs)) {
                sw.WriteLine(HEADER);

                var emptyRow = new Row {};

                foreach(var row in _data.GroupBy(r => new {r.Band, r.Pcl})
                    .Select(g => new {
                        g.Key.Band, 
                        g.Key.Pcl, 
                        avgLowTxPower = g.Where(r => r.TxPower < r.MinPower).DefaultIfEmpty(emptyRow).Average(r => r.TxPower),
                        avgInRangeTxPower = g.Where(r => r.TxPower > r.MinPower && r.TxPower < r.MaxPower).DefaultIfEmpty(emptyRow).Average(r => r.TxPower),
                        avgHighTxPower = g.Where(r => r.TxPower > r.MaxPower).DefaultIfEmpty(emptyRow).Average(r => r.TxPower),
                        passCount = g.Sum(r => r.IsPassed ? 1 : 0),
                        failCount = g.Sum(r => !r.IsPassed ? 1 : 0)
                    })
                    .OrderBy(r => r.Band)
                    .ThenBy(r => r.Pcl)) {
                    
                    sw.WriteLine($"{row.Band}, {row.Pcl}, {FormatFloat(row.avgLowTxPower)}, {FormatFloat(row.avgInRangeTxPower)}, {FormatFloat(row.avgHighTxPower)}, {row.passCount}, {row.failCount}");
                }
            }
        }

        private string FormatFloat(float value) {
            return string.Format(CultureInfo.InvariantCulture, $"{{0:{FLOAT_FORMAT}}}", value);
        }
    }
}