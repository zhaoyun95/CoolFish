using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CoolFishNS.Utilities
{
    // This logger will derive from the Microsoft.Build.Utilities.Logger class, 
    // which provides it with getters and setters for Verbosity and Parameters, 
    // and a default empty Shutdown() implementation. 
    internal class BasicFileLogger : Logger
    {
        private int indent;

        /// <summary>
        ///     Initialize is guaranteed to be called by MSBuild at the start of the build
        ///     before any events are raised.
        /// </summary>
        public override void Initialize(IEventSource eventSource)
        {
            // For brevity, we'll only register for certain event types. Loggers can also 
            // register to handle TargetStarted/Finished and other events.
            eventSource.ProjectStarted += eventSource_ProjectStarted;
            eventSource.TaskStarted += eventSource_TaskStarted;
            eventSource.MessageRaised += eventSource_MessageRaised;
            eventSource.WarningRaised += eventSource_WarningRaised;
            eventSource.ErrorRaised += eventSource_ErrorRaised;
            eventSource.ProjectFinished += eventSource_ProjectFinished;
        }

        private void eventSource_ErrorRaised(object sender, BuildErrorEventArgs e)
        {
            // BuildErrorEventArgs adds LineNumber, ColumnNumber, File, amongst other parameters 
            string line = String.Format(": ERROR {0}({1},{2}): ", e.File, e.LineNumber, e.ColumnNumber);
            WriteLineWithSenderAndMessage(line, e);
        }

        private void eventSource_WarningRaised(object sender, BuildWarningEventArgs e)
        {
            // BuildWarningEventArgs adds LineNumber, ColumnNumber, File, amongst other parameters 
            string line = String.Format(": Warning {0}({1},{2}): ", e.File, e.LineNumber, e.ColumnNumber);
            WriteLineWithSenderAndMessage(line, e);
        }

        private void eventSource_MessageRaised(object sender, BuildMessageEventArgs e)
        {
            // BuildMessageEventArgs adds Importance to BuildEventArgs 
            // Let's take account of the verbosity setting we've been passed in deciding whether to log the message 
            if ((e.Importance == MessageImportance.High && IsVerbosityAtLeast(LoggerVerbosity.Minimal))
                || (e.Importance == MessageImportance.Normal && IsVerbosityAtLeast(LoggerVerbosity.Normal))
                || (e.Importance == MessageImportance.Low && IsVerbosityAtLeast(LoggerVerbosity.Detailed))
                )
            {
                WriteLineWithSenderAndMessage(String.Empty, e);
            }
        }

        private void eventSource_TaskStarted(object sender, TaskStartedEventArgs e)
        {
            // TaskStartedEventArgs adds ProjectFile, TaskFile, TaskName 
            // To keep this log clean, this logger will ignore these events.
        }

        private void eventSource_ProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            // ProjectStartedEventArgs adds ProjectFile, TargetNames 
            // Just the regular message string is good enough here, so just display that.
            WriteLine(String.Empty, e);
            indent++;
        }

        private void eventSource_ProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            // The regular message string is good enough here too.
            indent--;
            WriteLine(String.Empty, e);
        }

        /// <summary>
        ///     Write a line to the log, adding the SenderName and Message
        ///     (these parameters are on all MSBuild event argument objects)
        /// </summary>
        private void WriteLineWithSenderAndMessage(string line, BuildEventArgs e)
        {
            WriteLine(e.SenderName + ": " + line, e);
        }

        /// <summary>
        ///     Just write a line to the log
        /// </summary>
        private void WriteLine(string line, BuildEventArgs e)
        {
            string output = string.Empty;
            for (int i = indent; i > 0; i--)
            {
                output += "\t";
            }


            Logging.Log(output + line + e.Message);
        }

        /// <summary>
        ///     Shutdown() is guaranteed to be called by MSBuild at the end of the build, after all
        ///     events have been raised.
        /// </summary>
        public override void Shutdown()
        {
        }
    }
}