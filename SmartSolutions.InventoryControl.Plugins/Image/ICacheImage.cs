using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.Plugins.Image
{
    public interface ICacheImage
    {
        #region Methods
        string SaveImageToDirectory(byte[] image, string imageName);
        byte[] GetImageFromPath(string imagePath);
        #endregion

        #region Properties
        string ImageFolderPath { get; set; }
        #endregion
    }
}
