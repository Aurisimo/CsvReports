using System;
using System.Globalization;
using CsvReports.Interfaces;
using CsvReports.Models;

namespace CsvReports {

    public class RowParser : IRowParser {
        public const int NUMEBR_OF_COLUMNS = 7;
        
        public bool TryParse(string input, out Row row) {
            try {
                row = Parse(input);
                return true;
            } catch {
                row = null;
                return false;
            }
        }

        public Row Parse(string input) {
            if (input == null) throw new ArgumentNullException(nameof(input));

            var columns = input.Split(",");
            if (columns.Length != NUMEBR_OF_COLUMNS) throw new FormatException("Invalid number of columns");
            
            var row = new Row();

            for(var i = 0; i < NUMEBR_OF_COLUMNS; i++) {
                var column = columns[i].Trim();

                switch(i) {
                    case 0:
                        if (column == "") throw new ArgumentOutOfRangeException(nameof(Row.Band));
                        row.Band = column;
                        break;
                    case 1:
                        int pcl;
                        if (!int.TryParse(column, out pcl)) 
                            throw new FormatException(nameof(Row.Pcl));
                        row.Pcl = pcl;
                        break;
                    case 2:
                        float txPower;
                        if (!float.TryParse(column, NumberStyles.Any, CultureInfo.InvariantCulture, out txPower))
                            throw new FormatException(nameof(Row.TxPower));
                        row.TxPower = txPower;
                        break;
                    case 3:
                        float targetPower;
                        if (!float.TryParse(column, NumberStyles.Any, CultureInfo.InvariantCulture, out targetPower))
                            throw new FormatException(nameof(Row.TargetPower));
                        row.TargetPower = targetPower;
                        break;
                    case 4:
                        float minPower;
                        if (!float.TryParse(column, NumberStyles.Any, CultureInfo.InvariantCulture, out minPower))
                            throw new FormatException(nameof(Row.MinPower));
                        row.MinPower = minPower;
                        break;
                    case 5:
                        float maxPower;
                        if (!float.TryParse(column, NumberStyles.Any, CultureInfo.InvariantCulture, out maxPower))
                            throw new FormatException(nameof(Row.MaxPower));
                        row.MaxPower = maxPower;
                        break;
                    default:
                        switch(column.ToUpper()) {
                            case "PASS":
                                row.IsPassed = true;
                                break;
                            case "FAIL":
                                row.IsPassed = false;
                                break;
                            default: 
                                throw new ArgumentOutOfRangeException($"{nameof(Row.IsPassed)}: {column}");
                        }
                        break;
                }
            }
            
            return row;
        }
    }
}