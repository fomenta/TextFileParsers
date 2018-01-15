// MalformedLineException.cs
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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace TextFileParsers
{
    /// <summary>
    /// Represents the exception that is thrown when the ReadFields method 
    /// cannot parse a row using the specified format.
    /// </summary>
    [Serializable()]
    public class MalformedLineException : FormatException
    {
        private long lineNumber;

        /// <summary>
        /// Initializes a new instance of the MalformedLineException class.
        /// </summary>
        public MalformedLineException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MalformedLineException class with 
        /// a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public MalformedLineException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MalformedLineException class with 
        /// a specified error message and a reference to the inner exception 
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The Exception object that is the cause of the current exception.</param>
        public MalformedLineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MalformedLineException class with 
        /// a specified error message and a line number.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="lineNumber">The line number of the malformed line.</param>
        public MalformedLineException(string message, long lineNumber)
            : base(message)
        {
            this.lineNumber = lineNumber;
        }

        /// <summary>
        /// Initializes a new instance of the MalformedLineException class with 
        /// a specified error message, a line number, and a reference to the 
        /// inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="lineNumber">The line number of the malformed line.</param>
        /// <param name="innerException">The Exception object that is the cause of the current exception.</param>
        public MalformedLineException(string message, long lineNumber, Exception innerException)
            : base(message, innerException)
        {
            this.lineNumber = lineNumber;
        }

        /// <summary>
        /// Initializes a new instance of the MalformedLineException class with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo object that holds the serialized 
        /// object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext structure that contains contextual 
        /// information about the source or destination.</param>
        protected MalformedLineException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                this.lineNumber = info.GetInt64("LineNumber");
            }
            else
            {
                this.lineNumber = -1;
            }
        }

        /// <summary>
        /// Sets the SerializationInfo object with information about the exception
        /// </summary>
        /// <param name="info">The SerializationInfo object that holds the serialized 
        /// object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext structure that contains contextual 
        /// information about the source or destination.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info != null)
            {
                info.AddValue("LineNumber", this.lineNumber, typeof(long));
            }
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Gets and sets the line number where the error was found.
        /// </summary>
        public long LineNumber
        {
            get
            {
                return this.lineNumber;
            }
            set
            {
                this.lineNumber = value;
            }
        }
    }
}
