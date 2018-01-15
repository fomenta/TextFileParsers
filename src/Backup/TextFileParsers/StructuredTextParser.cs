// StructuredTextParser.cs
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
    public abstract class StructuredTextParser : IDisposable
    {
        private TextReader reader;
        private string[] commentTokens;
        private long lineNumber;
        private bool ownsReader;

        /// <summary>
        /// Initializes a new instance of StructuredTextParser for reading from
        /// the file specified by file name.
        /// </summary>
        /// <param name="fileName">The name of the file to read from.</param>
        protected StructuredTextParser(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException();

            this.reader = new StreamReader(fileName);
            this.ownsReader = true;
        }

        /// <summary>
        /// Initializes a new instance of StructuredTextParser for reading from
        /// the specified stream.
        /// </summary>
        /// <param name="fs">The file stream to read from.</param>
        protected StructuredTextParser(Stream fs)
        {
            this.reader = new StreamReader(fs);
            this.ownsReader = true;
        }

        /// <summary>
        /// Initializes a new instance of StructuredTextParser for reading from
        /// the specified text reader.
        /// </summary>
        /// <param name="reader">The reader used as source.</param>
        protected StructuredTextParser(TextReader reader)
        {
            this.reader = reader;
            this.ownsReader = false;
        }

        /// <summary>
        /// Gets a value indicating wether the end of file has been reached.
        /// </summary>
        public bool EndOfFile
        {
            get
            {
                return (Peek() == -1);
            }
        }

        /// <summary>
        /// Indicates wether blank lines should be ignored.
        /// 
        /// The default value for this property is false.
        /// </summary>
        public bool IgnoreBlankLines { get; set; }

        /// <summary>
        /// Gets the current line number or -1 if there are no character left to read.
        /// </summary>
        public long LineNumber
        {
            get
            {
                return this.lineNumber;
            }
        }

        /// <summary>
        /// Indicates whether leading and trailing white space should be 
        /// trimmed from field values.
        /// 
        /// The default value for this property is false.
        /// </summary>
        public bool TrimWhiteSpace { get; set; }

        /// <summary>
        /// Gets the comment tokens.
        /// 
        /// When a comment token is found at the begining of the line, it indicates
        /// that the line is a comment and should be skipped.
        /// </summary>
        public string[] GetCommentTokens()
        {
            if (commentTokens == null)
                return commentTokens;

            return (string[])commentTokens.Clone();
        }

        /// <summary>
        /// Sets the comment tokens.
        /// 
        /// When a comment token is found at the begining of the line, it indicates
        /// that the line is a comment and should be skipped.
        /// </summary>
        /// <param name="commentTokens">Array of strings indicating the comment tokens.</param>
        public void SetCommentTokens(params string[] commentTokens)
        {
            if (commentTokens == null)
                this.commentTokens = null;

            this.commentTokens = (string[])commentTokens.Clone();
        }

        /// <summary>
        /// Closes the stream reader and file stream as they are no longer 
        /// needed.
        /// </summary>
        public void Close()
        {
            this.lineNumber = -1;

            if (ownsReader && reader != null)
                reader.Close();

            reader = null;
        }

        /// <summary>
        /// Reads the next character without advancing the file cursor.
        /// </summary>
        /// <returns>The code of the read character.</returns>
        public int Peek()
        {
            int c = reader.Peek();

            if (c == -1)
                this.lineNumber = -1;

            return c;
        }

        /// <summary>
        /// Reads the next character and advances the file cursor.
        /// </summary>
        /// <returns>The code of the read character.</returns>
        public int Read()
        {
            int c = reader.Read();

            if (c == -1)
                this.lineNumber = -1;

            return c;
        }

        /// <summary>
        /// When overriden by a derived class, reads the next text line, 
        /// parse it and returns the resulting fields as an array of strings.
        /// </summary>
        /// <returns>All the fields of the current line as an array of strings.</returns>
        /// <exception cref="MalformedLineException">
        /// Raised when a line cannot be parsed using the specified format.
        /// </exception>
        public abstract TextFields ReadFields();

        /// <summary>
        /// Reads the next line without parsing for fields.
        /// </summary>
        /// <returns>The next whole line.</returns>
        public string ReadLine()
        {
            if (EndOfFile == true)
                return null;

            string line = reader.ReadLine();
            this.lineNumber += 1;

            if (IgnoreLine(line))
                return ReadLine();

            if (TrimWhiteSpace == true)
                return line.Trim();

            return line.TrimEnd(new char[] { '\n', '\r' });
        }

        /// <summary>
        /// Reads until the end of the file stream without parsing for fields.
        /// </summary>
        /// <returns>The whole contents from the current position as 
        /// one string.
        /// </returns>
        public string ReadToEnd()
        {
            this.lineNumber = -1;
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Skips the next line.
        /// </summary>
        public void SkipLine()
        {
            ReadLine();
        }

        /// <summary>
        /// Skips the next given number of lines.
        /// </summary>
        /// <param name="lines">The number of lines to skip.</param>
        public void SkipLines(int lines)
        {
            for (int i = 0; i < lines; i++)
            {
                if (EndOfFile)
                    break;

                SkipLine();
            }
        }

        /// <summary>
        /// Determines wether the given line should be ignored by the reader.
        /// </summary>
        /// <param name="line">The source text line.</param>
        /// <returns>True if the line should be ignored or false otherwise.</returns>
        protected virtual bool IgnoreLine(string line)
        {
            if (line == null)
                return false;

            string str = line.Trim();

            if (IgnoreBlankLines && str.Length == 0)
                return true;

            if (commentTokens != null)
            {
                foreach (string commentToken in this.commentTokens)
                {
                    if (str.StartsWith(commentToken, StringComparison.Ordinal))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all leading and trailing white-space characters from each 
        /// field of the given array.
        /// </summary>
        /// <param name="fields">The array of fields to be trimmed.</param>
        /// <returns>A trimmed version of the array of fields.</returns>
        protected static string[] TrimFields(string[] fields)
        {
            int elems = fields.Length;
            string[] trimmedFields = new string[elems];

            for (int i = 0; i < elems; i++)
            {
                trimmedFields[i] = fields[i].Trim();
            }

            return trimmedFields;
        }

        #region IDisposable implementation

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Close();
            }

            disposed = true;
        }

        ~StructuredTextParser()
        {
            Dispose(false);
        }

        #endregion
    }
}
