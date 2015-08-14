using System.Numerics;
using Xunit;

namespace ken.Core.Types
{
    public class BigIntegerTests
    {
        [Fact]
        public void CanParseBigIntegerFromString()
        {
            var number = BigInteger.Parse("123456789");
            var expected = new BigInteger(123456789);
            Assert.Equal(expected, number);
        }

        [Fact]
        public void CanAddBigIntegers()
        {
            var number = BigInteger.Parse("18446744073709551615");
            var number2 = BigInteger.Parse("1");
            var expected = BigInteger.Parse("18446744073709551616");
            Assert.Equal(expected, number + number2);
        }
    }
}