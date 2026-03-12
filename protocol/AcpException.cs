using System;

namespace dotacp.protocol
{
    /// <summary>
    /// Represents an application-specific exception that includes an error code and optional contextual data.
    /// </summary>
    public class AcpException : Exception
    {
        /// <summary>
        /// The application-specific error code associated with this exception.
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// Optional contextual data associated with this exception.
        /// </summary>
        public object ErrorData { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AcpException"/> class with a specified error code, message, and optional contextual data.
        /// </summary>
        /// <param name="code">The application-specific error code.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="data">Optional contextual data associated with the exception.</param>
        public AcpException(int code, string message, object data = null)
            : base(message)
        {
            Code = code;
            ErrorData = data;
        }
    }
}
