using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MathResolver.Structure
{
    public class Number
    {
        private byte[] _bytes;
        private long _floatingPointIndex = -1;
        private bool _isNegative = false;
        private long _integerLength = 0;
        public Number()
        {
            _bytes = new byte[0];
            _floatingPointIndex = -1;
        }

        #region Constructor

        private Number(byte[] bytes, long startIndex, long length, long floatingPointIndex, bool isNegative)
        {
            _bytes = new byte[length];
            Array.Copy(bytes, startIndex, _bytes, 0, length);

            _isNegative = isNegative;
            SetFloatingPointIndex(floatingPointIndex);
        }

        public Number(string number)
        {
            number = RemoveWhiteSpace(number);
            var seperator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var sign = GetSign(number);

            if (sign == '-') _isNegative = true;

            var buffer = new byte[number.Length];
            var bufferSize = 0;
            var startIndex = 0;
            var endingZeros = 0;
            var countZeros = true;
            long floatingPointIndex = -1;
            foreach (var c in number)
            {
                if (char.IsNumber(c))
                {
                    buffer[bufferSize++] = (byte) char.GetNumericValue(c);

                    if (floatingPointIndex > -1)
                    {
                        if (c == '0')
                        {
                            endingZeros++;
                        }
                        else
                        {
                            endingZeros = 0;
                        }
                    }
                    else
                    {
                        if (c != '0')
                        {
                            countZeros = false;
                        }
                        else
                        {
                            if (countZeros)
                                startIndex++;
                        }
                    }
                }
                else
                {
                    if (c == seperator[0])
                    {
                        floatingPointIndex = bufferSize;
                        if (bufferSize == 1 && startIndex > 0)
                            startIndex = 0;
                    }
                }
            }

            bufferSize -= endingZeros;

            if (floatingPointIndex > -1)
                floatingPointIndex -= startIndex;

            _bytes = new byte[bufferSize - startIndex];
            Array.Copy(buffer, startIndex, _bytes, 0, _bytes.Length);

            Array.Reverse(_bytes);
            if (floatingPointIndex > -1)
                floatingPointIndex = _bytes.LongLength - floatingPointIndex;

            SetFloatingPointIndex(floatingPointIndex);
        }

        public Number(int number) : this(number.ToString())
        {
        }

        public Number(float number) : this(number.ToString())
        {
        }

        public Number(byte number) : this(number.ToString())
        {
        }

        public Number(short number) : this(number.ToString())
        {
        }

        public Number(double number) : this(number.ToString())
        {
        }

        public Number(long number) : this(number.ToString())
        {
        }

        public Number(uint number) : this(number.ToString())
        {
        }

        public Number(ushort number) : this(number.ToString())
        {
        }

        public Number(sbyte number) : this(number.ToString())
        {
        }

        public Number(ulong number) : this(number.ToString())
        {
        }

        public Number(object number) : this(number.ToString())
        {
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            if (_bytes == null || _bytes.Length == 0)
                return "0";

            var postfix = string.Empty;
            var maxLenght = _bytes.Length;
            if (maxLenght > 100)
            {
                maxLenght = 100;
                postfix = "...";
            }

            var result = new StringBuilder(maxLenght + (_isNegative ? 1 : 0) + (_floatingPointIndex > -1 ? 1 : 0) + postfix.Length);
            var seperator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (_isNegative)
                result.Append('-');

            for (var i = 0; i < maxLenght; i++)
            {
                var ii = _bytes.LongLength - i - 1;
                result.Append(_bytes[ii]);
                if (ii == _floatingPointIndex)
                    result.Append(seperator);
            }

            result.Append(postfix);
            return result.ToString();
        }
        #endregion

        #region Operators

        public static Number operator +(Number n1, Number n2)
        {
            if (!n1._isNegative && !n2._isNegative)
            {
                return AddInternal(n1, n2);
            }
            else if (!n1._isNegative && n2._isNegative)
            {
                var r = SubInternal(n1, n2);
                if (CompareInternal(n1, n2, true) == -1) r._isNegative = true;
                return r;
            }
            else if (n1._isNegative && !n2._isNegative)
            {
                var r = SubInternal(n2, n1);
                if (CompareInternal(n2, n1, true) == -1) r._isNegative = true;
                return r;
            }
            else
            {
                var r = AddInternal(n1, n2);
                r._isNegative = true;
                return r;
            }
        }

        public static Number operator -(Number n1, Number n2)
        {
            if (!n1._isNegative && !n2._isNegative)
            {
                return SubInternal(n1, n2);
            }
            else if (!n1._isNegative && n2._isNegative)
            {
                return AddInternal(n1, n2);
            }
            else if (n1._isNegative && !n2._isNegative)
            {
                var r = AddInternal(n1, n2);
                r._isNegative = true;
                return r;
            }
            else
            {
                var r = SubInternal(n1, n2);
                var c = CompareInternal(n1, n2, true);
                if(c == 1) r._isNegative = true;
                if(c == -1) r._isNegative = false;
                return r;
            }
        }
        #endregion

        #region Private Helper

        private void SetFloatingPointIndex(long floatingPointIndex)
        {
            _floatingPointIndex = floatingPointIndex;
            if (_bytes.LongLength > 0)
            {
                if (_floatingPointIndex > -1)
                {
                    _integerLength = _bytes.LongLength - _floatingPointIndex;
                }
                else
                {
                    _integerLength = _bytes.LongLength;
                }
            }
            else
            {
                _integerLength = 0;
            }
        }
        #endregion

        #region Privat static helper
        private static string RemoveWhiteSpace(string whiteSpace)
        {
            var result = new StringBuilder(whiteSpace.Length);
            foreach (var c in whiteSpace)
            {
                if (!char.IsWhiteSpace(c))
                    result.Append(c);
            }

            return result.ToString();
        }

        private static char GetSign(string number)
        {
            if (number[0] == '-')
                return number[0];
            return '+';
        }
        #endregion

        #region Math Helper
        
        private static Number AddInternal(Number n1, Number n2)
        {
            //if(n1._isNegative || n2._isNegative)
            //    throw new ArgumentException();

            var maxLength = Math.Max(n1._bytes.LongLength, n2._bytes.LongLength);
            var floatingNumbers = Math.Max(n1._floatingPointIndex, n2._floatingPointIndex);
            var integerNumbers = Math.Max(
                n1._floatingPointIndex > -1 ? n1._bytes.LongLength - n1._floatingPointIndex : n1._bytes.LongLength,
                n2._floatingPointIndex > -1 ? n2._bytes.LongLength - n2._floatingPointIndex : n2._bytes.LongLength);

            maxLength = Math.Max(maxLength, floatingNumbers + integerNumbers);

            var io1 = 0l;
            var io2 = 0l;

            if (n1._floatingPointIndex > -1 || n2._floatingPointIndex > -1)
            {
                if (n1._floatingPointIndex > n2._floatingPointIndex)
                {
                    io2 = n2._floatingPointIndex > -1 ? n1._floatingPointIndex - n2._floatingPointIndex : n1._floatingPointIndex;
                }
                else if (n1._floatingPointIndex < n2._floatingPointIndex)
                {
                    io1 = n1._floatingPointIndex > -1 ? n2._floatingPointIndex - n1._floatingPointIndex : n2._floatingPointIndex;
                }
            }
            
            var buffer = new byte[maxLength + 1];
            var buffersize = 0;

            var overflow = 0;
            for (var i = 0; i < buffer.LongLength; i++)
            {
                var i1 = i - io1;
                var i2 = i - io2;
                var a1 = (i1 < n1._bytes.LongLength && i1 >= 0) ? n1._bytes[i1] : 0;
                var a2 = (i2 < n2._bytes.LongLength && i2 >= 0) ? n2._bytes[i2] : 0;

                var r = a1 + a2 + overflow;

                if (r > 9)
                {
                    r -= 10;
                    overflow = 1;
                }
                else
                {
                    overflow = 0;
                }
                
                if(i == buffer.LongLength-1 && r == 0)
                    break;
                
                buffer[buffersize++] = (byte) r;
            }
          
            return new Number(buffer, 0, buffersize, floatingNumbers, false);
        }

        private static Number SubInternal(Number n1, Number n2)
        {
            //if (n1._isNegative || n2._isNegative)
            //    throw new ArgumentException();

            var switched = false;
            var compared = CompareInternal(n1, n2, true);
            if (compared == -1)
            {
                var nt = n1;
                n1 = n2;
                n2 = nt;
                switched = true;
            }

            var maxLength = Math.Max(n1._bytes.LongLength, n2._bytes.LongLength);
            var floatingNumbers = Math.Max(n1._floatingPointIndex, n2._floatingPointIndex);
            var integerNumbers = Math.Max(
                n1._floatingPointIndex > -1 ? n1._bytes.LongLength - n1._floatingPointIndex : n1._bytes.LongLength,
                n2._floatingPointIndex > -1 ? n2._bytes.LongLength - n2._floatingPointIndex : n2._bytes.LongLength);

            maxLength = Math.Max(maxLength, floatingNumbers + integerNumbers);

            var io1 = 0l;
            var io2 = 0l;

            if (n1._floatingPointIndex > -1 || n2._floatingPointIndex > -1)
            {
                if (n1._floatingPointIndex > n2._floatingPointIndex)
                {
                    io2 = n2._floatingPointIndex > -1 ? n1._floatingPointIndex - n2._floatingPointIndex : n1._floatingPointIndex;
                }
                else if (n1._floatingPointIndex < n2._floatingPointIndex)
                {
                    io1 = n1._floatingPointIndex > -1 ? n2._floatingPointIndex - n1._floatingPointIndex : n2._floatingPointIndex;
                }
            }

            var buffer = new byte[maxLength + 1];
            var buffersize = 0;

            var overflow = 0;
            for (var i = 0; i < buffer.LongLength; i++)
            {
                var i1 = i - io1;
                var i2 = i - io2;
                var a1 = (i1 < n1._bytes.LongLength && i1 >= 0) ? n1._bytes[i1] : 0;
                var a2 = (i2 < n2._bytes.LongLength && i2 >= 0) ? n2._bytes[i2] : 0;

                var r = a1 - a2 - overflow;

                if (r < 0)
                {
                    r = 10 + r;
                    overflow = 1;
                }
                else
                {
                    overflow = 0;
                }

                if (i == buffer.LongLength - 1 && r == 0)
                {
                    break;
                }

                buffer[buffersize++] = (byte)r;
            }

            for (var i = buffersize-1; i > 0 && i > floatingNumbers; i--)
            {
                if(buffer[i] != 0) break;
                buffersize--;
            }

            var index = 0;
            for (var i = 0; i < buffersize; i++)
            {
                if (buffer[i] != 0) break;
                index++;
            }

            return new Number(buffer, index, buffersize - index, floatingNumbers - index, switched);
        }

        //private static Number MultiplyInternal(Number n1, Number n2)
        //{

        //}

        private static int CompareInternal(Number n1, Number n2, bool ignoreSign=false)
        {
            if (ReferenceEquals(n1, n2))
                return 0;

            if (n1._bytes.LongLength == 0 && n2._bytes.LongLength == 0)
                return 0;

            if (!ignoreSign && (n1._isNegative ^ n2._isNegative))
            {
                if (n1._isNegative)
                    return -1;
                return 1;
            }

            if (n1._integerLength != n2._integerLength)
                return Math.Sign(n1._integerLength - n2._integerLength);

            for (var i = 0; i < n1._integerLength; i++)
            {
                var v1 = n1._bytes[n1._bytes.LongLength - i - 1];
                var v2 = n2._bytes[n2._bytes.LongLength - i - 1];

                if (v1 != v2) return Math.Sign(v1 - v2);
            }

            if(n1._floatingPointIndex != n2._floatingPointIndex)
                return Math.Sign(n1._floatingPointIndex - n2._floatingPointIndex);

            for (var i = 0; i < n1._floatingPointIndex; i++)
            {
                var v1 = n1._bytes[n1._bytes.LongLength - n1._integerLength - i];
                var v2 = n2._bytes[n2._bytes.LongLength - n2._integerLength - i];

                if (v1 != v2) return Math.Sign(v1 - v2);
            }

            return 0;
        }
        #endregion
    }
}
