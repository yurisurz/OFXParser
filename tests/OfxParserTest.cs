using Xunit;

namespace OFXParser.Tests
{
    public class OfxParserTests
    {
        private readonly OfxParser _ofxParser;

        public OfxParserTests()
        {
            _ofxParser = new OfxParser();
        }

        [Fact]
        public void Parse_ValidOfxContent_ReturnsNonNullRoot()
        {
            // Arrange
            string ofxContent = "<OFX><SIGNONMSGSRSV1><SONRS><STATUS><CODE>0</CODE></STATUS></SONRS></SIGNONMSGSRSV1></OFX>";

            // Act
            OfxEntry root = _ofxParser.Parse(ofxContent);

            // Assert
            Assert.NotNull(root);
        }

        [Fact]
        public void Parse_InvalidOfxContent_ReturnsNullRoot()
        {
            // Arrange (without OFX tag at the beginning)
            string ofxContent = "<SIGNONMSGSRSV1><SONRS><STATUS><CODE>0</CODE></SONRS></SIGNONMSGSRSV1>";

            // Act
            OfxEntry root = _ofxParser.Parse(ofxContent);

            // Assert
            Assert.Null(root);
        }

        [Fact]
        public void ParseToJSON_ValidOfxContent_ReturnsValidJsonString()
        {
            // Arrange
            string ofxContent = "<OFX><SIGNONMSGSRSV1><SONRS><STATUS><CODE>0</CODE></STATUS></SONRS></SIGNONMSGSRSV1></OFX>";

            // Act
            string json = _ofxParser.ParseToJSON(ofxContent);

            // Assert
            Assert.Equal("{\"OFX\":{\"SIGNONMSGSRSV1\":{\"SONRS\":{\"STATUS\":{\"CODE\":\"0\"}}}}}", json);
        }

        [Fact]
        public void ParseToJSON_InvalidOfxContent_ReturnsEmptyJsonObject()
        {
            // Arrange
            string ofxContent = "<OFX><SIGNONMSGSRSV1><SONRS><STATUS><CODE>0</CODE></SONRS></SIGNONMSGSRSV1>";

            // Act
            Assert.Throws<Exception>(()=>_ofxParser.ParseToJSON(ofxContent));
        }
    }
}