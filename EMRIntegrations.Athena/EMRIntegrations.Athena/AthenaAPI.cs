using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;

namespace EMRIntegrations.Athena
{
    public class AthenaAPI
    {
        Hashtable parameters = new Hashtable();
        APIConnection api = null;
        ArrayList departmentMasterList = new ArrayList();
        ArrayList departmentClinicalList = new ArrayList();

        public AthenaAPI()
        {
            Common objCommon = new Common();

            parameters.Add("api_baseurl", "https://api.athenahealth.com/");
            parameters.Add("api_verison", "preview1");
            parameters.Add("api_key", "7wcjy55g3db37yy7p4sa4kvp");
            parameters.Add("api_secretkey", "E4WJMh5ggDd6h6H");
            parameters.Add("api_practiceid", "195900");
            parameters.Add("athenaapiobject", null);
            parameters.Add("api_departmentid_patientmaster", null);
            parameters.Add("api_departmentid", null);
            parameters.Add("patientid", null);

            SetServiceObject();
        }

        #region Set Service Object

        private void SetServiceObject()
        {
            //parameters["api_key"] = "gjhu6u3k8qzfrcs3gr79zuay";
            //parameters["api_secretkey"] = "czmwCrm6V9GcEED";
            //parameters["api_verison"] = "preview1";
            //parameters["api_practiceid"] = "195900";
            //parameters["api_baseurl"] = "https://api.athenahealth.com/";

            //parameters["api_key"] = "gjhu6u3k8qzfrcs3gr79zuay";
            //parameters["api_secretkey"] = "czmwCrm6V9GcEED";
            //parameters["api_verison"] = "preview1";
            //parameters["api_practiceid"] = "13090";
            //parameters["api_baseurl"] = "https://api.athenahealth.com/";

            if (api == null)
            {
                api = new APIConnection(parameters["api_baseurl"].ToString(), parameters["api_verison"].ToString().Trim(), parameters["api_key"].ToString().Trim(), parameters["api_secretkey"].ToString().Trim(), parameters["api_practiceid"].ToString().Trim());
                parameters["athenaapiobject"] = api;
            }
            else
            {
                parameters["athenaapiobject"] = api;
            }

            if (api != null)
            {
                if (departmentMasterList == null || departmentMasterList.Count == 0) // To avoid unneccessary api call for each time 
                {
                    GetPatientMasterDepartment(parameters);
                }
                else
                {
                    parameters["api_departmentid_patientmaster"] = departmentMasterList;
                }

                if (departmentClinicalList == null || departmentClinicalList.Count == 0) // To avoid unneccessary api call for each time
                {
                    GetClinicalDepartmentList(parameters);
                }
                else
                {
                    parameters["api_departmentid"] = departmentClinicalList;
                }

                //GetPatientCarePlan("1447", "1", @"Data Source=WINDOWS-7J7SHKG\SQLEXPRESS;Initial Catalog=IntelliHStagingAthena;User ID=sa;Password=Sahil@123");
            }
        }

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

