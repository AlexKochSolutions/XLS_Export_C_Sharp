using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLS_Export_C_Sharp
{
    internal class PrintOption
    {
        // mode 1 = Console
        // mode 2 = File
        public static int printmode = 3;
        public static string _logPath = "log.txt";
        private static FileStream? _fileStream;
        private static StreamWriter? _streamWriter;

        public static void Print(string output)
        {
            int consoleprint = printmode & 1;
            int fileprint = printmode & 2;
            if (consoleprint != 0)
            {
                Console.WriteLine(output);
            }
            if (fileprint != 0)
            {
                Log(output);
            }
        }
        public static void StartLog(string logPath = "")
        {
            if (logPath != null && logPath.Length > 0)
            {
                _logPath = logPath;
            }
            InitializeLogFile();
        }

        private static void InitializeLogFile()
        {
            try
            {
                // 1. Delete the old file if it exists at startup
                if (File.Exists(_logPath))
                {
                    File.Delete(_logPath);
                }

                // 2. Open a continuous, exclusive file handle for appending
                // FileShare.Read allows other programs (like Notepad) to view the log while we write to it
                _fileStream = new FileStream(_logPath, FileMode.Append, FileAccess.Write, FileShare.Read);

                // StreamWriter handles converting strings into raw bytes smoothly
                _streamWriter = new StreamWriter(_fileStream, Encoding.UTF8);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Could not initialize log file: {ex.Message}");
                throw;
            }
        }

        // 3. Method to continuously append text during the program lifetime
        public static void Log(string message)
        {
            if (_streamWriter == null) return;

            lock (_streamWriter) // Ensures thread-safety if your app uses multiple threads
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
                _streamWriter.WriteLine(logEntry);

                // Forces C# to push the text immediately out of memory buffers into the OS file handle
                _streamWriter.Flush();
            }
        }

        // 4. Properly release the file handle when the application closes
        public static void Dispose()
        {
            _streamWriter?.Dispose();
            _fileStream?.Dispose();
        }
    }
}
