using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolFishNS.Utilities
{
    /// <summary>
    /// EventArgs for Message Logging
    /// </summary>
    public class MessageEventArgs : EventArgs
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public MessageEventArgs(string message)
        {
            this.Message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public MessageEventArgs(string message,Exception exception) : this(message)
        {
            this.Exception = exception;
        }
        /// <summary>
        /// Message that is passed
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Exception to be passed if there is one
        /// </summary>
        public Exception Exception { get; set; }
    }
}
