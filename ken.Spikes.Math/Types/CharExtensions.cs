using System;

namespace ken.Core.Types
{
    public static class CharExtensions
    {
        public static Int32 ToInt32(this char character)
        {
            return Int32.Parse(character.ToString());
        }
    }
}
