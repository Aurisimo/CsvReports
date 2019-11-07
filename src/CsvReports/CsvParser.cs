using System;
using System.Collections.Generic;
using System.IO;
using CsvReports.Interfaces;

namespace CsvReports {
    public class CsvParser {

        private IStreamReaderProvider _streamReaderProvider;
        private IRowParser _rowParser;

        public CsvParser(IStreamReaderProvider streamReaderProvider, IRowParser rowParser) {
            if (streamReaderProvider == null) throw new ArgumentNullException(nameof(streamReaderProvider));
            if (rowParser == null) throw new ArgumentNullException(nameof(rowParser));    

            _streamReaderProvider = streamReaderProvider;
            _rowParser = rowParser;
        }

        public IEnumerable<Row> Parse() {
            var result = new List<Row>();

            _streamReaderProvider.StreamReader.ReadLine();
            var line = string.Empty;
            while((line = _streamReaderProvider.StreamReader.ReadLine()) != null) {
                Console.WriteLine(line);
                
                var row = _rowParser.Parse(line);
                result.Add(row);
            }

            return result;
        }
    }

}