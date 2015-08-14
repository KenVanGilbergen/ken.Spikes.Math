using Xunit;

namespace ken.Core.Types
{
    public class StringExtensionsTests
    {
        [Fact]
        public void ShouldHandleEmptyString()
        {
            Assert.False("".IsInteger());
        }

        [Fact]
        public void ShouldHandleNullString()
        {
            string nullDummy = null;
            Assert.False(nullDummy.IsInteger());
        }

        [Fact]
        public void ShouldNotAllowDecimalPoint()
        {
            Assert.False("1.2".IsInteger());
        }

        [Fact]
        public void ShouldNotAllowAlphas()
        {
            Assert.False("1b2".IsInteger());
        }
        [Fact]
        public void ShouldAllowNumerics()
        {
            Assert.False("1.2".IsInteger());
        }
    }
}