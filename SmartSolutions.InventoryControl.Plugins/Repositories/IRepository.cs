using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.Plugins.Repositories
{
    /// <summary>
    /// Generic Interface for Repository to Create Repository Class
    /// </summary>
    public interface IRepository : IPlugin
    {
        DBTypes Type { get; }

        ConnectionInfo ConnectionInfo { get; }
        void Setup(ConnectionInfo info);

        List<Dictionary<string, object>> Query(out Exception error, string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null);
        List<Dictionary<string, object>> Query(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null);
        Task<List<Dictionary<string, object>>> QueryAsync(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null);

        List<List<Dictionary<string, object>>> QueryDataSet(out Exception error, string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null);
        List<List<Dictionary<string, object>>> QueryDataSet(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null);
        Task<List<List<Dictionary<string, object>>>> QueryDataSetAsync(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null);

        object QueryScalar(out Exception error, string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null);
        object QueryScalar(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null);
        Task<object> QueryScalarAsync(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null);

        int NonQuery(out Exception error, string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null);
        int NonQuery(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null);
        Task<int> NonQueryAsync(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null);

        bool IsValidConnection(ConnectionInfo connectionInfo = null, string log_filename = null);
        bool IsValidConnection(out Exception error, ConnectionInfo connectionInfo = null, string log_filename = null);
        Task<bool> IsValidConnectionAsync(ConnectionInfo connectionInfo = null, string log_filename = null);

        //AsyncTableQuery<T> AsQueryable();
        string ExecuteScript(out Exception error, string script = null, string filename = null, Dictionary<string, object> parameters = null, ConnectionInfo connectionInfo = null, bool write_log = true, string log_filename = null);
        string ExecuteScript(string script = null, string filename = null, Dictionary<string, object> parameters = null, ConnectionInfo connectionInfo = null, bool write_log = true, string log_filename = null);

        void BulkCopy(System.Data.DataTable dataTable, string log_filename = null);

        IRepositoryTransaction CreateTransaction(ConnectionInfo connectionInfo = null, string log_filename = null);
    }

    public enum DBTypes
    {
        MYSQL,
        SQLITE,
    }
}
