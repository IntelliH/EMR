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
    public class Appointments
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
                        dtModuleData = GetAppointmentsData(parameters);
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
        private DataTable GetAppointmentsData(Hashtable parameters)
        {
            try
            {
                string strModuleName = string.Empty;
                strModuleName = "Appointments";

                //DataSet dsModuleData = new DataSet(strModuleName);
                DataTable dtClinicaldata = new DataTable(strModuleName);
                DataTable dtTempata = new DataTable();

                // Do data processing here.
                APIConnection api = (APIConnection)parameters["athenaapiobject"];

                Dictionary<string, string> appbook = null;
                JObject jobj = null;
                JToken jtobj = null;

                ArrayList arydept = new ArrayList();
                arydept = (ArrayList)parameters["api_departmentid_patientmaster"];

                foreach (var value in arydept)
                {
                    string format = "MM/dd/yyyy";
                    DateTime StartDateTime = new DateTime();
                    //if (Convert.ToString(objConfiguration.GetDeltaIdentifier(parameters, connection.GetType().Name)) != string.Empty)
                    //{
                    //    StartDateTime = Convert.ToDateTime(objConfiguration.GetDeltaIdentifier(parameters, connection.GetType().Name));
                    //}
                    DateTime EndDateTime = DateTime.Now;

                    appbook = new Dictionary<string, string>()
                      {
                    {"patientid",  parameters["patientid"].ToString()},
                        {"departmentid", value.ToString() },
                        {"startdate", "01/01/2001"},
                        {"enddate", EndDateTime.ToString(format).Replace("-", "/")},
                      {"limit", "10000"}
                      };

                    jobj = (JObject)api.GET("appointments/booked", appbook);
                    jtobj = jobj["appointments"];
                    if (jtobj != null)
                    {
                        if (jtobj.HasValues && jtobj.SelectToken("error") != null)
                        {
                            throw new Exception(jtobj["error"].ToString());
                        }

                        List<appointments> objclinical = jtobj.ToObject<List<appointments>>();

                        dtTempata = GetModuleDataBase.ConvertToDataTable(objclinical);
                        dtClinicaldata.Merge(dtTempata);
                    }
                }

                if (dtClinicaldata != null && dtClinicaldata.Rows.Count > 0)
                {
                    dtClinicaldata.Columns["date"].ColumnName = "adate";
                    dtClinicaldata.Columns["starttime"].ColumnName = "atime";
                    dtClinicaldata.Columns["appointmentstatus"].ColumnName = "astatus";
                    dtClinicaldata.Columns["providerid"].ColumnName = "dcode";
                    dtClinicaldata.Columns["appointmenttype"].ColumnName = "atype";
                    dtClinicaldata.Columns["duration"].ColumnName = "aduration";
                    dtClinicaldata.Columns["lastmodified"].ColumnName = "updateddate";
                }

                if (!dtClinicaldata.Columns.Contains("daterecorded"))
                {
                    dtClinicaldata.Columns.Add("daterecorded");
                }

                if (!dtClinicaldata.Columns.Contains("anotes"))
                {
                    dtClinicaldata.Columns.Add("anotes");
                }

                if (!dtClinicaldata.Columns.Contains("comments"))
                {
                    dtClinicaldata.Columns.Add("comments");
                }

                foreach (DataRow dr in dtClinicaldata.Rows)
                {
                    dr["patientid"] = parameters["patientid"];
                    dr["daterecorded"] = dr["adate"];
                    dr["atime"] = dr["atime"].ToString().Replace(":", "");
                    dr["adate"] = dr["adate"].ToString();
                    dr["startcheckin"] = dr["startcheckin"].ToString();
                    dr["daterecorded"] = dr["daterecorded"].ToString();

                    jobj = (JObject)api.GET("appointments/" + dr["appointmentid"].ToString() + "/notes");
                    if (jobj.SelectToken("totalcount").ToString() != "0")
                    {
                        jtobj = jobj["notes"];
                        string strnote = "";
                        if (jtobj.Count() > 0)
                        {
                            foreach (var item in jtobj)
                            {
                                if (item.SelectToken("notetext") != null)
                                {
                                    strnote = string.Concat("Notes: ", strnote, item["notetext"].ToString());
                                    strnote = string.Concat(strnote, "; Created: ", item["created"].ToString());
                                    strnote = string.Concat(strnote, "; Created By: ", item["createdby"].ToString());
                                }
                            }
                        }

                        if (strnote.Length > 2000)
                        {
                            dr["comments"] = strnote;
                        }
                        else
                        {
                            dr["anotes"] = strnote;
                        }
                    }

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
        private class Department
        {
            public string departmentid { get; set; }
        }
        private class appointments
        {

            [JsonProperty("date")]
            public string Date { get; set; }

            [JsonProperty("appointmentid")]
            public string Appointmentid { get; set; }

            [JsonProperty("starttime")]
            public string Starttime { get; set; }

            [JsonProperty("departmentid")]
            public string Departmentid { get; set; }

            [JsonProperty("appointmentconfirmationid")]
            public string Appointmentconfirmationid { get; set; }

            [JsonProperty("appointmentstatus")]
            public string Appointmentstatus { get; set; }

            [JsonProperty("scheduledby")]
            public string Scheduledby { get; set; }

            [JsonProperty("patientid")]
            public string Patientid { get; set; }

            [JsonProperty("appointmentconfirmationname")]
            public string Appointmentconfirmationname { get; set; }

            [JsonProperty("duration")]
            public string Duration { get; set; }

            [JsonProperty("templateappointmenttypeid")]
            public string Templateappointmenttypeid { get; set; }

            [JsonProperty("lastmodifiedby")]
            public string Lastmodifiedby { get; set; }

            [JsonProperty("copay")]
            public int Copay { get; set; }

            [JsonProperty("appointmenttypeid")]
            public string Appointmenttypeid { get; set; }

            [JsonProperty("lastmodified")]
            public string Lastmodified { get; set; }

            [JsonProperty("appointmenttype")]
            public string Appointmenttype { get; set; }

            [JsonProperty("providerid")]
            public string Providerid { get; set; }

            [JsonProperty("scheduleddatetime")]
            public string Scheduleddatetime { get; set; }

            [JsonProperty("coordinatorenterprise")]
            public string Coordinatorenterprise { get; set; }

            [JsonProperty("templateappointmentid")]
            public string Templateappointmentid { get; set; }

            [JsonProperty("patientappointmenttypename")]
            public string Patientappointmenttypename { get; set; }

            [JsonProperty("encounterid")]
            public string Encounterid { get; set; }

            [JsonProperty("startcheckin")]
            public string Startcheckin { get; set; }

            [JsonProperty("encounterstatus")]
            public string Encounterstatus { get; set; }



        }

        private class AppointmentsNotes
        {

            [JsonProperty("notes")]
            public Note[] Notes { get; set; }

            [JsonProperty("totalcount")]
            public int Totalcount { get; set; }
        }

        private class Note
        {

            [JsonProperty("notetext")]
            public string Notetext { get; set; }

            [JsonProperty("noteid")]
            public string Noteid { get; set; }

            [JsonProperty("created")]
            public string Created { get; set; }

            [JsonProperty("createdby")]
            public string Createdby { get; set; }
        }
    }
}
