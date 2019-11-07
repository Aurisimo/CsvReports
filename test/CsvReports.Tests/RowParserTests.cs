using System;
using Xunit;
using CsvReports;

namespace CsvReports.Tests
{
    public class RowParserTests
    {
        private RowParser _cut;
        
        public RowParserTests() {
            _cut = new RowParser();
        }
        
        [Theory]
        [InlineData("GSM850, 5, 32.51429, 32.50000, 32.00000, 33.00000, PASS", "GSM850", 5, 32.51429F, 32.50000F, 32.00000F, 33.00000F, true)]
        [InlineData("GSM850, 5, 32.51429, 32.50000, 32.00000, 33.00000, Pass", "GSM850", 5, 32.51429F, 32.50000F, 32.00000F, 33.00000F, true)]
        [InlineData("GSM850, 5, 32.51429, 32.50000, 32.00000, 33.00000, pass", "GSM850", 5, 32.51429F, 32.50000F, 32.00000F, 33.00000F, true)]
        [InlineData("GSM850, 5, 32.51429, 32.50000, 32.00000, 33.00000, FAIL", "GSM850", 5, 32.51429F, 32.50000F, 32.00000F, 33.00000F, false)]
        [InlineData("GSM850,5,32.51429,32.50000,32.00000,33.00000,FAIL", "GSM850", 5, 32.51429F, 32.50000F, 32.00000F, 33.00000F, false)]
        public void Parse_ValidInput_ReturnsRow(
            string input, 
            string band, 
            int pcl, 
            float txPower,
            float targetPower,
            float minPower,
            float maxPower,
            bool isPassed)
        {
            var result = _cut.Parse(input);

            Assert.NotNull(result);
            Assert.Equal(band, result.Band);
            Assert.Equal(pcl, result.Pcl);
            Assert.Equal(txPower, result.TxPower);
            Assert.Equal(targetPower, result.TargetPower);
            Assert.Equal(minPower, result.MinPower);
            Assert.Equal(maxPower, result.MaxPower);
            Assert.Equal(isPassed, result.IsPassed);
        }

        [Fact]
        public void Parse_NullInput_ThrowsException() {
            Assert.Throws<ArgumentNullException>(() => _cut.Parse(null));
        }

        [Theory]
        [InlineData("GSM850, 5, 32.51429, 32.50000, 32.00000, 33.00000, PASS, X")]
        [InlineData("GSM850, 5, 32.51429, 32.50000, 32.00000, 33.00000")]
        [InlineData("GSM850")]
        [InlineData("")]
        public void Parse_InvalidNumberOfColumns_ThrowsException(string input) {
            Assert.Throws<FormatException>(() => _cut.Parse(input));
        }

        [Fact]
        public void Parse_EmptyBand_ThrowsException() {
            var input = ", 5, 32.51429, 32.50000, 32.00000, 33.00000, PASS";
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _cut.Parse(input));
            Assert.Contains(nameof(Row.Band), ex.Message);
        }

        [Theory]
        [InlineData("GSM850, , 32.51429, 32.50000, 32.00000, 33.00000, PASS")]
        [InlineData("GSM850,, 32.51429, 32.50000, 32.00000, 33.00000, PASS")]
        [InlineData("GSM850, X, 32.51429, 32.50000, 32.00000, 33.00000, PASS")]
        [InlineData("GSM850, 2147483648, 32.51429, 32.50000, 32.00000, 33.00000, PASS")]
        [InlineData("GSM850, -2147483649, 32.51429, 32.50000, 32.00000, 33.00000, PASS")]
        public void Parse_InvalidPcl_ThrowsException(string input) {
            var ex = Assert.Throws<FormatException>(() => _cut.Parse(input));
            Assert.Contains(nameof(Row.Pcl), ex.Message);
        }

        [Theory]
        [InlineData("GSM850, 5, , 32.50000, 32.00000, 33.00000, PASS")]
        [InlineData("GSM850, 5,, 32.50000, 32.00000, 33.00000, PASS")]
        [InlineData("GSM850, 5, 32 .50000, 32.50000, 32.00000, 33.00000, PASS")]
        [InlineData("GSM850, 5, 32_50000, 32.50000, 32.00000, 33.00000, PASS")]
        [InlineData("GSM850, 5, X, 32.50000, 32.00000, 33.00000, PASS")]
        public void Parse_InvalidTxPower_ThrowsException(string input) {
            var ex = Assert.Throws<FormatException>(() => _cut.Parse(input));
            Assert.Contains(nameof(Row.TxPower), ex.Message);
        }

        [Fact]
        public void Parse_InvalidTargetPower_ThrowsException() {
            var input = "GSM850, 5, 32.51429, X, 32.00000, 33.00000, PASS";
            var ex = Assert.Throws<FormatException>(() => _cut.Parse(input));
            Assert.Contains(nameof(Row.TargetPower), ex.Message);
        }

        [Fact]
        public void Parse_InvalidMinPower_ThrowsException() {
            var input = "GSM850, 5, 32.51429, 32.00000, X, 33.00000, PASS";
            var ex = Assert.Throws<FormatException>(() => _cut.Parse(input));
            Assert.Contains(nameof(Row.MinPower), ex.Message);
        }

        [Fact]
        public void Parse_InvalidMaxPower_ThrowsException() {
            var input = "GSM850, 5, 32.51429, 32.00000, 32.00000, X, PASS";
            var ex = Assert.Throws<FormatException>(() => _cut.Parse(input));
            Assert.Contains(nameof(Row.MaxPower), ex.Message);
        }

        [Theory]
        [InlineData("GSM850, 5, 32.51429, 32.50000, 32.00000, 33.00000,")]
        [InlineData("GSM850, 5, 32.51429, 32.50000, 32.00000, 33.00000, ")]
        [InlineData("GSM850, 5, 32.51429, 32.50000, 32.00000, 33.00000, X")]
        public void Parse_InvalidIsPassed_ThrowsException(string input) {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _cut.Parse(input));
            Assert.Contains(nameof(Row.IsPassed), ex.Message);
        }

    }
    
}

