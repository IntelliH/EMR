using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EMRIntegrations.Athena
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
                string strModuleName = string.Empty;
                strModuleName = "Medications";

                DataTable dtModuleData = new DataTable(strModuleName);
                DataTable dtClinicaldata = new DataTable();

                // Do data processing here.
                APIConnection api = (APIConnection)parameters["athenaapiobject"];
                List<medicationdetail> objclinical1 = new List<medicationdetail>();

                //ArrayList arydept = new ArrayList();
                //arydept = (ArrayList)parameters["api_departmentid"];

                //foreach (var value in arydept)
                //{
                Dictionary<string, string> dirlst = new Dictionary<string, string>()
                    {
                        {"departmentid", parameters["api_departmentid"].ToString()},
                        {"showndc","true" },
                        {"showrxnorm","true" }
                    };

                JObject jobj = (JObject)api.GET("chart/" + parameters["patientid"].ToString() + "/medications", dirlst);
                JToken jtobj = jobj["medications"];
                if (jtobj != null)
                {
                    if (jtobj.HasValues && jtobj.SelectToken("error") != null)
                    {
                        throw new Exception(jtobj["error"].ToString());
                    }

                    List<JToken> jtobjlst = new List<JToken>();
                    foreach (var item in jtobj)
                    {
                        foreach (var item1 in item)
                        {
                            jtobjlst.Add(item1);
                        }
                    }

                    medicationdetail objclinical = new medicationdetail();
                    objclinical1 = new List<medicationdetail>();

                    foreach (var subitem in jtobjlst)
                    {
                        objclinical = subitem.ToObject<medicationdetail>();
                        if (objclinical.Structuredsig != null)
                        {
                            objclinical.Dosagefrequencyvalue = objclinical.Structuredsig.Dosagefrequencyvalue;
                            objclinical.Dosageroute = objclinical.Structuredsig.Dosageroute;
                            objclinical.Dosageaction = objclinical.Structuredsig.Dosageaction;
                            objclinical.Dosageadditionalinstructions = objclinical.Structuredsig.Dosageadditionalinstructions;
                            objclinical.Dosagefrequencyunit = objclinical.Structuredsig.Dosagefrequencyunit;
                            objclinical.Dosagequantityunit = objclinical.Structuredsig.Dosagequantityunit;
                            objclinical.Dosagequantityvalue = objclinical.Structuredsig.Dosagequantityvalue;
                            objclinical.Dosagefrequencydescription = objclinical.Structuredsig.Dosagefrequencydescription;
                            objclinical.Dosagedurationunit = objclinical.Structuredsig.Dosagedurationunit;

                        }
                        if (objclinical.Events != null)
                        {
                            foreach (var item1 in objclinical.Events)
                            {
                                if (item1.Type.ToLower() == "start")
                                {
                                    objclinical.Startdate = item1.Eventdate;
                                }

                                if (item1.Type.ToLower() == "end")
                                {
                                    objclinical.Stopdate = item1.Eventdate;
                                }

                                if (item1.Type.ToLower() == "enter")
                                {
                                    objclinical.Enterdate = item1.Eventdate;
                                }

                                if (item1.Type.ToLower() == "hide")
                                {
                                    objclinical.Hidedate = item1.Eventdate;
                                }

                                objclinical.Type = item1.Type;
                            }
                        }

                        if (objclinical.Rxnorm != null)
                        {
                            string rxlst = "";
                            foreach (var rx in objclinical.Rxnorm)
                            {
                                rxlst = string.Concat(rxlst, rx, ",");
                            }
                            if (rxlst.Length > 0)
                            {
                                objclinical.Rxnormstr = rxlst.TrimEnd(',');
                            }
                        }

                        if (objclinical.Ndcoptions != null)
                        {
                            string Ndcoptionslst = "";
                            foreach (var rx in objclinical.Ndcoptions)
                            {
                                Ndcoptionslst = string.Concat(Ndcoptionslst, rx, ",");
                            }
                            if (Ndcoptionslst.Length > 0)
                            {
                                objclinical.Ndcoptionsstr = Ndcoptionslst.TrimEnd(',');
                            }
                        }
                        objclinical1.Add(objclinical);
                    }
                }
                //}

                dtClinicaldata = GetModuleDataBase.ConvertToDataTable(objclinical1);

                if (!dtClinicaldata.Columns.Contains("patientid"))
                {
                    dtClinicaldata.Columns.Add("patientid");
                }

                if (!dtClinicaldata.Columns.Contains("daterecorded"))
                {
                    dtClinicaldata.Columns.Add("daterecorded");
                }

                if (!dtClinicaldata.Columns.Contains("Active"))
                {
                    dtClinicaldata.Columns.Add("Active");
                }

                if (!dtClinicaldata.Columns.Contains("Active"))
                {
                    dtClinicaldata.Columns.Add("Active");
                }

                if (!dtClinicaldata.Columns.Contains("ndccode"))
                {
                    dtClinicaldata.Columns.Add("ndccode");
                }

                if (!dtClinicaldata.Columns.Contains("ndccode"))
                {
                    dtClinicaldata.Columns.Add("ndccode");
                }

                if (!dtClinicaldata.Columns.Contains("rxflag"))
                {
                    dtClinicaldata.Columns.Add("rxflag");
                }

                if (!dtClinicaldata.Columns.Contains("drugcode"))
                {
                    dtClinicaldata.Columns.Add("drugcode");
                }

                if (!dtClinicaldata.Columns.Contains("CodeSystemId"))
                {
                    dtClinicaldata.Columns.Add("CodeSystemId");
                }

                if (dtClinicaldata != null && dtClinicaldata.Rows.Count > 0)
                {
                    dtClinicaldata.Columns["dosageroute"].ColumnName = "doseroute";
                    dtClinicaldata.Columns["dosageaction"].ColumnName = "doseaction";
                    dtClinicaldata.Columns["dosagequantityvalue"].ColumnName = "Dosequantity";
                    dtClinicaldata.Columns["dosagequantityunit"].ColumnName = "DoseUnit";
                    dtClinicaldata.Columns["unstructuredsig"].ColumnName = "Direction";
                }

                foreach (DataRow dr in dtClinicaldata.Rows)
                {
                    dr["startdate"] = dr["startdate"].ToString(); //objCommonCleanup.ParseDateFromString(dr["startdate"].ToString(), "yyyyMMdd");
                    dr["stopdate"] = dr["stopdate"].ToString(); //objCommonCleanup.ParseDateFromString(dr["stopdate"].ToString(), "yyyyMMdd");
                    dr["patientid"] = parameters["patientid"];
                    dr["daterecorded"] = dr["Enterdate"].ToString(); //objCommonCleanup.ParseDateFromString(dr["Enterdate"].ToString(), "yyyyMMdd");

                    if (dr["Rxnormstr"].ToString().Trim().Length > 0)
                    {
                        if (dr["Ndcoptionsstr"].ToString().Trim().Length > 0)
                        {
                            dr["drugcode"] = dr["Ndcoptionsstr"].ToString().Split(',').Last();
                            dr["ndccode"] = dr["drugcode"];
                            dr["CodeSystemId"] = "NDC";
                        }
                        else
                        {
                            dr["drugcode"] = dr["Rxnormstr"].ToString().Split(',').Last();
                            dr["CodeSystemId"] = "RxNorm";
                        }
                    }
                    else
                    {
                        if (dr["Ndcoptionsstr"].ToString().Trim().Length > 0)
                        {
                            dr["drugcode"] = dr["Ndcoptionsstr"].ToString().Split(',').Last();
                            dr["ndccode"] = dr["drugcode"];
                            dr["CodeSystemId"] = "NDC";
                        }
                    }

                    string strStopDate = string.Empty;
                    if (dr["stopdate"].ToString() != "")
                    {
                        strStopDate = dr["stopdate"].ToString();
                    }

                    DateTime dtimeStopDate = DateTime.Now;
                    if (strStopDate.Length > 0)
                    {
                        dtimeStopDate = DateTime.ParseExact(strStopDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        if (dtimeStopDate >= DateTime.Now)
                        {
                            dr["stopdate"] = string.Empty;
                            dr["Active"] = "1";
                        }
                        else
                        {
                            dr["Active"] = "0";
                        }
                    }
                    else
                    {
                        dr["Active"] = "1";
                    }
                }
                dtClinicaldata = dtClinicaldata.DefaultView.ToTable(true);
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
            [JsonProperty("source")]
            public string Source { get; set; }

            [JsonProperty("createdby")]
            public string Createdby { get; set; }

            [JsonProperty("isstructuredsig")]
            public string Isstructuredsig { get; set; }

            [JsonProperty("medicationid")]
            public string Medicationid { get; set; }

            [JsonProperty("issafetorenew")]
            public string Issafetorenew { get; set; }

            [JsonProperty("medicationentryid")]
            public string Medicationentryid { get; set; }

            [JsonProperty("structuredsig")]
            public structuredsig Structuredsig { get; set; }

            [JsonProperty("events")]
            public List<events> Events { get; set; }

            [JsonProperty("medication")]
            public string Medication { get; set; }

            [JsonProperty("unstructuredsig")]
            public string Unstructuredsig { get; set; }

            [JsonProperty("dosagefrequencyvalue")]
            public string Dosagefrequencyvalue { get; set; }

            [JsonProperty("dosageroute")]
            public string Dosageroute { get; set; }

            [JsonProperty("dosageaction")]
            public string Dosageaction { get; set; }

            [JsonProperty("dosageadditionalinstructions")]
            public string Dosageadditionalinstructions { get; set; }

            [JsonProperty("dosagefrequencyunit")]
            public string Dosagefrequencyunit { get; set; }

            [JsonProperty("dosagequantityunit")]
            public string Dosagequantityunit { get; set; }

            [JsonProperty("dosagequantityvalue")]
            public string Dosagequantityvalue { get; set; }

            [JsonProperty("dosagefrequencydescription")]
            public string Dosagefrequencydescription { get; set; }

            [JsonProperty("dosagedurationunit")]
            public string Dosagedurationunit { get; set; }

            [JsonProperty("stopdate")]
            public string Stopdate { get; set; }

            [JsonProperty("startdate")]
            public string Startdate { get; set; }

            [JsonProperty("enterdate")]
            public string Enterdate { get; set; }

            [JsonProperty("hidedate")]
            public string Hidedate { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("providernote")]
            public string Providernote { get; set; }

            [JsonProperty("patientnote")]
            public string Patientnote { get; set; }

            [JsonProperty("orderingmode")]
            public string Orderingmode { get; set; }

            [JsonProperty("approvedby")]
            public string Approvedby { get; set; }

            [JsonProperty("route")]
            public string Route { get; set; }

            [JsonProperty("encounterid")]
            public string Encounterid { get; set; }

            [JsonProperty("ndcoptions")]
            public string[] Ndcoptions { get; set; }


            [JsonProperty("rxnorm")]
            public string[] Rxnorm { get; set; }

            [JsonProperty("ndcoptionsstr")]
            public string Ndcoptionsstr { get; set; }

            [JsonProperty("rxnormstr")]
            public string Rxnormstr { get; set; }


            [JsonProperty("stopreason")]
            public string Stopreason { get; set; }


        }

        private class structuredsig
        {

            [JsonProperty("dosagefrequencyvalue")]
            public string Dosagefrequencyvalue { get; set; }

            [JsonProperty("dosageroute")]
            public string Dosageroute { get; set; }

            [JsonProperty("dosageaction")]
            public string Dosageaction { get; set; }

            [JsonProperty("dosageadditionalinstructions")]
            public string Dosageadditionalinstructions { get; set; }

            [JsonProperty("dosagefrequencyunit")]
            public string Dosagefrequencyunit { get; set; }

            [JsonProperty("dosagequantityunit")]
            public string Dosagequantityunit { get; set; }

            [JsonProperty("dosagequantityvalue")]
            public string Dosagequantityvalue { get; set; }

            [JsonProperty("dosagefrequencydescription")]
            public string Dosagefrequencydescription { get; set; }

            [JsonProperty("dosagedurationunit")]
            public string Dosagedurationunit { get; set; }

        }

        private class events
        {

            [JsonProperty("eventdate")]
            public string Eventdate { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
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
                    JSONString.Append("\"Error\":" + "\"No Medication Found\",");
                    JSONString.Append("\"Medications\":{}");
                    JSONString.Append("}");
                    return JSONString.ToString();
                }

                JSONString.Append("\"Error\":" + "\"\",");
                JSONString.Append("\"Medications\":");
                JSONString.Append("[");

                int counter = 0;
                foreach (DataRow drow in table.Rows)
                {
                    counter++;

                    JSONString.Append("{");

                    JSONString.Append("\"CreatedBy\":" + "\"" + drow["Createdby"].ToString() + "\",");
                    JSONString.Append("\"MedicationID\":" + "\"" + drow["Medicationid"].ToString() + "\",");
                    JSONString.Append("\"MedicationName\":" + "\"" + drow["Medication"].ToString() + "\",");
                    JSONString.Append("\"Direction\":" + "\"" + drow["Direction"].ToString() + "\",");
                    JSONString.Append("\"DosageFrequencyValue\":" + "\"" + drow["Dosagefrequencyvalue"].ToString() + "\",");
                    JSONString.Append("\"DosageRoute\":" + "\"" + drow["doseroute"].ToString() + "\",");
                    JSONString.Append("\"DosageAdditionalInstructions\":" + "\"" + drow["Dosageadditionalinstructions"].ToString() + "\",");
                    JSONString.Append("\"DosageFrequencyUnit\":" + "\"" + drow["Dosagefrequencyunit"].ToString() + "\",");
                    JSONString.Append("\"DosageUnit\":" + "\"" + drow["DoseUnit"].ToString() + "\",");
                    JSONString.Append("\"DosageQuantity\":" + "\"" + drow["Dosequantity"].ToString() + "\",");
                    JSONString.Append("\"DosageFrequencyDescription\":" + "\"" + drow["Dosagefrequencydescription"].ToString() + "\",");
                    JSONString.Append("\"DosageDurationUnit\":" + "\"" + drow["Dosagedurationunit"].ToString() + "\",");
                    JSONString.Append("\"StopDate\":" + "\"" + drow["Stopdate"].ToString() + "\",");
                    JSONString.Append("\"StartDate\":" + "\"" + drow["Startdate"].ToString() + "\",");
                    JSONString.Append("\"EnterDate\":" + "\"" + drow["Enterdate"].ToString() + "\"");

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
    }
}
