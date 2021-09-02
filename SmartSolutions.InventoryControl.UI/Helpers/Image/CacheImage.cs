using SmartSolutions.InventoryControl.DAL;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.UI.Helpers.Image
{
    public class CacheImage
    {
        #region Private Members
        private readonly  string ImageFolderPath;
        #endregion

        #region Constructor
         public CacheImage()
        {
            try
            {
                ImageFolderPath = Path.Combine(Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetEntryAssembly().CodeBase).LocalPath),"ImageCache");
                if (!Directory.Exists(ImageFolderPath))
                    Directory.CreateDirectory(ImageFolderPath);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion


    }
}
