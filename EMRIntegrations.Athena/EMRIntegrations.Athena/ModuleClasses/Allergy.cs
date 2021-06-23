using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EMRIntegrations.Athena
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
                string strModuleName = string.Empty;
                strModuleName = "Allergy";

                DataSet dsModuleData = new DataSet(strModuleName);
                DataTable dtClinicaldata = new DataTable();

                // Do data processing here.
                APIConnection api = (APIConnection)parameters["athenaapiobject"];
                List<allergy> objallergies = new List<allergy>();

                //ArrayList arydept = new ArrayList();
                //arydept = (ArrayList)parameters["api_departmentid"];

                //foreach (var value in arydept)
                //{

                Dictionary<string, string> dirlst = new Dictionary<string, string>()
                     {
                        {"departmentid", parameters["api_departmentid"].ToString()}
                    };

                JObject jobj = (JObject)api.GET("chart/" + parameters["patientid"].ToString() + "/allergies", dirlst);
                JToken jtobj = jobj["allergies"];

                if (jtobj != null)
                {
                    if (jtobj.HasValues && jtobj.SelectToken("error") != null)
                    {
                        throw new Exception(jtobj["error"].ToString());
                    }

                    objallergies = jtobj.ToObject<List<allergy>>();

                    int count = 0;
                    foreach (var item in objallergies)
                    {
                        if (item.Reactions.Count() > 0)
                        {
                            foreach (var subitem in item.Reactions)
                            {
                                if (subitem.Reactionname != "")
                                {
                                    objallergies[count].Reaction = string.Concat(objallergies[count].Reaction, subitem.Reactionname, ",");
                                }

                                if (subitem.Snomedcode != "")
                                {
                                    objallergies[count].Snomed = string.Concat(objallergies[count].Snomed, subitem.Snomedcode, ",");
                                }

                                if (subitem.Severity != "")
                                {
                                    objallergies[count].Severity = string.Concat(objallergies[count].Severity, subitem.Severity, ",");
                                }

                                if (subitem.Severitysnomedcode != "")
                                {
                                    objallergies[count].Severitysnomedcode = string.Concat(objallergies[count].Severitysnomedcode, subitem.Severitysnomedcode, ",");
                                }
                            }
                            objallergies[count].Reaction = objallergies[count].Reaction.TrimEnd(',');
                            objallergies[count].Snomed = objallergies[count].Snomed.TrimEnd(',');
                            objallergies[count].Severity = objallergies[count].Severity.TrimEnd(',');
                            objallergies[count].Severitysnomedcode = objallergies[count].Severitysnomedcode.TrimEnd(',');
                        }
                        count += 1;

                    }
                }
                //}
                dtClinicaldata = GetModuleDataBase.ConvertToDataTable(objallergies);

                if (!dtClinicaldata.Columns.Contains("patientid"))
                {
                    dtClinicaldata.Columns.Add("patientid");
                }

                if (!dtClinicaldata.Columns.Contains("codesystemid"))
                {
                    dtClinicaldata.Columns.Add("codesystemid");
                }

                if (!dtClinicaldata.Columns.Contains("admitflag"))
                {
                    dtClinicaldata.Columns.Add("admitflag");
                }

                if (!dtClinicaldata.Columns.Contains("daterecorded"))
                {
                    dtClinicaldata.Columns.Add("daterecorded");
                }

                if (dtClinicaldata != null && dtClinicaldata.Rows.Count > 0)
                {
                    dtClinicaldata.Columns["Allergenname"].ColumnName = "allergicto";
                    dtClinicaldata.Columns["Snomed"].ColumnName = "Reactionsnomedcode";
                }

                foreach (DataRow dr in dtClinicaldata.Rows)
                {
                    dr["patientid"] = parameters["patientid"];
                    dr["admitflag"] = "1";
                    dr["onsetdate"] = dr["onsetdate"].ToString();
                    dr["Deactivatedate"] = dr["Deactivatedate"].ToString();
                    dr["daterecorded"] = DateTime.Now.ToString();
                }
                dtClinicaldata = dtClinicaldata.DefaultView.ToTable(true);
                //dsModuleData.Tables.Add(dtValidation);
                //dsModuleData.Tables.Add(dtClinicaldata);

                return dtClinicaldata;
            }
            catch (Exception)
            {
                throw;
            }
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
                    JSONString.Append("\"Error\":" + "\"No Allergy Found\",");
                    JSONString.Append("\"Allergy\":{}");
                    JSONString.Append("}");
                    return JSONString.ToString();
                }

                JSONString.Append("\"Error\":" + "\"\",");
                JSONString.Append("\"Allergy\":");
                JSONString.Append("[");

                int counter = 0;
                foreach (DataRow drow in table.Rows)
                {
                    counter++;

                    JSONString.Append("{");

                    JSONString.Append("\"allergicto\":" + "\"" + drow["allergicto"].ToString() + "\",");
                    JSONString.Append("\"Reaction\":" + "\"" + drow["Reaction"].ToString() + "\",");
                    JSONString.Append("\"Reactionsnomedcode\":" + "\"" + drow["Reactionsnomedcode"].ToString() + "\",");
                    JSONString.Append("\"Severity\":" + "\"" + drow["Severity"].ToString() + "\",");
                    JSONString.Append("\"Note\":" + "\"" + drow["Note"].ToString() + "\",");
                    JSONString.Append("\"Onsetdate\":" + "\"" + drow["Onsetdate"].ToString() + "\",");
                    JSONString.Append("\"Severitysnomedcode\":" + "\"" + drow["Severitysnomedcode"].ToString() + "\",");
                    JSONString.Append("\"Encounterid\":" + "\"" + drow["Encounterid"].ToString() + "\",");
                    JSONString.Append("\"Deactivatedate\":" + "\"" + drow["Deactivatedate"].ToString() + "\",");
                    JSONString.Append("\"patientid\":" + "\"" + drow["patientid"].ToString() + "\",");
                    JSONString.Append("\"admitflag\":" + "\"" + drow["admitflag"].ToString() + "\",");
                    JSONString.Append("\"daterecorded\":" + "\"" + drow["daterecorded"].ToString() + "\",");

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
            catch (Exception)
            {
                throw;
            }
        }

        private class allergy
        {
            [JsonProperty("allergenname")]
            public string Allergenname { get; set; }

            [JsonProperty("allergenid")]
            public string Allergenid { get; set; }

            [JsonProperty("reactions")]
            public reactions[] Reactions { get; set; }

            [JsonProperty("reaction")]
            public string Reaction { get; set; }

            [JsonProperty("snomed")]
            public string Snomed { get; set; }

            [JsonProperty("severity")]
            public string Severity { get; set; }

            [JsonProperty("note")]
            public string Note { get; set; }

            [JsonProperty("onsetdate")]
            public string Onsetdate { get; set; }

            [JsonProperty("severitysnomedcode")]
            public string Severitysnomedcode { get; set; }

            [JsonProperty("encounterid")]
            public string Encounterid { get; set; }

            [JsonProperty("deactivatedate")]
            public string Deactivatedate { get; set; }
        }

        private class reactions
        {
            [JsonProperty("reactionname")]
            public string Reactionname { get; set; }

            [JsonProperty("snomedcode")]
            public string Snomedcode { get; set; }

            [JsonProperty("severity")]
            public string Severity { get; set; }

            [JsonProperty("severitysnomedcode")]
            public string Severitysnomedcode { get; set; }
        }
    }
}
