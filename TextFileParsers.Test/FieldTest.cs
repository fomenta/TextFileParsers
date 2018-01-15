using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace TextFileParsers
{
    [TestFixture]
    public class TextFieldTest
    {
        [Test]
        public void ItemCount()
        {
            string text = "01732:Juan Perez:0100.00:11/05/2002";
            TextFields fields = new TextFields(text.Split(':'));
            Assert.AreEqual(4, fields.Count);
        }

        [Test]
        public void FieldsTypes()
        {
            string text = "01732:Juan Perez:0100.00:11/05/2002";
            TextFields fields = new TextFields(text.Split(':'));

            Assert.AreEqual("01732", fields[0]);
            Assert.AreEqual(1732, fields.GetInt16(0));
            Assert.AreEqual(1732, fields.GetInt32(0));
            Assert.AreEqual(1732, fields.GetInt64(0));

            Assert.AreEqual("Juan Perez", fields.GetString(1));

            Assert.AreEqual("0100.00", fields[2]);
            Assert.AreEqual(100.00f, fields.GetSingle(2));
            Assert.AreEqual(100.00f, fields.GetDouble(2));

            Assert.AreEqual("11/05/2002", fields[3]);
            Assert.AreEqual(new DateTime(2002, 11, 05), fields.GetDateTime(3));
        }

        [Test, ExpectedException(typeof(IndexOutOfRangeException))]
        public void FailsIfBadIndex()
        {
            string text = "01732:Juan Perez:0100.00:11/05/2002";
            TextFields fields = new TextFields(text.Split(':'));
            fields.GetString(4);
        }

        [Test, ExpectedException(typeof(FormatException))]
        public void WrongType()
        {
            string text = "01732:Juan Perez:0100.00:11/05/2002";
            TextFields fields = new TextFields(text.Split(':'));
            fields.GetInt32(1);
        }

        //[Test]
        //public void Enumeration()
        //{
        //    string text = "01732:Juan Perez:0100.00:11/05/2002";
        //    TextFields fields = new TextFields(text.Split(':'));

        //    int i = 0;
        //    foreach (object field in fields)
        //    {
        //        i++;
        //    }

        //    Assert.AreEqual(4, i);
        //}

    }
}
