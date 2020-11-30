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
        public string PostData(Hashtable parameters)
        {
            try
            {
                string responseData = string.Empty;
                responseData = PostDocumentUploadData(parameters);
                return responseData;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private string PostDocumentUploadData(Hashtable parameters)
        {
            try
            {
                var client = new RestClient("https://drchrono.com/api/documents");
                client.Timeout = -1;

                var request = new RestRequest(Method.POST);
                request.AddHeader("authorization", "bearer:" + parameters["access_token"].ToString() + "");
                request.AddFile("document", parameters["filepath"].ToString());
                request.AddParameter("patient", parameters["patientid"].ToString());
                request.AddParameter("doctor", parameters["doctorid"].ToString()); //269552
                request.AddParameter("date", parameters["documentdate"].ToString());
                request.AddParameter("description", parameters["documentdescription"].ToString());

                IRestResponse response = client.Execute(request);
                return response.Content.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GenerateAPIJSONString(DataTable dtPatientData, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                string JSONString = string.Empty;
                DataTable dtFinalData;
                dtFinalData = dtPatientData.Copy();

                foreach (DataColumn dColumnPatDemo in dtFinalData.Columns)
                {
                    if (dColumnPatDemo.ColumnName.ToLower() == "first_name")
                    {
                        dColumnPatDemo.ColumnName = "FirstName";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "middle_name")
                    {
                        dColumnPatDemo.ColumnName = "MiddleName";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "last_name")
                    {
                        dColumnPatDemo.ColumnName = "LastName";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "email")
                    {
                        dColumnPatDemo.ColumnName = "Email";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "date_of_birth")
                    {
                        dColumnPatDemo.ColumnName = "DateOfBirth";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "gender")
                    {
                        dColumnPatDemo.ColumnName = "Gender";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "home_phone")
                    {
                        dColumnPatDemo.ColumnName = "Phone";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "ethnicity")
                    {
                        dColumnPatDemo.ColumnName = "EthnicityCode";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "race")
                    {
                        dColumnPatDemo.ColumnName = "RaceCode";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "address")
                    {
                        dColumnPatDemo.ColumnName = "Address1";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "city")
                    {
                        dColumnPatDemo.ColumnName = "City";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "state")
                    {
                        dColumnPatDemo.ColumnName = "State";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "zip_code")
                    {
                        dColumnPatDemo.ColumnName = "ZipCode";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "social_security_number")
                    {
                        dColumnPatDemo.ColumnName = "SSN";
                    }
                }

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
                JSONString.Append("\"No Patient Found\"");
                JSONString.Append("}");
                return JSONString.ToString();
            }

            foreach (DataRow drow in table.Rows)
            {
                JSONString.Append("{");

                JSONString.Append("\"UserId\":" + "\"" + userid + "\",");
                JSONString.Append("\"Title\":" + "\"\",");
                JSONString.Append("\"FirstName\":" + "\"" + drow["FirstName"].ToString() + "\",");
                JSONString.Append("\"MiddleName\":" + "\"" + drow["MiddleName"].ToString() + "\",");
                JSONString.Append("\"LastName\":" + "\"" + drow["LastName"].ToString() + "\",");
                JSONString.Append("\"Suffix\":" + "\"\",");
                JSONString.Append("\"Gender\":" + "\"" + drow["Gender"].ToString() + "\",");
                JSONString.Append("\"DateOfBirth\":" + "\"" + drow["DateOfBirth"].ToString() + "\",");
                JSONString.Append("\"Email\":" + "\"" + drow["Email"].ToString() + "\",");
                JSONString.Append("\"Street\":" + "\"\",");
                JSONString.Append("\"City\":" + "\"" + drow["City"].ToString() + "\",");
                JSONString.Append("\"ZipCode\":" + "\"" + drow["ZipCode"].ToString() + "\",");
                JSONString.Append("\"Phone\":" + "\"" + drow["Phone"].ToString() + "\",");
                JSONString.Append("\"Address1\":" + "\"" + drow["Address1"].ToString() + "\",");
                JSONString.Append("\"Address2\":" + "\"\",");
                JSONString.Append("\"SSN\":" + "\"" + drow["SSN"].ToString() + "\",");
                JSONString.Append("\"MRN\":" + "\"\",");
                JSONString.Append("\"State\":" + "\"" + drow["State"].ToString() + "\",");
                JSONString.Append("\"Password\":" + "\"\",");
                JSONString.Append("\"Role\":" + "\"\",");
                JSONString.Append("\"heightFeet\":" + "\"\",");
                JSONString.Append("\"heightInch\":" + "\"\",");
                JSONString.Append("\"weight\":" + "\"\",");
                JSONString.Append("\"FacilityId\":" + "\"" + requestid + "\",");
                JSONString.Append("\"StateId\":" + "\"\",");
                JSONString.Append("\"EthnicityCode\":" + "\"" + drow["EthnicityCode"].ToString() + "\",");
                JSONString.Append("\"RaceCode\":" + "\"" + drow["RaceCode"].ToString() + "\",");
                JSONString.Append("\"NPI\":" + "\"\",");
                JSONString.Append("\"Status\":" + "\"\",");
                JSONString.Append("\"POSCode\":" + "\"\"");

                JSONString.Append("}");
                JSONString.Append("}");
            }
            return JSONString.ToString();
        }

    }
}