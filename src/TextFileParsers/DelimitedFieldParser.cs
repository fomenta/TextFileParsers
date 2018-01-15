// DelimitedFieldParser.cs
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
using System.IO;
using System.Text;
using System.Globalization;

namespace TextFileParsers
{
    /// <summary>
    /// Provides methods and properties for parsing delimited text files.
    /// </summary>
    /// <example>
    /// <code>
    /// using (DelimitedFieldParser parser = new DelimitedFieldParser("contacts.csv"))
    /// {
    ///     parser.SetDelimiters(',');
    ///     
    ///     while (!parser.EndOfFile)
    ///     {
    ///         TextFields fields = parser.ReadFields();
    ///         // Process fields here using TextFields.GetXXX methods
    ///     }
    /// }
    /// </code>
    /// </example>
    public class DelimitedFieldParser : StructuredTextParser
    {
        private enum DelimitedFieldParserState
        {
            InDelimiter,
            InTextData,
            InQuotedText,
            InClosingQuotes
        }

        private char[] delimiters = { ',' };
        private StringBuilder currentField;
        private List<string> fields = new List<string>();
        private DelimitedFieldParserState state = DelimitedFieldParserState.InDelimiter;
        private CultureInfo culture;

        /// <summary>
        /// Initializes a new instance of DelimitedFieldParser for reading from
        /// the file specified by file name using the default field delimiter.
        /// </summary>
        /// <param name="fileName">The name of the file to read from.</param>
        public DelimitedFieldParser(string fileName)
            : this(fileName, CultureInfo.InvariantCulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of DelimitedFieldParser for reading from
        /// the file specified by file name using the default field delimiter.
        /// </summary>
        /// <param name="fileName">The name of the file to read from.</param>
        /// <param name="culture">The culture-specific information to use for parsing the fields.</param>
        public DelimitedFieldParser(string fileName, CultureInfo culture)
            : base(fileName)
        {
            this.culture = culture;
        }

        /// <summary>
        /// Initializes a new instance of DelimitedFieldParser for reading from
        /// the specified stream using the default field delimiter.
        /// </summary>
        /// <param name="fs">The file stream to read from.</param>
        public DelimitedFieldParser(Stream fs)
            : this(fs, CultureInfo.InvariantCulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of DelimitedFieldParser for reading from
        /// the specified stream using the default field delimiter.
        /// </summary>
        /// <param name="fs">The file stream to read from.</param>
        /// <param name="culture">The culture-specific information to use for parsing the fields.</param>
        public DelimitedFieldParser(Stream fs, CultureInfo culture)
            : base(fs)
        {
            this.culture = culture;
        }

        /// <summary>
        /// Initializes a new instance of DelimitedFieldParser for reading from
        /// the specified text reader using the default field delimiter.
        /// </summary>
        /// <param name="reader">The reader used as source.</param>
        public DelimitedFieldParser(TextReader reader)
            : this(reader, CultureInfo.InvariantCulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of DelimitedFieldParser for reading from
        /// the specified text reader using the default field delimiter.
        /// </summary>
        /// <param name="reader">The reader used as source.</param>
        /// <param name="culture">The culture-specific information to use for parsing the fields.</param>
        public DelimitedFieldParser(TextReader reader, CultureInfo culture)
            : base(reader)
        {
            this.culture = culture;
        }

        /// <summary>
        /// Denotes whether fields are enclosed in quotation marks when parsing 
        /// a delimited file.
        /// 
        /// The default value for this property is false.
        /// </summary>
        public bool HasFieldsEnclosedInQuotes { get; set; }

        /// <summary>
        /// Indicates wether consecutive delimiters are treated as one.
        /// 
        /// The default value for this property is false.
        /// </summary>
        public bool SqueezeDelimiters { get; set; }

        /// <summary>
        /// Gets the delimiters for the parser.
        /// </summary>
        /// <returns></returns>
        public char[] GetDelimiters()
        {
            return (char[])this.delimiters.Clone();
        }

        /// <summary>
        /// Sets the field delimiters for the parser to the specified values.
        /// </summary>
        /// <param name="delimiters">The set of fields delimiters.</param>
        /// <exception cref="ArgumentNullException">Raised if delimiters argument is null.</exception>
        public void SetDelimiters(params char[] delimiters)
        {
            if (delimiters == null)
                throw new ArgumentNullException("delimiters");

            this.delimiters = (char[])delimiters.Clone();

            ValidateDelimiters();
        }

        private void ValidateDelimiters()
        {
            foreach (char delimiter in delimiters)
            {
                if (delimiter == '\r' || delimiter == '\n')
                    throw new ArgumentException("Invalid delimiter.");
            }
        }

        /// <summary>
        /// Reads the next line, parse it and returns the resulting fields 
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
            Initialize();

            foreach (char c in line)
                ParseChar(c);

            EndOfLineEvent();

            return ((string[])fields.ToArray());
        }

        private void Initialize()
        {
            NewField();
            fields.Clear();
            state = DelimitedFieldParserState.InDelimiter;
        }

        private void ParseChar(char c)
        {
            if (IsDelimiter(c))
                DelimiterCharEvent(c);
            else if (IsQuote(c))
                QuoteCharEvent(c);
            else
                DefaultCharEvent(c);
        }

        private static bool IsQuote(char c)
        {
            return c == '"';
        }

        private bool IsDelimiter(char c)
        {
            foreach (char delimiter in delimiters)
            {
                if (c == delimiter)
                    return true;
            }
            return false;
        }

        private void DelimiterCharEvent(char c)
        {
            switch (state)
            {
                case DelimitedFieldParserState.InDelimiter:
                    if (!SqueezeDelimiters)
                        AddField();
                    break;

                case DelimitedFieldParserState.InTextData:
                    AddField();
                    NewField();
                    state = DelimitedFieldParserState.InDelimiter;
                    break;

                case DelimitedFieldParserState.InQuotedText:
                    AppendChar(c);
                    break;

                case DelimitedFieldParserState.InClosingQuotes:
                    AddField();
                    NewField();
                    state = DelimitedFieldParserState.InDelimiter;
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        private void QuoteCharEvent(char c)
        {
            if (!HasFieldsEnclosedInQuotes)
            {
                DefaultCharEvent(c);
                return;
            }

            switch (state)
            {
                case DelimitedFieldParserState.InDelimiter:
                    state = DelimitedFieldParserState.InQuotedText;
                    break;

                case DelimitedFieldParserState.InTextData:
                    throw new MalformedLineException("Unexpected quote.", LineNumber);

                case DelimitedFieldParserState.InQuotedText:
                    state = DelimitedFieldParserState.InClosingQuotes;
                    break;

                case DelimitedFieldParserState.InClosingQuotes:
                    AddField();
                    NewField();
                    state = DelimitedFieldParserState.InQuotedText;
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        private void DefaultCharEvent(char c)
        {
            switch (state)
            {
                case DelimitedFieldParserState.InDelimiter:
                    AppendChar(c);
                    state = DelimitedFieldParserState.InTextData;
                    break;

                case DelimitedFieldParserState.InTextData:
                    AppendChar(c);
                    break;

                case DelimitedFieldParserState.InQuotedText:
                    AppendChar(c);
                    break;

                case DelimitedFieldParserState.InClosingQuotes:
                    throw new MalformedLineException("Expected delimiter not found.", LineNumber);

                default:
                    throw new InvalidOperationException();
            }
        }

        private void EndOfLineEvent()
        {
            switch (state)
            {
                case DelimitedFieldParserState.InDelimiter:
                    AddField();
                    break;

                case DelimitedFieldParserState.InTextData:
                    AddField();
                    break;

                case DelimitedFieldParserState.InQuotedText:
                    throw new MalformedLineException("Closing quote was expected.", LineNumber);

                case DelimitedFieldParserState.InClosingQuotes:
                    AddField();
                    NewField();
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        private void AppendChar(char c)
        {
            currentField.Append(c);
        }

        private void AddField()
        {
            fields.Add(currentField.ToString());
        }

        private void NewField()
        {
            currentField = new StringBuilder();
        }
    }
}
