using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bridor.EzPrint.Helpers
{
    public sealed class LogWriter
    {
        #region -  Private Fields  -
        //
        // Only instance of the log writer
        //
        private static LogWriter instance;
        //
        // sync lock to make the class thread safe
        //
        private static object syncLock = new object();
        //
        // Log file
        //
        private string logFile;
        //
        // Bytes in a MegaByte
        //
        private const long MEGABYTE = 1048576;
        #endregion

        #region -  Properties  -
        /// <summary>
        /// Get the single instance of the LogWriter Class
        /// </summary>
        public static LogWriter Instance {
            get {
                lock (syncLock) {
                    if (instance == null) {
                        instance = new LogWriter();
                    }
                    return instance;
                }
            }
        }
        /// <summary>
        /// Sets the maximum size of the log in MB (megabytes)
        /// </summary>
        public int MaximumSize { get; set; }
        /// <summary>
        /// Let's the user set a flag to write message denoted with verbose
        /// </summary>
        public bool Verbose { get; set; }
        #endregion

        #region -  Constructor  -
        private LogWriter() {
            // Set the log file path
            logFile = String.Format(@"{0}\{1}.log",
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
               Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
            // Set the default size to 3MB
            MaximumSize = 3;
        }
        #endregion

        #region -  Methods  -
        /// <summary>
        /// Writes inforamtion messages to the log file
        /// </summary>
        /// <param name="header">Log message header, generally the source of the message</param>
        /// <param name="messages">Information messages to write to the log separated by commas</param>
        public void Message(string header, params string[] messages) {
            WriteToLog("Info", header, messages);
        }
        /// <summary>
        /// Writes informaiton message to the log file using the format string
        /// </summary>
        /// <param name="header">Log message header, generally the source of the message</param>
        /// <param name="format">String format to display the values</param>
        /// <param name="values">values to use in the string format</param>
        public void MessageFormat(string header, string format, params string[] values) {
            WriteToLog("Info", header, String.Format(format, values));
        }
        /// <summary>
        /// Writes error messages to the log file
        /// </summary>
        /// <param name="header">Log message header, generally the source of the message</param>
        /// <param name="messages">Error messages to write to the log separated by commas</param>
        public void Error(string header, params string[] messages) {
            WriteToLog("Error", header, messages);
        }
        /// <summary>
        /// Writes error message to the log file using the format string
        /// </summary>
        /// <param name="header">Log message header, generally the source of the message</param>
        /// <param name="format">String format to display the values</param>
        /// <param name="values">values to use in the string format</param>
        public void ErrorFormat(string header, string format, params string[] values) {
            WriteToLog("Error", header, String.Format(format, values));
        }
        //
        // Write the messages to the log file.
        //
        private void WriteToLog(string type, string header, params string[] messages) {
            StreamWriter writer = null;

            try {
                writer = new StreamWriter(this.logFile, true);
                foreach (string message in messages) {
                    writer.WriteLine("{0}\t{1}\t{2}: {3}",
                        DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss.fff"),
                        type,
                        header,
                        message.Replace("\r\n", "<CRLF>"));
                }
            } catch { } finally {
                if (writer != null)
                    writer.Close();
            }
            BackupLog();
        }
        //
        // Checks the log files size and if it is more than the maximum size it creates a backup of it
        // and allows the wirter to create a new file on the next write
        //
        private void BackupLog() {
            FileInfo logInfo = new FileInfo(this.logFile);
            if (logInfo.Length > (MEGABYTE * this.MaximumSize)) {
                // The log file has reached the maximum size
                for (int i = 4; i >= 1; i--) {
                    if (File.Exists(String.Format("{0}.{1}", this.logFile, i))) {
                        if (i == 4) {
                            // This file needs to be replaced, delete the current one
                            File.Delete(String.Format("{0}.{1}", this.logFile, i));
                        } else {
                            File.Move(String.Format("{0}.{1}", this.logFile, i), String.Format("{0}.{1}", this.logFile, i + 1));
                        }
                    }
                }
                // Make the current one .1
                File.Move(this.logFile, String.Format("{0}.1", this.logFile));
            }
        }
        #endregion
    }
}
