using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;

namespace EMRIntegrations.Epic
{
    class Common
    {
        public bool InsertRecords(DataTable dtRecords, string tableName, string connectionstring, string requestid)
        {
            try
            {
                SqlConnection oConn = new SqlConnection
                {
                    ConnectionString = connectionstring // GetConnectionString();
                };

                string query = "insert into " + tableName + "";
                query += "(";

                foreach (DataColumn dColumn in dtRecords.Columns)
                {
                    query += "[" + dColumn.ColumnName + "],";
                }

                query = query.Substring(0, query.Length - 1);
                query += ", RequestID) values (";

                foreach (DataRow dRow in dtRecords.Rows)
                {
                    string querydata = string.Empty;
                    foreach (DataColumn dColumn in dtRecords.Columns)
                    {
                        querydata += "'" + dRow[dColumn.ColumnName].ToString().Replace("'", "''") + "',";
                    }

                    //querydata = querydata.Substring(0, querydata.Length - 1);
                    querydata +=  "'" + requestid + "')";

                    if (oConn.State == ConnectionState.Closed)
                    {
                        oConn.Open();
                    }

                    SqlCommand oCmd = new SqlCommand();
                    oCmd.Connection = oConn;
                    oCmd.CommandText = query + querydata;

                    try
                    {
                        oCmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }
    }
}
