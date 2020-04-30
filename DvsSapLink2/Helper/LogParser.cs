using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DvsSapLink2.Helper
{
    public static class LogParser
    {
        /// <summary>
        /// Get all log messages from the log file that match the given key
        /// </summary>
        /// <param name="path">Path to the log file</param>
        /// <param name="key">Key of the messages to read</param>
        /// <returns>Enumerable of all messages that match the key</returns>
        public static IEnumerable<string> ReadMessages(string path, string key)
        {
            return File.ReadLines(path)
                .Select(line => Regex.Match(line, $"^.*{key}\\s*(.*?)\\s*$"))
                .Where(match => match.Success)
                .Select(match => match.Groups[1].Value);
        }
    }
}