        /// <summary>
        /// Interface metod for get all patientIds.
        /// </summary>        
        /// <param name="connection">Source database conection</param>
        /// <returns>Returns patientmaster query string</returns>
        public DataTable GetPatientIDs()
        {
            DataTable dtPatientIDs = new DataTable();
            try
            {
                dtPatientIDs.Columns.Add("patientid");
                dtPatientIDs.Columns.Add("departmentid");

                JObject jobj = (JObject)api.GET("departments");
                JToken jtobj = jobj["departments"];
                List<Department> objdepartments = jtobj.ToObject<List<Department>>();

                Dictionary<string, string> appbook = null;
                jobj = null;
                jtobj = null;

                foreach (var value in objdepartments)
                {
                    string format = "yyyy/MM/dd";
                    DateTime today = DateTime.Now.AddYears(-16);
                    today = Convert.ToDateTime("2000/01/01");
                    DateTime nextyear = DateTime.Now;

                    appbook = new Dictionary<string, string>()
                      {
                        {"departmentid", value.departmentid.ToString() },
                        {"startdate", today.ToString(format)},
                        {"enddate", nextyear.ToString(format)},
                        {"limit", "10000"}
                      };

                    jobj = (JObject)api.GET("appointments/booked", appbook);

                    jtobj = jobj["appointments"];
                    var appdata = jtobj.ToObject<List<patientids>>();

                    foreach (var pat in appdata)
                    {
                        dtPatientIDs.Rows.Add(pat.patientid);
                    }
                }
                dtPatientIDs = dtPatientIDs.DefaultView.ToTable(true, new string[] { "patientid" });
                return dtPatientIDs;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetPatientDemographics(string patientid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                DataTable dtPatientData = new DataTable();

                parameters["patientid"] = patientid;

                PatientDemographics objPatientDemographics = new PatientDemographics();
                dtPatientData = objPatientDemographics.GetData(parameters);

                //Common objCommon = new Common();
                //objCommon.InsertRecords(dtPatientData, "IntelliHAthenaPatientDemographics", stagingdbconnectionstring, requestid);

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
                objCommon.InsertRecords(dtPatientData, "Athena.PatientDemographics", stagingdbconnectionstring, requestid);
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

        //Data Source=WINDOWS-7J7SHKG\SQLEXPRESS;Initial Catalog=IntelliHStagingAthena;User ID=sa;Password=Sahil@123
        public DataTable GetMedications(string patientid, string departmentid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                parameters["patientid"] = patientid;
                parameters["api_departmentid"] = departmentid;

                Medications objMedications = new Medications();
                DataTable dtMedications = new DataTable();
                dtMedications = objMedications.GetData(parameters);

                //Common objCommon = new Common();
                //objCommon.InsertRecords(dtMedications, "IntelliHAthenaMedications", stagingdbconnectionstring, requestid);

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
                objCommon.InsertRecords(dtMedications, "Athena.Medications", stagingdbconnectionstring, requestid);
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

        #region Get Patient Insurances

        //Data Source=WINDOWS-7J7SHKG\SQLEXPRESS;Initial Catalog=IntelliHStagingAthena;User ID=sa;Password=Sahil@123
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
                //objCommon.InsertRecords(dtInsurances, "IntelliHAthenaInsurances", stagingdbconnectionstring, requestid);

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
                objCommon.InsertRecords(dtInsurances, "Athena.Insurances", stagingdbconnectionstring, requestid);
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
                objCommon.InsertRecords(dtVitals, "IntelliHAthenaVitalSigns", stagingdbconnectionstring, requestid);

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
                //objCommon.InsertRecords(dtAllergy, "IntelliHAthenaAllergy", stagingdbconnectionstring, requestid);

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
                objCommon.InsertRecords(dtAllergy, "Athena.Allergy", stagingdbconnectionstring, requestid);
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

        #region Get LabResults

        public DataTable GetPatientLabResults(string patientid, string departmentid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                parameters["patientid"] = patientid;
                parameters["api_departmentid"] = departmentid;

                LabResults objLabResults = new LabResults();
                DataTable dtLabResults = new DataTable();
                dtLabResults = objLabResults.GetData(parameters);

                Common objCommon = new Common();
                objCommon.InsertRecords(dtLabResults, "IntelliHAthenaLabResults", stagingdbconnectionstring, requestid);

                return dtLabResults;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get Appointments

        public DataTable GetPatientAppointments(string patientid, string departmentid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                parameters["patientid"] = patientid;
                parameters["api_departmentid"] = departmentid;

                Appointments objAppointments = new Appointments();
                DataTable dtAppointments = new DataTable();
                dtAppointments = objAppointments.GetData(parameters);

                Common objCommon = new Common();
                objCommon.InsertRecords(dtAppointments, "IntelliHAthenaAppointments", stagingdbconnectionstring, requestid);

                return dtAppointments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get AppointmentTypes

        public DataTable GetAppointmentTypes(string departmentid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                parameters["api_departmentid"] = departmentid;

                AppointmentTypes objAppointmentTypes = new AppointmentTypes();
                DataTable dtAppointmentTypes = new DataTable();
                dtAppointmentTypes = objAppointmentTypes.GetData(parameters);

                Common objCommon = new Common();
                objCommon.InsertRecords(dtAppointmentTypes, "IntelliHAthenaAppointmentTypes", stagingdbconnectionstring, requestid);

                return dtAppointmentTypes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get OpenSchedule

        public DataTable GetOpenSlots(string departmentid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                parameters["api_departmentid"] = departmentid;

                OpenSlots objOpenSlots = new OpenSlots();
                DataTable dtOpenSlots = new DataTable();
                dtOpenSlots = objOpenSlots.GetData(parameters);

                Common objCommon = new Common();
                objCommon.InsertRecords(dtOpenSlots, "IntelliHAthenaOpenSlots", stagingdbconnectionstring, requestid);
                return dtOpenSlots;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get Problems

        public DataTable GetProblems(string patientid, string departmentid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                parameters["patientid"] = patientid;
                parameters["api_departmentid"] = departmentid;

                Problems objProblems = new Problems();
                DataTable dtProblems = new DataTable();
                dtProblems = objProblems.GetData(parameters);

                Common objCommon = new Common();
                objCommon.InsertRecords(dtProblems, "IntelliHAthenaDiagnosis", stagingdbconnectionstring, requestid);
                return dtProblems;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get MedicalHistory

        public DataTable GetPatientMedicalHistory(string patientid, string departmentid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                parameters["patientid"] = patientid;
                parameters["api_departmentid"] = departmentid;

                MedicalHistory objMedicalHistory = new MedicalHistory();
                DataTable dtMedicalHistory = new DataTable();
                dtMedicalHistory = objMedicalHistory.GetData(parameters);

                Common objCommon = new Common();
                objCommon.InsertRecords(dtMedicalHistory, "IntelliHAthenaMedicalHistory", stagingdbconnectionstring, requestid);
                return dtMedicalHistory;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get SurgicalHistory

        public DataTable GetSurgicalHistory(string patientid, string departmentid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                parameters["patientid"] = patientid;
                parameters["api_departmentid"] = departmentid;

                SurgicalHistory objSurgicalHistory = new SurgicalHistory();
                DataTable dtSurgicalHistory = new DataTable();
                dtSurgicalHistory = objSurgicalHistory.GetData(parameters);

                Common objCommon = new Common();
                objCommon.InsertRecords(dtSurgicalHistory, "IntelliHAthenaSurgicalHistory", stagingdbconnectionstring, requestid);
                return dtSurgicalHistory;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get RiskContract

        public DataTable GetPatientRiskContract(string patientid, string departmentid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                parameters["patientid"] = patientid;
                parameters["api_departmentid"] = departmentid;

                RiskContract objRiskContract = new RiskContract();
                DataTable dtRiskContract = new DataTable();
                dtRiskContract = objRiskContract.GetData(parameters);

                Common objCommon = new Common();
                objCommon.InsertRecords(dtRiskContract, "IntelliHAthenaRiskContract", stagingdbconnectionstring, requestid);

                return dtRiskContract;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get CarePlan

        public DataTable GetPatientCarePlan(string patientid, string departmentid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                parameters["patientid"] = patientid;
                parameters["api_departmentid"] = departmentid;

                CarePlan objCarePlan = new CarePlan();
                DataTable dtCarePlan = new DataTable();
                dtCarePlan = objCarePlan.GetData(parameters);

                Common objCommon = new Common();
                objCommon.InsertRecords(dtCarePlan, "IntelliHAthenaCarePlan", stagingdbconnectionstring, requestid);

                return dtCarePlan;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get CareTeam

        public DataTable GetPatientCareTeam(string patientid, string departmentid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                parameters["patientid"] = patientid;
                parameters["api_departmentid"] = departmentid;

                CareTeam objCareTeam = new CareTeam();
                DataTable dtCareTeam = new DataTable();
                dtCareTeam = objCareTeam.GetData(parameters);

                Common objCommon = new Common();
                objCommon.InsertRecords(dtCareTeam, "IntelliHAthenaCareTeam", stagingdbconnectionstring, requestid);

                return dtCareTeam;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get Patient Picture

        //Data Source=WINDOWS-7J7SHKG\SQLEXPRESS;Initial Catalog=IntelliHStagingAthena;User ID=sa;Password=Sahil@123
        public DataTable GetPatientPicture(string patientid, string departmentid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                parameters["patientid"] = patientid;
                parameters["api_departmentid"] = departmentid;

                PatientPicture objPatientPicture = new PatientPicture();
                DataTable dtPictures = new DataTable();
                dtPictures = objPatientPicture.GetData(parameters);

                //Common objCommon = new Common();
                //objCommon.InsertRecords(dtPictures, "IntelliHAthenaPictures", stagingdbconnectionstring, requestid);

                return dtPictures;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SavePictures(DataTable dtPictures, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                Common objCommon = new Common();
                objCommon.InsertRecords(dtPictures, "Athena.PatientPicture", stagingdbconnectionstring, requestid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetJSONPictures(DataTable dtPictures, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                PatientPicture objPictures = new PatientPicture();
                string JSONString = objPictures.GenerateAPIJSONString(dtPictures, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Post Clinical Documents

        //Data Source=WINDOWS-7J7SHKG\SQLEXPRESS;Initial Catalog=IntelliHStagingAthena;User ID=sa;Password=Sahil@123
        public string PostClinicalDocuments(string patientid, string departmentid, string filepath, string requestid)
        {
            try
            {
                parameters["patientid"] = patientid;
                parameters["api_departmentid"] = departmentid;

                ClinicalDocuments objClinicalDocuments = new ClinicalDocuments();

                string result = objClinicalDocuments.PostData(parameters, filepath);

                //Common objCommon = new Common();
                //objCommon.InsertRecords(dtInsurances, "IntelliHAthenaInsurances", stagingdbconnectionstring, requestid);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
