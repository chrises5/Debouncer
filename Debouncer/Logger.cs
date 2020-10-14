using System;
using System.Diagnostics;
using System.IO;

namespace Debouncer
{
    public class Logger
    {
        private readonly string _dataDir;
        private string LogFilePath => Path.Combine(_dataDir, "Exceptions.log");

        public static Logger Instance { get; private set; }

        public static void CreateInstance(string path)
        {
            if (Instance == null)
                Instance = new Logger(path);
        }

        private Logger(string pathName)
        {
            _dataDir = pathName;
        }

        public static void LogMessage<T>(string message, string methodName)
        {
            LogMessage($"{typeof(T)} {message}", methodName);
        }

        public static void LogMessage(string message, string methodName)
        {
            Debug.WriteLine(FormatString.FormatMessage(message, methodName));
        }

        public static void LogException(Exception ex, string methodName)
        {
            Instance.LogException(FormatString.FormatExceptionString(ex, methodName));
        }

        public static void LogException(Exception ex)
        {
            Instance.LogException(
                FormatString.FormatExceptionString(ex, new StackTrace().GetFrame(1).GetMethod().Name));
        }

        public static void LogException<T>(Exception ex, string methodName)
        {
            Instance.LogException(FormatString.FormatExceptionString<T>(ex, methodName));
        }

        private void LogException(string formattedExceptionText)
        {
            Debug.WriteLine(formattedExceptionText);
            File.AppendAllText(LogFilePath, formattedExceptionText);
        }
    }

    internal static class FormatString
    {
        public static string FormatExceptionString(Exception ex, string methodName)
        {
            var formatted = $"{DateTime.Now}: Exception caught in {methodName}\n{FormatException(ex)}";
            return formatted;
        }

        public static string FormatExceptionString<T>(Exception ex, string methodName)
        {
            return FormatExceptionString(ex, $"{methodName}<{typeof(T)}>");
        }

        public static string FormatMessage(string message, string methodName)
        {
            var formatted = $"{DateTime.Now}: {message} in {methodName}";
            return formatted;
        }

        private static string FormatException(Exception ex)
        {
            var formattedException = $"{ex.Message}\n";
            var innerEx = ex.InnerException;
            while (innerEx != null)
            {
                formattedException += $"{innerEx.Message}\n";
                innerEx = innerEx.InnerException;
            }

            formattedException += ex.StackTrace;
            return formattedException;
        }
    }
}