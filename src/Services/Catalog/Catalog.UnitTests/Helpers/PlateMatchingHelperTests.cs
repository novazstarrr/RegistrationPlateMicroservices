using Xunit;
using Catalog.Domain.Helpers;

namespace Catalog.UnitTests.Helpers
{
    public class PlateMatchingHelperTests
    {
        [Theory]
        [InlineData("ABC", "%[A4]%B%C%")]
        [InlineData("BC1", "%B%C%[I1]%")]
        [InlineData("123", "%[I1]%[Z2]%[E3]%")]
        public void BuildSqlLikePattern_CreatesValidPattern(string searchTerm, string expected)
        {
            // Act
            var pattern = PlateMatchingHelper.BuildSqlLikePattern(searchTerm);

            // Assert
            Assert.Equal(expected, pattern);
        }

        [Theory]
        [InlineData("A", "%[A4]%")]
        [InlineData("4", "%[A4]%")]
        [InlineData("Z", "%[Z2]%")]
        [InlineData("2", "%[Z2]%")]
        public void BuildSqlLikePattern_HandlesEquivalentCharacters(string searchTerm, string expected)
        {
            // Act
            var pattern = PlateMatchingHelper.BuildSqlLikePattern(searchTerm);

            // Assert
            Assert.Equal(expected, pattern);
        }

        [Theory]
        [InlineData("A B C", "%[A4]%B%C%")]
        [InlineData("abc", "%[A4]%B%C%")]
        [InlineData(" ABC ", "%[A4]%B%C%")]
        public void BuildSqlLikePattern_HandlesInputCleaning(string searchTerm, string expected)
        {
            // Act
            var pattern = PlateMatchingHelper.BuildSqlLikePattern(searchTerm);

            // Assert
            Assert.Equal(expected, pattern);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void BuildSqlLikePattern_WithEmptyInput_ReturnsWildcard(string searchTerm)
        {
            // Act
            var pattern = PlateMatchingHelper.BuildSqlLikePattern(searchTerm);

            // Assert
            Assert.Equal("%", pattern);
        }
    }
}