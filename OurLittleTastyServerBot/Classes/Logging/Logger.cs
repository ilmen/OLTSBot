using System;

namespace OurLittleTastyServerBot.Classes.Logging
{
    public class Logger
    {
        public static class Singleton
        {
            private static readonly Logger Instance = new Logger();

            public static Logger GetInstance()
            {
                return Instance;
            }
        }

        // TODO Нормально реализовать логгер

        private string GetTimeMark()
        {
            // TODO FIXME Не тестируемое
            var time = DateTime.Now;

            return time.ToLongTimeString();
        }

        public void WriteInfo(string text)
        {
            Console.WriteLine(GetTimeMark() + " INFO: " + text);
        }

        public void WriteDebug(string text)
        {
            Console.WriteLine(GetTimeMark() + " DEBUG: " + text);
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        public void WriteError(string text, Exception exception = null)
        {
            Console.WriteLine(GetTimeMark() + " ERROR: " + text + Environment.NewLine + (exception != null ? exception.Message : String.Empty) + Environment.NewLine + Environment.NewLine);
        }
    }
}