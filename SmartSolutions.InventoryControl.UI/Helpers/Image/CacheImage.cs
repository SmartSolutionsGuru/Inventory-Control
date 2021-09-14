using SmartSolutions.InventoryControl.DAL;
using SmartSolutions.InventoryControl.Plugins.Image;
using SmartSolutions.Util.LogUtils;
using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;

namespace SmartSolutions.InventoryControl.UI.Helpers.Image
{
    [Export(typeof(ICacheImage)),PartCreationPolicy(CreationPolicy.Shared)]
    public class CacheImage : ICacheImage
    {
        #region Private Members
        public string ImageFolderPath { get; set; }
        #endregion

        #region Constructor
        [ImportingConstructor]
        public CacheImage()
        {
            try
            {
                
                //ImageFolderPath = Path.Combine(Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetEntryAssembly().CodeBase).LocalPath), "ImageCache");
                ImageFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SmartSolutions", "ImageCache");
                if (!Directory.Exists(ImageFolderPath))
                    Directory.CreateDirectory(ImageFolderPath);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

        /// <summary>
        /// save Image to Image Given Path
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public  string SaveImageToDirectory(byte[] image, string imageName)
        {
            string retVal = string.Empty;
            try
            {
                if (image == null) return string.Empty;

                File.WriteAllBytes($"{AppSettings.ImageCachedFolderPath}\\{imageName}", image);
                retVal = Path.Combine(AppSettings.ImageCachedFolderPath, imageName);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        /// <summary>
        /// Get the Image From Image Path
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public byte[] GetImageFromPath(string imagePath)
        {
            if(imagePath == null) return null;
            byte[] image = null;
            try
            {
                Bitmap bitmap = new Bitmap(imagePath);
                image =  bitmap.ToByteArray();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return image;
        }
    }
}
