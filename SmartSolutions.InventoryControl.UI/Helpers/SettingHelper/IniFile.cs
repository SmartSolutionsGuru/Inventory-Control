using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SmartSolutions.InventoryControl.UI.Helpers.SettingHelper
{
    public class IniFile
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, string lpReturnString, int nSize, string lpFilename);

        public string path;

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <param name="INIPath"></param>
        public IniFile(string INIPath)
        {
            path = INIPath;
        }
        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <param name="Section"></param>
        /// Section name
        /// <param name="Key"></param>
        /// Key Name
        /// <param name="Value"></param>
        /// Value Name
        public void WriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        public string ReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }

        public System.Collections.Generic.IEnumerable<string> ReadKeys(string section)
        {
            string result = new string(' ', 32000);
            GetPrivateProfileString(section, null, "", result, 32000, this.path);
            return result.Split(new char[] { '\0' }).Where(x => !string.IsNullOrWhiteSpace(x));
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return ReadValue(Section, Key).Length > 0;
        }

        public void DeleteSection(string Section = null)
        {
            WriteValue(Section, null, null);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            WriteValue(Section, Key, null);
        }
    }
}
