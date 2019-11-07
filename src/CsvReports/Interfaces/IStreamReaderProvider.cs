using System;
using System.IO;

namespace CsvReports.Interfaces {
    public interface IStreamReaderProvider : IDisposable {
        StreamReader StreamReader { get; } 
    }
}