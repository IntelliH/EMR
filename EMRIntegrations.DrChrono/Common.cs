using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Xml;

namespace EMRIntegrations.DrChrono
{
    class Common
    {
        public bool InsertRecords(DataTable dtRecords, string tableName, string connectionstring, string requestid)
        {
            try
            {
                SqlConnection oConn = new SqlConnection();
                oConn.ConnectionString = connectionstring; // GetConnectionString();

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

        public string GetConnectionString()
        {
            SqlConnectionStringBuilder objconnectionstring = new SqlConnectionStringBuilder();
            try
            {
                XmlDocument objDoc = new XmlDocument();
                objDoc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration.xml"));

                string server = string.Empty;
                string database = string.Empty;
                string username = string.Empty;
                string password = string.Empty;

                foreach (XmlNode xnode in objDoc.SelectNodes("configuration/connectionstring"))
                {
                    foreach (XmlNode xnode1 in xnode.ChildNodes)
                    {
                        if (xnode1.Name == "server")
                        {
                            server = xnode1.InnerText;
                        }
                        if (xnode1.Name == "database")
                        {
                            database = xnode1.InnerText;
                        }
                        if (xnode1.Name == "username")
                        {
                            username = xnode1.InnerText;
                        }
                        if (xnode1.Name == "password")
                        {
                            password = xnode1.InnerText;
                        }
                    }
                }

                objconnectionstring.DataSource = server;
                objconnectionstring.InitialCatalog = database;
                objconnectionstring.UserID = username;
                objconnectionstring.Password = password;
            }
            catch (Exception)
            {
                throw;
            }
            return objconnectionstring.ToString();
        }

        public string DateFormatMMDDYYYY(string date)
        {
            try
            {
                string truncatedDate = date.Substring(0, 10).Replace("-", string.Empty);
                DateTime d = DateTime.ParseExact(truncatedDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                //Console.WriteLine(d.ToString("MM/dd/yyyy"));

                string finalDate = d.ToString("MM/dd/yyyy").Replace("-", "/");
                return finalDate;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
