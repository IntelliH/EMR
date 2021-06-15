using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace EMRIntegrations.DrChrono
{
    public class Vitals
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
                        dtModuleData = GetVitalsData(parameters);
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
        private DataTable GetVitalsData(Hashtable parameters)
        {
            try
            {
                string strModuleName = string.Empty;
                strModuleName = "Vitals";

                DataTable dtModuleData = new DataTable(strModuleName);
                DataTable dtClinicaldata = new DataTable();
                //DataTable dtValidation = new DataTable();
                //dtValidation = CreateValidationDataTable("xclinicalvalidations");
                DataTable dtvitalclinical = createvitaltable();
                // Do data processing here.
                APIConnection api = (APIConnection)parameters["athenaapiobject"];
                List<readings> objclinical1 = new List<readings>();

                //ArrayList arydept = new ArrayList();
                //arydept = (ArrayList)parameters["api_departmentid"];
                //foreach (var value in arydept)
                //{
                Dictionary<string, string> dirlst = new Dictionary<string, string>()
                    {
                        {"departmentid",parameters["api_departmentid"].ToString()}
                    };

                JObject jobj = (JObject)api.GET("chart/" + parameters["patientid"].ToString() + "/vitals", dirlst);
                JToken jtobj = jobj["vitals"];
                if (jtobj != null)
                {
                    if (jtobj.HasValues && jtobj.SelectToken("error") != null)
                    {
                        throw new Exception("Error: Operation was unsuccessful because of a client error.");
                    }

                    List<vital> objclinical = jtobj.ToObject<List<vital>>();
                    objclinical1 = new List<readings>();
                    readings[][] obredreadingsary;
                    List<readings[]> obredreadinglst;

                    foreach (var item in objclinical)
                    {
                        obredreadingsary = item.readings;
                        obredreadinglst = obredreadingsary.ToList<readings[]>();
                        foreach (var obredreadingsublist in obredreadinglst)
                        {
                            foreach (var subitem in obredreadingsublist)
                            {
                                objclinical1.Add(subitem);
                            }
                        }
                    }

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

                    DataRow newrow;
                    string strbpd = string.Empty;

                    DataTable dtReading = dtClinicaldata.DefaultView.ToTable(true, "Readingid", "Readingtaken");

                    string strvitalname = string.Empty;
                    string strvitalvalue = string.Empty;
                    string strbpcolumn = string.Empty;
                    string strbploccolumn = string.Empty;
                    string strcode = string.Empty;
                    foreach (DataRow item in dtReading.Rows)
                    {
                        //string strreadingdate = string.Empty;

                        newrow = dtvitalclinical.NewRow();
                        newrow["patientid"] = parameters["patientid"].ToString();
                        newrow["readingid"] = item["readingid"].ToString();
                        newrow["daterecorded"] = item["Readingtaken"].ToString(); //objCommonCleanup.ParseDateFromString(item["Readingtaken"].ToString(), "yyyyMMdd");

                        strbpcolumn = "bp_sitting";
                        strbploccolumn = "bp_sitting_location";

                        DataTable dtfilter = dtClinicaldata.AsEnumerable()
                           .Where(r => r.Field<string>("Readingtaken") == item["Readingtaken"].ToString() && r.Field<string>("Readingid") == item["Readingid"].ToString())
                                   .AsDataView().ToTable();

                        foreach (DataRow item1 in dtfilter.Rows)
                        {
                            newrow["encounterid"] = item1["Sourceid"].ToString();
                            newrow["codeset"] = item1["codeset"].ToString();
                            if (item1["clinicalelementid"].ToString() == "VITALS.BLOODPRESSURE.TYPE")
                            {
                                if (item1["value"].ToString() == "sitting")
                                {
                                    strbpcolumn = "bp_sitting";
                                    strbploccolumn = "bp_sitting_location";
                                }
                                else if (item1["value"].ToString() == "standing")
                                {
                                    strbpcolumn = "bp_standing";
                                    strbploccolumn = "bp_standing_location";
                                }
                                else if (item1["value"].ToString() == "supine")
                                {
                                    strbpcolumn = "bp_supine";
                                    strbploccolumn = "bp_supine_location";
                                }
                                else
                                {
                                    strbpcolumn = "bp_sitting";
                                    strbploccolumn = "bp_sitting_location";
                                }
                            }
                            else if (item1["clinicalelementid"].ToString() == "VITALS.BLOODPRESSURE.DIASTOLIC")
                            {

                                strbpd = item1["value"].ToString();
                            }
                            else if (item1["clinicalelementid"].ToString() == "VITALS.BLOODPRESSURE.SYSTOLIC")
                            {
                                if (strbpd != "")
                                {
                                    newrow[strbpcolumn] = item1["value"].ToString() + "/" + strbpd;
                                    strbpd = string.Empty;
                                }
                                else
                                {
                                    newrow[strbpcolumn] = "";
                                    newrow["original_" + strbpcolumn] = item1["value"].ToString();
                                }
                            }
                            else if (item1["clinicalelementid"].ToString() == "VITALS.BLOODPRESSURE.SITE")
                            {
                                newrow[strbploccolumn] = item1["value"].ToString();
                            }
                            else if (item1["clinicalelementid"].ToString() == "VITALS.WEIGHT")
                            {
                                newrow["original_WEIGHT"] = item1["value"].ToString();
                                try
                                {
                                    newrow["WEIGHT"] = item1["value"].ToString(); // objVitalCleanup.ConvertWeight(item1["value"].ToString(), WeightFormat.Gram, WeightFormat.BlackBox);
                                }
                                catch (Exception)
                                {
                                    throw; // dtValidation.Rows.Add(new object[] { parameters["patientid"].ToString(), "", "", "", "", strModuleName, string.Empty, "Invalid WEIGHT: " + item1["value"].ToString() });
                                }
                            }
                            else if (item1["clinicalelementid"].ToString() == "VITALS.HEIGHT")
                            {
                                newrow["original_HEIGHT"] = item1["value"].ToString();
                                try
                                {
                                    newrow["HEIGHT"] = item1["value"].ToString(); // objVitalCleanup.ConvertHeight(item1["value"].ToString(), HeightFormat.Centimeter, HeightFormat.BlackBox);
                                }
                                catch (Exception)
                                {
                                    throw; //dtValidation.Rows.Add(new object[] { parameters["patientid"].ToString(), "", "", "", "", strModuleName, string.Empty, "Invalid HEIGHT: " + item1["value"].ToString() });
                                }
                            }
                            else if (item1["clinicalelementid"].ToString() == "VITALS.BMI")
                            {
                                newrow["BMI"] = item1["value"].ToString();
                            }
                            else if (item1["clinicalelementid"].ToString() == "VITALS.PULSE.RATE")
                            {
                                newrow["PULSE"] = item1["value"].ToString();
                            }
                            else if (item1["clinicalelementid"].ToString() == "VITALS.TEMPERATURE")
                            {
                                newrow["TEMPERATURE"] = item1["value"].ToString();
                                newrow["origianl_TEMPERATURE"] = item1["value"].ToString();
                            }
                            else if (item1["clinicalelementid"].ToString() == "VITALS.HEADCIRCUMFERENCE")
                            {
                                newrow["headcm"] = item1["value"].ToString();
                            }
                            else if (item1["clinicalelementid"].ToString() == "VITALS.WAISTCIRCUMFERENCE")
                            {
                                newrow["waistcircumference"] = item1["value"].ToString();
                            }
                            else if (item1["clinicalelementid"].ToString() == "VITALS.NECKCIRCUMFERENCE")
                            {
                                newrow["neckcircumference"] = item1["value"].ToString();
                            }
                            else if (item1["clinicalelementid"].ToString() == "VITALS.PAINSCALE")
                            {
                                newrow["painlevel"] = item1["value"].ToString();
                            }
                            else
                            {
                                strvitalname = string.Concat(strvitalname, item1["clinicalelementid"].ToString(), ":", item1["value"].ToString());
                            }
                            strcode = string.Concat(strcode, item1["code"].ToString(), ",");
                        }
                        if (dtfilter.Rows.Count > 0)
                        {
                            newrow["code"] = strcode != "" ? newrow["codeset"] + ":" + strcode.TrimEnd(',') : "";
                            newrow["comments"] = strvitalname;
                            dtvitalclinical.Rows.Add(newrow);
                            strvitalname = string.Empty;
                            strcode = string.Empty;
                        }
                    }
                }
                //}

                dtvitalclinical = dtvitalclinical.DefaultView.ToTable(true);
                dtClinicaldata = dtvitalclinical.Copy();
                dtClinicaldata.TableName = strModuleName;
                //dsModuleData.Tables.Add(dtValidation);
                //dtModuleData = dtCliicaldata;

                return dtClinicaldata;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DataTable createvitaltable()
        {
            DataTable dtvitalclinical = new DataTable();

            dtvitalclinical.Columns.Add("Patientid");
            dtvitalclinical.Columns.Add("daterecorded");
            dtvitalclinical.Columns.Add("readingid");
            dtvitalclinical.Columns.Add("encounterid");
            dtvitalclinical.Columns.Add("bp_sitting");
            dtvitalclinical.Columns.Add("bp_standing");
            dtvitalclinical.Columns.Add("bp_supine");
            dtvitalclinical.Columns.Add("temperature");
            dtvitalclinical.Columns.Add("heartrate");
            dtvitalclinical.Columns.Add("weight");
            dtvitalclinical.Columns.Add("height");
            dtvitalclinical.Columns.Add("spo2");
            dtvitalclinical.Columns.Add("resp");
            dtvitalclinical.Columns.Add("bmi");
            dtvitalclinical.Columns.Add("headcm");
            dtvitalclinical.Columns.Add("timerecorded");
            dtvitalclinical.Columns.Add("neckcircumference");
            dtvitalclinical.Columns.Add("waistcircumference");
            dtvitalclinical.Columns.Add("lmp");
            dtvitalclinical.Columns.Add("painlevel");
            dtvitalclinical.Columns.Add("pulse");
            dtvitalclinical.Columns.Add("bp_standing_location");
            dtvitalclinical.Columns.Add("bp_sitting_location");
            dtvitalclinical.Columns.Add("bp_supine_location");
            dtvitalclinical.Columns.Add("original_bp_sitting");
            dtvitalclinical.Columns.Add("original_bp_standing");
            dtvitalclinical.Columns.Add("original_bp_supine");
            dtvitalclinical.Columns.Add("original_temperature");
            dtvitalclinical.Columns.Add("original_heartrate");
            dtvitalclinical.Columns.Add("original_weight");
            dtvitalclinical.Columns.Add("original_height");
            dtvitalclinical.Columns.Add("original_resp");
            dtvitalclinical.Columns.Add("original_headcm");
            dtvitalclinical.Columns.Add("origianl_temperature");
            dtvitalclinical.Columns.Add("comments");
            dtvitalclinical.Columns.Add("xmlcomments");
            dtvitalclinical.Columns.Add("vitalname");
            dtvitalclinical.Columns.Add("vitalvalue");
            dtvitalclinical.Columns.Add("codeset");
            dtvitalclinical.Columns.Add("code");
            return dtvitalclinical;
        }

        internal class vital
        {

            [JsonProperty("ordering")]
            public string Ordering { get; set; }

            [JsonProperty("abbreviation")]
            public string Abbreviation { get; set; }

            [JsonProperty("readings")]
            public readings[][] readings { get; set; }

            [JsonProperty("key")]
            public string Key { get; set; }

            [JsonProperty("vitalname")]
            public string Vitalname { get; set; }

            [JsonProperty("vitalvalue")]
            public string Vitalvalue { get; set; }
        }

        internal class readings
        {

            [JsonProperty("source")]
            public string Source { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("readingid")]
            public string Readingid { get; set; }

            [JsonProperty("clinicalelementid")]
            public string Clinicalelementid { get; set; }

            [JsonProperty("codedescription")]
            public string Codedescription { get; set; }

            [JsonProperty("sourceid")]
            public string Sourceid { get; set; }

            [JsonProperty("readingtaken")]
            public string Readingtaken { get; set; }

            [JsonProperty("codeset")]
            public string Codeset { get; set; }

            [JsonProperty("vitalid")]
            public string Vitalid { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("encounterid")]
            public string Encounterid { get; set; }


        }
    }
}
