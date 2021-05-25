namespace SmartSolutions.InventoryControl.Plugins.Repositories
{
    /// <summary>
    /// Genaric Connection Class for Creating Db Connection
    /// </summary>
    public class ConnectionInfo
    {
        #region Private Members
        private static ConnectionInfo m_Instance;
        #endregion

        #region public Members
        public static ConnectionInfo Instance => m_Instance = m_Instance ?? new ConnectionInfo();
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ConnectionInfo() { }
        #endregion

        #region Public Properties
        /// <summary>
        /// Conection string or IP Address for DB
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Name of the DB
        /// </summary>
        public string Database { get; set; }
        public string InitialCatalog { get; set; }

        /// <summary>
        /// Ip address for Remote connection to DB
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Port Number for Remote connection to DB
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// UserName for DB
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///  Password for DB
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Path of DB
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Name of Database
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Machine Id for 
        /// </summary>
        public int MachineId { get; set; }
        /// <summary>
        /// Enum For DbTypes
        /// </summary>
        public DBTypes Type { get; set; }
        #endregion
    }
}
