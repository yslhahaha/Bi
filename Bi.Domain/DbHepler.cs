using Bi.Utility;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Domain
{
    public partial class OraDb104
    {
        public static OraDb104 Create()
        {
            return new OraDb104();
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {

                        throw new DomainException(string.Format("Class: {0}, Property: {1}, Error: {2}", validationErrors.Entry.Entity.GetType().FullName,
                            validationError.PropertyName,
                            validationError.ErrorMessage), "error");
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                throw new DomainException(ex.Message, ex);
            }
        }

        string connectionString = ConfigurationManager.ConnectionStrings[ConnKey.Conn_Cnjsb01_Key].ConnectionString;

        /// <summary>  
        /// 执行查询语句，返回DataSet  
        /// </summary>  
        /// <param name="SQLString">查询语句</param>  
        /// <returns>DataSet</returns>  
        public DataTable ExecuteQuery(string SQLString, params OracleParameter[] parameters)
        {
            var resultTable = new DataTable();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                var command = new OracleCommand(SQLString, connection) { CommandType = CommandType.Text };

                command.Parameters.AddRange(parameters);

                var dataAdapter = new OracleDataAdapter(command);

                try
                {
                    dataAdapter.Fill(resultTable);
                }
                catch (OracleException ex)
                {
                    if (ex.Number == 4068)
                    {
                        dataAdapter.Fill(resultTable);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed) connection.Close();
                }

                return resultTable;
            }
        }

        public DataSet ExecuteSql(string sql)
        {
            var resultDs = new DataSet();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var command = new OracleCommand(sql, connection) { CommandType = CommandType.Text };

                var dataAdapter = new OracleDataAdapter(command);

                try
                {
                    dataAdapter.Fill(resultDs);
                }
                catch (OracleException ex)
                {
                    if (ex.Number == 4068)
                    {
                        dataAdapter.Fill(resultDs);
                    }
                    else
                    {
                        throw;
                    }
                }

                return resultDs;
            }
        }

        /// <summary>
        /// Execute a database query which does not include a select
        /// </summary>
        /// <param name="connString">Connection string to database</param>
        /// <param name="cmdType">Command type either stored procedure or SQL</param>
        /// <param name="cmdText">Acutall SQL Command</param>
        /// <param name="commandParameters">Parameters to bind to the command</param>
        /// <returns></returns>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);

                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                return val;
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="sqls">多条SQL语句</param>  
        public bool ExecuteNonQueryTran(CommandType cmdType, List<string> sqls)
        {
            bool result = false;

            OracleConnection connection = new OracleConnection(connectionString);
            if (connection.State != ConnectionState.Open)
                connection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.CommandType = cmdType;

            OracleTransaction trans = connection.BeginTransaction();
            cmd.Connection = connection;
            cmd.Transaction = trans;

            try
            {
                for (int n = 0; n < sqls.Count; n++)
                {
                    string strsql = sqls[n].ToString();

                    if (strsql.Trim().Length > 1)
                    {
                        cmd.CommandText = strsql;
                        cmd.ExecuteNonQuery();
                    }
                }

                trans.Commit();

                result = true;
            }
            catch (OracleException ex)
            {
                result = false;
                trans.Rollback();

                throw ex;
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Internal function to prepare a command for execution by the database
        /// </summary>
        /// <param name="cmd">Existing command object</param>
        /// <param name="conn">Database connection object</param>
        /// <param name="trans">Optional transaction object</param>
        /// <param name="cmdType">Command type, e.g. stored procedure</param>
        /// <param name="cmdText">Command test</param>
        /// <param name="commandParameters">Parameters for the command</param>
        private void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, OracleParameter[] commandParameters)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            if (trans != null)
                cmd.Transaction = trans;

            if (commandParameters != null)
            {
                foreach (OracleParameter parm in commandParameters)
                    cmd.Parameters.Add(parm);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public string CreateLogSql(TB_SYS_LOG log)
        {
            return @"INSERT INTO TB_SYS_LOG
                                    (LOGID,
                                    USERID,
                                    FUNNAME,
                                    LOGTIME,
                                    OPERATION,
                                    BIZCODE,
                                    BIZID,
                                    MEMO1,
                                    MEMO2,
                                    URL,
                                    IP)
                                VALUES
                                    ('" + Guid.NewGuid().ToString() + @"',
                                    '" + log.USERID + @"',
                                    '" + log.FUNNAME + @"',
                                    SYSDATE,
                                    '" + log.OPERATION + @"',
                                    '" + (string.IsNullOrEmpty(log.BIZCODE) ? "" : log.BIZCODE) + @"',
                                    '" + log.BIZID + @"',
                                    '" + log.MEMO1 + @"',
                                    '" + (string.IsNullOrEmpty(log.MEMO2) ? "" : log.MEMO2.Replace("'", " ")) + @"',
                                    '" + log.URL + @"',
                                    '" + log.IP + "')";
        }

    }
}
