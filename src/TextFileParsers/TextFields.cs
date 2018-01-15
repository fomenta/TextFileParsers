// TextField.cs
// 
// Author: Mauricio Trícoli <mtricoli@live.com.ar>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Collections;

namespace TextFileParsers
{
    /// <summary>
    /// Provides access to the field values within each line for a StructuredTextParser
    /// or a derivative class.
    /// </summary>
    public class TextFields
    {
        private string[] items;
        private CultureInfo culture;

        /// <summary>
        /// Initializes a new instance of TextFields from an array of strings.
        /// </summary>
        /// <param name="values">The array of string containing the values for each field.</param>
        public TextFields(string[] values)
            : this(values, CultureInfo.InvariantCulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of TextFields from an array of strings indicating
        /// the culture-specific information to use for parsing the fields.
        /// </summary>
        /// <param name="values">The array of string containing the values for each field.</param>
        /// <param name="cultureInfo">The culture-specific information to use for parsing the fields.</param>
        public TextFields(string[] values, CultureInfo cultureInfo)
        {
            this.items = (string[])values.Clone();
            this.culture = cultureInfo;
        }

        /// <summary>
        /// Gets the number of fields in the current record.
        /// </summary>
        public int Count
        {
            get { return items.Length; }
        }

        /// <summary>
        /// Gets the string value of the specified field.
        /// </summary>
        /// <param name="i">The zero-based field ordinal.</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="IndexOutOfRange">
        /// The index passed was outside the range of 0 through FieldCount.
        /// </exception>
        public string this[int ordinal]
        {
            get { return items[ordinal]; }
        }

        /// <summary>
        /// Gets the value of the specified field as a Boolean.
        /// </summary>
        /// <param name="i">The zero-based field ordinal.</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="IndexOutOfRange">
        /// The index passed was outside the range of 0 through FieldCount.
        /// </exception>
        public bool GetBoolean(int ordinal)
        {
            return Boolean.Parse(items[ordinal]);
        }

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified field.
        /// </summary>
        /// <param name="i">The zero-based field ordinal.</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="IndexOutOfRange">
        /// The index passed was outside the range of 0 through FieldCount.
        /// </exception>
        public byte GetByte(int ordinal)
        {
            return Byte.Parse(items[ordinal], culture);
        }

        /// <summary>
        /// Gets the character value of the specified field.
        /// </summary>
        /// <param name="i">The zero-based field ordinal.</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="IndexOutOfRange">
        /// The index passed was outside the range of 0 through FieldCount.
        /// </exception>
        public char GetChar(int ordinal)
        {
            return Char.Parse(items[ordinal]);
        }

        /// <summary>
        /// Gets the date and time data value of the specified field.
        /// </summary>
        /// <param name="i">The zero-based field ordinal.</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="IndexOutOfRange">
        /// The index passed was outside the range of 0 through FieldCount.
        /// </exception>
        public DateTime GetDateTime(int ordinal)
        {
            return DateTime.Parse(items[ordinal], culture);
        }

        /// <summary>
        /// Gets the fixed-position numeric value of the specified field.
        /// </summary>
        /// <param name="i">The zero-based field ordinal.</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="IndexOutOfRange">
        /// The index passed was outside the range of 0 through FieldCount.
        /// </exception>
        public decimal GetDecimal(int ordinal)
        {
            return Decimal.Parse(items[ordinal], culture);
        }

        /// <summary>
        /// Gets the double-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The zero-based field ordinal.</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="IndexOutOfRange">
        /// The index passed was outside the range of 0 through FieldCount.
        /// </exception>
        public double GetDouble(int ordinal)
        {
            return Double.Parse(items[ordinal], culture);
        }

        /// <summary>
        /// Gets the 16-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The zero-based field ordinal.</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="IndexOutOfRange">
        /// The index passed was outside the range of 0 through FieldCount.
        /// </exception>
        public short GetInt16(int ordinal)
        {
            return Int16.Parse(items[ordinal], culture);
        }

        /// <summary>
        /// Gets the 32-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The zero-based field ordinal.</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="IndexOutOfRange">
        /// The index passed was outside the range of 0 through FieldCount.
        /// </exception>
        public int GetInt32(int ordinal)
        {
            return Int32.Parse(items[ordinal], culture);
        }

        /// <summary>
        /// Gets the 64-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The zero-based field ordinal.</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="IndexOutOfRange">
        /// The index passed was outside the range of 0 through FieldCount.
        /// </exception>
        public long GetInt64(int ordinal)
        {
            return Int64.Parse(items[ordinal], culture);
        }

        /// <summary>
        /// Gets the single-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The zero-based field ordinal.</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="IndexOutOfRange">
        /// The index passed was outside the range of 0 through FieldCount.
        /// </exception>
        public float GetSingle(int ordinal)
        {
            return Single.Parse(items[ordinal], culture);
        }

        /// <summary>
        /// Gets the string value of the specified field.
        /// </summary>
        /// <param name="i">The zero-based field ordinal.</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="IndexOutOfRange">
        /// The index passed was outside the range of 0 through FieldCount.
        /// </exception>
        public string GetString(int ordinal)
        {
            return items[ordinal];
        }

        /// <summary>
        /// Gets an array containing each field as read from the input stream.
        /// </summary>
        /// <returns>An array of strings.</returns>
        public string[] ToArray()
        {
            return (string[])items.Clone();
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation
        /// using the underlying culture-specific list separator to delimit each field.
        /// </summary>
        /// <returns>The string representation of this instance.</returns>
        public override string ToString()
        {
            return ToString(culture.TextInfo.ListSeparator);
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation
        /// using the supplied separator to delimit each field.
        /// </summary>
        /// <param name="separator"></param>
        /// <returns>The string representation of this instance.</returns>
        public string ToString(string separator)
        {
            return string.Join(separator, items);
        }
    }
}
