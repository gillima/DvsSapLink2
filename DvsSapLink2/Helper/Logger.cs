using System;
using System.IO;

namespace DvsSapLink2.Helper
{
    public class Logger : IDisposable
    {
        private readonly StreamWriter stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        public Logger(string path)
        {
            this.stream = new StreamWriter(path, append: true);
        }

        /// <summary>
        /// Writes a log entry
        /// </summary>
        /// <param name="key">Log entry key</param>
        /// <param name="message">Message to log</param>
        public void Write(string key, string message)
        {
            var now = DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss");
            var line = $"{Environment.UserName,-14}{now,-23}{key,-7}{message}";
            this.stream.WriteLine(line);
            this.stream.Flush();
        }

        /// <summary>
        /// Releases all resources used by this <see cref="Logger"/> instance
        /// </summary>
        public void Dispose()
        {
            this.stream?.Dispose();
        }
    }
}