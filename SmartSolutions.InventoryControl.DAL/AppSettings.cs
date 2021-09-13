using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL
{
   public class AppSettings
    {
        /// <summary>
        /// The directory where the app is runing
        /// </summary>
        public static string AppDirectory => AppDomain.CurrentDomain?.BaseDirectory;

        /// <summary>
        /// The Design files to be used in the app
        /// </summary>
        public static Dictionary<string, string> DesignFiles { get; set; }

        /// <summary>
        /// The custom configurations to be used in the app
        /// </summary>
        public static Dictionary<string, string> Configurations { get; set; }
        /// <summary>
        /// The Property which Decide either databackup Path is Set or Not
        /// </summary>
        public static bool Backup { get; set; }
        /// <summary>
        /// Logged in User 
        /// </summary>
        public static Models.Authentication.IdentityUserModel LoggedInUser { get; set; }
        public static bool IsLoggedInUserAdmin { get; set; }
        /// <summary>
        /// Proprietor Information
        /// </summary>
        public static DAL.Models.ProprietorInformationModel Proprietor { get; set; }
        /// <summary>
        /// Path For Cached Images
        /// </summary>
        public static string ImageCachedFolderPath { get; set; }

    }
}
