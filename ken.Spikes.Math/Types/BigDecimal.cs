using System;
using System.Numerics;
using System.Text;

namespace ken.Core.Types
{
    /// <summary>
    /// Arbitrary precision decimal.
    /// All operations are exact, except for division. Division never determines more digits than the given precision.
    /// Based on http://stackoverflow.com/a/4524254
    /// Author: Jan Christoph Bernack (contact: jc.bernack at googlemail.com)
    /// Updates: Ken Van Gilbergen
    /// </summary>
    public struct BigDecimal: IComparable, IComparable<BigDecimal>
    {
        /// <summary>
        /// Specifies whether the significant digits should be truncated to the given precision after each operation.
        /// </summary>
        public static bool AlwaysTruncate = false;

        /// <summary>
        /// Sets the maximum precision of division operations.
        /// If AlwaysTruncate is set to true all operations are affected.
        /// </summary>
        public static int Precision = 1000;

        public BigInteger Mantissa { get; set; }
        public int Exponent { get; set; }

        private enum ParseState
        {
            Start,
            Integer,
            Decimal,
            E,
            Exponent,
        }

        public static BigDecimal Parse(string str)
        {
            if (str == null) throw new ArgumentNullException("str", "BigDecimal.Parse: Cannot parse null");
            ushort scale = (ushort)0;
            var stringBuilder = new StringBuilder();
            var exponentBuilder = (StringBuilder)null;
            var state = BigDecimal.ParseState.Start;
            Action<char> action1 = (Action<char>)(c =>
            {
                throw new FormatException(string.Concat(new object[4]
                        {
                            (object) "BigDecimal.Parse: invalid character '",
                            (object) c,
                            (object) "' in: ",
                            (object) str
                        }));
            });
            Action action2 = (Action)(() =>
            {
                exponentBuilder = new StringBuilder();
                state = ParseState.E;
            });
            foreach (char c in str)
            {
                switch (state)
                {
                    case ParseState.Start:
                        if (char.IsDigit(c) || (int)c == 45 || (int)c == 43)
                        {
                            state = BigDecimal.ParseState.Integer;
                            stringBuilder.Append(c);
                            break;
                        }
                        if ((int)c == 46)
                        {
                            state = ParseState.Decimal;
                            break;
                        }
                        action1(c);
                        break;
                    case ParseState.Integer:
                        if (char.IsDigit(c))
                        {
                            stringBuilder.Append(c);
                            break;
                        }
                        if ((int)c == 46)
                        {
                            state = ParseState.Decimal;
                            break;
                        }
                        if ((int)c == 101 || (int)c == 69)
                        {
                            action2();
                            break;
                        }
                        action1(c);
                        break;
                    case ParseState.Decimal:
                        if (char.IsDigit(c))
                        {
                            checked
                            {
                                ++scale;
                            }
                            stringBuilder.Append(c);
                            break;
                        }
                        if ((int)c == 101 || (int)c == 69)
                        {
                            action2();
                            break;
                        }
                        action1(c);
                        break;
                    case ParseState.E:
                        if (char.IsDigit(c) || (int)c == 45 || (int)c == 43)
                        {
                            state = BigDecimal.ParseState.Exponent;
                            exponentBuilder.Append(c);
                            break;
                        }
                        action1(c);
                        break;
                    case ParseState.Exponent:
                        if (char.IsDigit(c))
                        {
                            exponentBuilder.Append(c);
                            break;
                        }
                        action1(c);
                        break;
                }
            }
            if (stringBuilder.Length == 0 || stringBuilder.Length == 1 && !char.IsDigit(stringBuilder[0]))
                throw new FormatException("BigDecimal.Parse: string didn't contain a value: \"" + str + "\"");
            if (exponentBuilder != null &&
                (exponentBuilder.Length == 0 || stringBuilder.Length == 1 && !char.IsDigit(stringBuilder[0])))
                throw new FormatException("BigDecimal.Parse: string contained an 'E' but no exponent value: \"" + str +
                                          "\"");
            var bigInteger = BigInteger.Parse(stringBuilder.ToString());
            if (exponentBuilder != null)
            {
                int num1 = int.Parse(exponentBuilder.ToString());
                if (num1 > 0)
                {
                    if (num1 <= (int)scale)
                    {
                        scale -= (ushort)num1;
                    }
                    else
                    {
                        int exponent = num1 - (int)scale;
                        scale = (ushort)0;
                        bigInteger *= BigInteger.Pow((BigInteger)10, exponent);
                    }
                }
                else if (num1 < 0)
                {
                    int num2 = -num1 + (int)scale;
                    if (num2 <= (int)ushort.MaxValue)
                    {
                        scale = (ushort)num2;
                    }
                    else
                    {
                        scale = ushort.MaxValue;
                        bigInteger /= BigInteger.Pow((BigInteger)10, num2 - (int)ushort.MaxValue);
                    }
                }
            }
            return new BigDecimal(bigInteger, -scale);
        }

