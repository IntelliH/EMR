using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace EMRIntegrations.Athena
{
    public class LabResults
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
                string strModuleName = Convert.ToString(parameters["modulename"]);
                string strVersion = Convert.ToString(parameters["version"]);

                switch (strVersion)
                {
                    default:
                        dtModuleData = GetLabResultsData(parameters);
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
        private DataTable GetLabResultsData(Hashtable parameters)
        {
            try
            {
                string strModuleName = string.Empty;
                strModuleName = "LabResults";


                //DataSet dsModuleData = new DataSet(strModuleName);
                DataTable dtClinicaldata = new DataTable();

                // Do data processing here.
                APIConnection api = (APIConnection)parameters["athenaapiobject"];
                List<Analyte> objclinical = new List<Analyte>();

                //ArrayList arydept = new ArrayList();
                //arydept = (ArrayList)parameters["api_departmentid"];

                //foreach (var value in arydept)
                //{
                Dictionary<string, string> dirlst = new Dictionary<string, string>()
                {
                    {"departmentid", parameters["api_departmentid"].ToString()}
                };


                JObject jobj = (JObject)api.GET("chart/" + parameters["patientid"].ToString() + "/analytes", dirlst);
                JToken jtobj = jobj["analytes"];
                if (jtobj != null)
                {
                    if (jtobj.HasValues && jtobj.SelectToken("error") != null)
                    {
                        throw new Exception(jtobj["error"].ToString());
                    }
                    objclinical = jtobj.ToObject<List<Analyte>>();
                }
                //}

                dtClinicaldata = GetModuleDataBase.ConvertToDataTable(objclinical);

                if (!dtClinicaldata.Columns.Contains("patientid"))
                {
                    dtClinicaldata.Columns.Add("patientid");
                }

                if (!dtClinicaldata.Columns.Contains("daterecorded"))
                {
                    dtClinicaldata.Columns.Add("daterecorded");
                }

                if (!dtClinicaldata.Columns.Contains("ordernumber"))
                {
                    dtClinicaldata.Columns.Add("ordernumber");
                }

                if (!dtClinicaldata.Columns.Contains("ordertestcode"))
                {
                    dtClinicaldata.Columns.Add("ordertestcode");
                }

                if (!dtClinicaldata.Columns.Contains("ordertestname"))
                {
                    dtClinicaldata.Columns.Add("ordertestname");
                }

                if (!dtClinicaldata.Columns.Contains("resultcode"))
                {
                    dtClinicaldata.Columns.Add("resultcode");
                }

                if (dtClinicaldata != null && dtClinicaldata.Rows.Count > 0)
                {
                    dtClinicaldata.Columns["analytename"].ColumnName = "resultname";
                    dtClinicaldata.Columns["analytedate"].ColumnName = "resultdatetime";
                    dtClinicaldata.Columns["Valuestatus"].ColumnName = "abnormalflag";
                    dtClinicaldata.Columns["Referencerange"].ColumnName = "range";
                    dtClinicaldata.Columns["description"].ColumnName = "reportcomments";
                    dtClinicaldata.Columns["Note"].ColumnName = "observationnotes";
                    dtClinicaldata.Columns["Loinc"].ColumnName = "loincresultcode";
                    dtClinicaldata.Columns["Units"].ColumnName = "units";
                    dtClinicaldata.Columns["Resultstatus"].ColumnName = "resultstatus";
                }

                string strdate = string.Empty;
                foreach (DataRow dr in dtClinicaldata.Rows)
                {

                    dr["patientid"] = parameters["patientid"].ToString();
                    dr["resultcode"] = dr["resultname"];
                    dr["ordertestname"] = dr["resultname"];
                    dr["ordertestcode"] = dr["resultcode"];



                    dr["resultdatetime"] = dr["resultdatetime"].ToString();

                    strdate = "";
                    if (!string.IsNullOrEmpty(dr["resultdatetime"].ToString()))
                    {
                        strdate = dr["resultdatetime"].ToString();
                        if (!string.IsNullOrEmpty(strdate))
                        {
                            dr["ordernumber"] = strdate;
                            dr["daterecorded"] = strdate;
                        }
                        else
                        {
                            dr["daterecorded"] = DateTime.Now.ToString();
                        }
                    }
                    else
                    {
                        dr["daterecorded"] = DateTime.Now.ToString();
                    }

                }
                if (dtClinicaldata.Rows.Count > 0)
                {
                    DataTable dtfilter = dtClinicaldata.AsEnumerable()
                                 .Where(Filter => Filter.Field<string>("value") != null && Filter.Field<string>("value").Length > 0
                                 && Filter.Field<string>("ResultName") != null && Filter.Field<string>("ResultName").Length > 0)
                                 .AsDataView().ToTable();

                    dtClinicaldata = dtfilter.Copy();
                }

                dtClinicaldata = dtClinicaldata.DefaultView.ToTable(true);
                dtClinicaldata.TableName = strModuleName;
                //dsModuleData.Tables.Add(dtValidation);
                //dsModuleData.Tables.Add(dtClinicaldata);

                return dtClinicaldata;
            }
            catch (Exception)
            {
                throw;
            }
        }
        internal class Analyte
        {

            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("analytename")]
            public string Analytename { get; set; }

            [JsonProperty("analyteid")]
            public string Analyteid { get; set; }

            [JsonProperty("analytedate")]
            public string Analytedate { get; set; }

            [JsonProperty("note")]
            public string Note { get; set; }

            [JsonProperty("encounterid")]
            public string Encounterid { get; set; }

            [JsonProperty("valuestatus")]
            public string Valuestatus { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("referencerange")]
            public string Referencerange { get; set; }

            [JsonProperty("loinc")]
            public string Loinc { get; set; }

            [JsonProperty("units")]
            public string Units { get; set; }

            [JsonProperty("resultstatus")]
            public string Resultstatus { get; set; }

        }


    }
}
