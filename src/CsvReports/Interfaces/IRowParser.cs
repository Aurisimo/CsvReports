using System;
using CsvReports.Models;

namespace CsvReports.Interfaces {
    public interface IRowParser {
        Row Parse(string input);
        bool TryParse(string inoput, out Row row);
    }
}