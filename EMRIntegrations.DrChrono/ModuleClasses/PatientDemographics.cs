using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using RestSharp;
using System.Globalization;

namespace EMRIntegrations.DrChrono
{
    class PatientDemographics
    {
        public DataTable GetData(Hashtable parameters)
        {
            try
            {
                DataTable dtModuleData = new DataTable();
                dtModuleData = GetPatientDemographicsData(parameters);
                return dtModuleData;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private DataTable GetPatientDemographicsData(Hashtable parameters)
        {
            try
            {
                string strModuleName = "PatientDemographics";

                //DataSet dsModuleData = new DataSet(strModuleName);
                DataTable dtClinicaldata = new DataTable();

                //RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
                var client = new RestClient("" + parameters["api_baseurl"].ToString() + "api/patients/" + parameters["patientid"].ToString() + "");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("authorization", "bearer:" + parameters["access_token"].ToString() + "");

                IRestResponse response = client.Execute(request);

                JToken jtobj = (JToken)JsonConvert.DeserializeObject("[" + response.Content.Replace("[", "\"[").Replace("]", "]\"") + "]");

                List<Patients> Patientdata = jtobj.ToObject<List<Patients>>();

                dtClinicaldata = GetModuleDataBase.ConvertToDataTable(Patientdata);

                if (dtClinicaldata.Rows[0][dtClinicaldata.Columns["Detail"]].ToString().Trim().ToLower() == "not found.")
                    return new DataTable();

                dtClinicaldata.TableName = strModuleName;

                return dtClinicaldata;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected class Patients
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("chart_id")]
            public string Chart_id { get; set; }

            [JsonProperty("first_name")]
            public string First_name { get; set; }

            [JsonProperty("middle_name")]
            public string Middle_name { get; set; }

            [JsonProperty("last_name")]
            public string Last_name { get; set; }

            [JsonProperty("nick_name")]
            public string Nick_name { get; set; }

            [JsonProperty("date_of_birth")]
            public string Date_of_birth { get; set; }

            [JsonProperty("gender")]
            public string Gender { get; set; }

            [JsonProperty("social_security_number")]
            public string Social_security_number { get; set; }

            [JsonProperty("race")]
            public string Race { get; set; }

            [JsonProperty("ethnicity")]
            public string Ethnicity { get; set; }

            [JsonProperty("preferred_language")]
            public string Preferred_language { get; set; }

            [JsonProperty("patient_photo")]
            public string Patient_photo { get; set; }

            [JsonProperty("patient_photo_date")]
            public string Patient_photo_date { get; set; }

            [JsonProperty("patient_payment_profile")]
            public string Patient_payment_profile { get; set; }

            [JsonProperty("patient_status")]
            public string Patient_status { get; set; }

            [JsonProperty("home_phone")]
            public string Home_phone { get; set; }

            [JsonProperty("cell_phone")]
            public string Cell_phone { get; set; }

            [JsonProperty("office_phone")]
            public string Office_phone { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("address")]
            public string Address { get; set; }//pcp

            [JsonProperty("city")]
            public string City { get; set; }

            [JsonProperty("state")]
            public string State { get; set; }

            [JsonProperty("zip_code")]
            public string Zip_code { get; set; }

            [JsonProperty("emergency_contact_name")]
            public string Emergency_contact_name { get; set; }

            [JsonProperty("emergency_contact_phone")]
            public string Emergency_contact_phone { get; set; }

            [JsonProperty("emergency_contact_relation")]
            public string Emergency_contact_relation { get; set; }

            [JsonProperty("employer")]
            public string Employer { get; set; }

            [JsonProperty("employer_address")]
            public string Employer_address { get; set; }

            [JsonProperty("employer_city")]
            public string Employer_city { get; set; }

            [JsonProperty("employer_state")]
            public string Employer_state { get; set; }

            [JsonProperty("employer_zip_code")]
            public string Employer_zip_code { get; set; }

            [JsonProperty("disable_sms_messages")]
            public string Disable_sms_messages { get; set; }

            [JsonProperty("doctor")]
            public string Doctor { get; set; }

            [JsonProperty("primary_care_physician")]
            public string Primary_care_physician { get; set; }

            [JsonProperty("date_of_first_appointment")]
            public string Date_of_first_appointment { get; set; }

            [JsonProperty("date_of_last_appointment")]
            public string Date_of_last_appointment { get; set; }

            [JsonProperty("offices")]
            public string Offices { get; set; }

            [JsonProperty("default_pharmacy")]
            public string Default_pharmacy { get; set; }

            [JsonProperty("referring_source")]
            public string Referring_source { get; set; }

            [JsonProperty("copay")]
            public string Copay { get; set; }

            [JsonProperty("responsible_party_name")]
            public string Responsible_party_name { get; set; }

            [JsonProperty("responsible_party_relation")]
            public string Responsible_party_relation { get; set; }

            [JsonProperty("responsible_party_phone")]
            public string Responsible_party_phone { get; set; }

            [JsonProperty("responsible_party_email")]
            public string Responsible_party_email { get; set; }

            [JsonProperty("updated_at")]
            public string Updated_at { get; set; }

            [JsonProperty("detail")]
            public string Detail { get; set; }
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

                JSONString = DataTableToJSONPatientDemographics(dtFinalData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string DataTableToJSONPatientDemographics(DataTable table, string emrpatientid, string requestid, string emrid, string moduleid, string userid)
        {
            try
            {
                var JSONString = new StringBuilder();

                JSONString.Append("{");
                JSONString.Append("\"EMRPatientId\":" + "\"" + emrpatientid + "\",");
                JSONString.Append("\"EMRId\":" + "\"" + emrid + "\",");
                JSONString.Append("\"ModuleId\":" + "\"" + moduleid + "\",");
                JSONString.Append("\"RequestId\":" + "\"" + requestid + "\",");
                JSONString.Append("\"CreatedBy\":" + "\"\",");

                if (table.Rows.Count == 0)
                {
                    JSONString.Append("\"Error\":" + "\"No Patient Found\",");
                    JSONString.Append("\"EMRUserExtensionLogDetails\":{}");
                    JSONString.Append("}");
                    return JSONString.ToString();
                }

                JSONString.Append("\"Error\":" + "\"\",");
                JSONString.Append("\"EMRUserExtensionLogDetails\":");

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

                    if (!string.IsNullOrEmpty(drow["DateOfBirth"].ToString()))
                    {
                        string res = drow["DateOfBirth"].ToString().Replace("-", string.Empty);
                        DateTime d = DateTime.ParseExact(res, "yyyyMMdd", CultureInfo.InvariantCulture);
                        drow["DateOfBirth"] = d.ToString("MM/dd/yyyy").Replace("-", "/");
                        JSONString.Append("\"DateOfBirth\":" + "\"" + drow["DateOfBirth"].ToString() + "\",");
                    }
                    else
                    {
                        JSONString.Append("\"DateOfBirth\":" + "\"\",");
                    }

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
                    JSONString.Append("\"EthnicityCode\":" + "\"" + mapEthnicity(drow["EthnicityCode"].ToString()) + "\",");
                    JSONString.Append("\"RaceCode\":" + "\"" + mapRace(drow["RaceCode"].ToString()) + "\",");
                    JSONString.Append("\"DoctorId\":" + "\"" + drow["Doctor"].ToString() + "\",");
                    JSONString.Append("\"NPI\":" + "\"\",");
                    JSONString.Append("\"Status\":" + "\"\",");
                    JSONString.Append("\"POSCode\":" + "\"\"");

                    JSONString.Append("}");
                    JSONString.Append("}");
                }
                return JSONString.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        string mapRace(string race)
        {
            try
            {
                switch (race.ToLower())
                {
                    case "black":
                        return "Black or African American";
                    case "asian":
                        return "Asian";
                    case "indian":
                        return "American Indian or Alaska Native";
                    case "hawaiian":
                        return "Native Hawaiian or Other Pacific Islander";
                    case "white":
                        return "White";
                    case "other":
                        return "Others";
                    case "declined":
                        return string.Empty;
                    default:
                        return string.Empty;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        string mapEthnicity(string ethnicity)
        {
            try
            {
                switch (ethnicity.ToLower())
                {
                    case "hispanic":
                        return "Hispanic";
                    case "not_hispanic":
                        return "Non-hispanic";
                    case "declined":
                        return string.Empty;
                    default:
                        return string.Empty;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}