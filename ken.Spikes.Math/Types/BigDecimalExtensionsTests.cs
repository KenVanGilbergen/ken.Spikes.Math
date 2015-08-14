using System.Numerics;
using Xunit;
using Xunit.Extensions;

namespace ken.Core.Types
{
    public class BigDecimalExtensionsTests
    {
        public class FactoralTests
        {
            [Theory]
            [InlineData(0, 1)]
            [InlineData(1, 1)]
            [InlineData(2, 2)]
            [InlineData(3, 6)]
            [InlineData(4, 24)]
            [InlineData(5, 120)]
            public void CanCalculateFactoral(int integer, int factoral)
            {
                BigDecimal number = integer;
                Assert.Equal(factoral, number.Factoral());
            }

            [Fact]
            public void CanCalculateBigFactoral()
            {
                var number = BigDecimal.Parse("100");
                var expected =
                    BigDecimal.Parse(
                        "93326215443944152681699238856266700490715968264381621468592963895217599993229915608941463976156518286253697920827223758251185210916864000000000000000000000000");
                Assert.Equal(expected, number.Factoral());
            }
        }
    }
}