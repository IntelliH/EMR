using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.IO;
using System.Web.Mvc;
using EMRIntegrations.Endpoints.WellSky;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace EMRIntegrations.Controllers
{
    public class IntelliHEndPointController : ApiController
    {
        [System.Web.Http.HttpPost]
        public string PostHL7(JObject jsonFile)
        {
            //string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            try
            {
                string hl7msg = jsonFile["HL7Message"].ToString();

                //string jsonContent = jsonFile.ToString();
                //string hl7msg = "MSH|^~\\&|WellSky|44935|IntelliH|IntelliH-FACILITY|202104271600-0500||ADT^A01|2|P|2.4|\rEVN|A01|201908191159-0500\rPID|1||0000-000-002||Test^Patient^P||19640101|M|||123 Some Street^^Some City^TX^12345^^^^||1234567890||||||0001|||||||||||N\rPV1|1|U|^^^^^1||||0000000006^Another Test^Physician^|0000000004^Test^Physician^||||||||||||||||||||||||||||||||||Team E||201908190000-0500|\rPV2|||||||||||||||||||||||Some Hospice, LLC\rDG1|1|I10|Z51.1^Encounter for palliative care^I10|Encounter for palliative care||A|||||||||1\rDG1|2|I10|D63^Anemia in other chronic diseases classified elsewhere^I10|Anemia in other chronic diseases classified elsewhere||A|||||||||2\rDG1|3|I10|C25^Malignant neoplasm of head of pancreas^I10|Malignant neoplasm of head of pancreas||A|||||||||3\r";

                //File.WriteAllText(Path.Combine(path.Replace("file:\\", string.Empty), "WellSky", "WellSky_HL7Endpoint_Files_Called_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Millisecond + ".txt"), jsonContent);

                //string responseMessage = "MSH|^~\\&|IntelliH|IntelliH-FACILITY|WELLSKY|12345|20180821084905||ACK^A01|0228506632277834|P|2.4" + Environment.NewLine + "MSA|AA|633|Application Accept ADT message successfully received and queued for processing.";

                ACK objACK = new ACK();
                MSH oMSH = objACK.fetchMSHComponents(hl7msg);
                StringBuilder responseMessage = new StringBuilder();
                responseMessage.Append("MSH|");
                responseMessage.Append(string.Concat(oMSH.encodingcharactors, oMSH.fieldseparator));
                responseMessage.Append(string.Concat(oMSH.sendingapplication, oMSH.fieldseparator));
                responseMessage.Append(string.Concat(oMSH.sendingfacility, oMSH.fieldseparator));
                responseMessage.Append(string.Concat(oMSH.receivingapplication, oMSH.fieldseparator));
                responseMessage.Append(string.Concat(oMSH.receivingfacility, oMSH.fieldseparator));
                responseMessage.Append(string.Concat(oMSH.creationdatetime, oMSH.fieldseparator));
                responseMessage.Append(string.Concat(oMSH.security, oMSH.fieldseparator));
                responseMessage.Append(string.Concat(oMSH.messagetype,"^", oMSH.triggerevent, oMSH.fieldseparator));
                responseMessage.Append(string.Concat(oMSH.messagecontrolid, oMSH.fieldseparator));
                responseMessage.Append(string.Concat(oMSH.processingid, oMSH.fieldseparator));
                responseMessage.Append(oMSH.versionid);
                responseMessage.Append(Environment.NewLine);
                responseMessage.Append(string.Concat("MSA|AA|", oMSH.messagecontrolid, "|Application Accept ADT message successfully received and queued for processing."));

                saveHL7(oMSH.messagecontrolid, hl7msg);

                return responseMessage.ToString();

                //return data;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(path.Replace("file:\\", string.Empty), "WellSky", "WellSky_HL7Endpoint_Files_Error_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Millisecond + ".txt"), ex.Message + Environment.NewLine + ex.StackTrace);
                return ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }

        //[AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public string NetSmartPostHL7(JObject jsonFile)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            try
            {
                string jsonContent = jsonFile.ToString();

                File.WriteAllText(Path.Combine(path.Replace("file:\\", string.Empty), "NetSmart", "NetSmart_HL7Endpoint_Files_Called_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Millisecond + ".txt"), jsonContent);

                string responseMessage = "MSH|^~\\&|IntelliH|IntelliH-FACILITY|NETSMART|12345|20180821084905||ACK^A01|0228506632277834|P|2.4" + Environment.NewLine + "MSA|AA|633|Application Accept ADT message successfully received and queued for processing.";

                return responseMessage;

                //return data;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(path.Replace("file:\\", string.Empty), "NetSmart", "NetSmart_HL7Endpoint_Files_Error_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Millisecond + ".txt"), ex.Message + Environment.NewLine + ex.StackTrace);
                return ex.Message + Environment.NewLine + ex.StackTrace;
            }
        }

        private void saveHL7(string messagecontrolid, string hl7msg)
        {
            try
            {
                SqlConnection oConnStaging = new SqlConnection(ConfigurationManager.ConnectionStrings["EMRStagingDB"].ConnectionString);

                if (oConnStaging.State == ConnectionState.Closed)
                {
                    oConnStaging.Open();
                }

                SqlCommand oCmd = new SqlCommand();
                oCmd.CommandText = "insert into wellsky.hl7 (messagecontrolid, hl7msg, creationdatetime) values ('"+ messagecontrolid +"', '"+ hl7msg + "', getdate())";
                oCmd.Connection = oConnStaging;

                oCmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}