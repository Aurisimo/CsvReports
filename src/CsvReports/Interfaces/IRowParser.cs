using System;

namespace CsvReports.Interfaces {
    public interface IRowParser {
        Row Parse(string inout);
    }
}