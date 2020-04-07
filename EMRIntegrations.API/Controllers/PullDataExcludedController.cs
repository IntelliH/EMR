using EMRIntegrations.Core.Enums;
using EMRIntegrations.Core.Models;
using EMRIntegrations.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EMRIntegrations.API.Controllers
{
    public class PullDataExcludedController : Controller
    {
        Log objLog = new Log();

        ConnectionModel oMasterConnection = new ConnectionModel();

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public string GetModuleData(string EMRID, string ModuleID, string EMRPatientID)
        {
            try
            {
                //string EMRID = "1";
                //string ModuleID = "11";
                string UserID = string.Empty;
                string PatDemoAPIURL = "";
                //string EMRPatientID = "1447";
                string DepartmentID = string.Empty;

                // Get the variable values from JSON

                oMasterConnection.EMRIntegrationMasterConStr = ConfigurationManager.ConnectionStrings["EMRIntegrationMaster"].ConnectionString;
                SqlConnection oConnMaster = new SqlConnection(oMasterConnection.EMRIntegrationMasterConStr);
                oMasterConnection.oConnEmrMaster = oConnMaster;

                oMasterConnection.EmrStagingDBConStr = ConfigurationManager.ConnectionStrings["EMRStagingDB"].ConnectionString;
                SqlConnection oConnStaging = new SqlConnection(oMasterConnection.EmrStagingDBConStr);
                oMasterConnection.oConnEmrStagingDB = oConnStaging;

                string RequestID = objLog.LogRequest(oMasterConnection, Logs.Status.GotDataPullRequestFromIntelliH, EMRID, ModuleID, EMRPatientID);

                switch (EMRID)
                {
                    case "1"://Athena
                        objLog.LogRequest(oMasterConnection, Logs.Status.ConnectionRequestSentToEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                        EMRIntegrations.Athena.AthenaAPI objAthenaAPI = new EMRIntegrations.Athena.AthenaAPI();
                        objLog.LogRequest(oMasterConnection, Logs.Status.ConnectionEstablishedWithEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                        objLog.LogRequest(oMasterConnection, Logs.Status.DataPullRequestSendingToEMR, RequestID, EMRID, ModuleID, EMRPatientID);

                        switch (ModuleID)
                        {
                            case "11"://PatientDemographics
                                DataTable dtPatientData = objAthenaAPI.GetPatientDemographics(EMRPatientID, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objAthenaAPI.SavePatientDemographics(dtPatientData, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataSavedInStagingDatabase, RequestID, EMRID, ModuleID, EMRPatientID);
                                string JSONString = objAthenaAPI.GetJSONPatientDemographics(dtPatientData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);

                                PostDataToIntelliH("https://apis-dev.intellih.com/api/EMR/SaveDataFromStagingToIntelliH", JSONString);

                                break;
                            case "5"://Medication
                                DataTable dtMedication = objAthenaAPI.GetMedications(EMRPatientID, DepartmentID, oMasterConnection.EmrStagingDBConStr, RequestID);
                                objLog.LogRequest(oMasterConnection, Logs.Status.DataPulledInMemoryFromEMR, RequestID, EMRID, ModuleID, EMRPatientID);
                                objAthenaAPI.SaveMedications(dtMedication, oMasterConnection.EmrStagingDBConStr, RequestID);
                                break;
                            default:
                                break;
                        }
                        objLog.LogRequest(oMasterConnection, Logs.Status.DataSentToIntelliH, RequestID, EMRID, ModuleID, EMRPatientID);
                        break;
                    default:
                        return "EMRID is not found";
                }

                //if (EMRIntegration.Data.SqlHelper.TestSQLConnection(Startup.ConnectionString) == false)
                //{
                //    //Generate Log file   
                //    return new DataTable();
                //}

                return "Data has been posted to IntelliH database";
            }
            catch (Exception ex)
            {
                return ex.Message + " " + ex.StackTrace;
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