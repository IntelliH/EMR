using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EMRIntegrations.DrChrono
{
    public class Problemlist
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
                        dtModuleData = GetProblemlistData(parameters);
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
        private DataTable GetProblemlistData(Hashtable parameters)
        {
            try
            {
                string strModuleName = "Problemlist";

                DataTable dtClinicaldata = new DataTable();

                var client = new RestClient("" + parameters["api_baseurl"].ToString() + "api/problems?patient=" + parameters["patientid"].ToString() + "");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("authorization", "bearer:" + parameters["access_token"].ToString() + "");
                IRestResponse response = client.Execute(request);

                var data = (JObject)JsonConvert.DeserializeObject(response.Content);

                JToken jtobj = (JToken)JsonConvert.DeserializeObject(data.SelectToken("results").ToString());

                List<Problemlistdetail> Problemlistdata = jtobj.ToObject<List<Problemlistdetail>>();

                dtClinicaldata = GetModuleDataBase.ConvertToDataTable(Problemlistdata);

                dtClinicaldata.TableName = strModuleName;

                return dtClinicaldata;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private class Problemlistdetail
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("doctor")]
            public string Doctor { get; set; }

            [JsonProperty("patient")]
            public string Patient { get; set; }

            [JsonProperty("icd_version")]
            public string Icd_version { get; set; }

            [JsonProperty("icd_code")]
            public string Icd_code { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("notes")]
            public string Notes { get; set; }

            [JsonProperty("date_diagnosis")]
            public string Date_diagnosis { get; set; }

            [JsonProperty("date_onset")]
            public string Date_onset { get; set; }

            [JsonProperty("date_changed")]
            public string Date_changed { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("snomed_ct_code")]
            public string Snomed_ct_code { get; set; }

            [JsonProperty("info_url")]
            public string Info_url { get; set; }
        }

        public string GenerateAPIJSONString(DataTable dtProblemlist, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                string JSONString = string.Empty;
                DataTable dtFinalData;
                dtFinalData = dtProblemlist.Copy();

                JSONString = DataTableToJSONProblemlist(dtFinalData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string DataTableToJSONProblemlist(DataTable table, string emrpatientid, string requestid, string emrid, string moduleid, string userid)
        {
            var JSONString = new StringBuilder();

            JSONString.Append("{");
            JSONString.Append("\"EMRPatientId\":" + "\"" + emrpatientid + "\",");
            JSONString.Append("\"EMRId\":" + "\"" + emrid + "\",");
            JSONString.Append("\"ModuleId\":" + "\"" + moduleid + "\",");
            JSONString.Append("\"RequestId\":" + "\"" + requestid + "\",");
            JSONString.Append("\"CreatedBy\":" + "\"\",");
            JSONString.Append("\"Problemlist\":");

            if (table.Rows.Count == 0)
            {
                JSONString.Append("\"No Problemlist Found\"");
                JSONString.Append("}");
                return JSONString.ToString();
            }

            JSONString.Append("[");

            int counter = 0;
            foreach (DataRow drow in table.Rows)
            {
                counter++;

                JSONString.Append("{");

                JSONString.Append("\"Id\":" + "\"" + drow["Id"].ToString() + "\",");
                JSONString.Append("\"DoctorId\":" + "\"" + drow["Doctor"].ToString() + "\",");
                JSONString.Append("\"icd_version\":" + "\"" + drow["icd_version"].ToString() + "\",");
                JSONString.Append("\"icd_code\":" + "\"" + drow["icd_code"].ToString() + "\",");
                JSONString.Append("\"name\":" + "\"" + drow["name"].ToString() + "\",");
                JSONString.Append("\"status\":" + "\"" + drow["status"].ToString() + "\",");
                JSONString.Append("\"notes\":" + "\"" + drow["notes"].ToString() + "\",");
                JSONString.Append("\"date_diagnosis\":" + "\"" + drow["date_diagnosis"].ToString() + "\",");
                JSONString.Append("\"date_onset\":" + "\"" + drow["date_onset"].ToString() + "\",");
                JSONString.Append("\"date_changed\":" + "\"" + drow["date_changed"].ToString() + "\",");
                JSONString.Append("\"description\":" + "\"" + drow["description"].ToString() + "\",");
                JSONString.Append("\"snomed_ct_code\":" + "\"" + drow["snomed_ct_code"].ToString() + "\",");
                JSONString.Append("\"info_url\":" + "\"" + drow["info_url"].ToString() + "\"");

                if (counter == table.Rows.Count)
                {
                    JSONString.Append("}");
                }
                else
                {
                    JSONString.Append("},");
                }
            }
            JSONString.Append("]");
            JSONString.Append("}");
            return JSONString.ToString();
        }
    }
}
