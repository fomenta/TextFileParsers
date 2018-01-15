// FixedWidthFieldParser.cs
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
using System.IO;
using System.Globalization;

namespace TextFileParsers
{
    /// <summary>
    /// Provides methods and properties for parsing structured text files.
    /// </summary>
    /// <example>
    /// <code>
    /// using (FixedWidthFieldParser parser = new FixedWidthFieldParser("contacts.dat"))
    /// {
    ///     parser.SetFieldWidths(5, 15, 0);
    ///     parser.TrimWhiteSpace = true;
    ///     
    ///     while (!parser.EndOfFile)
    ///     {
    ///         TextFields fields = parser.ReadFields();
    ///         // Process fields here using TextFields.GetXXX methods
    ///     }
    /// }
    /// </code>
    /// </example>
    public class FixedWidthFieldParser : StructuredTextParser
    {
        private int[] fieldWidths = { 0 };
        private CultureInfo culture;

        /// <summary>
        /// Initializes a new instance of FixedWidthFieldParser for reading from
        /// the file specified by file name.
        /// </summary>
        /// <param name="fileName">The name of the file to read from.</param>
        public FixedWidthFieldParser(string fileName)
            : this(fileName, CultureInfo.InvariantCulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of FixedWidthFieldParser for reading from
        /// the file specified by file name.
        /// </summary>
        /// <param name="fileName">The name of the file to read from.</param>
        /// <param name="culture">The culture-specific information to use for parsing the fields.</param>
        public FixedWidthFieldParser(string fileName, CultureInfo culture)
            : base(fileName)
        {
            this.culture = culture;
        }

        /// <summary>
        /// Initializes a new instance of FixedWidthFieldParser for reading from
        /// the specified stream.
        /// </summary>
        /// <param name="fs">The file stream to read from.</param>
        public FixedWidthFieldParser(Stream fs)
            : this(fs, CultureInfo.InvariantCulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of FixedWidthFieldParser for reading from
        /// the specified stream.
        /// </summary>
        /// <param name="fs">The file stream to read from.</param>
        /// <param name="culture">The culture-specific information to use for parsing the fields.</param>
        public FixedWidthFieldParser(Stream fs, CultureInfo culture)
            : base(fs)
        {
            this.culture = culture;
        }

        /// <summary>
        /// Initializes a new instance of FixedWidthFieldParser for reading from
        /// the specified text reader.
        /// </summary>
        /// <param name="reader">The reader used as source.</param>
        public FixedWidthFieldParser(TextReader reader)
            : this(reader, CultureInfo.InvariantCulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of FixedWidthFieldParser for reading from
        /// the specified text reader.
        /// </summary>
        /// <param name="reader">The reader used as source.</param>
        /// <param name="culture">The culture-specific information to use for parsing the fields.</param>
        public FixedWidthFieldParser(TextReader reader, CultureInfo culture)
            : base(reader)
        {
            this.culture = culture;
        }

        /// <summary>
        /// Gets the fields width for the parser.
        /// 
        /// If the last entry in the array is less than or equal to zero, the 
        /// field is assumed to be of variable width.
        /// </summary>
        /// <returns></returns>
        public int[] GetFieldWidths()
        {
            return (int[])this.fieldWidths.Clone();
        }

        /// <summary>
        /// Sets the fields width for the parser to the specified values.
        /// 
        /// If the last entry in the array is less than or equal to zero, the 
        /// field is assumed to be of variable width. 
        /// </summary>
        /// <param name="fieldWidths">Array of integers indicating the width of each field.</param>
        public void SetFieldWidths(params int[] fieldWidths)
        {
            if (fieldWidths == null)
                throw new ArgumentNullException("fieldWidths");

            if (fieldWidths.Length == 0)
                throw new ArgumentException("fieldWidths cannot be empty.");

            this.fieldWidths = (int[])fieldWidths.Clone();
        }

        /// <summary>
        /// Reads the next line, parses it and returns the resulting fields 
        /// as an array of strings.
        /// </summary>
        /// <returns>All the fields of the current line as an array of strings.</returns>
        /// <exception cref="MalformedLineException">
        /// Raised when a line cannot be parsed using the specified format.
        /// </exception>
        public override TextFields ReadFields()
        {
            string line = ReadLine();

            string[] fields = ParseLine(line);

            if (TrimWhiteSpace)
                return new TextFields(TrimFields(fields), culture);

            return new TextFields(fields, culture);
        }

        private string[] ParseLine(string line)
        {
            if (line == null)
                return null;

            int index = 0;
            int elems = fieldWidths.Length;
            string[] fields = new string[elems];

            for (int i = 0; i < elems; i++)
            {
                fields[i] = GetFixedWidthField(line, index, fieldWidths[i]);
                index += fieldWidths[i];
            }

            return fields;
        }

        private static string GetFixedWidthField(string line, int index, int fieldWidth)
        {
            string str;

            if (fieldWidth > 0)
            {
                if (line.Length < (index + fieldWidth))
                    throw new MalformedLineException("The current line was shorter than expected.");

                str = line.Substring(index, fieldWidth);
            }
            else
            {
                if (index >= line.Length)
                {
                    str = string.Empty;
                }
                else
                {
                    str = line.Substring(index).TrimEnd(new char[] { '\r', '\n' });
                }
            }

            return str;
        }
    }
}
