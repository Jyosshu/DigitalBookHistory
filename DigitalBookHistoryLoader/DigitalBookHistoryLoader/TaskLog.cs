using System;
using System.Text;
using System.IO;

namespace DigitalBookHistoryLoader
{
    public class TaskLog
    {
        private StringBuilder sbLogfile = new StringBuilder();
        private FileInfo fiLogfile;

        public TaskLog(string logfileFullName)
        {
            fiLogfile = new FileInfo(logfileFullName);
        }

        public void AppendText(string textToAppend)
        {
            sbLogfile.Append(textToAppend);
        }

        public void AppendLine(string textToAppend)
        {
            sbLogfile.AppendLine(textToAppend);
        }

        public int Length()
        {
            return sbLogfile.Length;
        }

        public void Close()
        {
            try
            {
                using (StreamWriter sw = fiLogfile.AppendText())
                {
                    sw.Write(sbLogfile.ToString());
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("An exception was caught!  {0}", ex.Message);
            }
        }
    }
}
