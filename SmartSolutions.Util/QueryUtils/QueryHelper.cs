using System;
using System.Linq;
using System.IO;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.Util.QueryUtils
{
    public class QueryHelper
    {
        private static System.Reflection.Assembly m_EntryAssembly;
        private static System.Reflection.Assembly EntryAssembly => m_EntryAssembly = m_EntryAssembly ?? System.Reflection.Assembly.GetEntryAssembly();
        private static string QUERY_FILES_PATH = string.Join(".", EntryAssembly.GetName().Name, "Resources", "Queries");

        public static void SetRootPath(string path)
        {
            QUERY_FILES_PATH = path;
        }

        public static string GetQuery(string FileName, DBTypes Type)
        {
            string Command = null;
            try
            {
                var resultPath = string.Join(".", QUERY_FILES_PATH, $"{FileName}.sql");
                using (Stream sourceStream = EntryAssembly.GetManifestResourceStream(string.Join(".", QUERY_FILES_PATH, $"{FileName}.sql")))
                {
                    using (StreamReader reader = new StreamReader(sourceStream))
                    {
                        try
                        {
                            string steps = reader.ReadToEnd();
                            string SplitString = "/*" + Type + "-START*/";
                            string[] Result = steps.Split(new string[] { SplitString }, StringSplitOptions.None);
                            Result = Result?.ElementAtOrDefault(1)?.Split(new string[] { "/*" + Type + "-END*/" }, StringSplitOptions.None);
                            Command = Result?.ElementAtOrDefault(0);
                        }
                        catch (Exception ex)
                        {
                            LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
            }
            return Command;
        }
    }
}
