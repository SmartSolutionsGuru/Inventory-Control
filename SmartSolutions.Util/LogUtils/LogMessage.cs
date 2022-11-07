using SmartSolutions.InventoryControl.Plugins.Logs;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace SmartSolutions.Util.LogUtils
{
    /// <summary>
    /// Logger Class for writing Logs
    /// </summary>
    public static class LogMessage
    {
        #region [Private Members]
        private static ILogger log;
        #endregion

        #region [Constructor]
        static LogMessage()
        {
            log = InventoryControl.Plugins.IoC.IoCContanier.IoC?.GetInstance(typeof(ILogger)) as ILogger;
            //WriteLogInFile($"Logger Plugin {(log == null ? "Not Found" : $"({log.Name}) Found")}");
        }
        #endregion

        #region [Public Methods]  
        /// <summary>
        /// Method for Setting up Log File Name
        /// </summary>
        /// <param name="filename_prefix"></param>
        public static void Setup(string filename_prefix)
        {
            log?.Setup(filename_prefix);
        }
        //public static void Write(string text, Levels level = Levels.Info, bool isHourLogFile = false)
        //{
        //    if (log != null)
        //        log?.Write(text, (CloudGate.Plugins.Logs.LogLevels)(int)level, isHourLogFile);
        //    else
        //        WriteLogInFile(text);
        //}

        /// <summary>
        /// Method For Writing Log 
        /// </summary>
        /// <param name="textAction"></param>
        /// <param name="level"> Level Of Severity</param>
        /// <param name="isHourLogFile"> is Hour file logging or Not</param>
        public static void Write(Func<string> textAction, Levels level = Levels.Info, bool isHourLogFile = false)
        {
            if (log != null)
                log?.Write(textAction, (LogLevels)(int)level, isHourLogFile);
            else if (textAction != null)
                WriteLogInFile(textAction?.Invoke());
        }

        public static void Write(string text,
            Levels level = Levels.Info,
            bool isHourLogFile = false,
            [CallerMemberName] string origin = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (log != null)
                log?.Write(text, (LogLevels)(int)level, isHourLogFile);
            else
                WriteLogInFile(text);
        }

        public static Task<string> ReadAllAsync()
        {
            if (log != null)
                return log.ReadAllAsync();
            return Task.FromResult<string>(null);
        }

        public static void SetCurrentLevel(Levels level)
        {
            log?.SetCurrentLevel((LogLevels)(int)level);
        }

        public static Levels GetCurrentLevel()
        {
            return (Levels)(int)(log?.GetCurrentLevel() ?? LogLevels.Error);
        }
        #endregion

        #region [Private Methods]      
        private static void WriteLogInFile(string msg, bool isHrLogFile = false)
        {
            try
            {
                //System.Diagnostics.Debug.WriteLine(SoloDateTime.LocationNow.ToString("hh:mm:ss tt") + ": " + msg);
                //lock (objLock)
                {
                    var nFileName = isHrLogFile ? string.Format("{0}_{1}", DateTime.Now.Day, DateTime.Now.Hour) : DateTime.Now.ToString("yyyyMMdd");
                    nFileName = "_Log_" + nFileName + ".txt";

                    //if (System.Diagnostics.Debugger.IsAttached)
                    //    Console.WriteLine(SoloDateTime.LocationNow.ToString("hh:mm:ss tt") + ": " + msg);

                    string filePath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath), "Logs");
                    
                    if (!Directory.Exists(filePath))
                    {
                        //var writeStream = await file.
                        //Windows.Storage.StorageFolder directory = new Windows.Storage.StorageFolder(); 
                        Directory.CreateDirectory(filePath);
                    }

                    var fileStream = new FileStream(filePath + "\\" + nFileName, FileMode.OpenOrCreate, FileAccess.Write);
                    using (StreamWriter mStreamWriter = new StreamWriter(fileStream))
                    {
                        mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
                        mStreamWriter.Write(DateTime.Now.ToString("hh:mm:ss tt") + ": ");
                        mStreamWriter.WriteLine(msg);
                        mStreamWriter.Flush();
                        mStreamWriter.Close();
                        fileStream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region [Enum]      
        public enum Levels
        {
            [Description("FATAL")]
            Fatal = 0,
            [Description("ERROR")]
            Error = 1,
            [Description("WARN")]
            Warn = 2,
            [Description("INFO")]
            Info = 3,
            [Description("DEBUG")]
            Debug = 4,
            [Description("TRACE")]
            Trace = 5,
            [Description("OFF")]
            Off = 6
        }
        #endregion

    }
}
