﻿using System;
using System.Collections.Generic;

namespace SmartSolutions.InventoryControl.UI.Helpers.SettingHelper
{
    public class AppSettings
    {
        /// <summary>
        /// The directory where the app is running
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
        /// The Property which Decide either data backup Path is Set or Not
        /// </summary>
        public static bool Backup { get; set; }
        /// <summary>
        /// Logged in User 
        /// </summary>
        public static DAL.Models.Authentication.IdentityUserModel LoggedInUser { get; set; }
        /// <summary>
        /// Proprietor Information
        /// </summary>
        public static DAL.Models.ProprietorInformationModel Proprietor { get; set; }
    }
}