        public BigDecimal(BigInteger mantissa, int exponent): this()
        {
            Mantissa = mantissa;
            Exponent = exponent;
            Normalize();
            if (AlwaysTruncate)
            {
                Truncate();
            }
        }

        /// <summary>
        /// Removes trailing zeros on the mantissa
        /// </summary>
        public void Normalize()
        {
            if (Mantissa.IsZero)
            {
                Exponent = 0;
            }
            else
            {
                BigInteger remainder = 0;
                while (remainder == 0)
                {
                    var shortened = BigInteger.DivRem(Mantissa, 10, out remainder);
                    if (remainder == 0)
                    {
                        Mantissa = shortened;
                        Exponent++;
                    }
                }
            }
        }

        /// <summary>
        /// Truncate the number to the given precision by removing the least significant digits.
        /// </summary>
        /// <returns>The truncated number</returns>
        public BigDecimal Truncate(int precision)
        {
            // copy this instance (remember its a struct)
            var shortened = this;
            // save some time because the number of digits is not needed to remove trailing zeros
            shortened.Normalize();
            // remove the least significant digits, as long as the number of digits is higher than the given Precision
            while (NumberOfDigits(shortened.Mantissa) > precision)
            {
                shortened.Mantissa /= 10;
                shortened.Exponent++;
            }
            return shortened;
        }

        public BigDecimal Truncate()
        {
            return Truncate(Precision);
        }

        private static int NumberOfDigits(BigInteger value)
        {
            // do not count the sign
            return (value * value.Sign).ToString().Length;
        }

        #region Conversions

        public static implicit operator BigDecimal(int value)
        {
            return new BigDecimal(value, 0);
        }

        public static implicit operator BigDecimal(double value)
        {
            var mantissa = (BigInteger)value;
            var exponent = 0;
            double scaleFactor = 1;
            while (System.Math.Abs(value * scaleFactor - (double)mantissa) > 0)
            {
                exponent -= 1;
                scaleFactor *= 10;
                mantissa = (BigInteger)(value * scaleFactor);
            }
            return new BigDecimal(mantissa, exponent);
        }

        public static implicit operator BigDecimal(decimal value)
        {
            var mantissa = (BigInteger)value;
            var exponent = 0;
            decimal scaleFactor = 1;
            while ((decimal)mantissa != value * scaleFactor)
            {
                exponent -= 1;
                scaleFactor *= 10;
                mantissa = (BigInteger)(value * scaleFactor);
            }
            return new BigDecimal(mantissa, exponent);
        }

        public static explicit operator double(BigDecimal value)
        {
            return (double)value.Mantissa * System.Math.Pow(10, value.Exponent);
        }

        public static explicit operator float(BigDecimal value)
        {
            return Convert.ToSingle((double)value);
        }

        public static explicit operator decimal(BigDecimal value)
        {
            return (decimal)value.Mantissa * (decimal)System.Math.Pow(10, value.Exponent);
        }

        public static explicit operator int(BigDecimal value)
        {
            return (int)(value.Mantissa * BigInteger.Pow(10, value.Exponent));
        }

        public static explicit operator uint(BigDecimal value)
        {
            return (uint)(value.Mantissa * BigInteger.Pow(10, value.Exponent));
        }

        #endregion

        #region Operators

        public static BigDecimal operator +(BigDecimal value)
        {
            return value;
        }

        public static BigDecimal operator -(BigDecimal value)
        {
            value.Mantissa *= -1;
            return value;
        }

        public static BigDecimal operator ++(BigDecimal value)
        {
            return value + 1;
        }

        public static BigDecimal operator --(BigDecimal value)
        {
            return value - 1;
        }

        public static BigDecimal operator +(BigDecimal left, BigDecimal right)
        {
            return Add(left, right);
        }

        public static BigDecimal operator -(BigDecimal left, BigDecimal right)
        {
            return Add(left, -right);
        }

        private static BigDecimal Add(BigDecimal left, BigDecimal right)
        {
            return left.Exponent > right.Exponent
                ? new BigDecimal(AlignExponent(left, right) + right.Mantissa, right.Exponent)
                : new BigDecimal(AlignExponent(right, left) + left.Mantissa, left.Exponent);
        }

        public static BigDecimal operator *(BigDecimal left, BigDecimal right)
        {
            return new BigDecimal(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent);
        }

