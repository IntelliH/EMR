using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;

namespace EMRIntegrations.Athena
{
    public class SurgicalHistory
    {
        /// <summary>
        /// Get data from source
        /// </summary>
        /// <param name="connection">Source Database connection</param>
        /// <param name="parameters">number of parameters which are have version,modulename etc...</param>
        /// <returns>Return the dataset of 2 tables,one for module data and another is for errors or debug log or any validation message</returns>
        public DataTable GetData(Hashtable parameters)
        {
            try
            {
                DataTable dtModuleData = new DataTable();
                string strVersion = Convert.ToString(parameters["version"]);

                switch (strVersion)
                {
                    default:
                        dtModuleData = GetSurgicalHistoryData(parameters);
                        break;
                }
                return dtModuleData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get data from source for given module
        /// </summary>
        /// <param name="connection">Source Database connection</param>
        /// <param name="parameters">number of parameters which are have version,modulename etc...</param>
        /// <returns>Return the datasetof 2 tables,one for module data and another is for errors or debug log or any validation message</returns>
        private DataTable GetSurgicalHistoryData(Hashtable parameters)
        {
            try
            {
                string strModuleName = string.Empty;
                strModuleName = "SurgicalHistory";

                DataSet dsModuleData = new DataSet(strModuleName);
                DataTable dtClinicaldata = new DataTable();
                
                // Do data processing here.
                APIConnection api = (APIConnection)parameters["athenaapiobject"];
                List<Procedure> objclinical = new List<Procedure>();

                //ArrayList arydept = new ArrayList();
                //arydept = (ArrayList)parameters["api_departmentid"];

                //foreach (var value in arydept)
                //{
                    Dictionary<string, string> dirlst = new Dictionary<string, string>()
                    {
                        {"departmentid", parameters["api_departmentid"].ToString()}
                    };

                    JObject jobj = (JObject)api.GET("chart/" + parameters["patientid"].ToString() + "/surgicalhistory", dirlst);
                    JToken jtobj = jobj["procedures"];
                    if (jtobj != null)
                    {
                        if (jtobj.HasValues && jtobj.SelectToken("error") != null)
                        {
                            throw new Exception(jtobj["error"].ToString());
                        }
                        objclinical = jtobj.ToObject<List<Procedure>>();
                    }

                    dtClinicaldata = GetModuleDataBase.ConvertToDataTable(objclinical);

                if (!dtClinicaldata.Columns.Contains("patientid"))
                    dtClinicaldata.Columns.Add("patientid");

                string strsurgerydate = string.Empty;
                foreach (DataRow dr in dtClinicaldata.Rows)
                {
                    dr["patientid"] = parameters["patientid"].ToString();
                }

                if (dtClinicaldata.Rows.Count > 0)
                {
                    DataTable dtfilter = dtClinicaldata.AsEnumerable()
                                 .Where(Filter => Filter.Field<string>("description") != null && Filter.Field<string>("description").Length > 0)
                                 .AsDataView().ToTable();

                    dtClinicaldata = dtfilter.Copy();
                }
                //}
                dtClinicaldata = dtClinicaldata.DefaultView.ToTable(true);
                dtClinicaldata.TableName = strModuleName;

                return dtClinicaldata;
            }
            catch (Exception)
            {
                throw;
            }
        }
        internal class Procedure
        {

            [JsonProperty("source")]
            public string Source { get; set; }

            [JsonProperty("procedureid")]
            public string Procedureid { get; set; }

            [JsonProperty("proceduredate")]
            public string Proceduredate { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("procedurecode")]
            public string Procedurecode { get; set; }

            [JsonProperty("note")]
            public string Note { get; set; }

            [JsonProperty("encounterid")]
            public string Encounterid { get; set; }

            [JsonProperty("providerid")]
            public string Providerid { get; set; }


        }
    }
}
