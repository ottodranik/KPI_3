using System.IO;
using System.Threading;

namespace IIG.FileWorker
{
    public class BaseFileWorker
    {
        /// <summary>
        ///     Reads lines from given path
        /// </summary>
        /// <param name="path">Array of read string</param>
        /// <returns></returns>
        public static string[] ReadLines(string path)
        {
            return File.Exists(path)
                ? File.ReadAllLines(path)
                : null;
        }

        /// <summary>
        ///     Reads file as text
        /// </summary>
        /// <param name="path">Content of file</param>
        /// <returns></returns>
        public static string ReadAll(string path)
        {
            return File.Exists(path)
                ? File.ReadAllText(path)
                : null;
        }

        /// <summary>
        ///     Writes text to file
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="path">Path to file</param>
        /// <returns>Result of write: <see langword="true" /> if success, otherwise - <see langword="false" /></returns>
        public static bool Write(string text, string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            try
            {
                var sw = new StreamWriter(path);
                sw.Write(text);
                sw.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Tries to write text given number of times
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="path">Path to file</param>
        /// <param name="tries">Number of tries</param>
        /// <returns>Result of write: <see langword="true" /> if success, otherwise - <see langword="false" /></returns>
        public static bool TryWrite(string text, string path, int tries = 1)
        {
            if (string.IsNullOrEmpty(path) || tries < 1)
                return false;
            var counter = 0;
            var res = false;
            while (!res && counter++ < tries)
            {
                res = Write(text, path);
                Thread.Sleep(100);
            }

            return res;
        }

        /// <summary>
        ///     Tries to Copy Source File to Destination File
        /// </summary>
        /// <param name="from">Source File</param>
        /// <param name="to">Destination File</param>
        /// <param name="rewrite">Rewrite Destination File Flag</param>
        /// <param name="tries">Number of tries</param>
        /// <returns>Result of copy: <see langword="true" /> if success, otherwise - <see langword="false" /></returns>
        public static bool TryCopy(string from, string to, bool rewrite, int tries = 1)
        {
            if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(from) || tries < 1)
                return false;
            var counter = 0;
            var res = false;
            while (!res && counter++ < tries)
            {
                File.Copy(from, to, rewrite);
                res = File.Exists(to);
                Thread.Sleep(100);
            }

            return res;
        }

        /// <summary>
        ///     Returns Full Path of Given Path
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>Full Path if success, otherwise - <see langword="null" /></returns>
        public static string GetFullPath(string path)
        {
            return File.Exists(path)
                ? new FileInfo(path).FullName
                : null;
        }

        /// <summary>
        ///     Returns File Name from Given Path
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>File Name if success, otherwise - <see langword="null" /></returns>
        public static string GetFileName(string path)
        {
            return File.Exists(path)
                ? new FileInfo(path).Name
                : null;
        }

        /// <summary>
        ///     Returns Directory Path of Given Path
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>Directory Path if success, otherwise - <see langword="null" /></returns>
        public static string GetPath(string path)
        {
            return File.Exists(path)
                ? new FileInfo(path).Directory?.FullName
                : null;
        }

        /// <summary>
        ///     Creates Directory
        /// </summary>
        /// <param name="name">Directory Name</param>
        /// <returns>Full Path if success, otherwise - <see langword="null" /></returns>
        public static string MkDir(string name)
        {
            if (!Directory.Exists(name))
                Directory.CreateDirectory(name);
            return new DirectoryInfo(name).FullName;
        }
    }
}