using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;

namespace EMRIntegrations.DrChrono
{
    public class DrChronoAPI
    {
        Hashtable parameters = new Hashtable();
        APIConnection api = null;
        ArrayList departmentMasterList = new ArrayList();
        ArrayList departmentClinicalList = new ArrayList();

        public DrChronoAPI()
        {
            Common objCommon = new Common();

            parameters.Add("api_baseurl", "https://drchrono.com/");
            parameters.Add("access_token", null);
            parameters.Add("patientid", null);
            parameters.Add("drchronoapiobject", null);

            //SetServiceObject();
        }

        #region Set Service Object

        //private void SetServiceObject()
        //{
        //    if (api == null)
        //    {
        //        api = new APIConnection(parameters["api_baseurl"].ToString(), parameters["access_token"].ToString().Trim());
        //        parameters["drchronoapiobject"] = api;
        //    }
        //    else
        //    {
        //        parameters["drchronoapiobject"] = api;
        //    }
        //}

        private class patientids
        {
            public string patientid { get; set; }
        }

        private class Department
        {
            public string departmentid { get; set; }

            public string chartsharinggroupid { get; set; }
        }

        /// <summary>
        /// Description : Get the all department group and pick first department from each group for filter patient 
        /// </summary>
        /// <param name="parameters"></param>
        private void GetPatientMasterDepartment(Hashtable parameters)
        {
            if (departmentMasterList == null || departmentMasterList.Count == 0)
            {
                JObject jobj = (JObject)api.GET("departments");
                JToken jtobj = jobj["departments"];
                List<Department> objdepartments = jtobj.ToObject<List<Department>>();
                List<Department> lstchartsharinggroupid = new List<Department>();

                foreach (var item in objdepartments)
                {
                    departmentMasterList.Add(item.departmentid);
                }
                parameters["api_departmentid_patientmaster"] = departmentMasterList;
            }
            else
            {
                parameters["api_departmentid_patientmaster"] = departmentMasterList;
            }
        }

        /// <summary>
        /// Description : Get the all department group and pick first department from each group for filter patient 
        /// </summary>
        /// <param name="parameters"></param>
        private void GetClinicalDepartmentList(Hashtable parameters)
        {
            if (departmentClinicalList == null || departmentClinicalList.Count == 0)
            {
                JObject jobj = (JObject)api.GET("departments");
                JToken jtobj = jobj["departments"];
                List<Department> objdepartments = jtobj.ToObject<List<Department>>();
                List<Department> lstchartsharinggroupid = new List<Department>();

                foreach (var item in objdepartments)
                {
                    if (!lstchartsharinggroupid.Exists(m => m.chartsharinggroupid == item.chartsharinggroupid))
                    {
                        lstchartsharinggroupid.Add(item);
                        departmentClinicalList.Add(item.departmentid);
                    }
                }
                parameters["api_departmentid"] = departmentClinicalList;
            }
            else
            {
                parameters["api_departmentid"] = departmentClinicalList;
            }
        }

        #endregion

        #region ReadMappingFile

        public string ReadMappingFile(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = filename;
            string result = string.Empty;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }
        #endregion

        #region Get Patient Demographics

