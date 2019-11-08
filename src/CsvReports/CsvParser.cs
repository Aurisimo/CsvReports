using System;
using System.Collections.Generic;
using System.IO;
using CsvReports.Interfaces;
using CsvReports.Models;

namespace CsvReports {
    public class CsvParser {
        private const string HEADER = "Band, PCL, TX Power, Target Power, MIN Power, MAX Power, Check Result";

        private TextReader _textReader;
        private IRowParser _rowParser;

        public CsvParser(TextReader textReader, IRowParser rowParser) {
            if (textReader == null) throw new ArgumentNullException(nameof(textReader));
            if (rowParser == null) throw new ArgumentNullException(nameof(rowParser));    

            _textReader = textReader;
            _rowParser = rowParser;
        }

        public IEnumerable<Row> Parse() {
            var result = new List<Row>();

            var line = string.Empty;
            var wasHeaderFound = false;
            while((line = _textReader.ReadLine()) != null) {                
                if (line == HEADER) {
                    wasHeaderFound = true;
                } else if (wasHeaderFound) {
                    Row row;
                    if (_rowParser.TryParse(line, out row)) {
                        result.Add(row);
                    } else {
                        wasHeaderFound = false;
                    }
                }
            }

            return result;
        }
    }

}