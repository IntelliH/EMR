using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace EMRIntegrations.DrChrono
{
    public class GetModuleDataBase
    {
        #region Variable Declaration
        //public ParseFiles objParseXML;
        //public Configuration objConfiguration;
        //public Common objCommon;
        #endregion

        #region CommonMethod
        /// <summary>
        /// Get Master Data from database
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        //public IEnumerable<DataRow> GetMasterData(DbConnection connection, Hashtable parameters)
        //{
        //    try
        //    {
        //        return objCommon.GetMasterData(connection, objParseXML.EMRQueries, objConfiguration.GetDeltaIdentifier(parameters, connection.GetType().Name), parameters);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// Get PatientData from database
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="connection"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        //public DataTable GetPatientData(DbConnection connection, Hashtable parameters, bool useDbAdpter)
        //{
        //    try
        //    {
        //        return objCommon.GetPatientData(connection, parameters, objParseXML.GetModuleQuery(parameters["modulename"].ToString().ToLower()), objConfiguration.GetDeltaIdentifier(parameters, connection.GetType().Name));
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// the folder path where files will be created.
        /// </summary>        
        /// <returns>Returns the folder path where file will be created.</returns>
        //public string GetCustomPropertiesValue(Hashtable parameters, string propertieName)
        //{
        //    try
        //    {
        //        return objCommon.GetCustomPropertiesValue(parameters, propertieName);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        /// <summary>
        /// Create a table for xClinicalvalidations
        /// </summary>        
        /// <param name="tableName">Table name to create a table(xClinicalvalidations) </param>
        /// <returns>Returns the table xClinicalvalidations</returns>
        public DataTable CreateValidationDataTable(string tableName)
        {
            try
            {
                DataTable dtValidation = new DataTable(tableName);
                dtValidation.Columns.Add("patientid");
                dtValidation.Columns.Add("chartno");
                dtValidation.Columns.Add("lastname");
                dtValidation.Columns.Add("firstname");
                dtValidation.Columns.Add("dob");
                dtValidation.Columns.Add("module");
                dtValidation.Columns.Add("category");
                dtValidation.Columns.Add("message");
                return dtValidation;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataTable ConvertToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties

            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                var values = new object[Props.Length];

                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            //put a breakpoint here and check datatable

            return dataTable;

        }

        #endregion CommonMethod
    }
}
