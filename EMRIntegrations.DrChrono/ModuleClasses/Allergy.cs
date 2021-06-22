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
    public class Allergy
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
                        dtModuleData = GetAllergyData(parameters);
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
        private DataTable GetAllergyData(Hashtable parameters)
        {
            try
            {
                string strModuleName = "Allergies";

                DataTable dtClinicaldata = new DataTable();

                var client = new RestClient("" + parameters["api_baseurl"].ToString() + "api/allergies?patient=" + parameters["patientid"].ToString() + "&verbose=true");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("authorization", "bearer:" + parameters["access_token"].ToString() + "");

                IRestResponse response = client.Execute(request);

                var data = (JObject)JsonConvert.DeserializeObject(response.Content);

                JToken jtobj = (JToken)JsonConvert.DeserializeObject(data.SelectToken("results").ToString());

                List<allergydetail> allergydata = jtobj.ToObject<List<allergydetail>>();

                dtClinicaldata = GetModuleDataBase.ConvertToDataTable(allergydata);

                dtClinicaldata.TableName = strModuleName;

                return dtClinicaldata;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private class allergydetail
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("doctor")]
            public string Doctor { get; set; }

            [JsonProperty("patient")]
            public string Patient { get; set; }

            [JsonProperty("rxnorm")]
            public string RxNorm { get; set; }

            [JsonProperty("reaction")]
            public string Reaction { get; set; }

            [JsonProperty("snomed_reaction")]
            public string Snomed_Reaction { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("notes")]
            public string Notes { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }
        }

        public string GenerateAPIJSONString(DataTable dtAllergy, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                string JSONString = string.Empty;
                DataTable dtFinalData;
                dtFinalData = dtAllergy.Copy();

                JSONString = DataTableToJSONAllergy(dtFinalData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string DataTableToJSONAllergy(DataTable table, string emrpatientid, string requestid, string emrid, string moduleid, string userid)
        {
            var JSONString = new StringBuilder();

            JSONString.Append("{");
            JSONString.Append("\"EMRPatientId\":" + "\"" + emrpatientid + "\",");
            JSONString.Append("\"EMRId\":" + "\"" + emrid + "\",");
            JSONString.Append("\"ModuleId\":" + "\"" + moduleid + "\",");
            JSONString.Append("\"RequestId\":" + "\"" + requestid + "\",");
            JSONString.Append("\"CreatedBy\":" + "\"\",");
            JSONString.Append("\"Allergies\":");

            if (table.Rows.Count == 0)
            {
                JSONString.Append("\"No Allergy Found\"");
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
                JSONString.Append("\"Substance\":" + "\"" + drow["Description"].ToString() + "\",");
                JSONString.Append("\"Reaction\":" + "\"" + drow["Reaction"].ToString() + "\",");
                JSONString.Append("\"Severity\":" + "\"\",");
                JSONString.Append("\"ReportedOn\":" + "\"\",");
                JSONString.Append("\"AllergyGenCode \":" + "\"\",");
                JSONString.Append("\"RxNormCode \":" + "\"" + drow["RxNorm"].ToString() + "\",");
                JSONString.Append("\"AllergyStatus\":" + "\""+ drow["Status"].ToString() + "\",");
                JSONString.Append("\"Note\":" + "\"" + drow["Notes"].ToString() + "\"");
                
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
