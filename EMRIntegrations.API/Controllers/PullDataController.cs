using EMRIntegrations.Core.Enums;
using EMRIntegrations.Core.Models;
using EMRIntegrations.Data;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Http;

namespace EMRIntegrations.Controllers
{
    public class PullDataController : ApiController
    {
        Log objLog = new Log();

        ConnectionModel oMasterConnection = new ConnectionModel();

        public string GetModuleData(string EMRID, string ModuleID, string EMRPatientID, string AccessToken)
        {
            StringBuilder JSONStringReturn = new StringBuilder();
            string RequestID = string.Empty;

            try
            {
                string UserID = string.Empty;
                string DepartmentID = "1";
                string JSONString = string.Empty;

                // Get the variable values from JSON

                oMasterConnection.EMRIntegrationMasterConStr = ConfigurationManager.ConnectionStrings["EMRIntegrationMaster"].ConnectionString;
                SqlConnection oConnMaster = new SqlConnection(oMasterConnection.EMRIntegrationMasterConStr);
                oMasterConnection.oConnEmrMaster = oConnMaster;

                oMasterConnection.EmrStagingDBConStr = ConfigurationManager.ConnectionStrings["EMRStagingDB"].ConnectionString;
                SqlConnection oConnStaging = new SqlConnection(oMasterConnection.EmrStagingDBConStr);
                oMasterConnection.oConnEmrStagingDB = oConnStaging;

                switch (EMRID)
                {
                    case "1"://Athena
                        EMRIntegrations.Athena.AthenaAPI objAthenaAPI;
                        switch (ModuleID)
                        {
                            case "11"://PatientDemographics
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objAthenaAPI = new EMRIntegrations.Athena.AthenaAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtPatientData = objAthenaAPI.GetPatientDemographics(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objAthenaAPI.SavePatientDemographics(dtPatientData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objAthenaAPI.GetJSONPatientDemographics(dtPatientData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);

                                //PostDataToIntelliH("https://apis-dev.intellih.com/api/EMR/SaveDataFromStagingToIntelliH", JSONString);

                                break;
                            case "15"://Allergy
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objAthenaAPI = new EMRIntegrations.Athena.AthenaAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtAllergy = objAthenaAPI.GetAllergy(EMRPatientID, DepartmentID, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objAthenaAPI.SaveAllergy(dtAllergy, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objAthenaAPI.GetJSONAllergy(dtAllergy, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                                break;
                            case "5"://Medications
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objAthenaAPI = new EMRIntegrations.Athena.AthenaAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtMedication = objAthenaAPI.GetMedications(EMRPatientID, DepartmentID, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objAthenaAPI.SaveMedications(dtMedication, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objAthenaAPI.GetJSONMedications(dtMedication, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                                break;
                            case "12"://PatientInsurance
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objAthenaAPI = new EMRIntegrations.Athena.AthenaAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtInsurance = objAthenaAPI.GetInsurances(EMRPatientID, DepartmentID, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objAthenaAPI.SaveInsurances(dtInsurance, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objAthenaAPI.GetJSONInsurances(dtInsurance, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                                break;
                            case "13"://PatientPicture
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objAthenaAPI = new EMRIntegrations.Athena.AthenaAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtPicture = objAthenaAPI.GetPatientPicture(EMRPatientID, DepartmentID, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objAthenaAPI.SavePictures(dtPicture, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objAthenaAPI.GetJSONPictures(dtPicture, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                                break;
                            default:
                                JSONStringReturn.Append("{");
                                JSONStringReturn.Append("\"EMRPatientId\":" + "\"" + EMRPatientID + "\",");
                                JSONStringReturn.Append("\"EMRId\":" + "\"" + EMRID + "\",");
                                JSONStringReturn.Append("\"ModuleId\":" + "\"" + ModuleID + "\",");
                                JSONStringReturn.Append("\"RequestId\":" + "\"" + RequestID + "\",");
                                JSONStringReturn.Append("\"CreatedBy\":" + "\"\",");
                                JSONStringReturn.Append("\"EMRUserExtensionLogDetails\":");
                                JSONStringReturn.Append("\"ModuleID is not Found\"");
                                JSONStringReturn.Append("}");
                                return JSONStringReturn.ToString();
                        }
                        objLog.LogRequest(oMasterConnection, Logs.Status.DataSentToIntelliH, RequestID, EMRID, ModuleID, EMRPatientID);
                        break;
                    case "2"://Cerner
                        EMRIntegrations.CernerMillenium.CernerMilleniumAPI objCernerAPI;
                        switch (ModuleID)
                        {
                            case "11"://PatientDemographics
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objCernerAPI = new EMRIntegrations.CernerMillenium.CernerMilleniumAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtPatientData = objCernerAPI.GetPatientDemographics(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID);

                                if (dtPatientData.Rows.Count == 0)
                                {
                                    JSONStringReturn.Append("{");
                                    JSONStringReturn.Append("\"EMRPatientId\":" + "\"" + EMRPatientID + "\",");
                                    JSONStringReturn.Append("\"EMRId\":" + "\"" + EMRID + "\",");
                                    JSONStringReturn.Append("\"ModuleId\":" + "\"" + ModuleID + "\",");
                                    JSONStringReturn.Append("\"RequestId\":" + "\"" + RequestID + "\",");
                                    JSONStringReturn.Append("\"CreatedBy\":" + "\"\",");
                                    JSONStringReturn.Append("\"EMRUserExtensionLogDetails\":");
                                    JSONStringReturn.Append("\"No Patient Found\"");
                                    JSONStringReturn.Append("}");
                                    return JSONStringReturn.ToString();
                                }

                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objCernerAPI.SavePatientDemographics(dtPatientData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objCernerAPI.GetJSONPatientDemographics(dtPatientData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);

                                //PostDataToIntelliH("https://apis-dev.intellih.com/api/EMR/SaveDataFromStagingToIntelliH", JSONString);

                                break;
                            case "15"://Allergy
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objCernerAPI = new EMRIntegrations.CernerMillenium.CernerMilleniumAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtAllergyData = objCernerAPI.GetAllergy(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objCernerAPI.SaveAllergy(dtAllergyData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objCernerAPI.GetJSONAllergy(dtAllergyData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                                break;
                            case "5"://Medications
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objCernerAPI = new EMRIntegrations.CernerMillenium.CernerMilleniumAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtMedicationData = objCernerAPI.GetMedications(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objCernerAPI.SaveMedications(dtMedicationData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objCernerAPI.GetJSONMedications(dtMedicationData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);

                                //PostDataToIntelliH("https://apis-dev.intellih.com/api/EMR/SaveDataFromStagingToIntelliH", JSONString);

                                break;
                            default:
                                JSONStringReturn.Append("{");
                                JSONStringReturn.Append("\"EMRPatientId\":" + "\"" + EMRPatientID + "\",");
                                JSONStringReturn.Append("\"EMRId\":" + "\"" + EMRID + "\",");
                                JSONStringReturn.Append("\"ModuleId\":" + "\"" + ModuleID + "\",");
                                JSONStringReturn.Append("\"RequestId\":" + "\"" + RequestID + "\",");
                                JSONStringReturn.Append("\"CreatedBy\":" + "\"\",");
                                JSONStringReturn.Append("\"EMRUserExtensionLogDetails\":");
                                JSONStringReturn.Append("\"ModuleID is not Found\"");
                                JSONStringReturn.Append("}");
                                return JSONStringReturn.ToString();
                        }
                        objLog.LogRequest(oMasterConnection, Logs.Status.DataSentToIntelliH, RequestID, EMRID, ModuleID, EMRPatientID);
                        break;
                    case "3"://Epic
                        EMRIntegrations.Epic.EpicAPI objEpicAPI;
                        switch (ModuleID)
                        {
                            case "11"://PatientDemographics
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objEpicAPI = new EMRIntegrations.Epic.EpicAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtPatientData = objEpicAPI.GetPatientDemographics(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID);

                                if (dtPatientData.Rows.Count == 0)
                                {
                                    JSONStringReturn.Append("{");
                                    JSONStringReturn.Append("\"EMRPatientId\":" + "\"" + EMRPatientID + "\",");
                                    JSONStringReturn.Append("\"EMRId\":" + "\"" + EMRID + "\",");
                                    JSONStringReturn.Append("\"ModuleId\":" + "\"" + ModuleID + "\",");
                                    JSONStringReturn.Append("\"RequestId\":" + "\"" + RequestID + "\",");
                                    JSONStringReturn.Append("\"CreatedBy\":" + "\"\",");
                                    JSONStringReturn.Append("\"EMRUserExtensionLogDetails\":");
                                    JSONStringReturn.Append("\"No Patient Found\"");
                                    JSONStringReturn.Append("}");
                                    return JSONStringReturn.ToString();
                                }

                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objEpicAPI.SavePatientDemographics(dtPatientData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objEpicAPI.GetJSONPatientDemographics(dtPatientData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                                break;
                            case "15"://Allergy
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objEpicAPI = new EMRIntegrations.Epic.EpicAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtAllergyData = objEpicAPI.GetAllergy(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objEpicAPI.SaveAllergy(dtAllergyData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objEpicAPI.GetJSONAllergy(dtAllergyData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                                break;
                            case "5"://Medications
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objEpicAPI = new EMRIntegrations.Epic.EpicAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtMedicationData = objEpicAPI.GetMedications(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objEpicAPI.SaveMedications(dtMedicationData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objEpicAPI.GetJSONMedications(dtMedicationData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);

                                //PostDataToIntelliH("https://apis-dev.intellih.com/api/EMR/SaveDataFromStagingToIntelliH", JSONString);

                                break;
                            case "1"://Vitals
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objEpicAPI = new EMRIntegrations.Epic.EpicAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtVitalsData = objEpicAPI.GetVitals(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objEpicAPI.SaveVitals(dtVitalsData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objEpicAPI.GetJSONVitals(dtVitalsData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                                break;
                            default:
                                JSONStringReturn.Append("{");
                                JSONStringReturn.Append("\"EMRPatientId\":" + "\"" + EMRPatientID + "\",");
                                JSONStringReturn.Append("\"EMRId\":" + "\"" + EMRID + "\",");
                                JSONStringReturn.Append("\"ModuleId\":" + "\"" + ModuleID + "\",");
                                JSONStringReturn.Append("\"RequestId\":" + "\"" + RequestID + "\",");
                                JSONStringReturn.Append("\"CreatedBy\":" + "\"\",");
                                JSONStringReturn.Append("\"EMRUserExtensionLogDetails\":");
                                JSONStringReturn.Append("\"ModuleID is not Found\"");
                                JSONStringReturn.Append("}");
                                return JSONStringReturn.ToString();
                        }
                        objLog.LogRequest(oMasterConnection, Logs.Status.DataSentToIntelliH, RequestID, EMRID, ModuleID, EMRPatientID);
                        break;
                    case "5"://EpicR4
                        EMRIntegrations.EpicR4.EpicR4API objEpicR4API;
                        switch (ModuleID)
                        {
                            case "11"://PatientDemographics
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objEpicR4API = new EMRIntegrations.EpicR4.EpicR4API();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtPatientData = objEpicR4API.GetPatientDemographics(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID);

                                if (dtPatientData.Rows.Count == 0)
                                {
                                    JSONStringReturn.Append("{");
                                    JSONStringReturn.Append("\"EMRPatientId\":" + "\"" + EMRPatientID + "\",");
                                    JSONStringReturn.Append("\"EMRId\":" + "\"" + EMRID + "\",");
                                    JSONStringReturn.Append("\"ModuleId\":" + "\"" + ModuleID + "\",");
                                    JSONStringReturn.Append("\"RequestId\":" + "\"" + RequestID + "\",");
                                    JSONStringReturn.Append("\"CreatedBy\":" + "\"\",");
                                    JSONStringReturn.Append("\"EMRUserExtensionLogDetails\":");
                                    JSONStringReturn.Append("\"No Patient Found\"");
                                    JSONStringReturn.Append("}");
                                    return JSONStringReturn.ToString();
                                }

                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objEpicR4API.SavePatientDemographics(dtPatientData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objEpicR4API.GetJSONPatientDemographics(dtPatientData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                                break;
                            case "15"://Allergy
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objEpicR4API = new EMRIntegrations.EpicR4.EpicR4API();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtAllergyData = objEpicR4API.GetAllergy(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objEpicR4API.SaveAllergy(dtAllergyData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objEpicR4API.GetJSONAllergy(dtAllergyData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                                break;
                            case "5"://Medications
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objEpicR4API = new EMRIntegrations.EpicR4.EpicR4API();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtMedicationData = objEpicR4API.GetMedications(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objEpicR4API.SaveMedications(dtMedicationData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objEpicR4API.GetJSONMedications(dtMedicationData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);

                                //PostDataToIntelliH("https://apis-dev.intellih.com/api/EMR/SaveDataFromStagingToIntelliH", JSONString);

                                break;
                            default:
                                JSONStringReturn.Append("{");
                                JSONStringReturn.Append("\"EMRPatientId\":" + "\"" + EMRPatientID + "\",");
                                JSONStringReturn.Append("\"EMRId\":" + "\"" + EMRID + "\",");
                                JSONStringReturn.Append("\"ModuleId\":" + "\"" + ModuleID + "\",");
                                JSONStringReturn.Append("\"RequestId\":" + "\"" + RequestID + "\",");
                                JSONStringReturn.Append("\"CreatedBy\":" + "\"\",");
                                JSONStringReturn.Append("\"EMRUserExtensionLogDetails\":");
                                JSONStringReturn.Append("\"ModuleID is not Found\"");
                                JSONStringReturn.Append("}");
                                return JSONStringReturn.ToString();
                        }
                        objLog.LogRequest(oMasterConnection, Logs.Status.DataSentToIntelliH, RequestID, EMRID, ModuleID, EMRPatientID);
                        break;
                    case "6"://DrChrono
                        EMRIntegrations.DrChrono.DrChronoAPI objDrChronoAPI;
                        switch (ModuleID)
                        {
                            case "11"://PatientDemographics
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objDrChronoAPI = new EMRIntegrations.DrChrono.DrChronoAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtPatientData = objDrChronoAPI.GetPatientDemographics(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID, AccessToken);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objDrChronoAPI.SavePatientDemographics(dtPatientData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objDrChronoAPI.GetJSONPatientDemographics(dtPatientData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                                break;
                            case "5"://Medications
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objDrChronoAPI = new EMRIntegrations.DrChrono.DrChronoAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtMedicationData = objDrChronoAPI.GetMedications(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID, AccessToken);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objDrChronoAPI.SaveMedications(dtMedicationData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objDrChronoAPI.GetJSONMedications(dtMedicationData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                                break;
                            case "12"://Problemlist
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objDrChronoAPI = new EMRIntegrations.DrChrono.DrChronoAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtProblemlistData = objDrChronoAPI.GetProblemlist(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID, AccessToken);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objDrChronoAPI.SaveProblemlist(dtProblemlistData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objDrChronoAPI.GetJSONProblemlist(dtProblemlistData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                                break;
                            case "15"://Allergy
                                RequestID = InitialLog1(EMRID, ModuleID, EMRPatientID);
                                objDrChronoAPI = new EMRIntegrations.DrChrono.DrChronoAPI();
                                InitialLog2(EMRID, ModuleID, EMRPatientID, RequestID);

                                DataTable dtAllergyData = objDrChronoAPI.GetAllergy(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID, AccessToken);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objDrChronoAPI.SaveAllergy(dtAllergyData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                JSONString = objDrChronoAPI.GetJSONAllergy(dtAllergyData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                                break;
                            default:
                                JSONStringReturn.Append("{");
                                JSONStringReturn.Append("\"EMRPatientId\":" + "\"" + EMRPatientID + "\",");
                                JSONStringReturn.Append("\"EMRId\":" + "\"" + EMRID + "\",");
                                JSONStringReturn.Append("\"ModuleId\":" + "\"" + ModuleID + "\",");
                                JSONStringReturn.Append("\"RequestId\":" + "\"" + RequestID + "\",");
                                JSONStringReturn.Append("\"CreatedBy\":" + "\"\",");
                                JSONStringReturn.Append("\"Error\":" + "\"ModuleID is not found\",");
                                JSONStringReturn.Append("\"EMRUserExtensionLogDetails\":{}");
                                JSONStringReturn.Append("}");
                                return JSONStringReturn.ToString();
                        }
                        objLog.LogRequest(oMasterConnection, Logs.Status.DataSentToIntelliH, RequestID, EMRID, ModuleID, EMRPatientID);
                        break;
                    default:
                        JSONStringReturn.Append("{");
                        JSONStringReturn.Append("\"EMRPatientId\":" + "\"" + EMRPatientID + "\",");
                        JSONStringReturn.Append("\"EMRId\":" + "\"" + EMRID + "\",");
                        JSONStringReturn.Append("\"ModuleId\":" + "\"" + ModuleID + "\",");
                        JSONStringReturn.Append("\"RequestId\":" + "\"" + RequestID + "\",");
                        JSONStringReturn.Append("\"CreatedBy\":" + "\"\",");
                        JSONStringReturn.Append("\"Error\":" + "\"EMRID is not Found\",");
                        JSONStringReturn.Append("\"EMRUserExtensionLogDetails\":{}");
                        JSONStringReturn.Append("}");
                        return JSONStringReturn.ToString();
                }

                //if (EMRIntegration.Data.SqlHelper.TestSQLConnection(Startup.ConnectionString) == false)
                //{
                //    //Generate Log file   
                //    return new DataTable();
                //}

                return JSONString;
            }
            catch (Exception ex)
            {
                JSONStringReturn.Append("{");
                JSONStringReturn.Append("\"EMRPatientId\":" + "\"" + EMRPatientID + "\",");
                JSONStringReturn.Append("\"EMRId\":" + "\"" + EMRID + "\",");
                JSONStringReturn.Append("\"ModuleId\":" + "\"" + ModuleID + "\",");
                JSONStringReturn.Append("\"RequestId\":" + "\"" + RequestID + "\",");
                JSONStringReturn.Append("\"CreatedBy\":" + "\"\",");
                JSONStringReturn.Append("\"Error\":" + "\""+ ex.Message + "\",");
                JSONStringReturn.Append("\"EMRUserExtensionLogDetails\":{}");
                JSONStringReturn.Append("}");
                return JSONStringReturn.ToString();
            }
        }

        private string InitialLog1(string EMRID, string ModuleID, string EMRPatientID)
        {
            try
            {
                string RequestID = objLog.LogRequest(oMasterConnection, Logs.Status.GotDataPullRequestFromIntelliH, EMRID, ModuleID, EMRPatientID);
                objLog.LogRequest(oMasterConnection, Logs.Status.ConnectionRequestSentToEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                return RequestID;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string InitialLog2(string EMRID, string ModuleID, string EMRPatientID, string RequestID)
        {
            try
            {
                objLog.LogRequest(oMasterConnection, Logs.Status.ConnectionEstablishedWithEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                objLog.LogRequest(oMasterConnection, Logs.Status.DataPullRequestSendingToEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                return RequestID;
            }
            catch (Exception)
            {
                throw;
            }
        }

        void PostDataToIntelliH(string URL, string JSONString)
        {
            try
            {
                string result = string.Empty;

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(JSONString);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                try
                {
                    using (var response = httpWebRequest.GetResponse() as HttpWebResponse)
                    {
                        if (httpWebRequest.HaveResponse && response != null)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                result = reader.ReadToEnd();
                            }
                        }
                    }
                }
                catch (WebException e)
                {
                    if (e.Response != null)
                    {
                        using (var errorResponse = (HttpWebResponse)e.Response)
                        {
                            using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                            {
                                string error = reader.ReadToEnd();
                                result = error;
                            }
                        }

                    }
                }

                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                //request.Method = "POST";

                //System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                //Byte[] byteArray = encoding.GetBytes(JSONString);

                //request.ContentLength = byteArray.Length;
                //request.ContentType = @"application/json";

                //using (Stream dataStream = request.GetRequestStream())
                //{
                //    dataStream.Write(byteArray, 0, byteArray.Length);
                //}
                //long length = 0;
                //try
                //{
                //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                //    {
                //        length = response.ContentLength;
                //    }
                //}
                //catch (WebException ex)
                //{
                //    // Log exception and throw as for GET example above
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
