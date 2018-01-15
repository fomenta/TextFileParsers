// FixedWidthFieldParserTest.cs
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
using System.Text;
using NUnit.Framework;

namespace TextFileParsers
{
    [TestFixture]
    public class FixedWidthFieldParserTest
    {
        [Test]
        public void ReaderRemainsOpen()
        {
            string text = "01732Juan Perez     11052002";

            StringReader reader = new StringReader(text);

            FixedWidthFieldParser parser = new FixedWidthFieldParser(reader);
            parser.ReadToEnd();
            parser.Close();

            // Not owned reader should remain open after closing parser.
            Assert.AreEqual(-1, reader.Peek());
            reader.Close();
        }


        [Test]
        public void CanRemeberFieldWidths()
        {
            using (StringReader reader = new StringReader(""))
            {
                using (FixedWidthFieldParser parser = new FixedWidthFieldParser(reader))
                {
                    parser.SetFieldWidths(5, 3, 8);
                    int[] fieldWidths = parser.GetFieldWidths();
                    Assert.AreEqual(3, fieldWidths.Length);
                    Assert.AreEqual(5, fieldWidths[0]);
                    Assert.AreEqual(3, fieldWidths[1]);
                    Assert.AreEqual(8, fieldWidths[2]);
                }
            }
        }

        [Test]
        public void CanParseSingleField()
        {
            string text = "single record";

            using (StringReader reader = new StringReader(text))
            {
                using (FixedWidthFieldParser parser = new FixedWidthFieldParser(reader))
                {
                    TextFields fields = parser.ReadFields();
                    Assert.AreEqual(1, fields.Count);
                    Assert.AreEqual(text, fields[0]);
                }
            }
        }

        [Test]
        public void CanParseMultipleFields()
        {
            string text = "01732Juan Perez     11052002";

            using (StringReader reader = new StringReader(text))
            {
                using (FixedWidthFieldParser parser = new FixedWidthFieldParser(reader))
                {
                    parser.SetFieldWidths(5, 15, 8);
                    Assert.AreEqual(3, parser.GetFieldWidths().Length);
                    Assert.AreEqual("01732:Juan Perez     :11052002", parser.ReadFields().ToString(":"));
                }
            }
        }

        [Test]
        public void CanTrimWhiteSpace()
        {
            string text = "01732Juan Perez     11052002";

            using (StringReader reader = new StringReader(text))
            {
                using (FixedWidthFieldParser parser = new FixedWidthFieldParser(reader))
                {
                    parser.SetFieldWidths(5, 15, 8);
                    parser.TrimWhiteSpace = true;
                    Assert.AreEqual(3, parser.GetFieldWidths().Length);
                    Assert.AreEqual("01732:Juan Perez:11052002", parser.ReadFields().ToString(":"));
                }
            }
        }

        [Test]
        public void VariableLengthField()
        {
            string newLine = "\r\n";
            string text = "01732Juan Perez     jperez@mail.com" + newLine +
                "02310Guillermo Diez guillediez@mail.com";

            using (StringReader reader = new StringReader(text))
            {
                using (FixedWidthFieldParser parser = new FixedWidthFieldParser(reader))
                {
                    parser.SetFieldWidths(5, 15, 0);
                    parser.TrimWhiteSpace = true;
                    Assert.AreEqual("01732:Juan Perez:jperez@mail.com", parser.ReadFields().ToString(":"));
                    Assert.AreEqual("02310:Guillermo Diez:guillediez@mail.com", parser.ReadFields().ToString(":"));
                }
            }
        }

        [Test, ExpectedException(typeof(MalformedLineException))]
        public void FailsIfLineIsShorterThanExpected()
        {
            string text = "01732Juan Perez";

            using (StringReader reader = new StringReader(text))
            {
                using (FixedWidthFieldParser parser = new FixedWidthFieldParser(reader))
                {
                    parser.SetFieldWidths(5, 15, 8);
                    parser.ReadFields();
                }
            }
        }

        [Test]
        public void DefaultPropertiesTest()
        {
            using (StringReader reader = new StringReader(string.Empty))
            {
                using (FixedWidthFieldParser parser = new FixedWidthFieldParser(reader))
                {
                    Assert.IsFalse(parser.IgnoreBlankLines);
                    Assert.IsFalse(parser.TrimWhiteSpace);
                    Assert.AreEqual(0, parser.LineNumber);
                    Assert.AreEqual(1, parser.GetFieldWidths().Length);
                    Assert.AreEqual(0, parser.GetFieldWidths()[0]);
                }
            }
        }

        [Test]
        public void CanReadFromFile()
        {
            string textFile = Path.GetTempFileName();

            try
            {
                using (StreamWriter writer = new StreamWriter(textFile, false))
                {
                    writer.WriteLine("01732Juan Perez     jperez@mail.com");
                    writer.WriteLine("02310Guillermo Diez guillediez@mail.com");
                }

                using (FixedWidthFieldParser parser = new FixedWidthFieldParser(textFile))
                {
                    parser.SetFieldWidths(5, 15, 0);
                    parser.TrimWhiteSpace = true;
                    Assert.AreEqual("01732:Juan Perez:jperez@mail.com", parser.ReadFields().ToString(":"));
                    Assert.AreEqual("02310:Guillermo Diez:guillediez@mail.com", parser.ReadFields().ToString(":"));
                }
            }
            finally
            {
                File.Delete(textFile);
            }
        }

        [Test]
        public void CanReadTypedFields()
        {
            string text = "01732Juan Perez     0100.0011/05/2002";

            using (StringReader reader = new StringReader(text))
            {
                using (FixedWidthFieldParser parser = new FixedWidthFieldParser(reader))
                {
                    parser.SetFieldWidths(5, 15, 7, 10);
                    parser.TrimWhiteSpace = true;

                    TextFields fields = parser.ReadFields();
                    Assert.AreEqual(1732, fields.GetInt32(0));
                    Assert.AreEqual("Juan Perez", fields.GetString(1));
                    Assert.AreEqual(100.0f, fields.GetSingle(2));
                    Assert.AreEqual(new DateTime(2002, 11, 05), fields.GetDateTime(3));
                }
            }
        }

        [Test]
        public void AllCommentedLines()
        {
            string newLine = "\r\n";
            string text = "# 01732Juan Perez     jperez@mail.com" + newLine +
                "# 02310Guillermo Diez guillediez@mail.com";

            using (StringReader reader = new StringReader(text))
            {
                using (FixedWidthFieldParser parser = new FixedWidthFieldParser(reader))
                {
                    parser.SetFieldWidths(5, 15, 0);
                    parser.TrimWhiteSpace = true;
                    parser.SetCommentTokens("#");

                    Assert.IsFalse(parser.EndOfFile);
                    string line = parser.ReadLine();
                    Assert.IsNull(line);
                    Assert.IsTrue(parser.EndOfFile);
                }
            }

        }
    }
}