        public static BigDecimal operator /(BigDecimal dividend, BigDecimal divisor)
        {
            var exponentChange = Precision - (NumberOfDigits(dividend.Mantissa) - NumberOfDigits(divisor.Mantissa));
            if (exponentChange < 0)
            {
                exponentChange = 0;
            }
            dividend.Mantissa *= BigInteger.Pow(10, exponentChange);
            return new BigDecimal(dividend.Mantissa / divisor.Mantissa, dividend.Exponent - divisor.Exponent - exponentChange);
        }

        public static bool operator ==(BigDecimal left, BigDecimal right)
        {
            return left.Exponent == right.Exponent && left.Mantissa == right.Mantissa;
        }

        public static bool operator !=(BigDecimal left, BigDecimal right)
        {
            return left.Exponent != right.Exponent || left.Mantissa != right.Mantissa;
        }

        public static bool operator <(BigDecimal left, BigDecimal right)
        {
            return left.Exponent > right.Exponent ? AlignExponent(left, right) < right.Mantissa : left.Mantissa < AlignExponent(right, left);
        }

        public static bool operator >(BigDecimal left, BigDecimal right)
        {
            return left.Exponent > right.Exponent ? AlignExponent(left, right) > right.Mantissa : left.Mantissa > AlignExponent(right, left);
        }

        public static bool operator <=(BigDecimal left, BigDecimal right)
        {
            return left.Exponent > right.Exponent ? AlignExponent(left, right) <= right.Mantissa : left.Mantissa <= AlignExponent(right, left);
        }

        public static bool operator >=(BigDecimal left, BigDecimal right)
        {
            return left.Exponent > right.Exponent ? AlignExponent(left, right) >= right.Mantissa : left.Mantissa >= AlignExponent(right, left);
        }

        /// <summary>
        /// Returns the mantissa of value, aligned to the exponent of reference.
        /// Assumes the exponent of value is larger than of reference.
        /// </summary>
        private static BigInteger AlignExponent(BigDecimal value, BigDecimal reference)
        {
            return value.Mantissa * BigInteger.Pow(10, value.Exponent - reference.Exponent);
        }

        #endregion

        #region Additional mathematical functions

        public static BigDecimal Exp(double exponent)
        {
            var tmp = (BigDecimal)1;
            while (System.Math.Abs(exponent) > 100)
            {
                var diff = exponent > 0 ? 100 : -100;
                tmp *= System.Math.Exp(diff);
                exponent -= diff;
            }
            return tmp * System.Math.Exp(exponent);
        }

        public static BigDecimal Pow(double basis, double exponent)
        {
            var tmp = (BigDecimal)1;
            while (System.Math.Abs(exponent) > 100)
            {
                var diff = exponent > 0 ? 100 : -100;
                tmp *= System.Math.Pow(basis, diff);
                exponent -= diff;
            }
            return tmp * System.Math.Pow(basis, exponent);
        }

        #endregion

        public override string ToString()
        {
            return string.Concat(Mantissa.ToString(), "E", Exponent);
        }

        public bool Equals(BigDecimal other)
        {
            return other.Mantissa.Equals(Mantissa) && other.Exponent == Exponent;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is BigDecimal && Equals((BigDecimal)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Mantissa.GetHashCode() * 397) ^ Exponent;
            }
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(obj, null) || !(obj is BigDecimal))
            {
                throw new ArgumentException();
            }
            return CompareTo((BigDecimal)obj);
        }

        public int CompareTo(BigDecimal other)
        {
            return this < other ? -1 : (this > other ? 1 : 0);
        }

        public static BigDecimal E()
        {
            return E(BigDecimal.Precision);
        }

        public static BigDecimal E(int maxDenominator)
        {
            BigDecimal e = 2.5;
            BigDecimal numerator = 1;
            for (BigDecimal denominator = 3; denominator <= maxDenominator; denominator++)
            {
                var fraction = numerator/denominator.Factoral();
                e += fraction;
            }
            return e;
        }
    }
}

// Product you can buy with missing pieces for math: http://www.extremeoptimization.com/Default.aspx

//    public struct BigDecimal
//    {
//        private readonly BigInteger _value;
//        private readonly ushort _scale;

//        public BigDecimal(float value)
//        {
//            this = BigDecimal.Parse(value.ToString("R"));
//        }

//        public BigDecimal(double value)
//        {
//            this = BigDecimal.Parse(value.ToString("R"));
//        }

//        public BigDecimal(Decimal value)
//        {
//            this = BigDecimal.Parse(value.ToString());
//        }

//        public BigDecimal(long value)
//        {
//            this = new BigDecimal((BigInteger) value);
//        }

//        public BigDecimal(ulong value)
//        {
//            this = new BigDecimal((BigInteger) value);
//        }

//        public BigDecimal(BigInteger value)
//        {
//            this = new BigDecimal(value, (ushort) 0);
//        }

