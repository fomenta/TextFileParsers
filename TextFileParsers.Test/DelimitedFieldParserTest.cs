// DelimitedFieldParserTest.cs
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
    public class DelimitedFieldParserTest
    {
        [Test]
        public void ReaderRemainsOpen()
        {
            string text = "record 1";

            StringReader reader = new StringReader(text);

            DelimitedFieldParser parser = new DelimitedFieldParser(reader);
            parser.ReadToEnd();
            parser.Close();

            // Not owned reader should remain open after closing parser.
            Assert.AreEqual(-1, reader.Peek());
            reader.Close();
        }

        [Test]
        public void CanParseSingleField()
        {
            string text = "record 1";
            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    Assert.AreEqual("record 1", parser.ReadFields().ToString(string.Empty));
                }
            }
        }

        [Test]
        public void CanParseMultipleFields()
        {
            string newLine = "\r\n";
            string text = "a;bb;ccc;dddd" + newLine + "111;22;3";

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    parser.SetDelimiters(';');
                    Assert.AreEqual("a:bb:ccc:dddd", parser.ReadFields().ToString(":"));
                    Assert.AreEqual("111:22:3", parser.ReadFields().ToString(":"));
                }
            }
        }

        [Test]
        public void CanParseQuotedFields()
        {
            string text = "a,\"bb\",ccc";

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    parser.HasFieldsEnclosedInQuotes = true;
                    Assert.AreEqual("a|bb|ccc", parser.ReadFields().ToString("|"));
                }
            }
        }

        [Test]
        public void CanTreatQuotesAsNormalChars()
        {
            string text = "Michael \"Magic\" Jordan";

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    parser.HasFieldsEnclosedInQuotes = false;
                    TextFields fields = parser.ReadFields();
                    Assert.AreEqual(1, fields.Count);
                    Assert.AreEqual(text, fields[0]);
                }
            }
        }

        [Test, ExpectedExceptionAttribute(typeof(MalformedLineException))]
        public void FailsIfUnmatchedQuotes()
        {
            string text = "\"aaa";

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    parser.HasFieldsEnclosedInQuotes = true;
                    Assert.AreEqual("aaa", parser.ReadFields().ToString("|"));
                }
            }
        }

        [Test]
        public void CanParseCommaInQuotes()
        {
            string text = "a,\"b1,b2\",ccc";

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    parser.HasFieldsEnclosedInQuotes = true;
                    Assert.AreEqual("a|b1,b2|ccc", parser.ReadFields().ToString("|"));
                }
            }
        }

        [Test]
        public void FieldIsEmpty()
        {
            string text = "a,bb,ccc,,eeee";

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    Assert.AreEqual("a|bb|ccc||eeee", parser.ReadFields().ToString("|"));
                }
            }
        }

        [Test]
        public void QuotedFieldIsEmpty()
        {
            string text = "a,bb,ccc,\"\",eeee";

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    parser.HasFieldsEnclosedInQuotes = true;
                    Assert.AreEqual("a|bb|ccc||eeee", parser.ReadFields().ToString("|"));
                }
            }
        }

        [Test]
        public void LastFieldIsEmpty()
        {
            string text = "a,bb,ccc,";

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    Assert.AreEqual("a|bb|ccc|", parser.ReadFields().ToString("|"));
                }
            }
        }

        [Test]
        public void LastFieldIsQuoted()
        {
            string text = "a,bb,\"ccc\"";

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    parser.HasFieldsEnclosedInQuotes = true;
                    Assert.AreEqual("a|bb|ccc", parser.ReadFields().ToString("|"));
                }
            }
        }

        [Test, ExpectedException(typeof(MalformedLineException))]
        public void FailsIfUnexpectedQuote()
        {
            string text = "a\"bb\",ccc";

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    parser.HasFieldsEnclosedInQuotes = true;
                    parser.ReadFields();
                }
            }
        }

        [Test, ExpectedException(typeof(MalformedLineException))]
        public void FailsIfExpectedDelimiterNotFound()
        {
            string text = "a,\"bb\"ccc";

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    parser.HasFieldsEnclosedInQuotes = true;
                    parser.ReadFields();
                }
            }
        }

        [Test]
        public void CanSqueezeDelimiters()
        {
            string text = "a  bb  ccc  dddd";

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    parser.SetDelimiters(' ');
                    parser.SqueezeDelimiters = true;
                    Assert.AreEqual("a:bb:ccc:dddd", parser.ReadFields().ToString(":"));
                }
            }
        }

        [Test]
        public void MultipleDelimiters()
        {
            string text = "a bb\tccc\tdddd";

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    parser.SetDelimiters(' ', '\t');
                    Assert.AreEqual("a:bb:ccc:dddd", parser.ReadFields().ToString(":"));
                }
            }
        }

        [Test]
        public void CanSkipCommentedLines()
        {
            string newLine = "\r\n";
            string text = "#Comment line" + newLine + "a;bb;ccc;dddd";

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    parser.SetDelimiters(';');
                    parser.SetCommentTokens("#");
                    Assert.AreEqual("a:bb:ccc:dddd", parser.ReadFields().ToString(":"));
                }
            }
        }

        [Test]
        public void CanTrimWhiteSpace()
        {
            string newLine = "\r\n";
            string text = "a;bb ; ccc; dddd " + newLine;

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    parser.SetDelimiters(';');
                    parser.TrimWhiteSpace = true;
                    Assert.AreEqual("a:bb:ccc:dddd", parser.ReadFields().ToString(":"));
                }
            }
        }

        [Test]
        public void CanDetectEndOfData()
        {
            using (StringReader reader = new StringReader(string.Empty))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    Assert.IsTrue(parser.EndOfFile);
                }
            }
        }

        [Test]
        public void DefaultPropertiesTest()
        {
            using (StringReader reader = new StringReader(string.Empty))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    Assert.IsFalse(parser.HasFieldsEnclosedInQuotes);
                    Assert.IsFalse(parser.IgnoreBlankLines);
                    Assert.IsFalse(parser.SqueezeDelimiters);
                    Assert.IsFalse(parser.TrimWhiteSpace);
                    Assert.AreEqual(0, parser.LineNumber);
                }
            }
        }

        [Test, ExpectedExceptionAttribute(typeof(ArgumentNullException))]
        public void DelimitersCannotBeNull()
        {
            using (StringReader reader = new StringReader(string.Empty))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    parser.SetDelimiters(null);
                }
            }
        }

        [Test]
        public void DelimitersTest()
        {
            using (StringReader reader = new StringReader(string.Empty))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    char[] delimiters = { ',', ';' };
                    parser.SetDelimiters(delimiters);

                    delimiters[0] = '\t';

                    Assert.AreNotEqual('\t', parser.GetDelimiters()[0]);
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
                    writer.WriteLine("a,bb,ccc,dddd");
                }

                using (DelimitedFieldParser parser = new DelimitedFieldParser(textFile))
                {
                    parser.SetDelimiters(',');
                    Assert.AreEqual("a:bb:ccc:dddd", parser.ReadFields().ToString(":"));
                }
            }
            finally
            {
                File.Delete(textFile);
            }
        }

        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void FailsIfFileNotFound()
        {
            string fileName = Path.GetTempFileName();
            File.Delete(fileName);
            DelimitedFieldParser parser = new DelimitedFieldParser(fileName);
        }

        [Test]
        public void CanHandleUnixStyleNewLine()
        {
            string newLine = "\n";
            string text = "a;bb;ccc;dddd" + newLine + "e;ff;ggg;hhhh";

            using (StringReader reader = new StringReader(text))
            {
                using (DelimitedFieldParser parser = new DelimitedFieldParser(reader))
                {
                    parser.SetDelimiters(';');
                    Assert.AreEqual("a:bb:ccc:dddd", parser.ReadFields().ToString(":"));
                    Assert.AreEqual("e:ff:ggg:hhhh", parser.ReadFields().ToString(":"));
                }
            }
        }
    }
}
