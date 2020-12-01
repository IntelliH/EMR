using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using RestSharp;
using System.Net.Http;
using System.IO;
using System.Web;
using System.Net;
using System.Net.Http.Headers;

namespace EMRIntegrations.DrChrono
{
    class DocumentUpload
    {
        public DataTable PostData(Hashtable parameters)
        {
            try
            {
                DataTable responseData = new DataTable();
                responseData = PostDocumentUploadData(parameters);
                return responseData;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private DataTable PostDocumentUploadData(Hashtable parameters)
        {
            try
            {
                string strModuleName = "Document";

                var client = new RestClient("https://drchrono.com/api/documents");
                client.Timeout = -1;

                DataTable dtClinicaldata = new DataTable();

                var request = new RestRequest(Method.POST);
                request.AddHeader("authorization", "bearer:" + parameters["access_token"].ToString() + "");
                request.AddFile("document", parameters["filepath"].ToString());
                request.AddParameter("patient", parameters["patientid"].ToString());
                request.AddParameter("doctor", parameters["doctorid"].ToString()); //269552
                request.AddParameter("date", parameters["documentdate"].ToString());
                request.AddParameter("description", parameters["documentdescription"].ToString());

                IRestResponse response = client.Execute(request);

                var data = (JObject)JsonConvert.DeserializeObject(response.Content);

                //JToken jtobj = (JToken)JsonConvert.DeserializeObject(data.ToString().Replace("[", string.Empty).Replace("]", string.Empty).Replace("\r\n", string.Empty));

                dtClinicaldata.Columns.Add("id");
                dtClinicaldata.Columns.Add("doctor");
                dtClinicaldata.Columns.Add("document");
                dtClinicaldata.Columns.Add("date");
                dtClinicaldata.Columns.Add("description");

                DataRow dRowClinicalData = dtClinicaldata.NewRow();
                dRowClinicalData["id"] = data.SelectToken("id").ToString();
                dRowClinicalData["doctor"] = data.SelectToken("doctor").ToString();
                dRowClinicalData["document"] = data.SelectToken("document").ToString();
                dRowClinicalData["date"] = data.SelectToken("date").ToString();
                dRowClinicalData["description"] = data.SelectToken("description").ToString();

                dtClinicaldata.Rows.Add(dRowClinicalData);

                dtClinicaldata.TableName = strModuleName;

                return dtClinicaldata;

                //return response.Content.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private class documentdetail
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("doctor")]
            public string Doctor { get; set; }

            [JsonProperty("document")]
            public string Document { get; set; }

            [JsonProperty("date")]
            public string Date { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }
        }

        public string GenerateAPIJSONString(DataTable dtDocument, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                string JSONString = string.Empty;
                DataTable dtFinalData;
                dtFinalData = dtDocument.Copy();

                JSONString = DataTableToJSONDocumentUpload(dtFinalData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string DataTableToJSONDocumentUpload(DataTable table, string emrpatientid, string requestid, string emrid, string moduleid, string userid)
        {
            var JSONString = new StringBuilder();

            JSONString.Append("{");
            JSONString.Append("\"EMRPatientId\":" + "\"" + emrpatientid + "\",");
            JSONString.Append("\"EMRId\":" + "\"" + emrid + "\",");
            JSONString.Append("\"ModuleId\":" + "\"" + moduleid + "\",");
            JSONString.Append("\"RequestId\":" + "\"" + requestid + "\",");
            JSONString.Append("\"CreatedBy\":" + "\"\",");
            JSONString.Append("\"EMRUserExtensionLogDetails\":");

            if (table.Rows.Count == 0)
            {
                JSONString.Append("\"No Data Found\"");
                JSONString.Append("}");
                return JSONString.ToString();
            }

            foreach (DataRow drow in table.Rows)
            {
                JSONString.Append("{");

                JSONString.Append("\"Id\":" + "\"" + drow["Id"].ToString() + "\",");
                JSONString.Append("\"Doctor\":" + "\"" + drow["Doctor"].ToString() + "\",");
                JSONString.Append("\"DocumentPath\":" + "\"" + drow["Document"].ToString() + "\",");
                JSONString.Append("\"Date\":" + "\"" + drow["Date"].ToString() + "\",");
                JSONString.Append("\"Description\":" + "\"" + drow["Description"].ToString() + "\"");

                JSONString.Append("}");
                JSONString.Append("}");
            }
            return JSONString.ToString();
        }

    }
}