using System;
using System.Text;
using System.IO;

namespace MonoUtilities.Logging
{
    public class Logger
    {
        private readonly string file;
        private readonly string fileType;
        readonly Encoding enc;
        public Logger(string fileName, Encoding encoding)
        {
            string[] _file = fileName.Split('.');
            if (_file.Length > 1)
            {
                fileName = fileName.Substring(0, fileName.Length - (_file[_file.Length - 1].Length + 1));
                fileType = string.Format(".{0}", _file[_file.Length - 1]);
            }
            else
                fileType = ".log";

            file = fileName;

            enc = encoding;
        }

        public void Info(string message)
        {
            string[] args = { "INFO", DateTime.Now.ToString(), message };
            string _file = file + "_" + DateTime.Now.ToString("yyyy-MM-dd") + fileType;
            while (true)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(_file, true, enc))
                    {
                        writer.WriteLine(string.Format("[{0}] {1}: {2}", args));
                    }
                    break;
                }
                catch { }
            }

        }

        public void Warning(string message)
        {
            string[] args = { "WARNING", DateTime.Now.ToString(), message };
            string _file = file + "_" + DateTime.Now.ToString("yyyy-MM-dd") + fileType;
            while (true)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(_file, true, enc))
                    {
                        writer.WriteLine(string.Format("[{0}] {1}: {2}", args));
                    }
                    break;
                }
                catch { }
            }
        }

        public void Fatal(string message)
        {
            string[] args = { "FATAL", DateTime.Now.ToString(), message };
            string _file = file + "_" + DateTime.Now.ToString("yyyy-MM-dd") + fileType;
            while (true)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(_file, true, enc))
                    {
                        writer.WriteLine(string.Format("[{0}] {1}: {2}", args));
                    }
                    break;
                }
                catch { }
            }
        }

        public void WriteLine(string header, string message)
        {

            string[] args = { header, DateTime.Now.ToString(), message };
            string _file = file + "_" + DateTime.Now.ToString("yyyy-MM-dd") + fileType;
            while (true)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(_file, true, enc))
                    {
                        writer.WriteLine(string.Format("[{0}] {1}: {2}", args));
                    }
                    break;
                }
                catch { }

            }
        }
    }
}
