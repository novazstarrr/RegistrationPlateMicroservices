using Xunit;
using Catalog.Domain.Helpers;
using System.Text.RegularExpressions;

namespace Catalog.UnitTests.Helpers
{
    public class PlateMatchingHelperTests
    {
        [Theory]
        [InlineData("ABC", "ABC123")]
        [InlineData("BC1", "ABC123")]
        [InlineData("123", "ABC123")]
        public void BuildRegexPattern_CreatesValidPattern(string searchTerm, string plateNumber)
        {
            // Act
            var pattern = PlateMatchingHelper.BuildRegexPattern(searchTerm);

            // Assert
            Assert.Matches(pattern, plateNumber);
        }

        [Theory]
        [InlineData("A", "4")]
        [InlineData("4", "A")]
        [InlineData("Z", "2")]
        [InlineData("2", "Z")]
        public void BuildRegexPattern_MatchesEquivalentCharacters(string searchTerm, string plateNumber)
        {
            // Act
            var pattern = PlateMatchingHelper.BuildRegexPattern(searchTerm);

            // Assert
            Assert.Matches(pattern, plateNumber);
        }

        [Theory]
        [InlineData("ABC", "DEF123")]
        [InlineData("123", "456")]
        [InlineData("XYZ", "123")]
        public void BuildRegexPattern_DoesNotMatchInvalidPlates(string searchTerm, string plateNumber)
        {
            // Act
            var pattern = PlateMatchingHelper.BuildRegexPattern(searchTerm);

            // Assert
            Assert.DoesNotMatch(pattern, plateNumber);
        }

        [Theory]
        [InlineData("A B C", "ABC123")]
        [InlineData("abc", "ABC123")]
        [InlineData(" ABC ", "ABC123")]
        public void BuildRegexPattern_HandlesInputCleaning(string searchTerm, string plateNumber)
        {
            // Act
            var pattern = PlateMatchingHelper.BuildRegexPattern(searchTerm);

            // Assert
            Assert.Matches(pattern, plateNumber);
        }

        [Fact]
        public void BuildRegexPattern_WithEmptyInput_ReturnsEmptyString()
        {
            // Act
            var pattern = PlateMatchingHelper.BuildRegexPattern("");

            // Assert
            Assert.Equal(string.Empty, pattern);
        }

        [Fact]
        public void BuildRegexPattern_WithWhitespaceInput_ReturnsEmptyString()
        {
            // Act
            var pattern = PlateMatchingHelper.BuildRegexPattern("   ");

            // Assert
            Assert.Equal(string.Empty, pattern);
        }
    }
}