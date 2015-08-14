using System;
using Xunit;
using Xunit.Extensions;

namespace ken.Core.Types
{
    public class Int64ExtensionsTests
    {
        [Fact]
        public void Int64HasCorrectMaxRange()
        {
            Assert.Equal(Int64.MaxValue, 9223372036854775807);
        }
        
        /// <summary>
        /// http://compoasso.free.fr/primelistweb/page/prime/liste_online_en.php
        /// </summary>
        /// <param name="number"></param>
        /// <param name="isPrime"></param>
        [Theory]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(2, true)]
        [InlineData(3, true)]      
        [InlineData(5, true)]     
        [InlineData(7, true)]    
        [InlineData(9, false)]    
        [InlineData(11, true)]     
        [InlineData(13, true)]     
        [InlineData(17, true)]     
        [InlineData(19, true)]     
        [InlineData(23, true)]     
        [InlineData(29, true)]
        [InlineData(31, true)]     
        [InlineData(37, true)]   
        [InlineData(41, true)]   
        [InlineData(43, true)]   
        [InlineData(47, true)]  
        [InlineData(53, true)]  
        [InlineData(59, true)] 
        [InlineData(61, true)] 
        [InlineData(67, true)] 
        [InlineData(71, true)] 
        [InlineData(353, true)]  
        [InlineData(557, true)]   
        [InlineData(821, true)]
        [InlineData(1021, true)]   
        [InlineData(1031, true)]  
        [InlineData(7717, true)]
        [InlineData(1000000006, false)]
        [InlineData(1000000007, true)]
        [InlineData(1000000009, true)]
        [InlineData(1000000011, false)]
        [InlineData(1000000033, true)]
        [InlineData(1000000123, true)]	
        [InlineData(7182818303, true)]
        [InlineData(7182818307, false)]
        [InlineData(7182818309, true)]
        public void ShouldCorrectlyIdentifyPrimeNumber(Int64 number, bool isPrime)
        {
            Assert.Equal(isPrime, number.IsPrime());
        }
    }
}