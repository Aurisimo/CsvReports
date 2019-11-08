using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CsvReports;
using CsvReports.Interfaces;
using CsvReports.Models;
using Moq;
using Xunit;

namespace CsvParserTests {

    public class CsvParserTests {
        private const string HEADER = "Band, PCL, TX Power, Target Power, MIN Power, MAX Power, Check Result"; 
        private const string DATA = "SM850, 5, 32.51429, 32.50000, 32.00000, 33.00000, PASS";

        private readonly string[] _incosnsistentData = new string[] {
            "[AGC Calibration]",
            "Band: (GSM850)",
            "Arfcn, Gain offset, MAX, MIN, Check Result",
            "140, 3.375, 8.500, -8.500, PASS",
            "150, 3.375, 8.500, -8.500, PASS",
            "160, 3.250, 8.500, -8.500, PASS"
        };

        private Mock<IRowParser> _rowParserMock;

        public CsvParserTests() {
            _rowParserMock = new Mock<IRowParser>();
        }

        [Fact]
        public void Initialize_NullStreamReaderProvider_ThrowsException() {
            Assert.Throws<ArgumentNullException>(() => new CsvParser(null, _rowParserMock.Object));
        }

        [Fact]
        public void Initialize_NullRowParser_ThrowsException() {
            Assert.Throws<ArgumentNullException>(() => new CsvParser(new Mock<TextReader>().Object, null));
        }

        [Fact]
        public void Parse_HeaderAnd3RowsAndInconsistentData_ReturnsRows() {
            using (var ms = new MemoryStream()) 
            using (var sw = new StreamWriter(ms))
            using (var sr = new StreamReader(ms))
            {
                var lineSplitted = DATA.Split(", ");
                
                var row = new Row {
                    Band = lineSplitted[0], 
                    Pcl = int.Parse(lineSplitted[1]), 
                    TxPower = float.Parse(lineSplitted[2]), 
                    TargetPower = float.Parse(lineSplitted[3]), 
                    MinPower = float.Parse(lineSplitted[4]), 
                    MaxPower = float.Parse(lineSplitted[5]), 
                    IsPassed = lineSplitted[6] == "PASS"
                };

                foreach (var line in _incosnsistentData) {
                    sw.WriteLine(line);
                }

                sw.WriteLine(HEADER);
                sw.WriteLine(DATA);
                sw.WriteLine(DATA);
                sw.WriteLine(DATA);

                foreach (var line in _incosnsistentData) {
                    sw.WriteLine(line);
                }
                sw.Flush();
                ms.Position = 0;

                _rowParserMock.Setup(m => m.TryParse(DATA, out row)).Returns(true);

                var cut = new CsvParser(sr, _rowParserMock.Object);

                var result = cut.Parse();
                Assert.Equal(3, result.Count());

                var resultFirstRow = result.First();
                Assert.Equal(row.Band, resultFirstRow.Band);
                Assert.Equal(row.Pcl, resultFirstRow.Pcl);
                Assert.Equal(row.TxPower, resultFirstRow.TxPower);
                Assert.Equal(row.TargetPower, resultFirstRow.TargetPower);
                Assert.Equal(row.MinPower, resultFirstRow.MinPower);
                Assert.Equal(row.MaxPower, resultFirstRow.MaxPower);
                Assert.Equal(row.IsPassed, resultFirstRow.IsPassed);
            } 
        }

        [Fact]
        public void Parse_OnlyHeader_ReturnsNoRows() {
            using (var ms = new MemoryStream()) 
            using (var sw = new StreamWriter(ms))
            using (var sr = new StreamReader(ms))
            {
                sw.WriteLine(HEADER);
                sw.Flush();
                ms.Position = 0;

                _rowParserMock.Setup(m => m.Parse(It.IsAny<string>())).Returns(new Row {});

                var cut = new CsvParser(sr, _rowParserMock.Object);
                var result = cut.Parse();
                Assert.Empty(result);
            } 
        }
    }
}


