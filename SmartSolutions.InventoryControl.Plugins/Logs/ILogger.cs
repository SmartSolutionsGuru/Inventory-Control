using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.Plugins.Logs
{
    public interface ILogger : IPlugin
    {
        void Setup(string filename_prefix);
        // void Write(string text, LogLevels level = LogLevels.Info, bool isHourLogFile = false);
        void Write(string text, LogLevels level = LogLevels.Info, bool isHourLogFile = false, [CallerMemberName] string origin = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0);
        void Write(Func<string> textAction, LogLevels level = LogLevels.Info, bool isHourLogFile = false);
        Task<string> ReadAllAsync();
        void SetCurrentLevel(LogLevels level);
        LogLevels GetCurrentLevel();
    }

    public enum LogLevels
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
}
