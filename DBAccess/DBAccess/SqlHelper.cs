namespace DBAccess
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Configuration;

    public class SqlHelper
    {
        public static string strconn = ConfigurationManager.AppSettings["SQLConnStr"].ToString();

        public static int ExecuteProcess(string proName)
        {
            int num;
            SqlConnection connection = new SqlConnection(strconn);
            try
            {
                SqlCommand command2 = new SqlCommand(proName, connection) {
                    CommandType = CommandType.StoredProcedure
                };
                SqlCommand command = command2;
                if (((proName != "pro_AutoContinue") && (proName != "pro_projects")) && ((proName != "pro_AutoRepeal") && (proName != "pro_Z_Update_WebSite_IsState")))
                {
                    SqlParameter parameter = command.Parameters.Add("@num", SqlDbType.Int);
                    parameter.Direction = ParameterDirection.Output;
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    command.ExecuteNonQuery();
                    return (int) parameter.Value;
                }
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                command.ExecuteNonQuery();
                num = 1;
            }
            catch
            {
                num = 0;
            }
            finally
            {
                connection.Close();
            }
            return num;
        }

        public static int ExecuteProcess(string proName, SqlParameter[] pars)
        {
            int num;
            SqlConnection connection = new SqlConnection(strconn);
            try
            {
                SqlCommand command2 = new SqlCommand(proName, connection) {
                    CommandType = CommandType.StoredProcedure
                };
                SqlCommand command = command2;
                foreach (SqlParameter parameter in pars)
                {
                    command.Parameters.Add(parameter);
                }
                SqlParameter parameter2 = command.Parameters.Add("@num", SqlDbType.Int);
                parameter2.Direction = ParameterDirection.Output;
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                command.ExecuteNonQuery();
                num = (int) parameter2.Value;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message, exception);
            }
            finally
            {
                connection.Close();
            }
            return num;
        }

        public static DataSet GetAllInfo(string cmdText)
        {
            DataSet set;
            SqlConnection selectConnection = new SqlConnection(strconn);
            DataSet dataSet = null;
            try
            {
                if (selectConnection.State != ConnectionState.Open)
                {
                    selectConnection.Open();
                }
                SqlDataAdapter adapter = new SqlDataAdapter(cmdText, selectConnection);
                dataSet = new DataSet();
                adapter.Fill(dataSet);
                set = dataSet;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message, exception);
            }
            finally
            {
                selectConnection.Close();
            }
            return set;
        }

        public static DataSet GetAllInfo(SqlParameter[] pars, string cmdText)
        {
            DataSet set;
            SqlConnection connection = new SqlConnection(strconn);
            DataSet dataSet = null;
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                SqlCommand command2 = new SqlCommand(cmdText, connection) {
                    CommandType = CommandType.StoredProcedure
                };
                SqlCommand selectCommand = command2;
                foreach (SqlParameter parameter in pars)
                {
                    selectCommand.Parameters.Add(parameter);
                }
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                dataSet = new DataSet();
                adapter.Fill(dataSet);
                set = dataSet;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message, exception);
            }
            finally
            {
                connection.Close();
            }
            return set;
        }

        public static int getResults(string sql)
        {
            SqlConnection connection = new SqlConnection(strconn);
            int num = 0;
            try
            {
                SqlCommand command = new SqlCommand(sql, connection);
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                num = command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message, exception);
            }
            finally
            {
                connection.Close();
            }
            return num;
        }

        public static string GetValue(string sql)
        {
            SqlConnection connection = new SqlConnection(strconn);
            string str = string.Empty;
            try
            {
                SqlCommand command = new SqlCommand(sql, connection);
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                str = command.ExecuteScalar().ToString();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message, exception);
            }
            finally
            {
                connection.Close();
            }
            return str;
        }
    }
}

