using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EMRIntegrations.Athena
{
    public class PatientInsurance
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
                dtModuleData = GetPatientInsuranceData(parameters);
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
        /// <param name="parameters">number of parameters which are have version,modulename etc...</param>
        /// <returns>Return the datatable with module data</returns>
        private DataTable GetPatientInsuranceData(Hashtable parameters)
        {
            try
            {
                string strModuleName = "PatientInsurance";
                DataTable dtClinicaldata = new DataTable();

                APIConnection api = (APIConnection)parameters["athenaapiobject"];

                JObject jobj = (JObject)api.GET("patients" + "/" + parameters["patientid"].ToString() + "/" + "insurances");
                JToken jtobj = jobj["insurances"];
                if (jtobj != null)
                {
                    if (jtobj.HasValues && jtobj.SelectToken("error") != null)
                    {
                        return new DataTable();
                        //dtValidation.Rows.Add(new object[] { parameters["patientid"].ToString(), "", "", "", "", strModuleName, string.Empty, jtobj["error"].ToString() + parameters["patientid"].ToString() });
                        //{ throw new Exception(jtobj["error"].ToString()); }

                    }
                    // Do data processing here.
                    List<patientinsurance> objclinical = jtobj.ToObject<List<patientinsurance>>();

                    dtClinicaldata = GetModuleDataBase.ConvertToDataTable(objclinical);
                    if (dtClinicaldata != null && dtClinicaldata.Rows.Count > 0)
                    {
                        dtClinicaldata.Columns["sequencenumber"].ColumnName = "itype";
                        dtClinicaldata.Columns["insurancepackageid"].ColumnName = "icode";
                        dtClinicaldata.Columns["insuranceplanname"].ColumnName = "iname";
                        dtClinicaldata.Columns["insurancephone"].ColumnName = "iphone";
                        dtClinicaldata.Columns["insuranceidnumber"].ColumnName = "ipolicy";
                        dtClinicaldata.Columns["policynumber"].ColumnName = "igroup";
                        dtClinicaldata.Columns["relationshiptoinsured"].ColumnName = "irela2hld";
                        dtClinicaldata.Columns["insurancestate"].ColumnName = "istate";
                        dtClinicaldata.Columns["insurancezip"].ColumnName = "izip";
                        dtClinicaldata.Columns["insurancecity"].ColumnName = "icity";
                        dtClinicaldata.Columns["insuranceaddress"].ColumnName = "iaddra";
                        dtClinicaldata.Columns["insuranceaddress2"].ColumnName = "iaddrb";
                        dtClinicaldata.Columns["issuedate"].ColumnName = "ibegin";
                        dtClinicaldata.Columns["expirationdate"].ColumnName = "iend";

                        dtClinicaldata.Columns["insurancepolicyholder"].ColumnName = "ihname";
                        dtClinicaldata.Columns["insurancepolicyholderlastname"].ColumnName = "ihlname";
                        dtClinicaldata.Columns["insurancepolicyholderfirstname"].ColumnName = "ihfname";
                        dtClinicaldata.Columns["insurancepolicyholdermiddlename"].ColumnName = "ihminit";
                        dtClinicaldata.Columns["insurancepolicyholderssn"].ColumnName = "ihssn";
                        dtClinicaldata.Columns["insurancepolicyholdersex"].ColumnName = "ihsex";
                        dtClinicaldata.Columns["insurancepolicyholdercity"].ColumnName = "ihcity";
                        dtClinicaldata.Columns["insurancepolicyholderstate"].ColumnName = "ihstate";
                        dtClinicaldata.Columns["insurancepolicyholderzip"].ColumnName = "ihzip";
                        dtClinicaldata.Columns["insurancepolicyholderdob"].ColumnName = "ihdob";
                        dtClinicaldata.Columns["insurancepolicyholderaddress1"].ColumnName = "ihaddra";
                        dtClinicaldata.Columns["insurancepolicyholderaddress2"].ColumnName = "ihaddrb";

                    }

                    DataTable dtPatientInsurance = new DataTable();
                    dtPatientInsurance = PreparePatientInsuranceTable(dtPatientInsurance);
                    DataTable dtReturn = dtPatientInsurance.Clone();

                    if (dtClinicaldata != null && dtClinicaldata.Rows.Count > 0)
                    {
                        if (!dtClinicaldata.Columns.Contains("Patientid"))
                        {
                            dtClinicaldata.Columns.Add("Patientid");
                        }

                        if (!dtClinicaldata.Columns.Contains("Original_irela2hld"))
                        {
                            dtClinicaldata.Columns.Add("Original_irela2hld");
                        }

                        if (!dtClinicaldata.Columns.Contains("Original_iphone"))
                        {
                            dtClinicaldata.Columns.Add("Original_iphone");
                        }

                        DataRow newRow = dtReturn.NewRow();
                        foreach (DataRow dr in dtClinicaldata.Rows)
                        {

                            dr["PatientID"] = parameters["patientid"];
                            newRow["patientid"] = dr["PatientID"];

                            if (dr["itype"].ToString() == "1")
                            {
                                if (dr["ipolicy"].ToString().Trim().Length > 0
                                     || dr["igroup"].ToString().Trim().Length > 0)
                                {
                                    newRow["ibegin1"] = dr["ibegin"].ToString();
                                    newRow["iend1"] = dr["iend"].ToString();
                                    newRow["itype1"] = dr["insurancetype"].ToString();
                                    //'Insurance Holder Information
                                    //newRow["irela2hld1"] = RelationshipMapping(dr["irela2hld"].ToString());
                                    newRow["irela2hld1"] = dr["irela2hld"].ToString();
                                    newRow["Original_irela2hld1"] = dr["irela2hld"].ToString();
                                    newRow["ihlname1"] = dr["ihlname"].ToString();
                                    newRow["ihfname1"] = dr["ihfname"].ToString();
                                    newRow["ihminit1"] = dr["ihminit"].ToString();
                                    newRow["ihaddr1a"] = dr["IHADDRA"].ToString();
                                    newRow["ihaddr1b"] = dr["IHADDRB"].ToString();
                                    newRow["ihcity1"] = dr["ihcity"].ToString();
                                    newRow["ihstate1"] = dr["ihstate"].ToString();
                                    newRow["ihzip1"] = dr["ihzip"].ToString();
                                    newRow["ihsex1"] = dr["ihsex"].ToString();
                                    newRow["ihssn1"] = dr["ihssn"].ToString();

                                    //string strDateOfBirthPrimary = string.Empty;
                                    //strDateOfBirthPrimary = dr["ihdob"].ToString();
                                    //string strValidationDateOfBirth = objCommonCleanup.ParseDateFromString(strDateOfBirthPrimary, "yyyyMMdd");
                                    newRow["ihdob1"] = dr["ihdob"].ToString();

                                    //'Insurance Company Information
                                    newRow["icode1"] = dr["icode"].ToString();
                                    newRow["ipolicy1"] = dr["ipolicy"].ToString();
                                    newRow["igroup1"] = dr["igroup"].ToString();
                                    newRow["iname1"] = dr["iname"].ToString();
                                    newRow["iaddr1a"] = dr["IAddrA"].ToString();
                                    newRow["iaddr1b"] = dr["IAddrB"].ToString();
                                    newRow["icity1"] = dr["icity"].ToString();
                                    newRow["istate1"] = dr["istate"].ToString();
                                    newRow["izip1"] = dr["izip"].ToString();
                                    newRow["iphone1"] = dr["iphone"].ToString();
                                    dr["Original_iphone"] = dr["iphone"].ToString();
                                    newRow["Original_iphone1"] = dr["Original_iphone"].ToString();
                                    //newRow["icopay1"] = dr["icopay"].ToString();

                                    StringBuilder strComments = new StringBuilder();
                                    strComments.Append("Insurancetype: " + dr["Insurancetype"].ToString() + ", ");
                                    strComments.Append("Eligibilityreason: " + dr["Eligibilityreason"].ToString() + ", ");
                                    strComments.Append("Ircname: " + dr["Ircname"].ToString() + ", ");
                                    strComments.Append("Relationshiptoinsuredid: " + dr["Relationshiptoinsuredid"].ToString() + ", ");
                                    strComments.Append("Eligibilitymessage: " + dr["Eligibilitymessage"].ToString() + ", ");
                                    strComments.Append("Eligibilitystatus: " + dr["eligibilitystatus"].ToString());

                                    newRow["icomments1"] = strComments.ToString();
                                }
                            }
                            if (dr["itype"].ToString() == "2")
                            {
                                if ((newRow["ipolicy1"].ToString().Trim().Length > 0
                                     || newRow["igroup1"].ToString().Trim().Length > 0)
                                     && (dr["ipolicy"].ToString().Trim().Length > 0
                                     || dr["igroup"].ToString().Trim().Length > 0))
                                {
                                    newRow["ibegin2"] = dr["ibegin"].ToString();
                                    newRow["iend2"] = dr["iend"].ToString();
                                    newRow["itype2"] = dr["insurancetype"].ToString();
                                    //'Insurance Holder Information
                                    // newRow["irela2hld2"] = RelationshipMapping(dr["irela2hld"].ToString());
                                    newRow["irela2hld2"] = dr["irela2hld"].ToString();
                                    newRow["Original_irela2hld2"] = dr["irela2hld"].ToString();
                                    newRow["ihlname2"] = dr["ihlname"].ToString();
                                    newRow["ihfname2"] = dr["ihfname"].ToString();
                                    newRow["ihminit2"] = dr["ihminit"].ToString();
                                    newRow["ihaddr2a"] = dr["IHADDRA"].ToString();
                                    newRow["ihaddr2b"] = dr["IHADDRB"].ToString();
                                    newRow["ihcity2"] = dr["ihcity"].ToString();
                                    newRow["ihstate2"] = dr["ihstate"].ToString();
                                    newRow["ihzip2"] = dr["ihzip"].ToString();
                                    newRow["ihsex2"] = dr["ihsex"].ToString();
                                    newRow["ihssn2"] = dr["ihssn"].ToString();

                                    //string strDateOfBirthSecondary = string.Empty;
                                    //strDateOfBirthSecondary = dr["ihdob"].ToString();

                                    //string strValidationDateOfBirth = objCommonCleanup.ParseDateFromString(strDateOfBirthSecondary, "yyyyMMdd");
                                    newRow["ihdob2"] = dr["ihdob"].ToString();

                                    //'Insurance Company Information
                                    newRow["icode2"] = dr["icode"].ToString();
                                    newRow["ipolicy2"] = dr["ipolicy"].ToString();
                                    newRow["igroup2"] = dr["igroup"].ToString();
                                    newRow["iname2"] = dr["iname"].ToString();
                                    newRow["iaddr2a"] = dr["IAddrA"].ToString();
                                    newRow["iaddr2b"] = dr["IAddrB"].ToString();
                                    newRow["icity2"] = dr["icity"].ToString();
                                    newRow["istate2"] = dr["istate"].ToString();
                                    newRow["izip2"] = dr["izip"].ToString();
                                    newRow["iphone2"] = dr["iphone"].ToString();
                                    dr["Original_iphone"] = dr["iphone"].ToString();
                                    newRow["Original_iphone2"] = dr["Original_iphone"].ToString();
                                    //newRow["icopay2"] = dr["icopay"].ToString();

                                    StringBuilder strComments = new StringBuilder();
                                    strComments.Append("Insurancetype: " + dr["Insurancetype"].ToString() + ", ");
                                    strComments.Append("Eligibilityreason: " + dr["Eligibilityreason"].ToString() + ", ");
                                    strComments.Append("Ircname: " + dr["Ircname"].ToString() + ", ");
                                    strComments.Append("Relationshiptoinsuredid: " + dr["Relationshiptoinsuredid"].ToString() + ", ");
                                    strComments.Append("Eligibilitymessage: " + dr["Eligibilitymessage"].ToString() + ", ");
                                    strComments.Append("Eligibilitystatus: " + dr["eligibilitystatus"].ToString());

                                    newRow["icomments2"] = strComments.ToString();
                                }
                            }
                        }
                        dtReturn.Rows.Add(newRow);
                    }
                    dtClinicaldata = dtReturn;
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

        internal class patientinsurance
        {
            [JsonProperty("insurancepolicyholdercountrycode")]
            public string Insurancepolicyholdercountrycode { get; set; }

            [JsonProperty("sequencenumber")]
            public string Sequencenumber { get; set; }

            [JsonProperty("insurancepolicyholderssn")]
            public object Insurancepolicyholderssn { get; set; }

            [JsonProperty("insuranceplanname")]
            public string Insuranceplanname { get; set; }

            [JsonProperty("insurancetype")]
            public string Insurancetype { get; set; }

            [JsonProperty("insurancepolicyholderlastname")]
            public string Insurancepolicyholderlastname { get; set; }

            [JsonProperty("insurancephone")]
            public string Insurancephone { get; set; }

            [JsonProperty("insuranceidnumber")]
            public string Insuranceidnumber { get; set; }

            [JsonProperty("insuranceid")]
            public string Insuranceid { get; set; }

            [JsonProperty("insurancepolicyholder")]
            public string Insurancepolicyholder { get; set; }

            [JsonProperty("eligibilitylastchecked")]
            public string Eligibilitylastchecked { get; set; }

            [JsonProperty("eligibilitystatus")]
            public string Eligibilitystatus { get; set; }

            [JsonProperty("insurancepolicyholderfirstname")]
            public string Insurancepolicyholderfirstname { get; set; }

            [JsonProperty("insurancepolicyholdermiddlename")]
            public string Insurancepolicyholdermiddlename { get; set; }


            [JsonProperty("insurancepackageid")]
            public string Insurancepackageid { get; set; }

            [JsonProperty("insurancepolicyholdersex")]
            public string Insurancepolicyholdersex { get; set; }

            [JsonProperty("eligibilityreason")]
            public string Eligibilityreason { get; set; }

            [JsonProperty("insurancepolicyholdercountryiso3166")]
            public string Insurancepolicyholdercountryiso3166 { get; set; }

            [JsonProperty("ircname")]
            public string Ircname { get; set; }

            [JsonProperty("insurancepolicyholderdob")]
            public string Insurancepolicyholderdob { get; set; }

            [JsonProperty("insurancepolicyholderstate")]
            public string Insurancepolicyholderstate { get; set; }

            [JsonProperty("insurancepolicyholderzip")]
            public string Insurancepolicyholderzip { get; set; }

            [JsonProperty("relationshiptoinsuredid")]
            public string Relationshiptoinsuredid { get; set; }

            [JsonProperty("relationshiptoinsured")]
            public string Relationshiptoinsured { get; set; }

            [JsonProperty("insurancepolicyholderaddress1")]
            public string Insurancepolicyholderaddress1 { get; set; }

            [JsonProperty("insurancepolicyholderaddress2")]
            public string Insurancepolicyholderaddress2 { get; set; }

            [JsonProperty("eligibilitymessage")]
            public string Eligibilitymessage { get; set; }

            [JsonProperty("insurancepolicyholdercity")]
            public string Insurancepolicyholdercity { get; set; }

            [JsonProperty("insurancestate")]
            public string Insurancestate { get; set; }

            [JsonProperty("insurancezip")]
            public string Insurancezip { get; set; }

            [JsonProperty("insurancecity")]
            public string Insurancecity { get; set; }

            [JsonProperty("insuranceaddress")]
            public string Insuranceaddress { get; set; }

            [JsonProperty("insuranceaddress2")]
            public string Insuranceaddress2 { get; set; }

            [JsonProperty("policynumber")]
            public string Policynumber { get; set; }

            [JsonProperty("issuedate")]
            public string Issuedate { get; set; }

            [JsonProperty("Expirationdate")]
            public string Expirationdate { get; set; }
        }

        private DataTable PreparePatientInsuranceTable(DataTable dtPatientInsurance)
        {
            dtPatientInsurance = new DataTable();
            dtPatientInsurance.Columns.Add("patientid");
            dtPatientInsurance.Columns.Add("createdt");
            dtPatientInsurance.Columns.Add("icode1");
            dtPatientInsurance.Columns.Add("iname1");
            dtPatientInsurance.Columns.Add("iaddr1a");
            dtPatientInsurance.Columns.Add("iaddr1b");
            dtPatientInsurance.Columns.Add("icity1");
            dtPatientInsurance.Columns.Add("istate1");
            dtPatientInsurance.Columns.Add("izip1");
            dtPatientInsurance.Columns.Add("iphone1");
            dtPatientInsurance.Columns.Add("original_iphone1");
            dtPatientInsurance.Columns.Add("ifax1");
            dtPatientInsurance.Columns.Add("ipolicy1");
            dtPatientInsurance.Columns.Add("igroup1");
            dtPatientInsurance.Columns.Add("igroupname1");
            dtPatientInsurance.Columns.Add("irela2hld1");
            dtPatientInsurance.Columns.Add("original_irela2hld1");
            dtPatientInsurance.Columns.Add("itype1");
            dtPatientInsurance.Columns.Add("iactive1");
            dtPatientInsurance.Columns.Add("ibegin1");
            dtPatientInsurance.Columns.Add("iend1");
            dtPatientInsurance.Columns.Add("ihcode1");
            dtPatientInsurance.Columns.Add("ihname1");
            dtPatientInsurance.Columns.Add("ihlname1");
            dtPatientInsurance.Columns.Add("ihfname1");
            dtPatientInsurance.Columns.Add("ihminit1");
            dtPatientInsurance.Columns.Add("ihaddr1a");
            dtPatientInsurance.Columns.Add("ihaddr1b");
            dtPatientInsurance.Columns.Add("ihcity1");
            dtPatientInsurance.Columns.Add("ihstate1");
            dtPatientInsurance.Columns.Add("ihzip1");
            dtPatientInsurance.Columns.Add("ihhomephn1");
            dtPatientInsurance.Columns.Add("ihworkphn1");
            dtPatientInsurance.Columns.Add("ihdob1");
            dtPatientInsurance.Columns.Add("ihsex1");
            dtPatientInsurance.Columns.Add("ihssn1");
            dtPatientInsurance.Columns.Add("hid1");
            dtPatientInsurance.Columns.Add("ih1employer");
            dtPatientInsurance.Columns.Add("ih1emplid");
            dtPatientInsurance.Columns.Add("ih1eaddr1");
            dtPatientInsurance.Columns.Add("ih1eaddr2");
            dtPatientInsurance.Columns.Add("ih1ecity");
            dtPatientInsurance.Columns.Add("ih1estate");
            dtPatientInsurance.Columns.Add("ih1ezip");
            dtPatientInsurance.Columns.Add("ih1ephone");
            dtPatientInsurance.Columns.Add("ih1efax");
            dtPatientInsurance.Columns.Add("icode2");
            dtPatientInsurance.Columns.Add("iname2");
            dtPatientInsurance.Columns.Add("iaddr2a");
            dtPatientInsurance.Columns.Add("iaddr2b");
            dtPatientInsurance.Columns.Add("icity2");
            dtPatientInsurance.Columns.Add("istate2");
            dtPatientInsurance.Columns.Add("izip2");
            dtPatientInsurance.Columns.Add("iphone2");
            dtPatientInsurance.Columns.Add("original_iphone2");
            dtPatientInsurance.Columns.Add("ifax2");
            dtPatientInsurance.Columns.Add("ipolicy2");
            dtPatientInsurance.Columns.Add("igroup2");
            dtPatientInsurance.Columns.Add("igroupname2");
            dtPatientInsurance.Columns.Add("irela2hld2");
            dtPatientInsurance.Columns.Add("original_irela2hld2");
            dtPatientInsurance.Columns.Add("itype2");
            dtPatientInsurance.Columns.Add("iactive2");
            dtPatientInsurance.Columns.Add("ibegin2");
            dtPatientInsurance.Columns.Add("iend2");
            dtPatientInsurance.Columns.Add("ihcode2");
            dtPatientInsurance.Columns.Add("ihname2");
            dtPatientInsurance.Columns.Add("ihlname2");
            dtPatientInsurance.Columns.Add("ihfname2");
            dtPatientInsurance.Columns.Add("ihminit2");
            dtPatientInsurance.Columns.Add("ihaddr2a");
            dtPatientInsurance.Columns.Add("ihaddr2b");
            dtPatientInsurance.Columns.Add("ihcity2");
            dtPatientInsurance.Columns.Add("ihstate2");
            dtPatientInsurance.Columns.Add("ihzip2");
            dtPatientInsurance.Columns.Add("ihhomephn2");
            dtPatientInsurance.Columns.Add("ihworkphn2");
            dtPatientInsurance.Columns.Add("ihdob2");
            dtPatientInsurance.Columns.Add("ihsex2");
            dtPatientInsurance.Columns.Add("ihssn2");
            dtPatientInsurance.Columns.Add("hid2");
            dtPatientInsurance.Columns.Add("ih2employer");
            dtPatientInsurance.Columns.Add("ih2emplid");
            dtPatientInsurance.Columns.Add("ih2eaddr1");
            dtPatientInsurance.Columns.Add("ih2eaddr2");
            dtPatientInsurance.Columns.Add("ih2ecity");
            dtPatientInsurance.Columns.Add("ih2estate");
            dtPatientInsurance.Columns.Add("ih2ezip");
            dtPatientInsurance.Columns.Add("ih2ephone");
            dtPatientInsurance.Columns.Add("ih2efax");
            dtPatientInsurance.Columns.Add("original_ihworkphn1");
            dtPatientInsurance.Columns.Add("original_ihhomephn1");
            dtPatientInsurance.Columns.Add("original_ihworkphn2");
            dtPatientInsurance.Columns.Add("original_ihhomephn2");
            dtPatientInsurance.Columns.Add("ihcellphone1");
            dtPatientInsurance.Columns.Add("ihcellphone2");
            dtPatientInsurance.Columns.Add("icomments1");
            dtPatientInsurance.Columns.Add("icomments2");
            dtPatientInsurance.Columns.Add("Insurancetype");
            dtPatientInsurance.Columns.Add("Eligibilityreason");
            dtPatientInsurance.Columns.Add("Ircname");
            dtPatientInsurance.Columns.Add("Relationshiptoinsuredid");
            dtPatientInsurance.Columns.Add("Eligibilitymessage");

            return dtPatientInsurance;
        }

        public string GenerateAPIJSONString(DataTable dtInsurances, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                string JSONString = string.Empty;
                DataTable dtFinalData;
                dtFinalData = dtInsurances.Copy();

                JSONString = DataTableToJSONInsurances(dtFinalData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string DataTableToJSONInsurances(DataTable table, string emrpatientid, string requestid, string emrid, string moduleid, string userid)
        {
            var JSONString = new StringBuilder();

            JSONString.Append("{");
            JSONString.Append("\"EMRPatientId\":" + "\"" + emrpatientid + "\",");
            JSONString.Append("\"EMRId\":" + "\"" + emrid + "\",");
            JSONString.Append("\"ModuleId\":" + "\"" + moduleid + "\",");
            JSONString.Append("\"RequestId\":" + "\"" + requestid + "\",");
            JSONString.Append("\"CreatedBy\":" + "\"\",");
            JSONString.Append("\"Insurances\":");

            if (table.Rows.Count == 0)
            {
                JSONString.Append("\"No Insurance Found\"");
                JSONString.Append("}");
                return JSONString.ToString();
            }

            JSONString.Append("[");

            int counter = 0;
            foreach (DataRow drow in table.Rows)
            {
                counter++;

                JSONString.Append("{");

                JSONString.Append("\"createdt\":" + "\"" + drow["createdt"].ToString() + "\",");
                JSONString.Append("\"icode1\":" + "\"" + drow["icode1"].ToString() + "\",");
                JSONString.Append("\"iname1\":" + "\"" + drow["iname1"].ToString() + "\",");
                JSONString.Append("\"iaddr1a\":" + "\"" + drow["iaddr1a"].ToString() + "\",");
                JSONString.Append("\"iaddr1b\":" + "\"" + drow["iaddr1b"].ToString() + "\",");
                JSONString.Append("\"icity1\":" + "\"" + drow["icity1"].ToString() + "\",");
                JSONString.Append("\"istate1\":" + "\"" + drow["istate1"].ToString() + "\",");
                JSONString.Append("\"izip1\":" + "\"" + drow["izip1"].ToString() + "\",");
                JSONString.Append("\"iphone1\":" + "\"" + drow["iphone1"].ToString() + "\",");
                JSONString.Append("\"original_iphone1\":" + "\"" + drow["original_iphone1"].ToString() + "\",");
                JSONString.Append("\"ifax1\":" + "\"" + drow["ifax1"].ToString() + "\",");
                JSONString.Append("\"ipolicy1\":" + "\"" + drow["ipolicy1"].ToString() + "\",");
                JSONString.Append("\"igroup1\":" + "\"" + drow["igroup1"].ToString() + "\",");
                JSONString.Append("\"igroupname1\":" + "\"" + drow["igroupname1"].ToString() + "\",");
                JSONString.Append("\"irela2hld1\":" + "\"" + drow["irela2hld1"].ToString() + "\",");
                JSONString.Append("\"original_irela2hld1\":" + "\"" + drow["original_irela2hld1"].ToString() + "\",");
                JSONString.Append("\"itype1\":" + "\"" + drow["itype1"].ToString() + "\",");
                JSONString.Append("\"iactive1\":" + "\"" + drow["iactive1"].ToString() + "\",");
                JSONString.Append("\"ibegin1\":" + "\"" + drow["ibegin1"].ToString() + "\",");
                JSONString.Append("\"iend1\":" + "\"" + drow["iend1"].ToString() + "\",");
                JSONString.Append("\"ihcode1\":" + "\"" + drow["ihcode1"].ToString() + "\",");
                JSONString.Append("\"ihname1\":" + "\"" + drow["ihname1"].ToString() + "\",");
                JSONString.Append("\"ihlname1\":" + "\"" + drow["ihlname1"].ToString() + "\",");
                JSONString.Append("\"ihfname1\":" + "\"" + drow["ihfname1"].ToString() + "\",");
                JSONString.Append("\"ihminit1\":" + "\"" + drow["ihminit1"].ToString() + "\",");
                JSONString.Append("\"ihaddr1a\":" + "\"" + drow["ihaddr1a"].ToString() + "\",");
                JSONString.Append("\"ihaddr1b\":" + "\"" + drow["ihaddr1b"].ToString() + "\",");
                JSONString.Append("\"ihcity1\":" + "\"" + drow["ihcity1"].ToString() + "\",");
                JSONString.Append("\"ihstate1\":" + "\"" + drow["ihstate1"].ToString() + "\",");
                JSONString.Append("\"ihzip1\":" + "\"" + drow["ihzip1"].ToString() + "\",");
                JSONString.Append("\"ihhomephn1\":" + "\"" + drow["ihhomephn1"].ToString() + "\",");
                JSONString.Append("\"ihworkphn1\":" + "\"" + drow["ihworkphn1"].ToString() + "\",");
                JSONString.Append("\"ihdob1\":" + "\"" + drow["ihdob1"].ToString() + "\",");
                JSONString.Append("\"ihsex1\":" + "\"" + drow["ihsex1"].ToString() + "\",");
                JSONString.Append("\"ihssn1\":" + "\"" + drow["ihssn1"].ToString() + "\",");
                JSONString.Append("\"hid1\":" + "\"" + drow["hid1"].ToString() + "\",");
                JSONString.Append("\"ih1employer\":" + "\"" + drow["ih1employer"].ToString() + "\",");
                JSONString.Append("\"ih1emplid\":" + "\"" + drow["ih1emplid"].ToString() + "\",");
                JSONString.Append("\"ih1eaddr1\":" + "\"" + drow["ih1eaddr1"].ToString() + "\",");
                JSONString.Append("\"ih1eaddr2\":" + "\"" + drow["ih1eaddr2"].ToString() + "\",");
                JSONString.Append("\"ih1ecity\":" + "\"" + drow["ih1ecity"].ToString() + "\",");
                JSONString.Append("\"ih1estate\":" + "\"" + drow["ih1estate"].ToString() + "\",");
                JSONString.Append("\"ih1ezip\":" + "\"" + drow["ih1ezip"].ToString() + "\",");
                JSONString.Append("\"ih1ephone\":" + "\"" + drow["ih1ephone"].ToString() + "\",");
                JSONString.Append("\"ih1efax\":" + "\"" + drow["ih1efax"].ToString() + "\",");
                JSONString.Append("\"icode2\":" + "\"" + drow["icode2"].ToString() + "\",");
                JSONString.Append("\"iname2\":" + "\"" + drow["iname2"].ToString() + "\",");
                JSONString.Append("\"iaddr2a\":" + "\"" + drow["iaddr2a"].ToString() + "\",");
                JSONString.Append("\"iaddr2b\":" + "\"" + drow["iaddr2b"].ToString() + "\",");
                JSONString.Append("\"icity2\":" + "\"" + drow["icity2"].ToString() + "\",");
                JSONString.Append("\"istate2\":" + "\"" + drow["istate2"].ToString() + "\",");
                JSONString.Append("\"izip2\":" + "\"" + drow["izip2"].ToString() + "\",");
                JSONString.Append("\"iphone2\":" + "\"" + drow["iphone2"].ToString() + "\",");
                JSONString.Append("\"original_iphone2\":" + "\"" + drow["original_iphone2"].ToString() + "\",");
                JSONString.Append("\"ifax2\":" + "\"" + drow["ifax2"].ToString() + "\",");
                JSONString.Append("\"ipolicy2\":" + "\"" + drow["ipolicy2"].ToString() + "\",");
                JSONString.Append("\"igroup2\":" + "\"" + drow["igroup2"].ToString() + "\",");
                JSONString.Append("\"igroupname2\":" + "\"" + drow["igroupname2"].ToString() + "\",");
                JSONString.Append("\"irela2hld2\":" + "\"" + drow["irela2hld2"].ToString() + "\",");
                JSONString.Append("\"original_irela2hld2\":" + "\"" + drow["original_irela2hld2"].ToString() + "\",");
                JSONString.Append("\"itype2\":" + "\"" + drow["itype2"].ToString() + "\",");
                JSONString.Append("\"iactive2\":" + "\"" + drow["iactive2"].ToString() + "\",");
                JSONString.Append("\"ibegin2\":" + "\"" + drow["ibegin2"].ToString() + "\",");
                JSONString.Append("\"iend2\":" + "\"" + drow["iend2"].ToString() + "\",");
                JSONString.Append("\"ihcode2\":" + "\"" + drow["ihcode2"].ToString() + "\",");
                JSONString.Append("\"ihname2\":" + "\"" + drow["ihname2"].ToString() + "\",");
                JSONString.Append("\"ihlname2\":" + "\"" + drow["ihlname2"].ToString() + "\",");
                JSONString.Append("\"ihfname2\":" + "\"" + drow["ihfname2"].ToString() + "\",");
                JSONString.Append("\"ihminit2\":" + "\"" + drow["ihminit2"].ToString() + "\",");
                JSONString.Append("\"ihaddr2a\":" + "\"" + drow["ihaddr2a"].ToString() + "\",");
                JSONString.Append("\"ihaddr2b\":" + "\"" + drow["ihaddr2b"].ToString() + "\",");
                JSONString.Append("\"ihcity2\":" + "\"" + drow["ihcity2"].ToString() + "\",");
                JSONString.Append("\"ihstate2\":" + "\"" + drow["ihstate2"].ToString() + "\",");
                JSONString.Append("\"ihzip2\":" + "\"" + drow["ihzip2"].ToString() + "\",");
                JSONString.Append("\"ihhomephn2\":" + "\"" + drow["ihhomephn2"].ToString() + "\",");
                JSONString.Append("\"ihworkphn2\":" + "\"" + drow["ihworkphn2"].ToString() + "\",");
                JSONString.Append("\"ihdob2\":" + "\"" + drow["ihdob2"].ToString() + "\",");
                JSONString.Append("\"ihsex2\":" + "\"" + drow["ihsex2"].ToString() + "\",");
                JSONString.Append("\"ihssn2\":" + "\"" + drow["ihssn2"].ToString() + "\",");
                JSONString.Append("\"hid2\":" + "\"" + drow["hid2"].ToString() + "\",");
                JSONString.Append("\"ih2employer\":" + "\"" + drow["ih2employer"].ToString() + "\",");
                JSONString.Append("\"ih2emplid\":" + "\"" + drow["ih2emplid"].ToString() + "\",");
                JSONString.Append("\"ih2eaddr1\":" + "\"" + drow["ih2eaddr1"].ToString() + "\",");
                JSONString.Append("\"ih2eaddr2\":" + "\"" + drow["ih2eaddr2"].ToString() + "\",");
                JSONString.Append("\"ih2ecity\":" + "\"" + drow["ih2ecity"].ToString() + "\",");
                JSONString.Append("\"ih2estate\":" + "\"" + drow["ih2estate"].ToString() + "\",");
                JSONString.Append("\"ih2ezip\":" + "\"" + drow["ih2ezip"].ToString() + "\",");
                JSONString.Append("\"ih2ephone\":" + "\"" + drow["ih2ephone"].ToString() + "\",");
                JSONString.Append("\"ih2efax\":" + "\"" + drow["ih2efax"].ToString() + "\",");
                JSONString.Append("\"original_ihworkphn1\":" + "\"" + drow["original_ihworkphn1"].ToString() + "\",");
                JSONString.Append("\"original_ihhomephn1\":" + "\"" + drow["original_ihhomephn1"].ToString() + "\",");
                JSONString.Append("\"original_ihworkphn2\":" + "\"" + drow["original_ihworkphn2"].ToString() + "\",");
                JSONString.Append("\"original_ihhomephn2\":" + "\"" + drow["original_ihhomephn2"].ToString() + "\",");
                JSONString.Append("\"ihcellphone1\":" + "\"" + drow["ihcellphone1"].ToString() + "\",");
                JSONString.Append("\"ihcellphone2\":" + "\"" + drow["ihcellphone2"].ToString() + "\",");
                JSONString.Append("\"icomments1\":" + "\"" + drow["icomments1"].ToString() + "\",");
                JSONString.Append("\"icomments2\":" + "\"" + drow["icomments2"].ToString() + "\",");
                JSONString.Append("\"Insurancetype\":" + "\"" + drow["Insurancetype"].ToString() + "\",");
                JSONString.Append("\"Eligibilityreason\":" + "\"" + drow["Eligibilityreason"].ToString() + "\",");
                JSONString.Append("\"Ircname\":" + "\"" + drow["Ircname"].ToString() + "\",");
                JSONString.Append("\"Relationshiptoinsuredid\":" + "\"" + drow["Relationshiptoinsuredid"].ToString() + "\",");
                JSONString.Append("\"Eligibilitymessage\":" + "\"" + drow["Eligibilitymessage"].ToString() + "\"");


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
