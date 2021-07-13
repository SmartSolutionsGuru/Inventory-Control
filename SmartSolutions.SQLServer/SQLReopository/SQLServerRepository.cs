using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.QueryUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSolutions.SQLServer.SQLReopository
{
    [Export(typeof(IRepository)),PartCreationPolicy(CreationPolicy.Shared)]
    public class SQLServerRepository : IRepository
    {
        #region Private Members
        public string Name => "SQLServer Repository";
        public DBTypes Type => DBTypes.SQLServer;
        private ConnectionInfo _connectionInfo;
        public ConnectionInfo ConnectionInfo => _connectionInfo;
        #endregion

        #region Public Methods
        public void Setup(ConnectionInfo info)
        {
            _connectionInfo = info;
        }
        private void CreateConnection(ConnectionInfo connectionInfo = null)
        {
            var info = connectionInfo ?? _connectionInfo ?? ConnectionInfo.Instance ?? new ConnectionInfo();
            if (string.IsNullOrEmpty(info.ConnectionString))
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(info.IPAddress))
                    param["Server"] = info.IPAddress;
                if (!string.IsNullOrEmpty(info.Port))
                    param["Port"] = info.Port;
                if (!string.IsNullOrEmpty(info.Database))
                    param["Database"] = info.Database;
                if (!string.IsNullOrEmpty(info.UserName))
                    param["Uid"] = info.UserName;
                if (!string.IsNullOrEmpty(info.Password))
                    param["Pwd"] = info.Password;

                if (param.Count <= 0) throw new ArgumentException("Error While Creating Connection String");
                info.ConnectionString = string.Format("{0}; ConvertZeroDateTime=True; SSL Mode=None;", string.Join("; ", param.Select(x => $"{x.Key}={x.Value}")));
            }
            _connectionInfo = _connectionInfo ?? info;
        }
        public List<Dictionary<string, object>> Query(out Exception error, string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            List<Dictionary<string, object>> retVal = null;
            error = null;
            DbConnection conn = null;
            try
            {
                // Initialize the connection
                CreateConnection(connectionInfo);

                try
                {
                    conn = transaction?.Connection ?? new SqlConnection((connectionInfo ?? _connectionInfo).ConnectionString);
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    if (!string.IsNullOrEmpty(filename))
                    {
                        query = QueryHelper.GetQuery(filename, DBTypes.SQLServer);
                        if (write_log || System.Diagnostics.Debugger.IsAttached)
                            LogMessage.Write(() => $"DB QUERY - filename: {filename}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                    }
                    else if (!string.IsNullOrEmpty(procedure))
                    {
                        query = procedure;
                        if (write_log || System.Diagnostics.Debugger.IsAttached)
                            LogMessage.Write(() => $"DB QUERY - procedure: {procedure}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                    }
                    else
                    {
                        if (write_log || System.Diagnostics.Debugger.IsAttached)
                            LogMessage.Write(() => $"DB QUERY - query: {query}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                    }

                    DbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = query;
                    if (timeout.HasValue)
                        cmd.CommandTimeout = timeout.Value;

                    if (!string.IsNullOrEmpty(procedure) && procedure?.Equals(query) == true)
                        cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        var keys = parameters.Keys.ToList();
                        foreach (var key in keys)
                        {
                            var value = parameters[key];

                            var parameter = cmd.CreateParameter();
                            parameter.ParameterName = key;
                            parameter.DbType = GetDbType(value);
                            if (value == null)
                                value = DBNull.Value;
                            else
                                parameter.Value = value;

                            cmd.Parameters.Add(parameter);
                        }
                    }

                    if (out_parameters != null)
                    {
                        var keys = out_parameters.Keys.ToList();
                        foreach (var key in keys)
                        {
                            var value = parameters[key];

                            var parameter = cmd.CreateParameter();
                            parameter.ParameterName = key;
                            parameter.DbType = GetDbType(value);
                            parameter.Value = value;
                            parameter.Direction = ParameterDirection.Output;

                            cmd.Parameters.Add(parameter);
                        }
                    }

                    //var reader = cmd.ExecuteReader();
                    //while (reader.Read())
                    //{
                    //    retVal = retVal ?? new List<Dictionary<string, object>>();
                    //    Dictionary<string, object> dict = new Dictionary<string, object>();
                    //    for (int i = 0; i < reader.FieldCount; i++)
                    //    {
                    //        dict[reader.GetName(i)] = reader[i];
                    //    }

                    //    if (out_parameters != null)
                    //    {
                    //        var keys = out_parameters.Keys.ToList();
                    //        foreach (var key in keys)
                    //        {
                    //            dict[key] = db.GetParameter(cmd, key)?.Value;
                    //        }
                    //    }
                    //    retVal.Add(dict);
                    //}

                    DbDataAdapter dataAdapter = new SqlDataAdapter();
                    dataAdapter.SelectCommand = cmd;
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    if (dataTable.Rows.Count > 0)
                    {
                        retVal = new List<Dictionary<string, object>>();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            Dictionary<string, object> dict = new Dictionary<string, object>();
                            foreach (DataColumn c in dataTable.Columns)
                            {
                                dict[c.ColumnName] = row[c];
                            }

                            retVal.Add(dict);
                        }
                    }

                    if (out_parameters != null)
                    {
                        var keys = out_parameters.Keys.ToList();
                        foreach (var key in keys)
                        {
                            out_parameters[key] = cmd.Parameters[key]?.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //LogMessage.Write(ex.ToString, LogMessage.Levels.Fatal);
                    error = ex;
                }
                finally
                {
                    if (conn != transaction?.Connection) conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                //LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
                error = ex;
            }

            return retVal;
        }
        public List<Dictionary<string, object>> Query(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            Exception ex;
            var result = Query(out ex, query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout);
            if (ex != null)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
            }
            return result;
        }
        public async Task<List<Dictionary<string, object>>> QueryAsync(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            List<Dictionary<string, object>> retVal = null;

            await Task.Run(() =>
            {
                retVal = Query(query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout);
            });
            return retVal;
        }
        public List<List<Dictionary<string, object>>> QueryDataSet(out Exception error, string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            List<List<Dictionary<string, object>>> retVal = null;
            error = null;
            DbConnection conn = null;
            try
            {
                // Initialize the connection
                CreateConnection(connectionInfo);

                //using (DbHelper db = new DbHelper(DbHelper.connectionInfo))
                try
                {
                    conn = transaction?.Connection ?? new SqlConnection((connectionInfo ?? _connectionInfo).ConnectionString);
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    if (!string.IsNullOrEmpty(filename))
                    {
                        query = QueryHelper.GetQuery(filename, DBTypes.SQLServer);
                        if (write_log || System.Diagnostics.Debugger.IsAttached)
                            LogMessage.Write(() => $"DB QUERY - filename: {filename}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                    }
                    else if (!string.IsNullOrEmpty(procedure))
                    {
                        query = procedure;
                        if (write_log || System.Diagnostics.Debugger.IsAttached)
                            LogMessage.Write(() => $"DB QUERY - procedure: {procedure}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                    }
                    else
                    {
                        if (write_log || System.Diagnostics.Debugger.IsAttached)
                            LogMessage.Write(() => $"DB QUERY - query: {query}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                    }

                    DbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = query;
                    if (timeout.HasValue)
                        cmd.CommandTimeout = timeout.Value;

                    if (!string.IsNullOrEmpty(procedure) && procedure?.Equals(query) == true)
                        cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        var keys = parameters.Keys.ToList();
                        foreach (var key in keys)
                        {
                            var value = parameters[key];

                            var parameter = cmd.CreateParameter();
                            parameter.ParameterName = key;
                            parameter.DbType = GetDbType(value);
                            parameter.Value = value;

                            cmd.Parameters.Add(parameter);
                        }
                    }

                    if (out_parameters != null)
                    {
                        var keys = out_parameters.Keys.ToList();
                        foreach (var key in keys)
                        {
                            var value = parameters[key];

                            var parameter = cmd.CreateParameter();
                            parameter.ParameterName = key;
                            parameter.DbType = GetDbType(value);
                            parameter.Value = value;
                            parameter.Direction = ParameterDirection.Output;

                            cmd.Parameters.Add(parameter);
                        }
                    }

                    //var reader = cmd.ExecuteReader();
                    //while (reader.Read())
                    //{
                    //    retVal = retVal ?? new List<List<Dictionary<string, object>>>();
                    //    Dictionary<string, object> dict = new Dictionary<string, object>();
                    //    for (int i = 0; i < reader.FieldCount; i++)
                    //    {
                    //        dict[reader.GetName(i)] = reader[i];
                    //    }

                    //    if (out_parameters != null)
                    //    {
                    //        var keys = out_parameters.Keys.ToList();
                    //        foreach (var key in keys)
                    //        {
                    //            dict[key] = db.GetParameter(cmd, key)?.Value;
                    //        }
                    //    }
                    //    retVal.Add(dict);
                    //}

                    DbDataAdapter dataAdapter = new SqlDataAdapter();
                    dataAdapter.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    dataAdapter.Fill(ds);
                    if (ds.Tables.Count > 0)
                    {
                        retVal = new List<List<Dictionary<string, object>>>();
                        foreach (DataTable table in ds.Tables)
                        {
                            var table_data = new List<Dictionary<string, object>>();
                            foreach (DataRow row in table.Rows)
                            {
                                Dictionary<string, object> dict = new Dictionary<string, object>();
                                foreach (DataColumn c in table.Columns)
                                {
                                    dict[c.ColumnName] = row[c];
                                }

                                table_data.Add(dict);
                            }
                            retVal.Add(table_data);
                        }
                    }

                    if (out_parameters != null)
                    {
                        var keys = out_parameters.Keys.ToList();
                        foreach (var key in keys)
                        {
                            out_parameters[key] = cmd.Parameters[key]?.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //LogMessage.Write(ex.ToString, LogMessage.Levels.Fatal);
                    error = ex;
                }
                finally
                {
                    if (conn != transaction?.Connection) conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                //LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
                error = ex;
            }

            return retVal;
        }
        public List<List<Dictionary<string, object>>> QueryDataSet(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            Exception ex;
            var result = QueryDataSet(out ex, query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout);
            if (ex != null)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
            }
            return result;
        }
        public async Task<List<List<Dictionary<string, object>>>> QueryDataSetAsync(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            var retVal = await Task.Run(() => QueryDataSet(query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout));
            return retVal;
        }
        public object QueryScalar(out Exception error, string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            object retVal = null;
            error = null;
            DbConnection conn = null;
            try
            {
                // Initialize the connection
                CreateConnection(connectionInfo);

                //using (DbHelper db = new DbHelper(DbHelper.connectionInfo))
                try
                {
                    conn = transaction?.Connection ?? new SqlConnection((connectionInfo ?? _connectionInfo).ConnectionString);
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    if (!string.IsNullOrEmpty(filename))
                    {
                        query = QueryHelper.GetQuery(filename, DBTypes.SQLServer);
                        if (write_log || System.Diagnostics.Debugger.IsAttached)
                            LogMessage.Write(() => $"DB QUERY - filename: {filename}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                    }
                    else if (!string.IsNullOrEmpty(procedure))
                    {
                        query = procedure;
                        if (write_log || System.Diagnostics.Debugger.IsAttached)
                            LogMessage.Write(() => $"DB QUERY - procedure: {procedure}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                    }
                    else
                    {
                        if (write_log || System.Diagnostics.Debugger.IsAttached)
                            LogMessage.Write(() => $"DB QUERY - query: {query}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                    }

                    DbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = query;
                    if (timeout.HasValue)
                        cmd.CommandTimeout = timeout.Value;

                    if (!string.IsNullOrEmpty(procedure) && procedure?.Equals(query) == true)
                        cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        var keys = parameters.Keys.ToList();
                        foreach (var key in keys)
                        {
                            var value = parameters[key];

                            var parameter = cmd.CreateParameter();
                            parameter.ParameterName = key;
                            parameter.DbType = GetDbType(value);
                            parameter.Value = value;

                            cmd.Parameters.Add(parameter);
                        }
                    }

                    if (out_parameters != null)
                    {
                        var keys = out_parameters.Keys.ToList();
                        foreach (var key in keys)
                        {
                            var value = parameters[key];

                            var parameter = cmd.CreateParameter();
                            parameter.ParameterName = key;
                            parameter.DbType = GetDbType(value);
                            parameter.Value = value;
                            parameter.Direction = ParameterDirection.Output;

                            cmd.Parameters.Add(parameter);
                        }
                    }

                    retVal = cmd.ExecuteScalar();

                    if (out_parameters != null)
                    {
                        var keys = out_parameters.Keys.ToList();
                        foreach (var key in keys)
                        {
                            out_parameters[key] = cmd.Parameters[key]?.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
                    error = ex;
                }
            }
            catch (Exception ex)
            {
                //LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
                error = ex;
            }
            finally
            {
                if (conn != transaction?.Connection) conn.Dispose();
            }
            return retVal;
        }
        public object QueryScalar(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            Exception ex;
            var result = QueryScalar(out ex, query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout);
            if (ex != null)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
            }
            return result;
        }
        public async Task<object> QueryScalarAsync(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            object retVal = null;
            await Task.Run(() =>
            {
                retVal = QueryScalar(query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout);
            });
            return retVal;
        }
        public int NonQuery(out Exception error, string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            int retVal = 0;
            error = null;
            DbConnection conn = null;
            try
            {
                // Initialize the connection
                CreateConnection(connectionInfo);

                //using (DbHelper db = new DbHelper(DbHelper.connectionInfo))
                try
                {
                    conn = transaction?.Connection ?? new SqlConnection((connectionInfo ?? _connectionInfo).ConnectionString);
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    if (!string.IsNullOrEmpty(filename))
                    {
                        query = QueryHelper.GetQuery(filename, DBTypes.SQLServer);
                        if (write_log || System.Diagnostics.Debugger.IsAttached)
                            LogMessage.Write(() => $"DB QUERY - filename: {filename}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                    }
                    else if (!string.IsNullOrEmpty(procedure))
                    {
                        query = procedure;
                        if (write_log || System.Diagnostics.Debugger.IsAttached)
                            LogMessage.Write(() => $"DB QUERY - procedure: {procedure}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                    }
                    else
                    {
                        if (write_log || System.Diagnostics.Debugger.IsAttached)
                            LogMessage.Write(() => $"DB QUERY - query: {query}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                    }

                    DbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = query;
                    if (timeout.HasValue)
                        cmd.CommandTimeout = timeout.Value;

                    if (!string.IsNullOrEmpty(procedure) && procedure?.Equals(query) == true)
                        cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        var keys = parameters.Keys.ToList();
                        foreach (var key in keys)
                        {
                            var value = parameters[key];

                            var parameter = cmd.CreateParameter();
                            parameter.ParameterName = key;
                            parameter.DbType = GetDbType(value);
                            parameter.Value = value;

                            cmd.Parameters.Add(parameter);
                        }
                    }

                    if (out_parameters != null)
                    {
                        var keys = out_parameters.Keys.ToList();
                        foreach (var key in keys)
                        {
                            var value = parameters[key];

                            var parameter = cmd.CreateParameter();
                            parameter.ParameterName = key;
                            parameter.DbType = GetDbType(value);
                            parameter.Value = value;
                            parameter.Direction = ParameterDirection.Output;

                            cmd.Parameters.Add(parameter);
                        }
                    }

                    retVal = cmd.ExecuteNonQuery();

                    if (out_parameters != null)
                    {
                        var keys = out_parameters.Keys.ToList();
                        foreach (var key in keys)
                        {
                            out_parameters[key] = cmd.Parameters[key]?.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex?.Message?.ToLower()?.Contains("duplicate") == true)
                        retVal = -999;
                    else
                        error = ex;
                    //LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
                }
                finally
                {
                    if (conn != transaction?.Connection) conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                //LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
                error = ex;
            }
            return retVal;
        }
        public int NonQuery(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            Exception ex;
            var result = NonQuery(out ex, query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout);
            if (ex != null)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
            }
            return result;
        }
        public async Task<int> NonQueryAsync(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            int retVal = 0;
            await Task.Run(() =>
            {
                retVal = NonQuery(query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout);
            });
            return retVal;
        }
        public bool IsValidConnection(out Exception error, ConnectionInfo connectionInfo = null, string log_filename = null)
        {
            bool retVal = false;
            error = null;
            try
            {
                CreateConnection(connectionInfo);
                using (SqlConnection conn = new SqlConnection((connectionInfo ?? _connectionInfo).ConnectionString))
                {
                    conn.Open();
                    retVal = conn.State == ConnectionState.Open;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
            return retVal;
        }
        public bool IsValidConnection(ConnectionInfo connectionInfo = null, string log_filename = null)
        {
            Exception ex;
            var result = IsValidConnection(out ex, connectionInfo);
            if (ex != null)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Warn);
            }
            return result;
        }
        public async Task<bool> IsValidConnectionAsync(ConnectionInfo connectionInfo = null, string log_filename = null)
        {
            bool retVal = false;
            await Task.Run(() =>
            {
                retVal = IsValidConnection(connectionInfo);
            });
            return retVal;
        }
        public void BulkCopy(DataTable dataTable, string log_filename = null)
        {
            try
            {

            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public IRepositoryTransaction CreateTransaction(ConnectionInfo connectionInfo = null, string log_filename = null)
        {
            IRepositoryTransaction retVal = null;
            try
            {
                CreateConnection(connectionInfo);
                SqlConnection connection = new SqlConnection((connectionInfo ?? _connectionInfo).ConnectionString);
                connection.Open();
                var transaction = connection.BeginTransaction();
                retVal = new SQLServerRepositoryTransaction(connection, transaction);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
            }
            return retVal;
        }
        #endregion

        #region ScriptExecution
        public string ExecuteScript(out Exception error, string script = null, string filename = null, Dictionary<string, object> parameters = null, ConnectionInfo connectionInfo = null, bool write_log = true, string log_filename = null)
        {
            string scriptResult = null;
            error = null;
            return scriptResult;
        }
        public string ExecuteScript(string script = null, string filename = null, Dictionary<string, object> parameters = null, ConnectionInfo connectionInfo = null, bool write_log = true, string log_filename = null)
        {
            string scriptResult = null;
            
            return scriptResult;
        }
        #endregion

        #region Helpers
        private DbType GetDbType(object value)
        {
            DbType dbType = DbType.String;
            try
            {
                switch (value?.GetType().ToString())
                {
                    case "System.Int16":
                        dbType = DbType.Int16;
                        break;
                    //case "integer":
                    case "int":
                    case "System.Int32":
                        dbType = DbType.Int32;
                        break;
                    case "System.Int64":
                        dbType = DbType.Int64;
                        break;
                    case "System.UInt16":
                        dbType = DbType.UInt16;
                        break;
                    case "System.UInt32":
                    case "uint":
                        dbType = DbType.UInt32;
                        break;
                    case "System.UInt64":
                        dbType = DbType.UInt64;
                        break;
                    case "System.Double":
                    case "double":
                        dbType = DbType.Double;
                        break;
                    case "System.Decimal":
                    case "float":
                        dbType = DbType.Decimal;
                        break;
                    case "System.DateTime":
                        dbType = DbType.DateTime;
                        break;
                    case "System.TimeSpan":
                        dbType = DbType.Time;
                        break;
                    case "bool":
                    case "boolean":
                    case "System.Boolean":
                        dbType = DbType.Boolean;
                        break;
                    case "System.Byte[]":
                        dbType = DbType.Binary;
                        break;
                    case "System.String":
                    case "string":
                    default:
                        dbType = DbType.String;
                        break;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString());
            }

            return dbType;
        }
        #endregion
    }
}
