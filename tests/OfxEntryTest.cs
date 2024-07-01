using Xunit;

namespace OFXParser.Tests
{
    public class OfxEntryTests
    {
        [Fact]
        public void OfxEntry_Constructor_WithValues_SetsPropertiesCorrectly()
        {
            // Arrange
            string tag = "TAG";
            object value = "VALUE";

            // Act
            OfxEntry entry = new OfxEntry(tag, value);

            // Assert
            Assert.Equal(tag, entry.Tag);
            Assert.Equal(value, entry.Value);
            Assert.NotNull(entry.Children);
            Assert.Null(entry.Parent);
        }

        [Fact]
        public void OfxEntry_Constructor_WithParent_SetsParentPropertyCorrectly()
        {
            // Arrange
            string tag = "TAG";
            object value = "VALUE";
            OfxEntry parent = new OfxEntry("PARENT", "PARENT_VALUE");

            // Act
            OfxEntry entry = new OfxEntry(tag, value, parent);

            // Assert
            Assert.Equal(tag, entry.Tag);
            Assert.Equal(value, entry.Value);
            Assert.NotNull(entry.Children);
            Assert.Equal(parent, entry.Parent);
        }
    }
}