using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoolFishNS.Utilities
{
    /// <summary>
    ///     This class handles all logging done for Coolfish. From simple output, to writing to files, to logging exceptions
    ///     and user messages. It is event based, and thread safe.
    /// </summary>
    /// <example>Logging.Write("Oh noes! We failed again!");</example>
    public class Logging : IDisposable
    {
        #region Delegates

        /// <summary>
        ///     Occurs when Logging.Write is called
        /// </summary>
        public static event WriteDelegate OnWrite;

        /// <summary>
        ///     Occurs when Logging.Log is called
        /// </summary>
        public static event LogDelegate OnLog;

        /// <summary>
        ///     Delegate method that log events should conform to if they are to be fired
        /// </summary>
        public delegate void WriteDelegate(object sender, MessageEventArgs e);

        /// <summary>
        ///     Delegate method that log events should conform to if they are to be fired
        /// </summary>
        public delegate void LogDelegate(object sender, MessageEventArgs e);

        #endregion

        /// <summary>
        ///     The active logging instance for logging stuff to a file/textbox
        /// </summary>
        public static Logging Instance;

        private Thread _processThread;
        private readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();
        private readonly ManualResetEvent _terminate = new ManualResetEvent(false);
        private readonly ManualResetEvent _waiting = new ManualResetEvent(false);
        private readonly ManualResetEvent _hasNewItems = new ManualResetEvent(false);

        internal Logging()
        {
            _processThread = new Thread(ProcessQueue) {IsBackground = true};
            _processThread.Start();
        }

        private static void WriteAsync(object format, params object[] args)
        {
            OnWrite("Logging.Write", new MessageEventArgs(string.Format(format.ToString(), args)));
        }

        private static void LogAsync(object format, params object[] args)
        {

            OnLog("Logging.Log", new MessageEventArgs(string.Format(format.ToString(), args)));
            
        }

        /// <summary>
        ///     Writes the specified message to the message queue.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public static void Write(object format, params object[] args)
        {
            if (format != null)
            {
                Instance._queue.Enqueue(() => WriteAsync(format, args));
                Instance._hasNewItems.Set();
                
            }
            
        }

        /// <summary>
        ///     Logs the specified message to the message queue.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public static void Log(object format, params object[] args)
        {
             if (format != null)
            {
                Instance._queue.Enqueue(() => LogAsync(format, args));
                Instance._hasNewItems.Set();
                
            }
        }

        private void ProcessQueue()
        {
            while (true)
            {
                try
                {
                    _waiting.Set();
                    int i = WaitHandle.WaitAny(new WaitHandle[] { _hasNewItems, _terminate });


                    _hasNewItems.Reset();
                    _waiting.Reset();
                    while (!_queue.IsEmpty)
                    {
                        Action item;
                        if (_queue.TryDequeue(out item))
                        {
                                item();
                            
                        }
                    }

                    if (i == 1)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
                

            }
        }

        /// <summary>
        ///     Blocks the current thread until all messages have been flushed from the logging queue
        /// </summary>
        public static void Flush()
        {
            Instance._waiting.WaitOne();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _terminate.Set();
                _processThread = null;
                try
                {
                    _waiting.Dispose();
                    _terminate.Dispose();
                    _hasNewItems.Dispose();
                }
                catch (Exception ex)
                {
                    
                    Log(ex);
                }
                
            }
        }

        ~Logging()
        {
            Dispose(false);
        }

        #endregion
    }
}