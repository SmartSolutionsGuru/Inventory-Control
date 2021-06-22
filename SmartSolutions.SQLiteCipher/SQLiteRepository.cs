using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.QueryUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel.Composition;
using SmartSolutions.InventoryControl.Plugins.Repositories;

namespace SmartSolutions.InventoryControl.SQLiteCipher
{
    [Export(typeof(IRepository)), PartCreationPolicy(CreationPolicy.Shared)]
    public class SQLiteRepository : IRepository
    {
        private const string DB_DATE_FORMAT = "yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'ffffff";
        private const SQLite.SQLiteOpenFlags OpenFlags = SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.FullMutex;

        private Dictionary<string, SmartSQLiteConnection> DbConnections = new Dictionary<string, SmartSQLiteConnection>();

        public string Name => "SQLite Repository Cipher";
        public DBTypes Type => DBTypes.SQLITE;

        private ConnectionInfo _connectionInfo;
        public ConnectionInfo ConnectionInfo => _connectionInfo;

        internal IRepositoryTransaction ActiveTransaction { get; private set; } = null;

        private static ManualResetEvent transactionEndedEvent = new ManualResetEvent(false);


        public void Setup(ConnectionInfo info)
        {
            _connectionInfo = info;

        }

        private void CreateConnection(ConnectionInfo connectionInfo)
        {

            var info = connectionInfo ?? _connectionInfo ?? ConnectionInfo.Instance ?? new ConnectionInfo();
            if (string.IsNullOrEmpty(info.Password))
                info.Password = null;
            if (string.IsNullOrEmpty(info.ConnectionString))
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(info.Path))
                    param["Data Source"] = info.Path;


                if (param.Count <= 0) throw new ArgumentException("Error While Creating Connection String");
                info.ConnectionString = string.Format("{0};", string.Join("; ", param.Select(x => $"{x.Key}={x.Value}")));
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

                string uuid = Guid.NewGuid().ToString();


