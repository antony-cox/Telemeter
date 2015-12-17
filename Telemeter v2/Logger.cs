using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Telemeter_v2
{
    public class Logger
    {
        string path;
        
        public enum Type
        {
            Info, Error, Warning
        }
        
        public Logger()
        {
            path = GetPath() + "\\log.txt";
            Trace.Listeners.Add(new TextWriterTraceListener(path));
            Trace.AutoFlush = true;
            Trace.Indent();
            DisposeLog();
        }

        private string GetPath()
        {
            string[] pathArrray = System.Reflection.Assembly.GetEntryAssembly().Location.ToString().Split('\\');
            string returnPath = "";

            for (int i = 0; i < pathArrray.Length - 1; i++)
            {
                returnPath += pathArrray[i] + "\\";
            }

            return returnPath;
        }

        private void DisposeLog()
        {
            File.WriteAllText(path, String.Empty);
        }

        public void Write(string message, Type type)
        {
            string date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();

            switch (type)
            {
                case Type.Info:
                    Trace.TraceInformation("INFO:\t" + date + "\t" + message + "..");
                    break;
                case Type.Error:
                    Trace.TraceError("ERROR:\t" + date + "\t" + message);
                    break;
                case Type.Warning:
                    Trace.TraceWarning("WARNING:\t" + date + "\t" + message);
                    break;
            }
        }
    }
}
