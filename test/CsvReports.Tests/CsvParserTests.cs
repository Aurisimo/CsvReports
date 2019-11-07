using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CsvReports;
using CsvReports.Interfaces;
using Moq;
using Xunit;

namespace CsvParserTests {

    public class CsvParserTests {
        
        private const string HEADER = "Band, PCL, TX Power, Target Power, MIN Power, MAX Power, Check Result"; 
        private const string DATA = "SM850, 5, 32.51429, 32.50000, 32.00000, 33.00000, PASS";
        private Mock<IStreamReaderProvider> _streamProviderMock;
        private Mock<IRowParser> _rowParserMock;

        public CsvParserTests() {
            _streamProviderMock = new Mock<IStreamReaderProvider>();


            _rowParserMock = new Mock<IRowParser>();
        }

        [Fact]
        public void Initialize_NullStreamReaderProvider_ThrowsException() {
            Assert.Throws<ArgumentNullException>(() => new CsvParser(null, _rowParserMock.Object));
        }

        [Fact]
        public void Initialize_NullRowParser_ThrowsException() {
            Assert.Throws<ArgumentNullException>(() => new CsvParser(_streamProviderMock.Object, null));
        }

        [Fact]
        public void Parse_HeaderAnd3Rows_ReturnsRows() {
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

                sw.WriteLine(HEADER);
                sw.WriteLine(DATA);
                sw.WriteLine(DATA);
                sw.WriteLine(DATA);
                sw.Flush();
                ms.Position = 0;

                _streamProviderMock.Setup(p => p.StreamReader).Returns(sr);
                _rowParserMock.Setup(m => m.Parse(It.IsAny<string>())).Returns(row);

                var cut = new CsvParser(_streamProviderMock.Object, _rowParserMock.Object);

                var result = cut.Parse();
                var resultFirstRow = result.First();
                Assert.Equal(3, result.Count());
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

                _streamProviderMock.Setup(p => p.StreamReader).Returns(sr);
                _rowParserMock.Setup(m => m.Parse(It.IsAny<string>())).Returns(new Row {});

                var cut = new CsvParser(_streamProviderMock.Object, _rowParserMock.Object);
                var result = cut.Parse();
                Assert.Empty(result);
            } 
        }

        [Fact]
        public void Parse_RowParserThrowsException_ThrowsException() {
            using (var ms = new MemoryStream()) 
            using (var sw = new StreamWriter(ms))
            using (var sr = new StreamReader(ms))
            {
                sw.WriteLine(HEADER);
                sw.WriteLine(DATA);
                sw.Flush();
                ms.Position = 0;

                _streamProviderMock.Setup(p => p.StreamReader).Returns(sr);
                _rowParserMock.Setup(m => m.Parse(It.IsAny<string>())).Throws(new Exception());

                var cut = new CsvParser(_streamProviderMock.Object, _rowParserMock.Object);

                Assert.Throws<Exception>(() => cut.Parse());
            } 
        }
    }

}


