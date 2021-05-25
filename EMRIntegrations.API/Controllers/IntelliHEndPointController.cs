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

namespace EMRIntegrations.Controllers
{
    public class IntelliHEndPointController : ApiController
    {
        [System.Web.Http.HttpPost]
        public string PostHL7(JObject jsonFile)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            try
            {
                string jsonContent = jsonFile.ToString();

                File.WriteAllText(Path.Combine(path.Replace("file:\\", string.Empty), "WellSky", "WellSky_HL7Endpoint_Files_Called_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Millisecond + ".txt"), jsonContent);

                string responseMessage = "MSH|^~\\&|IntelliH|IntelliH-FACILITY|WELLSKY|12345|20180821084905||ACK^A01|0228506632277834|P|2.4" + Environment.NewLine + "MSA|AA|633|Application Accept ADT message successfully received and queued for processing.";

                return responseMessage;

                //return data;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(path.Replace("file:\\", string.Empty), "WellSky","WellSky_HL7Endpoint_Files_Error_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Millisecond + ".txt"), ex.Message + Environment.NewLine + ex.StackTrace);
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
    }
}