//        public BigDecimal(BigInteger value, ushort scale)
//        {
//            this._value = value;
//            this._scale = scale;
//        }

//        public static implicit operator BigDecimal(sbyte value)
//        {
//            return new BigDecimal((long) value);
//        }

//        public static implicit operator BigDecimal(byte value)
//        {
//            return new BigDecimal((long) value);
//        }

//        public static implicit operator BigDecimal(short value)
//        {
//            return new BigDecimal((long) value);
//        }

//        public static implicit operator BigDecimal(ushort value)
//        {
//            return new BigDecimal((long) value);
//        }

//        public static implicit operator BigDecimal(int value)
//        {
//            return new BigDecimal((long) value);
//        }

//        public static implicit operator BigDecimal(uint value)
//        {
//            return new BigDecimal((long) value);
//        }

//        public static implicit operator BigDecimal(long value)
//        {
//            return new BigDecimal(value);
//        }

//        public static implicit operator BigDecimal(ulong value)
//        {
//            return new BigDecimal(value);
//        }

//        public static implicit operator BigDecimal(Decimal value)
//        {
//            return new BigDecimal(value);
//        }

//        public static implicit operator BigDecimal(BigInteger value)
//        {
//            return new BigDecimal(value);
//        }

//        public static BigDecimal operator +(BigDecimal left, BigDecimal right)
//        {
//            ushort scale = BigDecimal.SameScale(ref left, ref right);
//            return new BigDecimal(left._value + right._value, scale);
//        }

//        public static BigDecimal operator -(BigDecimal left, BigDecimal right)
//        {
//            ushort scale = BigDecimal.SameScale(ref left, ref right);
//            return new BigDecimal(left._value - right._value, scale);
//        }

//        public static BigDecimal operator *(BigDecimal left, BigDecimal right)
//        {
//            BigInteger bigInteger = left._value*right._value;
//            int num = (int) left._scale + (int) right._scale;
//            if (num > (int) ushort.MaxValue)
//            {
//                bigInteger /= BigInteger.Pow((BigInteger) 10, num - (int) ushort.MaxValue);
//                num = (int) ushort.MaxValue;
//            }
//            return new BigDecimal(bigInteger, (ushort) num);
//        }

//        public static BigDecimal Parse(string str)
//        {
//            if (str == null)
//                throw new ArgumentNullException("str", "BigDecimal.Parse: Cannot parse null");
//            ushort scale = (ushort) 0;
//            StringBuilder stringBuilder = new StringBuilder();
//            StringBuilder exponentBuilder = (StringBuilder) null;
//            BigDecimal.ParseState state = BigDecimal.ParseState.Start;
//            Action<char> action1 = (Action<char>) (c =>
//            {
//                throw new FormatException(string.Concat(new object[4]
//                {
//                    (object) "BigDecimal.Parse: invalid character '",
//                    (object) c,
//                    (object) "' in: ",
//                    (object) str
//                }));
//            });
//            Action action2 = (Action) (() =>
//            {
//                exponentBuilder = new StringBuilder();
//                state = BigDecimal.ParseState.E;
//            });
//            foreach (char c in str)
//            {
//                switch (state)
//                {
//                    case BigDecimal.ParseState.Start:
//                        if (char.IsDigit(c) || (int) c == 45 || (int) c == 43)
//                        {
//                            state = BigDecimal.ParseState.Integer;
//                            stringBuilder.Append(c);
//                            break;
//                        }
//                        if ((int) c == 46)
//                        {
//                            state = BigDecimal.ParseState.Decimal;
//                            break;
//                        }
//                        action1(c);
//                        break;
//                    case BigDecimal.ParseState.Integer:
//                        if (char.IsDigit(c))
//                        {
//                            stringBuilder.Append(c);
//                            break;
//                        }
//                        if ((int) c == 46)
//                        {
//                            state = BigDecimal.ParseState.Decimal;
//                            break;
//                        }
//                        if ((int) c == 101 || (int) c == 69)
//                        {
//                            action2();
//                            break;
//                        }
//                        action1(c);
//                        break;
//                    case BigDecimal.ParseState.Decimal:
//                        if (char.IsDigit(c))
//                        {
//                            checked
//                            {
//                                ++scale;
//                            }
//                            stringBuilder.Append(c);
//                            break;
//                        }
//                        if ((int) c == 101 || (int) c == 69)
//                        {
//                            action2();
//                            break;
//                        }
//                        action1(c);
//                        break;
//                    case BigDecimal.ParseState.E:
//                        if (char.IsDigit(c) || (int) c == 45 || (int) c == 43)
//                        {
//                            state = BigDecimal.ParseState.Exponent;
//                            exponentBuilder.Append(c);
//                            break;
//                        }
//                        action1(c);
//                        break;
//                    case BigDecimal.ParseState.Exponent:
//                        if (char.IsDigit(c))
//                        {
//                            exponentBuilder.Append(c);
//                            break;
//                        }
//                        action1(c);
//                        break;
//                }
//            }
//            if (stringBuilder.Length == 0 || stringBuilder.Length == 1 && !char.IsDigit(stringBuilder[0]))
//                throw new FormatException("BigDecimal.Parse: string didn't contain a value: \"" + str + "\"");
//            if (exponentBuilder != null &&
//                (exponentBuilder.Length == 0 || stringBuilder.Length == 1 && !char.IsDigit(stringBuilder[0])))
//                throw new FormatException("BigDecimal.Parse: string contained an 'E' but no exponent value: \"" + str +
//                                          "\"");
//            BigInteger bigInteger = BigInteger.Parse(stringBuilder.ToString());
//            if (exponentBuilder != null)
//            {
//                int num1 = int.Parse(exponentBuilder.ToString());
//                if (num1 > 0)
//                {
//                    if (num1 <= (int) scale)
//                    {
//                        scale -= (ushort) num1;
//                    }
//                    else
//                    {
//                        int exponent = num1 - (int) scale;
//                        scale = (ushort) 0;
//                        bigInteger *= BigInteger.Pow((BigInteger) 10, exponent);
//                    }
//                }
//                else if (num1 < 0)
//                {
//                    int num2 = -num1 + (int) scale;
//                    if (num2 <= (int) ushort.MaxValue)
//                    {
//                        scale = (ushort) num2;
//                    }
//                    else
//                    {
//                        scale = ushort.MaxValue;
//                        bigInteger /= BigInteger.Pow((BigInteger) 10, num2 - (int) ushort.MaxValue);
//                    }
//                }
//            }
//            return new BigDecimal(bigInteger, scale);
//        }

