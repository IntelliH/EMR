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
    public class Problems
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
                        dtModuleData = GetProblemsData(parameters);
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
        private DataTable GetProblemsData(Hashtable parameters)
        {
            try
            {
                string strModuleName = string.Empty;
                strModuleName = "Diagnosis";


                DataSet dsModuleData = new DataSet(strModuleName);
                DataTable dtClinicaldata = new DataTable();
                //DataTable dtValidation = new DataTable();
                //dtValidation = CreateValidationDataTable("xclinicalvalidations");

                // Do data processing here.
                APIConnection api = (APIConnection)parameters["athenaapiobject"];
                List<problems> objclinical = new List<problems>();

                //ArrayList arydept = new ArrayList();
                //arydept = (ArrayList)parameters["api_departmentid"];

                //foreach (var value in arydept)
                //{
                Dictionary<string, string> dirlst = new Dictionary<string, string>()
                    {
                        {"departmentid", parameters["api_departmentid"].ToString()}
                    };

                JObject jobj = (JObject)api.GET("chart/" + parameters["patientid"].ToString() + "/problems", dirlst);
                JToken jtobj = jobj["problems"];
                if (jtobj != null)
                {
                    if (jtobj.HasValues && jtobj.SelectToken("error") != null)
                    {
                        throw new Exception(jtobj["error"].ToString());
                    }

                    objclinical = jtobj.ToObject<List<problems>>();

                    List<problems> objclinical1 = new List<problems>();

                    problems objproblems = new problems();

                    foreach (var item in objclinical)
                    {

                        if (item.Events != null)
                        {
                            item.Source = item.Events[0].Source;
                            item.Status = item.Events[0].Status;
                            item.Eventtype = item.Events[0].Eventtype;
                            item.Startdate = item.Events[0].Startdate;
                            item.CreatedDate = item.Events[0].CreatedDate;
                            item.OnSetDate = item.Events[0].OnSetDate;
                            item.Laterality = item.Events[0].Laterality;
                            item.Note = item.Events[0].Note;
                            item.Eventcount = item.Events.Count().ToString();
                        }
                    }
                    // int count = 0;
                    //Diagnoses records
                    foreach (var item in objclinical)
                    {
                        if (item.Events != null && item.Events[0].Diagnoses != null && item.Events[0].Diagnoses.Count() > 0)
                        {
                            objproblems = new problems();
                            objproblems.Source = item.Events[0].Source;
                            objproblems.Status = item.Events[0].Status;
                            objproblems.Eventtype = item.Events[0].Eventtype;
                            objproblems.Startdate = item.Events[0].Startdate;
                            objproblems.CreatedDate = item.Events[0].CreatedDate;
                            objproblems.OnSetDate = item.Events[0].OnSetDate;
                            objproblems.Laterality = item.Events[0].Laterality;
                            objproblems.Note = item.Events[0].Note;
                            objproblems.Eventcount = item.Events.Count().ToString();
                            objproblems.Codeset = item.Events[0].Diagnoses[0].Codeset;
                            objproblems.Code = item.Events[0].Diagnoses[0].Code;
                            objproblems.Name = item.Events[0].Diagnoses[0].Name;
                            objproblems.LastModifiedBy = item.LastModifiedBy;
                            objproblems.LastModifiedDateTime = item.LastModifiedDateTime;

                            objclinical1.Add(objproblems);
                        }
                    }
                    if (objclinical1.Count() > 0)
                    {
                        objclinical.AddRange(objclinical1);
                    }
                }
                //}

                dtClinicaldata = GetModuleDataBase.ConvertToDataTable(objclinical);

                if (!dtClinicaldata.Columns.Contains("patientid"))
                {
                    dtClinicaldata.Columns.Add("patientid");
                }
                
                if (dtClinicaldata != null && dtClinicaldata.Rows.Count > 0)
                {
                    dtClinicaldata.Columns["codeset"].ColumnName = "codesystemid";
                    dtClinicaldata.Columns["name"].ColumnName = "problem";
                }

                string strstartdate = string.Empty;
                foreach (DataRow dr in dtClinicaldata.Rows)
                {

                    dr["patientid"] = parameters["patientid"].ToString();
                    //dr["lifecycle"] = "CHRONIC"; //set hard coded lifecycle as per api documention mentioned like:The status of this problem event: CHRONIC or ACUTE

                    //strstartdate = "";
                    //if (!string.IsNullOrEmpty(dr["startdate"].ToString()))
                    //{
                    //    strstartdate = dr["startdate"].ToString();
                    //    if (!string.IsNullOrEmpty(strstartdate))
                    //    {
                    //        dr["daterecorded"] = strstartdate;
                    //    }
                    //    else
                    //    {
                    //        dr["daterecorded"] = DateTime.Now.ToString();
                    //    }
                    //}
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

        internal class problems
        {
            [JsonProperty("problemid")]
            public string Problemid { get; set; }

            [JsonProperty("events")]
            public Event[] Events { get; set; }

            [JsonProperty("codeset")]
            public string Codeset { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }
            [JsonProperty("source")]
            public string Source { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("eventtype")]
            public string Eventtype { get; set; }

            [JsonProperty("startdate")]
            public string Startdate { get; set; }

            [JsonProperty("createddate")]
            public string CreatedDate { get; set; }

            [JsonProperty("onsetdate")]
            public string OnSetDate { get; set; }

            [JsonProperty("diagnoses")]
            public object[] Diagnoses { get; set; }

            [JsonProperty("laterality")]
            public string Laterality { get; set; }

            [JsonProperty("note")]
            public string Note { get; set; }

            [JsonProperty("eventcount")]
            public string Eventcount { get; set; }

            [JsonProperty("diagnosescount")]
            public string Diagnosescount { get; set; }

            [JsonProperty("encounterid")]
            public string Encounterid { get; set; }

            [JsonProperty("lastmodifiedby")]
            public string LastModifiedBy { get; set; }

            [JsonProperty("lastmodifieddatetime")]
            public string LastModifiedDateTime { get; set; }
        }

        internal class Event
        {
            [JsonProperty("source")]
            public string Source { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("eventtype")]
            public string Eventtype { get; set; }

            [JsonProperty("startdate")]
            public string Startdate { get; set; }

            [JsonProperty("createddate")]
            public string CreatedDate { get; set; }

            [JsonProperty("onsetdate")]
            public string OnSetDate { get; set; }

            [JsonProperty("diagnoses")]
            public Diagnosis[] Diagnoses { get; set; }

            [JsonProperty("laterality")]
            public string Laterality { get; set; }

            [JsonProperty("note")]
            public string Note { get; set; }

            [JsonProperty("encounterid")]
            public string Encounterid { get; set; }
        }

        internal class Diagnosis
        {

            [JsonProperty("codeset")]
            public string Codeset { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }
        }
    }
}
