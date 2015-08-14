using System.Numerics;
using Xunit;
using Xunit.Extensions;

namespace ken.Core.Types
{
    public class BigIntegerExtensionsTests
    {
        [Fact]
        public void ShouldStoreLargerThanInt64()
        {
            var bigInt = BigInteger.Parse("9223372036854775808");
            Assert.True(bigInt.IsEven);
        }

        public class IsPrimeTests
        {
            [Fact]
            public void ZeroIsNoPrime()
            {
                Assert.False(BigInteger.Parse("0").IsPrime());
            }

            [Fact]
            public void OneIsNoPrime()
            {
                Assert.False(BigInteger.Parse("1").IsPrime());
            }

            [Theory]
            [InlineData("2")]
            [InlineData("3")]
            [InlineData("5")]
            [InlineData("7")]
            [InlineData("11")]
            [InlineData("13")]
            [InlineData("17")]
            [InlineData("19")]
            [InlineData("23")]
            [InlineData("29")]
            [InlineData("31")]
            [InlineData("37")]
            [InlineData("41")]
            [InlineData("43")]
            [InlineData("47")]
            [InlineData("53")]
            [InlineData("59")]
            [InlineData("61")]
            [InlineData("67")]
            [InlineData("71")]
            [InlineData("73")]
            [InlineData("79")]
            [InlineData("83")]
            [InlineData("89")]
            [InlineData("97")]
            [InlineData("101")]
            [InlineData("103")]
            [InlineData("107")]
            [InlineData("109")]
            [InlineData("113")]
            [InlineData("127")]
            [InlineData("131")]
            [InlineData("137")]
            [InlineData("139")]
            [InlineData("149")]
            [InlineData("151")]
            [InlineData("157")]
            [InlineData("163")]
            [InlineData("167")]
            [InlineData("173")]
            [InlineData("179")]
            [InlineData("181")]
            [InlineData("191")]
            [InlineData("193")]
            [InlineData("197")]
            [InlineData("199")]
            [InlineData("211")]
            [InlineData("223")]
            [InlineData("227")]
            [InlineData("229")]
            [InlineData("233")]
            [InlineData("239")]
            [InlineData("241")]
            [InlineData("251")]
            [InlineData("257")]
            [InlineData("263")]
            [InlineData("269")]
            [InlineData("271")]
            [InlineData("277")]
            [InlineData("281")]
            [InlineData("283")]
            [InlineData("293")]
            [InlineData("307")]
            public void FirstXPrimes(string prime)
            {
                Assert.True(BigInteger.Parse(prime).IsPrime());
            }

            [Fact]
            public void A1031IsPrime()
            {
                Assert.True(BigInteger.Parse("1031").IsPrime());
            }

            [Fact]
            public void A7717IsPrime()
            {
                Assert.True(BigInteger.Parse("7717").IsPrime());
            }

            [Fact]
            public void BigIntegerWith10DigitsThatIsPrime()
            {
                Assert.True(BigInteger.Parse("1000000007").IsPrime());
            }

            [Fact]
            public void Int64ThatIsNoPrime()
            {
                Assert.False(BigInteger.Parse("7182818307").IsPrime());
            }

            [Fact]
            public void Int64ThatIsPrime()
            {
                Assert.True(BigInteger.Parse("7182818309").IsPrime());
            }

            [Fact]
            public void BigIntegerThatIsNoPrime()
            {
                Assert.False(BigInteger.Parse("9223372036854775808").IsPrime());
            }

            [Fact]
            public void BigIntegerWith20DigitsThatIsPrime()
            {
                // https://primes.utm.edu/lists/small/small.html
                Assert.True(BigInteger.Parse("12764787846358441471").IsPrime());
            }

            [Fact]
            public void BigIntegerWith20DigitsThatIsNoPrime()
            {
                Assert.False(BigInteger.Parse("12764787846358441473").IsPrime());
            }

            [Fact]
            public void BigIntegerWith30DigitsThatIsPrime()
            {
                // https://primes.utm.edu/lists/small/small.html
                Assert.True(BigInteger.Parse("671998030559713968361666935769").IsPrime());
            }

            [Fact]
            public void BigIntegerWith30DigitsThatIsNoPrime()
            {
                Assert.False(BigInteger.Parse("671998030559713968361666935771").IsPrime());
            }
        }

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
                var number = new BigInteger(integer);
                Assert.Equal(factoral, number.Factoral());
            }

            [Fact]
            public void CanCalculateBigFactoral()
            {
                var number = BigInteger.Parse("100");
                var expected =
                    BigInteger.Parse(
                        "93326215443944152681699238856266700490715968264381621468592963895217599993229915608941463976156518286253697920827223758251185210916864000000000000000000000000");
                Assert.Equal(expected, number.Factoral());
            }
        }
    }
}