//        private BigDecimal Upscale(ushort newScale)
//        {
//            if ((int) newScale < (int) this._scale)
//                throw new InvalidOperationException("Cannot upscale a BigDecimal to a smaller scale!");
//            return new BigDecimal(this._value*BigInteger.Pow((BigInteger) 10, (int) newScale - (int) this._scale),
//                newScale);
//        }

//        private static ushort SameScale(ref BigDecimal left, ref BigDecimal right)
//        {
//            ushort newScale = System.Math.Max(left._scale, right._scale);
//            left = left.Upscale(newScale);
//            right = right.Upscale(newScale);
//            return newScale;
//        }

//        public override string ToString()
//        {
//            if ((int) this._scale == 0)
//                return this._value.ToString() + ".";
//            string str = this._value.ToString();
//            if (str.Length > (int) this._scale)
//                return str.Insert(str.Length - (int) this._scale, ".");
//            return "0." + new string('0', (int) this._scale - str.Length) + str;
//        }

//        private enum ParseState
//        {
//            Start,
//            Integer,
//            Decimal,
//            E,
//            Exponent,
//        }
//    }
//}

//public struct BigDecimal : IConvertible, IFormattable, IComparable, IComparable<BigDecimal>, IEquatable<BigDecimal>
    //{
    //    public static readonly BigDecimal MinusOne = new BigDecimal(BigInteger.MinusOne, 0);
    //    public static readonly BigDecimal Zero = new BigDecimal(BigInteger.Zero, 0);
    //    public static readonly BigDecimal One = new BigDecimal(BigInteger.One, 0);

    //    private readonly BigInteger _unscaledValue;
    //    private readonly int _scale;

    //    public BigDecimal(double value)
    //        : this((decimal) value)
    //    {
    //    }

    //    public BigDecimal(float value)
    //        : this((decimal) value)
    //    {
    //    }

    //    public BigDecimal(decimal value)
    //    {
    //        var bytes = FromDecimal(value);

    //        var unscaledValueBytes = new byte[12];
    //        Array.Copy(bytes, unscaledValueBytes, unscaledValueBytes.Length);

    //        var unscaledValue = new BigInteger(unscaledValueBytes);
    //        var scale = bytes[14];

    //        if (bytes[15] == 128)
    //            unscaledValue *= BigInteger.MinusOne;

    //        _unscaledValue = unscaledValue;
    //        _scale = scale;
    //    }

    //    public BigDecimal(int value)
    //        : this(new BigInteger(value), 0)
    //    {
    //    }

    //    public BigDecimal(long value)
    //        : this(new BigInteger(value), 0)
    //    {
    //    }

    //    public BigDecimal(uint value)
    //        : this(new BigInteger(value), 0)
    //    {
    //    }

    //    public BigDecimal(ulong value)
    //        : this(new BigInteger(value), 0)
    //    {
    //    }

    //    public static BigDecimal Parse(string number)
    //    {
    //        var bigInteger = BigInteger.Parse(number);
    //        return new BigDecimal(bigInteger, 0);
    //    }    

    //    public BigDecimal(BigInteger unscaledValue, int scale)
    //    {
    //        _unscaledValue = unscaledValue;
    //        _scale = scale;
    //    }

    //    public BigDecimal(byte[] value)
    //    {
    //        byte[] number = new byte[value.Length - 4];
    //        byte[] flags = new byte[4];

    //        Array.Copy(value, 0, number, 0, number.Length);
    //        Array.Copy(value, value.Length - 4, flags, 0, 4);

    //        _unscaledValue = new BigInteger(number);
    //        _scale = BitConverter.ToInt32(flags, 0);
    //    }

    //    public bool IsEven
    //    {
    //        get { return _unscaledValue.IsEven; }
    //    }

    //    public bool IsOne
    //    {
    //        get { return _unscaledValue.IsOne; }
    //    }

    //    public bool IsPowerOfTwo
    //    {
    //        get { return _unscaledValue.IsPowerOfTwo; }
    //    }

    //    public bool IsZero
    //    {
    //        get { return _unscaledValue.IsZero; }
    //    }

    //    public int Sign
    //    {
    //        get { return _unscaledValue.Sign; }
    //    }

    //    public override string ToString()
    //    {
    //        var number = _unscaledValue.ToString("G");

    //        if (_scale > 0)
    //            return number.Insert(number.Length - _scale, ".");

    //        return number;
    //    }

    //    public byte[] ToByteArray()
    //    {
    //        var unscaledValue = _unscaledValue.ToByteArray();
    //        var scale = BitConverter.GetBytes(_scale);

    //        var bytes = new byte[unscaledValue.Length + scale.Length];
    //        Array.Copy(unscaledValue, 0, bytes, 0, unscaledValue.Length);
    //        Array.Copy(scale, 0, bytes, unscaledValue.Length, scale.Length);

    //        return bytes;
    //    }

    //    private static byte[] FromDecimal(decimal d)
    //    {
    //        byte[] bytes = new byte[16];

    //        int[] bits = decimal.GetBits(d);
    //        int lo = bits[0];
    //        int mid = bits[1];
    //        int hi = bits[2];
    //        int flags = bits[3];

    //        bytes[0] = (byte) lo;
    //        bytes[1] = (byte) (lo >> 8);
    //        bytes[2] = (byte) (lo >> 0x10);
    //        bytes[3] = (byte) (lo >> 0x18);
    //        bytes[4] = (byte) mid;
    //        bytes[5] = (byte) (mid >> 8);
    //        bytes[6] = (byte) (mid >> 0x10);
    //        bytes[7] = (byte) (mid >> 0x18);
    //        bytes[8] = (byte) hi;
    //        bytes[9] = (byte) (hi >> 8);
    //        bytes[10] = (byte) (hi >> 0x10);
    //        bytes[11] = (byte) (hi >> 0x18);
    //        bytes[12] = (byte) flags;
    //        bytes[13] = (byte) (flags >> 8);
    //        bytes[14] = (byte) (flags >> 0x10);
    //        bytes[15] = (byte) (flags >> 0x18);

    //        return bytes;
    //    }

    //    #region Operators

    //    public static bool operator ==(BigDecimal left, BigDecimal right)
    //    {
    //        return left.Equals(right);
    //    }

    //    public static bool operator !=(BigDecimal left, BigDecimal right)
    //    {
    //        return !left.Equals(right);
    //    }

    //    public static bool operator >(BigDecimal left, BigDecimal right)
    //    {
    //        return (left.CompareTo(right) > 0);
    //    }

    //    public static bool operator >=(BigDecimal left, BigDecimal right)
    //    {
    //        return (left.CompareTo(right) >= 0);
    //    }

    //    public static bool operator <(BigDecimal left, BigDecimal right)
    //    {
    //        return (left.CompareTo(right) < 0);
    //    }

    //    public static bool operator <=(BigDecimal left, BigDecimal right)
    //    {
    //        return (left.CompareTo(right) <= 0);
    //    }

    //    public static bool operator ==(BigDecimal left, decimal right)
    //    {
    //        return left.Equals(right);
    //    }

    //    public static bool operator !=(BigDecimal left, decimal right)
    //    {
    //        return !left.Equals(right);
    //    }

    //    public static bool operator >(BigDecimal left, decimal right)
    //    {
    //        return (left.CompareTo(right) > 0);
    //    }

    //    public static bool operator >=(BigDecimal left, decimal right)
    //    {
    //        return (left.CompareTo(right) >= 0);
    //    }

    //    public static bool operator <(BigDecimal left, decimal right)
    //    {
    //        return (left.CompareTo(right) < 0);
    //    }

    //    public static bool operator <=(BigDecimal left, decimal right)
    //    {
    //        return (left.CompareTo(right) <= 0);
    //    }

    //    public static bool operator ==(decimal left, BigDecimal right)
    //    {
    //        return left.Equals(right);
    //    }

    //    public static bool operator !=(decimal left, BigDecimal right)
    //    {
    //        return !left.Equals(right);
    //    }

    //    public static bool operator >(decimal left, BigDecimal right)
    //    {
    //        return (left.CompareTo(right) > 0);
    //    }

    //    public static bool operator >=(decimal left, BigDecimal right)
    //    {
    //        return (left.CompareTo(right) >= 0);
    //    }

    //    public static bool operator <(decimal left, BigDecimal right)
    //    {
    //        return (left.CompareTo(right) < 0);
    //    }

    //    public static bool operator <=(decimal left, BigDecimal right)
    //    {
    //        return (left.CompareTo(right) <= 0);
    //    }

    //    #endregion

    //    #region Explicity and Implicit Casts

    //    public static explicit operator byte(BigDecimal value)
    //    {
    //        return value.ToType<byte>();
    //    }

    //    public static explicit operator sbyte(BigDecimal value)
    //    {
    //        return value.ToType<sbyte>();
    //    }

    //    public static explicit operator short(BigDecimal value)
    //    {
    //        return value.ToType<short>();
    //    }

    //    public static explicit operator int(BigDecimal value)
    //    {
    //        return value.ToType<int>();
    //    }

    //    public static explicit operator long(BigDecimal value)
    //    {
    //        return value.ToType<long>();
    //    }

    //    public static explicit operator ushort(BigDecimal value)
    //    {
    //        return value.ToType<ushort>();
    //    }

    //    public static explicit operator uint(BigDecimal value)
    //    {
    //        return value.ToType<uint>();
    //    }

    //    public static explicit operator ulong(BigDecimal value)
    //    {
    //        return value.ToType<ulong>();
    //    }

    //    public static explicit operator float(BigDecimal value)
    //    {
    //        return value.ToType<float>();
    //    }

    //    public static explicit operator double(BigDecimal value)
    //    {
    //        return value.ToType<double>();
    //    }

    //    public static explicit operator decimal(BigDecimal value)
    //    {
    //        return value.ToType<decimal>();
    //    }

    //    public static explicit operator BigInteger(BigDecimal value)
    //    {
    //        var scaleDivisor = BigInteger.Pow(new BigInteger(10), value._scale);
    //        var scaledValue = BigInteger.Divide(value._unscaledValue, scaleDivisor);
    //        return scaledValue;
    //    }

    //    public static implicit operator BigDecimal(byte value)
    //    {
    //        return new BigDecimal(value);
    //    }

    //    public static implicit operator BigDecimal(sbyte value)
    //    {
    //        return new BigDecimal(value);
    //    }

    //    public static implicit operator BigDecimal(short value)
    //    {
    //        return new BigDecimal(value);
    //    }

    //    public static implicit operator BigDecimal(int value)
    //    {
    //        return new BigDecimal(value);
    //    }

    //    public static implicit operator BigDecimal(long value)
    //    {
    //        return new BigDecimal(value);
    //    }

    //    public static implicit operator BigDecimal(ushort value)
    //    {
    //        return new BigDecimal(value);
    //    }

    //    public static implicit operator BigDecimal(uint value)
    //    {
    //        return new BigDecimal(value);
    //    }

    //    public static implicit operator BigDecimal(ulong value)
    //    {
    //        return new BigDecimal(value);
    //    }

    //    public static implicit operator BigDecimal(float value)
    //    {
    //        return new BigDecimal(value);
    //    }

    //    public static implicit operator BigDecimal(double value)
    //    {
    //        return new BigDecimal(value);
    //    }

    //    public static implicit operator BigDecimal(decimal value)
    //    {
    //        return new BigDecimal(value);
    //    }

    //    public static implicit operator BigDecimal(BigInteger value)
    //    {
    //        return new BigDecimal(value, 0);
    //    }

    //    #endregion

    //    public T ToType<T>() where T : struct
    //    {
    //        return (T) ((IConvertible) this).ToType(typeof (T), null);
    //    }

    //    object IConvertible.ToType(Type conversionType, IFormatProvider provider)
    //    {
    //        var scaleDivisor = BigInteger.Pow(new BigInteger(10), this._scale);
    //        var remainder = BigInteger.Remainder(this._unscaledValue, scaleDivisor);
    //        var scaledValue = BigInteger.Divide(this._unscaledValue, scaleDivisor);

    //        if (scaledValue > new BigInteger(Decimal.MaxValue))
    //            throw new ArgumentOutOfRangeException("value",
    //                "The value " + this._unscaledValue + " cannot fit into " + conversionType.Name + ".");

    //        var leftOfDecimal = (decimal) scaledValue;
    //        var rightOfDecimal = ((decimal) remainder)/((decimal) scaleDivisor);

    //        var value = leftOfDecimal + rightOfDecimal;
    //        return Convert.ChangeType(value, conversionType);
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        return ((obj is BigDecimal) && Equals((BigDecimal) obj));
    //    }

    //    public override int GetHashCode()
    //    {
    //        return _unscaledValue.GetHashCode() ^ _scale.GetHashCode();
    //    }

    //    #region IConvertible Members

    //    TypeCode IConvertible.GetTypeCode()
    //    {
    //        return TypeCode.Object;
    //    }

    //    bool IConvertible.ToBoolean(IFormatProvider provider)
    //    {
    //        return Convert.ToBoolean(this);
    //    }

    //    byte IConvertible.ToByte(IFormatProvider provider)
    //    {
    //        return Convert.ToByte(this);
    //    }

    //    char IConvertible.ToChar(IFormatProvider provider)
    //    {
    //        throw new InvalidCastException("Cannot cast BigDecimal to Char");
    //    }

    //    DateTime IConvertible.ToDateTime(IFormatProvider provider)
    //    {
    //        throw new InvalidCastException("Cannot cast BigDecimal to DateTime");
    //    }

    //    decimal IConvertible.ToDecimal(IFormatProvider provider)
    //    {
    //        return Convert.ToDecimal(this);
    //    }

    //    double IConvertible.ToDouble(IFormatProvider provider)
    //    {
    //        return Convert.ToDouble(this);
    //    }

    //    short IConvertible.ToInt16(IFormatProvider provider)
    //    {
    //        return Convert.ToInt16(this);
    //    }

    //    int IConvertible.ToInt32(IFormatProvider provider)
    //    {
    //        return Convert.ToInt32(this);
    //    }

    //    long IConvertible.ToInt64(IFormatProvider provider)
    //    {
    //        return Convert.ToInt64(this);
    //    }

    //    sbyte IConvertible.ToSByte(IFormatProvider provider)
    //    {
    //        return Convert.ToSByte(this);
    //    }

    //    float IConvertible.ToSingle(IFormatProvider provider)
    //    {
    //        return Convert.ToSingle(this);
    //    }

    //    string IConvertible.ToString(IFormatProvider provider)
    //    {
    //        return Convert.ToString(this);
    //    }

    //    ushort IConvertible.ToUInt16(IFormatProvider provider)
    //    {
    //        return Convert.ToUInt16(this);
    //    }

    //    uint IConvertible.ToUInt32(IFormatProvider provider)
    //    {
    //        return Convert.ToUInt32(this);
    //    }

    //    ulong IConvertible.ToUInt64(IFormatProvider provider)
    //    {
    //        return Convert.ToUInt64(this);
    //    }

    //    #endregion

    //    #region IFormattable Members

    //    public string ToString(string format, IFormatProvider formatProvider)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion

    //    #region IComparable Members

    //    public int CompareTo(object obj)
    //    {
    //        if (obj == null)
    //            return 1;

    //        if (!(obj is BigDecimal))
    //            throw new ArgumentException("Compare to object must be a BigDecimal", "obj");

    //        return CompareTo((BigDecimal) obj);
    //    }

    //    #endregion

    //    #region IComparable<BigDecimal> Members

    //    public int CompareTo(BigDecimal other)
    //    {
    //        var unscaledValueCompare = this._unscaledValue.CompareTo(other._unscaledValue);
    //        var scaleCompare = this._scale.CompareTo(other._scale);

    //        // if both are the same value, return the value
    //        if (unscaledValueCompare == scaleCompare)
    //            return unscaledValueCompare;

    //        // if the scales are both the same return unscaled value
    //        if (scaleCompare == 0)
    //            return unscaledValueCompare;

    //        var scaledValue = BigInteger.Divide(this._unscaledValue, BigInteger.Pow(new BigInteger(10), this._scale));
    //        var otherScaledValue = BigInteger.Divide(other._unscaledValue,
    //            BigInteger.Pow(new BigInteger(10), other._scale));

    //        return scaledValue.CompareTo(otherScaledValue);
    //    }

    //    #endregion

    //    #region IEquatable<BigDecimal> Members

    //    public bool Equals(BigDecimal other)
    //    {
    //        return this._scale == other._scale && this._unscaledValue == other._unscaledValue;
    //    }

    //    #endregion
    //}