        public DataTable GetPatientDemographics(string patientid, string stagingdbconnectionstring, string requestid, string accesstoken)
        {
            try
            {
                DataTable dtPatientData = new DataTable();

                parameters["access_token"] = accesstoken;
                parameters["patientid"] = patientid;

                PatientDemographics objPatientDemographics = new PatientDemographics();
                dtPatientData = objPatientDemographics.GetData(parameters);

                //Common objCommon = new Common();
                //objCommon.InsertRecords(dtPatientData, "IntelliHdrchronoPatientDemographics", stagingdbconnectionstring, requestid);

                return dtPatientData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SavePatientDemographics(DataTable dtPatientData, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                Common objCommon = new Common();
                objCommon.InsertRecords(dtPatientData, "drchrono.PatientDemographics", stagingdbconnectionstring, requestid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetJSONPatientDemographics(DataTable dtPatientData, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                PatientDemographics objPatientDemographics = new PatientDemographics();
                string JSONString = objPatientDemographics.GenerateAPIJSONString(dtPatientData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public string GenerateAPIJSONString(DataTable dtPatientData, string APIURL, string RequestID, string EMRID, string ModuleID, string UserID)
        //{
        //    try
        //    {
        //        string JSONString = string.Empty;
        //        DataTable dtFinalData;
        //        dtFinalData = dtPatientData.Copy();

        //        foreach (DataColumn dColumnPatDemo in dtFinalData.Columns)
        //        {
        //            if (dColumnPatDemo.ColumnName == "pprefix")
        //            {
        //                dColumnPatDemo.ColumnName = "Title";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "psuffix")
        //            {
        //                dColumnPatDemo.ColumnName = "Suffix";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "pfname")
        //            {
        //                dColumnPatDemo.ColumnName = "FirstName";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "pminit")
        //            {
        //                dColumnPatDemo.ColumnName = "MiddleName";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "plname")
        //            {
        //                dColumnPatDemo.ColumnName = "LastName";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "pemail")
        //            {
        //                dColumnPatDemo.ColumnName = "Email";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "pdob")
        //            {
        //                dColumnPatDemo.ColumnName = "DateOfBirth";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "psex")
        //            {
        //                dColumnPatDemo.ColumnName = "Gender";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "phomephn")
        //            {
        //                dColumnPatDemo.ColumnName = "Phone";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "ethnicity")
        //            {
        //                dColumnPatDemo.ColumnName = "EthnicityCode";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "prace")
        //            {
        //                dColumnPatDemo.ColumnName = "RaceCode";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "paddr1")
        //            {
        //                dColumnPatDemo.ColumnName = "Address1";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "paddr2")
        //            {
        //                dColumnPatDemo.ColumnName = "Address2";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "pcity")
        //            {
        //                dColumnPatDemo.ColumnName = "City";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "pstate")
        //            {
        //                dColumnPatDemo.ColumnName = "State";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "pzip")
        //            {
        //                dColumnPatDemo.ColumnName = "ZipCode";
        //            }
        //            else if (dColumnPatDemo.ColumnName == "pssn")
        //            {
        //                dColumnPatDemo.ColumnName = "SSN";
        //            }
        //        }

        //        JSONString = DataTableToJSONPatientDemographics(dtFinalData, RequestID, EMRID, ModuleID, UserID);
        //        return JSONString;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public string DataTableToJSONPatientDemographics(DataTable table, string requestid, string emrid, string moduleid, string userid)
        //{
        //    var JSONString = new StringBuilder();

        //    foreach (DataRow drow in table.Rows)
        //    {
        //        JSONString.Append("{");
        //        JSONString.Append("\"EMRPatientId\":" + "\"" + drow["Patientid"].ToString() + "\",");
        //        JSONString.Append("\"EMRId\":" + "\"" + emrid + "\",");
        //        JSONString.Append("\"ModuleId\":" + "\"" + moduleid + "\",");
        //        JSONString.Append("\"RequestId\":" + "\"" + requestid + "\",");
        //        JSONString.Append("\"CreatedBy\":" + "\"\",");
        //        JSONString.Append("\"EMRUserExtensionLogDetails\": {");

        //        JSONString.Append("\"UserId\":" + "\"" + userid + "\",");
        //        JSONString.Append("\"Title\":" + "\"" + drow["Title"].ToString() + "\",");
        //        JSONString.Append("\"FirstName\":" + "\"" + drow["FirstName"].ToString() + "\",");
        //        JSONString.Append("\"MiddleName\":" + "\"" + drow["MiddleName"].ToString() + "\",");
        //        JSONString.Append("\"LastName\":" + "\"" + drow["LastName"].ToString() + "\",");
        //        JSONString.Append("\"Suffix\":" + "\"" + drow["Suffix"].ToString() + "\",");
        //        JSONString.Append("\"Gender\":" + "\"" + drow["Gender"].ToString() + "\",");
        //        JSONString.Append("\"DateOfBirth\":" + "\"" + drow["DateOfBirth"].ToString() + "\",");
        //        JSONString.Append("\"Email\":" + "\"" + drow["Email"].ToString() + "\",");
        //        JSONString.Append("\"Street\":" + "\"\",");
        //        JSONString.Append("\"City\":" + "\"" + drow["City"].ToString() + "\",");
        //        JSONString.Append("\"ZipCode\":" + "\"" + drow["ZipCode"].ToString() + "\",");
        //        JSONString.Append("\"Phone\":" + "\"" + drow["Phone"].ToString() + "\",");
        //        JSONString.Append("\"Address1\":" + "\"" + drow["Address1"].ToString() + "\",");
        //        JSONString.Append("\"Address2\":" + "\"" + drow["Address2"].ToString() + "\",");
        //        JSONString.Append("\"SSN\":" + "\"" + drow["SSN"].ToString() + "\",");
        //        JSONString.Append("\"MRN\":" + "\"\",");
        //        JSONString.Append("\"State\":" + "\"" + drow["State"].ToString() + "\",");
        //        JSONString.Append("\"Password\":" + "\"\",");
        //        JSONString.Append("\"Role\":" + "\"\",");
        //        JSONString.Append("\"heightFeet\":" + "\"\",");
        //        JSONString.Append("\"heightInch\":" + "\"\",");
        //        JSONString.Append("\"weight\":" + "\"\",");
        //        JSONString.Append("\"FacilityId\":" + "\"" + requestid + "\",");
        //        JSONString.Append("\"StateId\":" + "\"\",");
        //        JSONString.Append("\"EthnicityCode\":" + "\"" + drow["EthnicityCode"].ToString() + "\",");
        //        JSONString.Append("\"RaceCode\":" + "\"" + drow["RaceCode"].ToString() + "\",");
        //        JSONString.Append("\"NPI\":" + "\"\",");
        //        JSONString.Append("\"Status\":" + "\"\",");
        //        JSONString.Append("\"POSCode\":" + "\"\"");

        //        JSONString.Append("}");
        //        JSONString.Append("}");
        //    }
        //    return JSONString.ToString();
        //}

        public DataTable MapPatientDemographics(DataTable dtPatientData)
        {
            try
            {
                string mappingstr = ReadMappingFile("PatientDemographics.json");

                return new DataTable();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get Patient Medications

        //Data Source=WINDOWS-7J7SHKG\SQLEXPRESS;Initial Catalog=IntelliHStagingdrchrono;User ID=sa;Password=Sahil@123
        public DataTable GetMedications(string patientid, string stagingdbconnectionstring, string requestid, string accesstoken)
        {
            try
            {
                parameters["access_token"] = accesstoken;
                parameters["patientid"] = patientid;

                Medications objMedications = new Medications();
                DataTable dtMedications = new DataTable();
                dtMedications = objMedications.GetData(parameters);

                //Common objCommon = new Common();
                //objCommon.InsertRecords(dtMedications, "IntelliHdrchronoMedications", stagingdbconnectionstring, requestid);

                return dtMedications;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveMedications(DataTable dtMedications, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                Common objCommon = new Common();
                objCommon.InsertRecords(dtMedications, "drchrono.Medications", stagingdbconnectionstring, requestid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetJSONMedications(DataTable dtMedications, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                Medications objMedications = new Medications();
                string JSONString = objMedications.GenerateAPIJSONString(dtMedications, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Post Document

        //Data Source=WINDOWS-7J7SHKG\SQLEXPRESS;Initial Catalog=IntelliHStagingdrchrono;User ID=sa;Password=Sahil@123
        public DataTable PostDocument(string patientid, string stagingdbconnectionstring, string requestid, string accesstoken)
        {
            try
            {
                parameters["access_token"] = accesstoken;
                parameters["patientid"] = patientid;

                DocumentUpload objDocumentUpload = new DocumentUpload();
                DataTable dtDocumentUpload = new DataTable();
                dtDocumentUpload = objDocumentUpload.PostData(parameters);

                //Common objCommon = new Common();
                //objCommon.InsertRecords(dtMedications, "IntelliHdrchronoMedications", stagingdbconnectionstring, requestid);

                return dtDocumentUpload;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveDocumentUpload(DataTable dtMedications, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                Common objCommon = new Common();
                objCommon.InsertRecords(dtMedications, "drchrono.Medications", stagingdbconnectionstring, requestid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetJSONDocumentUpload(DataTable dtMedications, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                Medications objMedications = new Medications();
                string JSONString = objMedications.GenerateAPIJSONString(dtMedications, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get Patient Insurances

        //Data Source=WINDOWS-7J7SHKG\SQLEXPRESS;Initial Catalog=IntelliHStagingdrchrono;User ID=sa;Password=Sahil@123
        public DataTable GetInsurances(string patientid, string departmentid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                parameters["patientid"] = patientid;
                parameters["api_departmentid"] = departmentid;

                PatientInsurance objPatientInsurance = new PatientInsurance();
                DataTable dtInsurances = new DataTable();
                dtInsurances = objPatientInsurance.GetData(parameters);

                //Common objCommon = new Common();
                //objCommon.InsertRecords(dtInsurances, "IntelliHdrchronoInsurances", stagingdbconnectionstring, requestid);

                return dtInsurances;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveInsurances(DataTable dtInsurances, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                Common objCommon = new Common();
                objCommon.InsertRecords(dtInsurances, "drchrono.Insurances", stagingdbconnectionstring, requestid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetJSONInsurances(DataTable dtInsurances, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                PatientInsurance objInsurances = new PatientInsurance();
                string JSONString = objInsurances.GenerateAPIJSONString(dtInsurances, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get VitalSigns

        public DataTable GetPatientVitals(string patientid, string departmentid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                parameters["patientid"] = patientid;
                parameters["api_departmentid"] = departmentid;

                Vitals objVitals = new Vitals();
                DataTable dtVitals = new DataTable();
                dtVitals = objVitals.GetData(parameters);

                Common objCommon = new Common();
                objCommon.InsertRecords(dtVitals, "IntelliHdrchronoVitalSigns", stagingdbconnectionstring, requestid);

                return dtVitals;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get Allergy

        public DataTable GetAllergy(string patientid, string departmentid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                parameters["patientid"] = patientid;
                parameters["api_departmentid"] = departmentid;

                Allergy objAllergy = new Allergy();
                DataTable dtAllergy = new DataTable();
                dtAllergy = objAllergy.GetData(parameters);

                //Common objCommon = new Common();
                //objCommon.InsertRecords(dtAllergy, "IntelliHdrchronoAllergy", stagingdbconnectionstring, requestid);

                return dtAllergy;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveAllergy(DataTable dtAllergy, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                Common objCommon = new Common();
                objCommon.InsertRecords(dtAllergy, "drchrono.Allergy", stagingdbconnectionstring, requestid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetJSONAllergy(DataTable dtAllergy, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                Allergy objAllergy = new Allergy();
                string JSONString = objAllergy.GenerateAPIJSONString(dtAllergy, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

    }
}
