using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using log4net;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ServidorSorrySliders
{
    public class Logger
    {
        public ILog Log { get; set; }

        public Logger(Type declaringType)
        {
            Log = LogManager.GetLogger(declaringType);
        }
        
        public Logger(Type declaringType, string nombreInterfaz)
        {
            Log = LogManager.GetLogger(declaringType+$" {nombreInterfaz}");
        }
        public void LogDebug(string message, Exception ex)
        {
            Log.Debug(message, ex);
        }
        public void LogDebug(string message)
        {
            Log.Debug(message);
        }
        public void LogInfo(string message, Exception ex)
        {
            Log.Info(message, ex);
        }
        public void LogInfo(string message)
        {
            Log.Info(message);
        }
        public void LogError(string message, Exception ex)
        {
            Log.Error(message, ex);
        }
        public void LogError(string message)
        {
            Log.Error(message);
        }
        public void LogWarn(string message, Exception ex)
        {
            Log.Warn(message, ex);
        }
        public void LogWarn(string message)
        {
            Log.Warn(message);
        }
        public void LogFatal(string message, Exception ex)
        {
            Log.Fatal(message, ex);
        }
        public void LogFatal(string message)
        {
            Log.Fatal(message);
        }
    }
}
