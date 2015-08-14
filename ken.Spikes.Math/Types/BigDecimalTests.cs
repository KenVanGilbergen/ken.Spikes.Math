using System;
using ken.Core.Math;
using Xunit;

namespace ken.Core.Types
{
    public class BigDecimalTests
    {
        [Fact]
        public void CanHoldZero()
        {
            BigDecimal number = 0;
            Assert.Equal(0, number);
        }

        [Fact]
        public void CanParseIntegerFromString()
        {
            var number = BigDecimal.Parse("123456789");
            BigDecimal expected = 123456789;
            Assert.Equal(expected, number);
        }

        [Fact]
        public void CanParseDecimalFromString()
        {
            var number = BigDecimal.Parse("123456789.444555");
            BigDecimal expected = 123456789.444555;
            Assert.Equal(expected, number);
        }

        [Fact]
        public void CanParseExponentFromString()
        {
            var number = BigDecimal.Parse("123E7");
            BigDecimal expected = 1230000000;
            Assert.Equal(expected, number);
        }

        [Fact]
        public void CanParseBiggerExponentFromString()
        {
            var number = BigDecimal.Parse("123E+10");
            var expected = new BigDecimal(123, 10);
            Assert.Equal(expected, number);
        }

        [Fact]
        public void CanParseNegativeExponentFromString()
        {
            var number = BigDecimal.Parse("123456789E-1");
            BigDecimal expected = 12345678.9;
            Assert.Equal(expected, number);
        }

        [Fact]
        public void CanAddBigIntegers()
        {
            var number = BigDecimal.Parse("18446744073709551615");
            var number2 = BigDecimal.Parse("1");
            var expected = BigDecimal.Parse("18446744073709551616");
            Assert.Equal(expected, number + number2);
        }

        [Fact]
        public void ShouldApproximateE()
        {
            var digits = BigDecimal.Precision;
            var expected = BigDecimal.Parse(StringConstants.Math.e.Left(digits + 1));

            var sut = BigDecimal.E();
            var result = sut.Truncate(digits);
            Console.WriteLine(sut);

            Assert.Equal(expected, result);
        }
    }
}