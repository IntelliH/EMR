using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace EMRIntegrations.Athena
{
    public class ClinicalDocuments
    {
        public string PostData(Hashtable parameters, string filePath)
        {
            try
            {
                Byte[] bytes = File.ReadAllBytes(filePath);
                String file = Convert.ToBase64String(bytes);

                //string file = File.ReadAllText( filePath);
                
                string result = PostClinicalDocument(parameters, file);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string PostClinicalDocument(Hashtable parameters, string fileContent)
        {
            try
            {
                APIConnection api = (APIConnection)parameters["athenaapiobject"];

                Dictionary<string, string> dirlst = new Dictionary<string, string>()
                    {
                        { "attachmentcontents", "\"" + fileContent + "\""},
                        { "attachmenttype", "PDF"},
                        { "departmentid", parameters["api_departmentid"].ToString()},
                        { "documentsubclass", "CLINICALDOCUMENT"}
                    };

                JObject jobj = (JObject)api.POST("patients/" + parameters["patientid"].ToString() + "/documents/clinicaldocument", dirlst);
                //JToken jtobj = jobj["clinicaldocument"];
                //if (jobj != null)
                //{
                //    if (jobj.HasValues && jobj.SelectToken("error") != null)
                //    {
                //        return string.Empty;
                //        //dtValidation.Rows.Add(new object[] { parameters["patientid"].ToString(), "", "", "", "", strModuleName, string.Empty, jtobj["error"].ToString() + parameters["patientid"].ToString() });
                //        //{ throw new Exception(jtobj["error"].ToString()); }

                //    }
                //}
                return jobj.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
