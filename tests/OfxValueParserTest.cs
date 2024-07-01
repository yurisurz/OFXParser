using Xunit;

namespace OFXParser.Tests
{
    public class OfxValueParserTests
    {
        private readonly OfxValueParser _ofxValueParser;

        public OfxValueParserTests()
        {
            _ofxValueParser = new OfxValueParser();
        }

        [Fact]
        public void ParseValue_DateTag_ReturnsDateTime()
        {
            // Arrange
            string tagName = "DTSTART";
            string value = "20240408100000[-03:EST]";

            // Act
            object result = _ofxValueParser.ParseValue(tagName, value);

            // Assert
            Assert.IsType<DateTime>(result);
            Assert.Equal(new DateTime(2024, 04, 08), result);
        }

        [Fact]
        public void ParseValue_AmountTag_ReturnsDecimal()
        {
            // Arrange
            string tagName = "TRNAMT";
            string value = "100.50";

            // Act
            object result = _ofxValueParser.ParseValue(tagName, value);

            // Assert
            Assert.IsType<decimal>(result);
            Assert.Equal(100.50m, result);
        }

        [Fact]
        public void ParseValue_StringTag_ReturnsString()
        {
            // Arrange
            string tagName = "NAME";
            string value = "John Doe";

            // Act
            object result = _ofxValueParser.ParseValue(tagName, value);

            // Assert
            Assert.IsType<string>(result);
            Assert.Equal("John Doe", result);
        }

        [Fact]
        public void ParseValue_UnknownTag_ReturnsString()
        {
            // Arrange
            string tagName = "UNKNOWN";
            string value = "12345";

            // Act
            object result = _ofxValueParser.ParseValue(tagName, value);

            // Assert
            Assert.IsType<string>(result);
            Assert.Equal("12345", result);
        }
    }
}