using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EMRIntegrations.DrChrono
{
    public class Medications
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
                dtModuleData = GetMedicationsData(parameters);
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
        private DataTable GetMedicationsData(Hashtable parameters)
        {
            try
            {
                string strModuleName = "Medications";

                //DataSet dsModuleData = new DataSet(strModuleName);
                DataTable dtClinicaldata = new DataTable();

                //RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
                var client = new RestClient("" + parameters["api_baseurl"].ToString() + "api/medications?patient=" + parameters["patientid"].ToString() + "");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("authorization", "bearer:" + parameters["access_token"].ToString() + "");

                IRestResponse response;

                try
                {
                    response = client.Execute(request);
                }
                catch (Exception)
                {
                    throw new Exception("Error: Operation was unsuccessful because of a client error.");
                }

                var data = (JObject)JsonConvert.DeserializeObject(response.Content);

                JToken jtobj = (JToken)JsonConvert.DeserializeObject(data.SelectToken("results").ToString());

                List<medicationdetail> Medicationdata = jtobj.ToObject<List<medicationdetail>>();

                dtClinicaldata = GetModuleDataBase.ConvertToDataTable(Medicationdata);

                dtClinicaldata.TableName = strModuleName;

                return dtClinicaldata;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private class medicationdetail
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("doctor")]
            public string Doctor { get; set; }

            [JsonProperty("patient")]
            public string Patient { get; set; }

            [JsonProperty("appointment")]
            public string Appointment { get; set; }

            [JsonProperty("date_prescribed")]
            public string Date_prescribed { get; set; }

            [JsonProperty("date_started_taking")]
            public string Date_started_taking { get; set; }

            [JsonProperty("date_stopped_taking")]
            public string Date_stopped_taking { get; set; }

            [JsonProperty("notes")]
            public string Notes { get; set; }

            [JsonProperty("order_status")]
            public string Order_status { get; set; }

            [JsonProperty("number_refills")]
            public string Number_refills { get; set; }

            [JsonProperty("dispense_quantity")]
            public string Dispense_quantity { get; set; }

            [JsonProperty("dosage_quantity")]
            public string Dosage_quantity { get; set; }

            [JsonProperty("dosage_units")]
            public string Dosage_units { get; set; }

            [JsonProperty("rxnorm")]
            public string Rxnorm { get; set; }

            [JsonProperty("route")]
            public string Route { get; set; }

            [JsonProperty("frequency")]
            public string Frequency { get; set; }

            [JsonProperty("prn")]
            public string Prn { get; set; }

            [JsonProperty("indication")]
            public string Indication { get; set; }

            [JsonProperty("signature_note")]
            public string Signature_note { get; set; }

            [JsonProperty("pharmacy_note")]
            public string Pharmacy_note { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("daw")]
            public string Daw { get; set; }

            [JsonProperty("ndc")]
            public string Ndc { get; set; }
        }

        public string GenerateAPIJSONString(DataTable dtMedications, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                string JSONString = string.Empty;
                DataTable dtFinalData;
                dtFinalData = dtMedications.Copy();

                JSONString = DataTableToJSONMedications(dtFinalData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string DataTableToJSONMedications(DataTable table, string emrpatientid, string requestid, string emrid, string moduleid, string userid)
        {
            var JSONString = new StringBuilder();

            JSONString.Append("{");
            JSONString.Append("\"EMRPatientId\":" + "\"" + emrpatientid + "\",");
            JSONString.Append("\"EMRId\":" + "\"" + emrid + "\",");
            JSONString.Append("\"ModuleId\":" + "\"" + moduleid + "\",");
            JSONString.Append("\"RequestId\":" + "\"" + requestid + "\",");
            JSONString.Append("\"CreatedBy\":" + "\"\",");
            JSONString.Append("\"Medications\":");

            if (table.Rows.Count == 0)
            {
                JSONString.Append("\"No Medication Found\"");
                JSONString.Append("}");
                return JSONString.ToString();
            }

            JSONString.Append("[");

            int counter = 0;
            foreach (DataRow drow in table.Rows)
            {
                counter++;

                JSONString.Append("{");

                JSONString.Append("\"CreatedBy\":" + "\"\",");
                JSONString.Append("\"MedicationID\":" + "\"" + drow["Id"].ToString() + "\",");
                JSONString.Append("\"MedicationName\":" + "\"" + drow["Name"].ToString() + "\",");
                JSONString.Append("\"Direction\":" + "\"\",");
                JSONString.Append("\"DosageFrequencyValue\":" + "\"" + drow["Frequency"].ToString() + "\",");
                JSONString.Append("\"DosageRoute\":" + "\"" + drow["Route"].ToString() + "\",");
                JSONString.Append("\"DosageAdditionalInstructions\":" + "\"" + drow["Notes"].ToString() + "\",");
                JSONString.Append("\"DosageFrequencyUnit\":" + "\"\",");
                JSONString.Append("\"DosageUnit\":" + "\"" + drow["Dosage_units"].ToString() + "\",");
                JSONString.Append("\"DosageQuantity\":" + "\"" + drow["Dosage_quantity"].ToString() + "\",");
                JSONString.Append("\"DosageFrequencyDescription\":" + "\"\",");
                JSONString.Append("\"DosageDurationUnit\":" + "\"" + drow["Dispense_quantity"].ToString() + "\",");

                if (!string.IsNullOrEmpty(drow["Date_stopped_taking"].ToString()))
                {
                    string res = drow["Date_stopped_taking"].ToString().Replace("-", string.Empty);
                    DateTime d = DateTime.ParseExact(res, "yyyyMMdd", CultureInfo.InvariantCulture);
                    drow["Date_stopped_taking"] = d.ToString("MM/dd/yyyy").Replace("-", "/");
                    JSONString.Append("\"StopDate\":" + "\"" + drow["Date_stopped_taking"].ToString() + "\",");
                }
                else
                {
                    JSONString.Append("\"StopDate\":" + "\"\",");
                }

                if (!string.IsNullOrEmpty(drow["Date_started_taking"].ToString()))
                {
                    string res = drow["Date_started_taking"].ToString().Replace("-", string.Empty);
                    DateTime d = DateTime.ParseExact(res, "yyyyMMdd", CultureInfo.InvariantCulture);
                    drow["Date_started_taking"] = d.ToString("MM/dd/yyyy").Replace("-", "/");
                    JSONString.Append("\"StartDate\":" + "\"" + drow["Date_started_taking"].ToString() + "\",");
                }
                else
                {
                    JSONString.Append("\"StartDate\":" + "\"\",");
                }

                JSONString.Append("\"EnterDate\":" + "\"\"");

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