                try
                {
                    var db_path = (connectionInfo ?? _connectionInfo).Path;
                    if (transaction?.Connection != null)
                        conn = transaction?.Connection;
                    else if (DbConnections.ContainsKey(db_path))
                        conn = DbConnections[db_path];
                    else
                        conn = DbConnections[db_path] = new SmartSQLiteConnection(new SQLite.SQLiteConnection(new SQLite.SQLiteConnectionString((connectionInfo ?? _connectionInfo).Path, key: (connectionInfo ?? _connectionInfo).Password, storeDateTimeAsTicks: false, storeTimeSpanAsTicks: false, openFlags: OpenFlags, dateTimeStringFormat: DB_DATE_FORMAT)));

                    if (transaction == null && conn == ActiveTransaction?.Connection)
                        transactionEndedEvent.WaitOne();
                    //lock (conn)
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();
#if DEBUG
                        write_log = System.Diagnostics.Debugger.IsAttached || write_log;
#endif
                        if (!string.IsNullOrEmpty(filename))
                        {
                            query = QueryHelper.GetQuery(filename, DBTypes.SQLITE);
                            if (write_log)
                                LogMessage.Write($"DB QUERY - ({uuid}) filename: {filename}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                        }
                        else if (!string.IsNullOrEmpty(procedure))
                        {
                            query = procedure;
                            if (write_log)
                                LogMessage.Write($"DB QUERY - ({uuid}) procedure: {procedure}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                        }
                        else
                        {
                            if (write_log)
                                LogMessage.Write($"DB QUERY - ({uuid}) query: {query}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
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

                                if ((key == "@p_tablename" || key == "@p_database_name") && value is string)
                                    cmd.CommandText = cmd.CommandText?.Replace(key, value as string);

                                var parameter = cmd.CreateParameter();
                                parameter.ParameterName = key;
                                parameter.DbType = GetDbType(value);
                                //parameter.SqliteType = GetSqliteType(parameter.DbType);
                                if (value == null)
                                    parameter.Value = DBNull.Value;
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

                                if ((key == "@p_tablename" || key == "@p_database_name") && value is string)
                                    cmd.CommandText = cmd.CommandText?.Replace(key, value as string);

                                var parameter = cmd.CreateParameter();
                                parameter.ParameterName = key;
                                parameter.DbType = GetDbType(value);
                                if (value == null)
                                    parameter.Value = DBNull.Value;
                                else
                                    parameter.Value = value;
                                parameter.Direction = ParameterDirection.Output;

                                cmd.Parameters.Add(parameter);
                            }
                        }

                        var reader = cmd.ExecuteReader();

                        if (write_log)
                            LogMessage.Write($"DB QUERY Completed - ({uuid})", LogMessage.Levels.Debug);
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                retVal = retVal ?? new List<Dictionary<string, object>>();
                                Dictionary<string, object> dict = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    dict[reader.GetName(i)] = reader[i] != DBNull.Value ? reader[i] : null;
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
                }
                catch (Exception ex)
                {
                    error = ex;
                }
                finally
                {

                }

                if (write_log)
                    LogMessage.Write($"DB QUERY Returning - ({uuid})", LogMessage.Levels.Debug);
            }
            catch (Exception ex)
            {
                error = ex;
            }

            return retVal;
        }

        public List<Dictionary<string, object>> Query(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            Exception ex;
            var result = Query(out ex, query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout, log_filename: log_filename);
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
                retVal = Query(query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout, log_filename: log_filename);
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

                string uuid = Guid.NewGuid().ToString();

                //lock (obj_lock)
                {
                    //using (DbHelper db = new DbHelper(DbHelper.connectionInfo))
                    try
                    {
                        var db_path = (connectionInfo ?? _connectionInfo).Path;
                        if (transaction?.Connection != null)
                            conn = transaction?.Connection;
                        else if (DbConnections.ContainsKey(db_path))
                            conn = DbConnections[db_path];
                        else
                            conn = DbConnections[db_path] = new SmartSQLiteConnection(new SQLite.SQLiteConnection(new SQLite.SQLiteConnectionString((connectionInfo ?? _connectionInfo).Path, key: (connectionInfo ?? _connectionInfo).Password, storeDateTimeAsTicks: false, storeTimeSpanAsTicks: false, openFlags: OpenFlags, dateTimeStringFormat: DB_DATE_FORMAT)));

                        if (transaction == null && conn == ActiveTransaction?.Connection)
                            transactionEndedEvent.WaitOne();
                        //lock (conn)
                        {
                            if (conn.State == ConnectionState.Closed)
                                conn.Open();

#if DEBUG
                            write_log = System.Diagnostics.Debugger.IsAttached || write_log;
#endif
                            if (!string.IsNullOrEmpty(filename))
                            {
                                query = QueryHelper.GetQuery(filename, DBTypes.SQLITE);
                                if (write_log)
                                    LogMessage.Write($"DB QUERYDATASET - ({uuid}) filename: {filename}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                            }
                            else if (!string.IsNullOrEmpty(procedure))
                            {
                                query = procedure;
                                if (write_log)
                                    LogMessage.Write($"DB QUERYDATASET - ({uuid}) procedure: {procedure}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                            }
                            else
                            {
                                if (write_log)
                                    LogMessage.Write($"DB QUERYDATASET - ({uuid}) query: {query}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
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

                                    if ((key == "@p_tablename" || key == "@p_database_name") && value is string)
                                        cmd.CommandText = cmd.CommandText?.Replace(key, value as string);

                                    var parameter = cmd.CreateParameter();
                                    parameter.ParameterName = key;
                                    parameter.DbType = GetDbType(value);

                                    if (value == null)
                                        parameter.Value = DBNull.Value;
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

                            var reader = cmd.ExecuteReader();
                            if (write_log)
                                LogMessage.Write($"DB QUERYDATASET Completed - ({uuid})", LogMessage.Levels.Debug);
                            do
                            {
                                retVal = retVal ?? new List<List<Dictionary<string, object>>>();
                                List<Dictionary<string, object>> table_data = null;
                                while (reader.Read())
                                {
                                    table_data = table_data ?? new List<Dictionary<string, object>>();
                                    Dictionary<string, object> dict = new Dictionary<string, object>();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        dict[reader.GetName(i)] = reader[i] != DBNull.Value ? reader[i] : null;
                                    }

                                    table_data.Add(dict);
                                }
                                retVal.Add(table_data);
                            } while (reader.NextResult());

                            if (out_parameters != null)
                            {
                                var keys = out_parameters.Keys.ToList();
                                foreach (var key in keys)
                                {
                                    out_parameters[key] = cmd.Parameters[key]?.Value;
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
                    }
                    catch (Exception ex)
                    {
                        //LogMessage.Write(ex.ToString, LogMessage.Levels.Fatal, filename: log_filename);
                        error = ex;
                    }
                    finally
                    {

                    }
                }
                if (write_log)
                    LogMessage.Write($"DB QUERYDATASET Returning - ({uuid})", LogMessage.Levels.Debug);
            }
            catch (Exception ex)
            {
                //LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal, filename: log_filename);
                error = ex;
            }

            return retVal;
        }

        public List<List<Dictionary<string, object>>> QueryDataSet(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            Exception ex;
            var result = QueryDataSet(out ex, query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout, log_filename: log_filename);
            if (ex != null)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
            }
            return result;
        }

        public async Task<List<List<Dictionary<string, object>>>> QueryDataSetAsync(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            var retVal = await Task.Run(() => QueryDataSet(query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout, log_filename: log_filename));
            return retVal;
        }

        public object QueryScalar(out Exception error, string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            object retVal = null;
            error = null;
            DbConnection conn = null;
            try
            {
                //if (transaction == null)
                //    _pool.WaitOne();

                // Initialize the connection
                CreateConnection(connectionInfo);

                string uuid = Guid.NewGuid().ToString();

                //lock (obj_lock)
                {
                    //using (DbHelper db = new DbHelper(DbHelper.connectionInfo))
                    //using (SqliteConnection conn = new SqliteConnection(ConnectionInfo.Instance.ConnectionString))
                    {
                        try
                        {
                            var db_path = (connectionInfo ?? _connectionInfo).Path;
                            if (transaction?.Connection != null)
                                conn = transaction?.Connection;
                            else if (DbConnections.ContainsKey(db_path))
                                conn = DbConnections[db_path];
                            else
                                conn = DbConnections[db_path] = new SmartSQLiteConnection(new SQLite.SQLiteConnection(new SQLite.SQLiteConnectionString((connectionInfo ?? _connectionInfo).Path, key: (connectionInfo ?? _connectionInfo).Password, storeDateTimeAsTicks: false, storeTimeSpanAsTicks: false, openFlags: OpenFlags, dateTimeStringFormat: DB_DATE_FORMAT)));

                            if (transaction == null && conn == ActiveTransaction?.Connection)
                                transactionEndedEvent.WaitOne();
                            //lock (conn)
                            {
                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();
#if DEBUG
                                write_log = System.Diagnostics.Debugger.IsAttached || write_log;
#endif

                                if (!string.IsNullOrEmpty(filename))
                                {
                                    query = QueryHelper.GetQuery(filename, DBTypes.SQLITE);
                                    if (write_log)
                                        LogMessage.Write($"DB QUERYSCALAR - ({uuid}) filename: {filename}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                                }
                                else if (!string.IsNullOrEmpty(procedure))
                                {
                                    query = procedure;
                                    if (write_log)
                                        LogMessage.Write($"DB QUERYSCALAR - ({uuid}) procedure: {procedure}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                                }
                                else
                                {
                                    if (write_log)
                                        LogMessage.Write($"DB QUERYSCALAR - ({uuid}) query: {query}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
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

                                        if ((key == "@p_tablename" || key == "@p_database_name") && value is string)
                                            cmd.CommandText = cmd.CommandText?.Replace(key, value as string);

                                        var parameter = cmd.CreateParameter();
                                        parameter.ParameterName = key;
                                        parameter.DbType = GetDbType(value);
                                        if (value == null)
                                            parameter.Value = DBNull.Value;
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

                                retVal = cmd.ExecuteScalar();
                                if (write_log)
                                    LogMessage.Write($"DB QUERYSCALAR Completed - ({uuid})", LogMessage.Levels.Debug);
                                if (out_parameters != null)
                                {
                                    var keys = out_parameters.Keys.ToList();
                                    foreach (var key in keys)
                                    {
                                        out_parameters[key] = cmd.Parameters[key]?.Value;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            error = ex;
                        }
                        finally
                        {

                        }
                    }
                }
                if (write_log)
                    LogMessage.Write($"DB QUERYSCALAR Returning - ({uuid})", LogMessage.Levels.Debug);
            }
            catch (Exception ex)
            {
                //LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal, filename: log_filename);
                error = ex;
            }
            return retVal;
        }

        public object QueryScalar(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            Exception ex;
            var result = QueryScalar(out ex, query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout, log_filename: log_filename);
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
                retVal = QueryScalar(query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout, log_filename: log_filename);
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
                //if (transaction == null)
                //    _pool.WaitOne();

                // Initialize the connection
                CreateConnection(connectionInfo);

                string uuid = Guid.NewGuid().ToString();
                //lock (obj_lock)
                {
                    //using (DbHelper db = new DbHelper(DbHelper.connectionInfo))
                    try
                    {
                        var db_path = (connectionInfo ?? _connectionInfo).Path;
                        if (transaction?.Connection != null)
                            conn = transaction?.Connection;
                        else if (DbConnections.ContainsKey(db_path))
                            conn = DbConnections[db_path];
                        else
                            conn = DbConnections[db_path] = new SmartSQLiteConnection(new SQLite.SQLiteConnection(new SQLite.SQLiteConnectionString((connectionInfo ?? _connectionInfo).Path, key: (connectionInfo ?? _connectionInfo).Password, storeDateTimeAsTicks: false, storeTimeSpanAsTicks: false, openFlags: OpenFlags, dateTimeStringFormat: DB_DATE_FORMAT)));

                        if (transaction == null && conn == ActiveTransaction?.Connection)
                            transactionEndedEvent.WaitOne();
                        //lock (conn)
                        {
                            if (conn.State == ConnectionState.Closed)
                                conn.Open();

#if DEBUG
                            write_log = System.Diagnostics.Debugger.IsAttached || write_log;
#endif

                            if (!string.IsNullOrEmpty(filename))
                            {
                                query = QueryHelper.GetQuery(filename, DBTypes.SQLITE);
                                if (write_log)
                                    LogMessage.Write($"DB NONQUERY - ({uuid}) filename: {filename}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                            }
                            else if (!string.IsNullOrEmpty(procedure))
                            {
                                query = procedure;
                                if (write_log)
                                    LogMessage.Write($"DB NONQUERY - ({uuid}) procedure: {procedure}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
                            }
                            else
                            {
                                if (write_log)
                                    LogMessage.Write($"DB NONQUERY - ({uuid}) query: {query}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
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

                                    if ((key == "@p_tablename" || key == "@p_database_name") && value is string)
                                        cmd.CommandText = cmd.CommandText?.Replace(key, value as string);

                                    var parameter = cmd.CreateParameter();
                                    parameter.ParameterName = key;
                                    parameter.DbType = GetDbType(value);
                                    if (value == null)
                                        parameter.Value = DBNull.Value;
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
                                    if (value == null)
                                        parameter.Value = DBNull.Value;
                                    else
                                        parameter.Value = value;
                                    parameter.Direction = ParameterDirection.Output;

                                    cmd.Parameters.Add(parameter);
                                }
                            }
                            retVal = cmd.ExecuteNonQuery();

                            if (write_log)
                                LogMessage.Write($"DB NONQUERY Completed - ({uuid})", LogMessage.Levels.Debug);
                            if (out_parameters != null)
                            {
                                var keys = out_parameters.Keys.ToList();
                                foreach (var key in keys)
                                {
                                    out_parameters[key] = cmd.Parameters[key]?.Value;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex?.Message?.Contains("UNIQUE constraint failed") == true)
                            retVal = -999;
                        else
                            error = ex;
                        //LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal, filename: log_filename);
                    }
                    finally
                    {
                        //if (conn != transaction?.Connection) conn.Dispose();
                        ////if (transaction == null)
                        ////    _pool.Release();
                    }
                }
                if (write_log)
                    LogMessage.Write($"DB NONQUERY Returning - ({uuid})", LogMessage.Levels.Debug);
            }
            catch (Exception ex)
            {
                //LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal, filename: log_filename);
                error = ex;
            }
            return retVal;
        }

        public int NonQuery(string query = null, string procedure = null, string filename = null, Dictionary<string, object> parameters = null, Dictionary<string, object> out_parameters = null, IRepositoryTransaction transaction = null, ConnectionInfo connectionInfo = null, bool write_log = true, int? timeout = null, string log_filename = null)
        {
            Exception ex;
            var result = NonQuery(out ex, query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout, log_filename: log_filename);
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
                retVal = NonQuery(query, procedure, filename, parameters, out_parameters, transaction, connectionInfo, write_log, timeout, log_filename: log_filename);
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
                //lock (obj_lock)
                {
                    //using (var conn = new SoloSQLiteConnection(new SQLite.SQLiteConnection(new SQLite.SQLiteConnectionString((connectionInfo ?? _connectionInfo).Path, key: (connectionInfo ?? _connectionInfo).Password, storeDateTimeAsTicks: false, storeTimeSpanAsTicks: false, openFlags: SQLite.SQLiteOpenFlags.ReadOnly, dateTimeStringFormat: DB_DATE_FORMAT))))
                    {
                        //if (conn.State == ConnectionState.Closed)
                        //    conn.Open();
                        //retVal = conn.State == ConnectionState.Open;
                        //conn.Close();
                        retVal = QueryScalar("SELECT 1", connectionInfo: connectionInfo, log_filename: log_filename)?.ToString()?.Equals("1") == true;
                    }
                }
            }
            catch (Exception ex)
            {
                //LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal, filename: log_filename);
                error = ex;
            }
            return retVal;
        }

        public bool IsValidConnection(ConnectionInfo connectionInfo = null, string log_filename = null)
        {
            Exception ex;
            var result = IsValidConnection(out ex, connectionInfo, log_filename: log_filename);
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
                retVal = IsValidConnection(connectionInfo, log_filename: log_filename);
            });
            return retVal;
        }

        public string ExecuteScript(out Exception error, string script = null, string filename = null, Dictionary<string, object> parameters = null, ConnectionInfo connectionInfo = null, bool write_log = true, string log_filename = null)
        {
            string uuid = Guid.NewGuid().ToString();

            if (!string.IsNullOrEmpty(filename))
            {
                script = QueryHelper.GetQuery(filename, DBTypes.SQLITE);
                if (write_log)
                    LogMessage.Write($"DB EXECUTESCRIPT - ({uuid}) filename: {filename}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
            }
            else
            {
                if (write_log)
                    LogMessage.Write($"DB EXECUTESCRIPT - ({uuid}) script: {script}, parameters: {string.Join(" ", (parameters?.Select(x => $"SET {x.Key}={(x.Value == null ? "NULL" : $"'{x.Value}'")};").ToArray() ?? new string[] { "N/A" }))}", LogMessage.Levels.Debug);
            }

            string script_result = null;
            if (!string.IsNullOrWhiteSpace(script))
            {
                script_result = QueryScalar(error: out error, query: script, parameters: parameters, connectionInfo: connectionInfo, write_log: false)?.ToString();
                if (error != null)
                    script_result = "script_Error: " + error?.Message;
                else
                {
                    script_result = "script_ScriptCompleted!";
                }
            }
            else
            {
                script_result = "script_Error: NotValidScript";
                error = null;
            }

            if (write_log)
                LogMessage.Write($"DB EXECUTESCRIPT Completed - ({uuid})", LogMessage.Levels.Debug);

            return script_result;
            //return (QueryScalar(error: out error, query: script, filename: filename, parameters: parameters, connectionInfo: connectionInfo, write_log: write_log) as string);
        }

        public string ExecuteScript(string script = null, string filename = null, Dictionary<string, object> parameters = null, ConnectionInfo connectionInfo = null, bool write_log = true, string log_filename = null)
        {
            string scriptResult = null;
            Exception ex;
            scriptResult = ExecuteScript(out ex, script, filename, parameters, connectionInfo, write_log, log_filename: log_filename);
            if (ex != null)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
            }
            return scriptResult;
        }

        public void BulkCopy(DataTable dataTable, string log_filename = null)
        {
            throw new NotImplementedException();
        }

        public IRepositoryTransaction CreateTransaction(ConnectionInfo connectionInfo = null, string log_filename = null)
        {
            IRepositoryTransaction retVal = null;
            try
            {
                //_pool?.WaitOne();
                CreateConnection(connectionInfo);
                SmartSQLiteConnection conn = null;
                var db_path = (connectionInfo ?? _connectionInfo).Path;
                if (DbConnections.ContainsKey(db_path))
                    conn = DbConnections[db_path];
                else
                    conn = DbConnections[db_path] = new SmartSQLiteConnection(new SQLite.SQLiteConnection(new SQLite.SQLiteConnectionString((connectionInfo ?? _connectionInfo).Path, key: (connectionInfo ?? _connectionInfo).Password, storeDateTimeAsTicks: false, storeTimeSpanAsTicks: false, openFlags: OpenFlags, dateTimeStringFormat: DB_DATE_FORMAT)));

                if (conn == ActiveTransaction?.Connection)
                    transactionEndedEvent.WaitOne();

                conn.Open();
                var transaction = conn.BeginTransaction();
                retVal = new SQLiteRepositoryTransaction(this, conn, transaction/*, _pool*/);
                transactionEndedEvent.Reset();
                ActiveTransaction = retVal;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
            }
            return retVal;
        }

        internal void EndTransaction()
        {
            transactionEndedEvent.Set();
            ActiveTransaction = null;
        }

        private DbType GetDbType(object value)
        {

            DbType dbType = DbType.String;
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

            return dbType;
        }

        private SQLite.SQLite3.ColType GetSqliteType(DbType value)
        {
            SQLite.SQLite3.ColType sqliteType = SQLite.SQLite3.ColType.Text;
            switch (value)
            {
                case DbType.Binary:
                case DbType.Object:
                    sqliteType = SQLite.SQLite3.ColType.Blob;
                    break;
                case DbType.Decimal:
                case DbType.Double:
                    sqliteType = SQLite.SQLite3.ColType.Float;
                    break;
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                case DbType.Boolean:
                case DbType.Byte:
                case DbType.SByte:
                case DbType.Single:
                    sqliteType = SQLite.SQLite3.ColType.Integer;
                    break;
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.Time:
                case DbType.VarNumeric:
                case DbType.Xml:
                case DbType.Guid:
                default:
                    sqliteType = SQLite.SQLite3.ColType.Text;
                    break;
            }

            return sqliteType;
        }

        ~SQLiteRepository()
        {
            foreach (var item in DbConnections)
            {
                try
                {
                    item.Value?.Close();
                }
                catch (Exception ex)
                {
                    LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
                }
            }
        }
    }
}
