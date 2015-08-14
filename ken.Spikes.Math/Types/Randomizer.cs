using System.Numerics;

namespace ken.Core.Types
{
    public static class Randomizer
    {
        public static BigInteger GetBigInteger(BigInteger min, BigInteger max)
        {
            var upper = max - min;
            return min + GetBigIntegerBelow(upper);
        }

        public static BigInteger GetBigIntegerBelow(BigInteger bound)
        {
            var source = System.Security.Cryptography.RandomNumberGenerator.Create();

            //Get a byte buffer capable of holding any value below the bound
            var buffer = (bound << 16).ToByteArray(); // << 16 adds two bytes, which decrease the chance of a retry later on

            //Compute where the last partial fragment starts, in order to retry if we end up in it
            var generatedValueBound = BigInteger.One << (buffer.Length * 8 - 1); //-1 accounts for the sign bit
            var validityBound = generatedValueBound - generatedValueBound % bound;

            while (true)
            {
                //generate a uniformly random value in [0, 2^(buffer.Length * 8 - 1))
                source.GetBytes(buffer);
                buffer[buffer.Length - 1] &= 0x7F; //force sign bit to positive
                var r = new BigInteger(buffer);

                //return unless in the partial fragment
                if (r >= validityBound) continue;
                return r % bound;
            }
        }
    }
}