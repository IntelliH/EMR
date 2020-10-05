using EMRIntegrations.EpicR4.ModuleClasses;
using System;
using System.Collections;
using System.Data;

namespace EMRIntegrations.EpicR4
{
    public class EpicR4API
    {
        Hashtable parameters = new Hashtable();

        #region Get Patient Demographics
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

        public void SavePatientDemographics(DataTable dtPatientDemographics, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                Common objCommon = new Common();
                objCommon.InsertRecords(dtPatientDemographics, "Epic.PatientDemographics", stagingdbconnectionstring, requestid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetJSONPatientDemographics(DataTable dtPatientDemographics, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                PatientDemographics objPatientDemographics = new PatientDemographics();
                string JSONString = objPatientDemographics.GenerateAPIJSONString(dtPatientDemographics, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Get Patient Medications
        public DataTable GetMedications(string patientid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                DataTable dtMedicationData = new DataTable();

                parameters["patientid"] = patientid;

                Medications objMedications = new Medications();
                dtMedicationData = objMedications.GetData(parameters);

                //Common objCommon = new Common();
                //objCommon.InsertRecords(dtPatientData, "IntelliHAthenaPatientDemographics", stagingdbconnectionstring, requestid);

                return dtMedicationData;
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
                objCommon.InsertRecords(dtMedications, "Epic.Medications", stagingdbconnectionstring, requestid);
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

        #region Get Patient Allergies
        public DataTable GetAllergy(string patientid, string stagingdbconnectionstring, string requestid)
        {
            try
            {
                DataTable dtAllergyData = new DataTable();

                parameters["patientid"] = patientid;

                Allergy objAllergy = new Allergy();
                dtAllergyData = objAllergy.GetData(parameters);

                //Common objCommon = new Common();
                //objCommon.InsertRecords(dtPatientData, "IntelliHAthenaPatientDemographics", stagingdbconnectionstring, requestid);

                return dtAllergyData;
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
                objCommon.InsertRecords(dtAllergy, "Epic.Allergy", stagingdbconnectionstring, requestid);
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
