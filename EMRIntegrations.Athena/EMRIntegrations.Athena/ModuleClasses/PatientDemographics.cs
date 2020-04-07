using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EMRIntegrations.Athena
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

                APIConnection api = (APIConnection)parameters["athenaapiobject"];
                JToken jtobj = (JToken)api.GET("patients" + "/" + parameters["patientid"]);
                if (jtobj != null)
                {
                    if (jtobj.HasValues && jtobj.SelectToken("error") != null)
                    {
                        return new DataTable();
                        //dtValidation.Rows.Add(new object[] { parameters["patientid"].ToString(), "", "", "", "", strModuleName, string.Empty, jtobj["error"].ToString() + parameters["patientid"].ToString() });
                        //{ throw new Exception(jtobj["error"].ToString()); }
                    }

                    List<Patients> Patientdata = jtobj.ToObject<List<Patients>>();

                    dtClinicaldata = GetModuleDataBase.ConvertToDataTable(Patientdata);
                    if (dtClinicaldata != null && dtClinicaldata.Rows.Count > 0)
                    {
                        dtClinicaldata.Columns["status"].ColumnName = "pstatus";
                        dtClinicaldata.Columns["lastname"].ColumnName = "plname";
                        dtClinicaldata.Columns["firstname"].ColumnName = "pfname";
                        dtClinicaldata.Columns["middlename"].ColumnName = "pminit";
                        dtClinicaldata.Columns["email"].ColumnName = "pemail";
                        dtClinicaldata.Columns["address1"].ColumnName = "paddr1";
                        dtClinicaldata.Columns["address2"].ColumnName = "paddr2";
                        dtClinicaldata.Columns["city"].ColumnName = "pcity";
                        dtClinicaldata.Columns["state"].ColumnName = "pstate";
                        dtClinicaldata.Columns["zip"].ColumnName = "pzip";
                        dtClinicaldata.Columns["sex"].ColumnName = "psex";
                        dtClinicaldata.Columns["homephone"].ColumnName = "phomephn";
                        dtClinicaldata.Columns["mobilephone"].ColumnName = "pcellphone";
                        dtClinicaldata.Columns["ssn"].ColumnName = "pssn";
                        dtClinicaldata.Columns["dob"].ColumnName = "pdob";
                        dtClinicaldata.Columns["primaryproviderid"].ColumnName = "dcode";
                        dtClinicaldata.Columns["maritalstatus"].ColumnName = "pmarital";
                        dtClinicaldata.Columns["racename"].ColumnName = "prace";
                        dtClinicaldata.Columns["suffix"].ColumnName = "psuffix";
                        dtClinicaldata.Columns["prefix"].ColumnName = "pprefix";
                        dtClinicaldata.Columns["Lastappointment"].ColumnName = "lastvisit";
                        dtClinicaldata.Columns["firstappointment"].ColumnName = "firstvisit";
                        dtClinicaldata.Columns["language6392code"].ColumnName = "language";

                        dtClinicaldata.Columns["contactrelationship"].ColumnName = "emgcontactrelation";
                        dtClinicaldata.Columns["Contactname"].ColumnName = "emgname";
                        dtClinicaldata.Columns["contacthomephone"].ColumnName = "emgphone";

                        dtClinicaldata.Columns["Employername"].ColumnName = "ename";
                        dtClinicaldata.Columns["employeraddress"].ColumnName = "eaddr1";
                        dtClinicaldata.Columns["employeraddress2"].ColumnName = "eaddr2";
                        dtClinicaldata.Columns["employerphone"].ColumnName = "ephone";
                        dtClinicaldata.Columns["employercity"].ColumnName = "ecity";
                        dtClinicaldata.Columns["employerstate"].ColumnName = "estate";
                        dtClinicaldata.Columns["employerzip"].ColumnName = "ezip";
                        dtClinicaldata.Columns["employerfax"].ColumnName = "efax";

                        dtClinicaldata.Columns["guarantorlastname"].ColumnName = "glname";
                        dtClinicaldata.Columns["guarantorfirstname"].ColumnName = "gfname";
                        dtClinicaldata.Columns["guarantormiddlename"].ColumnName = "gminit";
                        dtClinicaldata.Columns["guarantoremail"].ColumnName = "gemail";
                        dtClinicaldata.Columns["guarantoraddress1"].ColumnName = "gaddr1";
                        dtClinicaldata.Columns["guarantoraddress2"].ColumnName = "gaddr2";
                        dtClinicaldata.Columns["guarantorcity"].ColumnName = "gcity";
                        dtClinicaldata.Columns["guarantorstate"].ColumnName = "gstate";
                        dtClinicaldata.Columns["guarantorzip"].ColumnName = "gzip";
                        dtClinicaldata.Columns["guarantorsex"].ColumnName = "gsex";
                        dtClinicaldata.Columns["guarantorphone"].ColumnName = "ghomephn";
                        dtClinicaldata.Columns["guarantorssn"].ColumnName = "gssn";
                        dtClinicaldata.Columns["guarantordob"].ColumnName = "gdob";
                        dtClinicaldata.Columns["guarantorracename"].ColumnName = "grace";
                        dtClinicaldata.Columns["guarantorsuffix"].ColumnName = "gsuffix";
                        dtClinicaldata.Columns["guarantorprefix"].ColumnName = "gprefix";
                    }

                    string dobdate = string.Empty;

                    if (!dtClinicaldata.Columns.Contains("original_pstatus"))
                    {
                        dtClinicaldata.Columns.Add("original_pstatus");
                    }

                    if (!dtClinicaldata.Columns.Contains("pchartno"))
                    {
                        dtClinicaldata.Columns.Add("pchartno");
                    }

                    if (!dtClinicaldata.Columns.Contains("pchartno"))
                    {
                        dtClinicaldata.Columns.Add("pchartno");
                    }

                    if (!dtClinicaldata.Columns.Contains("original_prace"))
                    {
                        dtClinicaldata.Columns.Add("original_prace");
                    }

                    if (!dtClinicaldata.Columns.Contains("original_ethnicity"))
                    {
                        dtClinicaldata.Columns.Add("original_ethnicity");
                    }

                    if (!dtClinicaldata.Columns.Contains("original_pmarital"))
                    {
                        dtClinicaldata.Columns.Add("original_pmarital");
                    }

                    if (!dtClinicaldata.Columns.Contains("original_prela2grn"))
                    {
                        dtClinicaldata.Columns.Add("original_prela2grn");
                    }

                    if (!dtClinicaldata.Columns.Contains("daterecorded"))
                    {
                        dtClinicaldata.Columns.Add("daterecorded");
                    }

                    if (!dtClinicaldata.Columns.Contains("custom3"))
                    {
                        dtClinicaldata.Columns.Add("custom3");
                    }

                    if (!dtClinicaldata.Columns.Contains("custom2"))
                    {
                        dtClinicaldata.Columns.Add("custom2");
                    }

                    if (!dtClinicaldata.Columns.Contains("custom1"))
                    {
                        dtClinicaldata.Columns.Add("custom1");
                    }

                    if (!dtClinicaldata.Columns.Contains("prela2grn"))
                    {
                        dtClinicaldata.Columns.Add("prela2grn");
                    }

                    if (!dtClinicaldata.Columns.Contains("ethnicity"))
                    {
                        dtClinicaldata.Columns.Add("ethnicity");
                    }

                    string strprelatogrn = "";
                    foreach (DataRow dr in dtClinicaldata.Rows)
                    {
                        if (string.IsNullOrEmpty(dr["emgphone"].ToString()))
                        {
                            dr["emgphone"] = dr["contactmobilephone"].ToString();
                        }

                        switch (dr["guarantorrelationshiptopatient"].ToString())
                        {
                            case "1":
                                strprelatogrn = "Self";
                                break;
                            case "2":
                                strprelatogrn = "Spouse";
                                break;
                            case "3":
                                strprelatogrn = "Child";
                                break;
                            case "4":
                                strprelatogrn = "Other";
                                break;
                            case "5":
                                strprelatogrn = "Grandparent";
                                break;
                            case "6":
                                strprelatogrn = "Grandchild";
                                break;
                            case "7":
                                strprelatogrn = "Nephew or Niece";
                                break;
                            case "9":
                                strprelatogrn = "Foster Child";
                                break;
                            case "10":
                                strprelatogrn = "Ward";
                                break;
                            case "11":
                                strprelatogrn = "Stepson or Stepdaughter";
                                break;
                            case "12":
                                strprelatogrn = "Employee";
                                break;
                            case "13":
                                strprelatogrn = "Unknown";
                                break;
                            case "14":
                                strprelatogrn = "Handicapped Dependent";
                                break;
                            case "15":
                                strprelatogrn = "Sponsored Dependent";
                                break;
                            case "16":
                                strprelatogrn = "Dependent of a Minor Dependent";
                                break;
                            case "17":
                                strprelatogrn = "Significant Other";
                                break;
                            case "18":
                                strprelatogrn = "Mother";
                                break;
                            case "19":
                                strprelatogrn = "Father";
                                break;
                            case "21":
                                strprelatogrn = "Emancipated Minor";
                                break;
                            case "22":
                                strprelatogrn = "Organ Donor";
                                break;
                            case "23":
                                strprelatogrn = "Cadaver Donor";
                                break;
                            case "24":
                                strprelatogrn = "Injured Plaintiff";
                                break;
                            case "25":
                                strprelatogrn = "Child(Ins.not Financially Respons.)";
                                break;
                            case "26":
                                strprelatogrn = "Life Partner";
                                break;
                            case "27":
                                strprelatogrn = "Child(Mother's Insurance)";
                                break;
                            case "28":
                                strprelatogrn = "Child(Father's Insurance)";
                                break;
                            case "29":
                                strprelatogrn = "Child(of Mother, Ins.not Financially Respons.)";
                                break;
                            case "30":
                                strprelatogrn = "Child(of Father, Ins.not Financially Respons.)";
                                break;
                            case "31":
                                strprelatogrn = "Stepson or Stepdaughter(Stepmother's Insurance)";
                                break;
                            case "32":
                                strprelatogrn = "Stepson or Stepdaughter(Stepfather's Insurance)";
                                break;
                        }

                        dr["prela2grn"] = strprelatogrn;
                        dr["original_prela2grn"] = dr["prela2grn"];

                        dr["daterecorded"] = DateTime.Now.ToString(); // objCommonCleanup.ParseDateFromString(DateTime.Now.ToString(), "yyyyMMdd");
                        dr["lastvisit"] = dr["lastvisit"].ToString(); // objCommonCleanup.ParseDateFromString(dr["lastvisit"].ToString() , "yyyyMMdd");
                        dr["firstvisit"] = dr["firstvisit"].ToString(); // objCommonCleanup.ParseDateFromString(dr["firstvisit"].ToString(), "yyyyMMdd");

                        if (string.IsNullOrEmpty(dr["paddr1"].ToString()) && !string.IsNullOrEmpty(dr["paddr2"].ToString()))
                        {
                            dr["paddr1"] = dr["paddr2"];
                            dr["paddr2"] = string.Empty;
                        }

                        if (string.IsNullOrEmpty(dr["gaddr1"].ToString()) && !string.IsNullOrEmpty(dr["gaddr2"].ToString()))
                        {
                            dr["gaddr1"] = dr["gaddr2"];
                            dr["gaddr2"] = string.Empty;
                        }

                        dr["original_pstatus"] = dr["pstatus"];
                        dr["original_prace"] = dr["prace"];
                        dr["original_pmarital"] = dr["pmarital"];

                        dr["patientid"] = parameters["patientid"].ToString();  //need to discuss
                        dr["pchartno"] = dr["patientid"];

                        if (dr["pstatus"].ToString().ToLower() == "active")
                        {
                            dr["pstatus"] = "A";
                        }
                        else if (dr["pstatus"].ToString().ToLower() == "inactive")
                        {
                            dr["pstatus"] = "I";
                        }
                        else if (dr["pstatus"].ToString().ToLower() == "prospective")
                        {
                            dr["pstatus"] = "A";
                        }

                        //if (!string.IsNullOrEmpty(dr["pdob"].ToString()))
                        //{
                        //    dobdate = dr["pdob"].ToString(); // objCommonCleanup.ParseDateFromString(dr["pdob"].ToString(), "yyyyMMdd");

                        //    if (string.IsNullOrEmpty(dobdate))
                        //    {
                        //        dr["comments"] = "Patient DOB: " + dr["pdob"].ToString();
                        //        dr["pdob"] = string.Empty;
                        //    }
                        //    else
                        //    {
                        //        //DateTime.Compare()
                        //        var date = new DateTime(Convert.ToInt32(dobdate.Substring(0, 4)), Convert.ToInt32(dobdate.Substring(4, 2)), Convert.ToInt32(dobdate.Substring(6, 2)), 0, 0, 0);

                        //        if (date > DateTime.Now)
                        //        {
                        //            dr["pdob"] = string.Empty;
                        //        }
                        //        else
                        //        {
                        //            dr["pdob"] = dobdate;
                        //        }
                        //    }
                        //}

                        //if (!string.IsNullOrEmpty(dr["gdob"].ToString()))
                        //{
                        //    dobdate = dr["gdob"].ToString(); // objCommonCleanup.ParseDateFromString(dr["gdob"].ToString(), "yyyyMMdd");

                        //    if (string.IsNullOrEmpty(dobdate))
                        //    {
                        //        dr["comments"] = "Guarantor DOB: " + dr["gdob"].ToString();
                        //        dr["gdob"] = string.Empty;
                        //    }
                        //    else
                        //    {
                        //        var date = new DateTime(Convert.ToInt32(dobdate.Substring(0, 4)), Convert.ToInt32(dobdate.Substring(4, 2)), Convert.ToInt32(dobdate.Substring(6, 2)), 0, 0, 0);
                        //        if (date > DateTime.Now)
                        //        {
                        //            dr["gdob"] = string.Empty;
                        //        }
                        //        else
                        //        {
                        //            dr["gdob"] = dobdate;
                        //        }
                        //    }
                        //}

                        string strethnicity = string.Empty;
                        if (dr["ethnicitycode"].ToString() == "2135-2")
                        {
                            strethnicity = @"Hispanic or Latino\/Spanish";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2137 -8")
                        {
                            strethnicity = "Spaniard";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2138 -6")
                        {
                            strethnicity = "Andalusian";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2139 -4")
                        {
                            strethnicity = "Asturian";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2140 -2")
                        {
                            strethnicity = "Castillian";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2141 -0")
                        {
                            strethnicity = "Catalonian";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2142 -8")
                        {
                            strethnicity = "Belearic Islander";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2143 -6")
                        {
                            strethnicity = "Gallego";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2144 -4")
                        {
                            strethnicity = "Valencian";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2145 -1")
                        {
                            strethnicity = "Canarian";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2146 -9")
                        {
                            strethnicity = "Spanish Basque";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2148 -5")
                        {
                            strethnicity = "Mexican";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2149 -3")
                        {
                            strethnicity = "Mexican American";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2150 -1")
                        {
                            strethnicity = "Mexicano";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2151 -9")
                        {
                            strethnicity = "Chicano";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2152 -7")
                        {
                            strethnicity = "La Raza";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2153 -5")
                        {
                            strethnicity = "Mexican American Indian";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2155 -0")
                        {
                            strethnicity = "Central American";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2156-8")
                        {
                            strethnicity = "Costa Rican";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2157-6")
                        {
                            strethnicity = "Guatemalan";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2158-4")
                        {
                            strethnicity = "Honduran";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2159-2")
                        {
                            strethnicity = "Nicaraguan";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2160-0")
                        {
                            strethnicity = "Panamanian";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2161-8")
                        {
                            strethnicity = "Salvadoran";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2162-6")
                        {
                            strethnicity = "Central American Indian";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2163-4")
                        {
                            strethnicity = "Canal Zone";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2165-9")
                        {
                            strethnicity = "South American";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2166-7")
                        {
                            strethnicity = "Argentinean";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2167-5")
                        {
                            strethnicity = "Bolivian";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2168-3")
                        {
                            strethnicity = "Chilean";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2169-1")
                        {
                            strethnicity = "Colombian";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2170-9")
                        {
                            strethnicity = "Ecuadorian";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2171-7")
                        {
                            strethnicity = "Paraguayan";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2172-5")
                        {
                            strethnicity = "Peruvian";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2173-3")
                        {
                            strethnicity = "Uruguayan";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2174-1")
                        {
                            strethnicity = "Venezuelan";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2175-8")
                        {
                            strethnicity = "South American Indian";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2176-6")
                        {
                            strethnicity = "Criollo";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2178-2")
                        {
                            strethnicity = @"Latin American\/Latin) Latino";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2180-8")
                        {
                            strethnicity = "Puerto Rican";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2182-4")
                        {
                            strethnicity = "Cuban";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2184-0")
                        {
                            strethnicity = "Dominican";
                        }
                        else if (dr["ethnicitycode"].ToString() == "2186-5")
                        {
                            strethnicity = "Not Hispanic or Latino";
                        }
                        else
                        {
                            strethnicity = dr["ethnicitycode"].ToString();
                        }

                        if (strethnicity.Length > 0)
                        {
                            dr["original_ethnicity"] = strethnicity;
                        }
                        else
                        {
                            dr["original_ethnicity"] = dr["ethnicitycode"].ToString();
                        }

                        dr["ethnicity"] = strethnicity;
                    }
                }
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

        private class Patients
        {
            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("homephone")]
            public string Homephone { get; set; }

            [JsonProperty("guarantorstate")]
            public string guarantorstate { get; set; }

            [JsonProperty("guarantordob")]
            public string Guarantordob { get; set; }

            [JsonProperty("guarantorfirstname")]
            public string Guarantorfirstname { get; set; }

            [JsonProperty("guarantormiddlename")]
            public string Guarantormiddlename { get; set; }

            [JsonProperty("guarantoremail")]
            public string Guarantoremail { get; set; }

            [JsonProperty("driverslicense")]
            public string Driverslicense { get; set; }

            [JsonProperty("occupationcode")]
            public string Occupationcode { get; set; }

            [JsonProperty("departmentid")]
            public string Departmentid { get; set; }

            [JsonProperty("industrycode")]
            public string Industrycode { get; set; }

            [JsonProperty("contacthomephone")]
            public string Contacthomephone { get; set; }

            [JsonProperty("guarantorlastname")]
            public string Guarantorlastname { get; set; }

            [JsonProperty("guarantorcountrycode")]
            public string Guarantorcountrycode { get; set; }

            [JsonProperty("guarantorssn")]
            public string Guarantorssn { get; set; }

            [JsonProperty("guarantorcity")]
            public string Guarantorcity { get; set; }

            [JsonProperty("guarantorzip")]
            public string Guarantorzip { get; set; }

            [JsonProperty("guarantorsex")]
            public string Guarantorsex { get; set; }

            [JsonProperty("sex")]
            public string Sex { get; set; }

            [JsonProperty("ethnicitycode")]
            public string Ethnicitycode { get; set; }

            [JsonProperty("primaryproviderid")]
            public string Primaryproviderid { get; set; }//pcp

            [JsonProperty("zip")]
            public string Zip { get; set; }

            [JsonProperty("guarantoraddresssameaspatient")]
            public string Guarantoraddresssameaspatient { get; set; }

            [JsonProperty("guarantoraddress1")]
            public string Guarantoraddress1 { get; set; }

            [JsonProperty("guarantoraddress2")]
            public string Guarantoraddress2 { get; set; }

            [JsonProperty("guarantorcountrycode3166")]
            public string Guarantorcountrycode3166 { get; set; }

            [JsonProperty("employerphone")]
            public string Employerphone { get; set; }

            [JsonProperty("employername")]
            public string Employername { get; set; }

            [JsonProperty("employercity")]
            public string Employercity { get; set; }

            [JsonProperty("employerstate")]
            public string Employerstate { get; set; }

            [JsonProperty("employerzip")]
            public string Employerzip { get; set; }

            [JsonProperty("employerfax")]
            public string Employerfax { get; set; }

            [JsonProperty("employeraddress")]
            public string Employeraddress { get; set; }

            [JsonProperty("employeraddress2")]
            public string Employeraddress2 { get; set; }

            [JsonProperty("contactmobilephone")]
            public string Contactmobilephone { get; set; }

            [JsonProperty("mobilephone")]
            public string Mobilephone { get; set; }

            [JsonProperty("nextkinphone")]
            public string Nextkinphone { get; set; }

            [JsonProperty("portaltermsonfile")]
            public string Portaltermsonfile { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("lastname")]
            public string Lastname { get; set; }

            [JsonProperty("city")]
            public string City { get; set; }

            [JsonProperty("middlename")]
            public string Middlename { get; set; }

            [JsonProperty("ssn")]
            public string Ssn { get; set; }

            [JsonProperty("privacyinformationverified")]
            public string Privacyinformationverified { get; set; }

            [JsonProperty("primarydepartmentid")]
            public string Primarydepartmentid { get; set; }

            //   [JsonProperty("balances")]
            //        public Balance[] Balances { get; set; }

            [JsonProperty("emailexists")]
            public string Emailexists { get; set; }

            [JsonProperty("racename")]
            public string Racename { get; set; }

            [JsonProperty("language6392code")]
            public string Language6392code { get; set; }

            [JsonProperty("patientphoto")]
            public string Patientphoto { get; set; }

            [JsonProperty("caresummarydeliverypreference")]
            public string Caresummarydeliverypreference { get; set; }

            [JsonProperty("firstname")]
            public string Firstname { get; set; }

            [JsonProperty("state")]
            public string State { get; set; }

            [JsonProperty("patientid")]
            public string Patientid { get; set; }

            [JsonProperty("dob")]
            public string Dob { get; set; }

            [JsonProperty("guarantorrelationshiptopatient")]
            public string Guarantorrelationshiptopatient { get; set; }

            [JsonProperty("address1")]
            public string Address1 { get; set; }

            [JsonProperty("address2")]
            public string Address2 { get; set; }

            [JsonProperty("guarantorphone")]
            public string Guarantorphone { get; set; }

            [JsonProperty("guarantorracename")]
            public string Guarantorracename { get; set; }

            [JsonProperty("countrycode")]
            public string Countrycode { get; set; }

            [JsonProperty("consenttotext")]
            public string Consenttotext { get; set; }

            [JsonProperty("countrycode3166")]
            public string Countrycode3166 { get; set; }

            [JsonProperty("maritalstatus")]
            public string Maritalstatus { get; set; }

            [JsonProperty("notes")]
            public string Notes { get; set; }

            [JsonProperty("ethnicityid")]
            public string Ethnicityid { get; set; }

            [JsonProperty("Raceid")]
            public string Raceid { get; set; }

            [JsonProperty("comments")]
            public string Comments { get; set; }

            [JsonProperty("contactpreference")]
            public string Contactpreference { get; set; }

            [JsonProperty("guarantorcontactpreference")]
            public string GuarantorContactpreference { get; set; }

            [JsonProperty("suffix")]
            public string Suffix { get; set; }

            [JsonProperty("prefix")]
            public string Prefix { get; set; }

            [JsonProperty("guarantorsuffix")]
            public string Guarantorsuffix { get; set; }

            [JsonProperty("guarantorprefix")]
            public string Guarantorprefix { get; set; }

            [JsonProperty("lastappointment")]
            public string Lastappointment { get; set; }

            [JsonProperty("firstappointment")]
            public string Firstappointment { get; set; }

            [JsonProperty("patientphotourl")]
            public string Patientphotourl { get; set; }

            [JsonProperty("Portalaccessgiven")]
            public string Portalaccessgiven { get; set; }

            [JsonProperty("contactpreference_appointment_email")]
            public string Contactpreference_appointment_email { get; set; }

            [JsonProperty("homebound")]
            public string Homebound { get; set; }

            [JsonProperty("contactpreference_appointment_sms")]
            public string Contactpreference_appointment_sms { get; set; }

            [JsonProperty("contactpreference_billing_phone")]
            public string Contactpreference_billing_phone { get; set; }

            [JsonProperty("contactpreference_announcement_phone")]
            public string Contactpreference_announcement_phone { get; set; }

            [JsonProperty("employerid")]
            public string Employerid { get; set; }

            [JsonProperty("contactpreference_lab_sms")]
            public string Contactpreference_lab_sms { get; set; }

            [JsonProperty("contactpreference_lab_email")]
            public string Contactpreference_lab_email { get; set; }

            [JsonProperty("contactpreference_announcement_sms")]
            public string Contactpreference_announcement_sms { get; set; }

            [JsonProperty("consenttocall")]
            public string Consenttocall { get; set; }

            [JsonProperty("contactpreference_billing_email")]
            public string Contactpreference_billing_email { get; set; }

            [JsonProperty("contactpreference_announcement_email")]
            public string Contactpreference_announcement_email { get; set; }

            [JsonProperty("contactpreference_appointment_phone")]
            public string Contactpreference_appointment_phone { get; set; }

            [JsonProperty("contactpreference_billing_sms")]
            public string Contactpreference_billing_sms { get; set; }

            [JsonProperty("maritalstatusname")]
            public string Maritalstatusname { get; set; }

            [JsonProperty("onlinestatementonly")]
            public string Onlinestatementonly { get; set; }

            [JsonProperty("contactpreference_lab_phone")]
            public string Contactpreference_lab_phone { get; set; }

            [JsonProperty("contactrelationship")]
            public string Contactrelationship { get; set; }

            [JsonProperty("contactname")]
            public string Contactname { get; set; }
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
                    if (dColumnPatDemo.ColumnName == "pprefix")
                    {
                        dColumnPatDemo.ColumnName = "Title";
                    }
                    else if (dColumnPatDemo.ColumnName == "psuffix")
                    {
                        dColumnPatDemo.ColumnName = "Suffix";
                    }
                    else if (dColumnPatDemo.ColumnName == "pfname")
                    {
                        dColumnPatDemo.ColumnName = "FirstName";
                    }
                    else if (dColumnPatDemo.ColumnName == "pminit")
                    {
                        dColumnPatDemo.ColumnName = "MiddleName";
                    }
                    else if (dColumnPatDemo.ColumnName == "plname")
                    {
                        dColumnPatDemo.ColumnName = "LastName";
                    }
                    else if (dColumnPatDemo.ColumnName == "pemail")
                    {
                        dColumnPatDemo.ColumnName = "Email";
                    }
                    else if (dColumnPatDemo.ColumnName == "pdob")
                    {
                        dColumnPatDemo.ColumnName = "DateOfBirth";
                    }
                    else if (dColumnPatDemo.ColumnName == "psex")
                    {
                        dColumnPatDemo.ColumnName = "Gender";
                    }
                    else if (dColumnPatDemo.ColumnName == "phomephn")
                    {
                        dColumnPatDemo.ColumnName = "Phone";
                    }
                    else if (dColumnPatDemo.ColumnName == "ethnicity")
                    {
                        dColumnPatDemo.ColumnName = "EthnicityCode";
                    }
                    else if (dColumnPatDemo.ColumnName == "prace")
                    {
                        dColumnPatDemo.ColumnName = "RaceCode";
                    }
                    else if (dColumnPatDemo.ColumnName == "paddr1")
                    {
                        dColumnPatDemo.ColumnName = "Address1";
                    }
                    else if (dColumnPatDemo.ColumnName == "paddr2")
                    {
                        dColumnPatDemo.ColumnName = "Address2";
                    }
                    else if (dColumnPatDemo.ColumnName == "pcity")
                    {
                        dColumnPatDemo.ColumnName = "City";
                    }
                    else if (dColumnPatDemo.ColumnName == "pstate")
                    {
                        dColumnPatDemo.ColumnName = "State";
                    }
                    else if (dColumnPatDemo.ColumnName == "pzip")
                    {
                        dColumnPatDemo.ColumnName = "ZipCode";
                    }
                    else if (dColumnPatDemo.ColumnName == "pssn")
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
                JSONString.Append("\"Title\":" + "\"" + drow["Title"].ToString() + "\",");
                JSONString.Append("\"FirstName\":" + "\"" + drow["FirstName"].ToString() + "\",");
                JSONString.Append("\"MiddleName\":" + "\"" + drow["MiddleName"].ToString() + "\",");
                JSONString.Append("\"LastName\":" + "\"" + drow["LastName"].ToString() + "\",");
                JSONString.Append("\"Suffix\":" + "\"" + drow["Suffix"].ToString() + "\",");
                JSONString.Append("\"Gender\":" + "\"" + drow["Gender"].ToString() + "\",");
                JSONString.Append("\"DateOfBirth\":" + "\"" + drow["DateOfBirth"].ToString() + "\",");
                JSONString.Append("\"Email\":" + "\"" + drow["Email"].ToString() + "\",");
                JSONString.Append("\"Street\":" + "\"\",");
                JSONString.Append("\"City\":" + "\"" + drow["City"].ToString() + "\",");
                JSONString.Append("\"ZipCode\":" + "\"" + drow["ZipCode"].ToString() + "\",");
                JSONString.Append("\"Phone\":" + "\"" + drow["Phone"].ToString() + "\",");
                JSONString.Append("\"Address1\":" + "\"" + drow["Address1"].ToString() + "\",");
                JSONString.Append("\"Address2\":" + "\"" + drow["Address2"].ToString() + "\",");
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