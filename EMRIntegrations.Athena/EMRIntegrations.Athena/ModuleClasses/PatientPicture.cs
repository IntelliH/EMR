using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EMRIntegrations.Athena
{
    public class PatientPicture
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
                string strModuleName = Convert.ToString(parameters["modulename"]);
                string strVersion = Convert.ToString(parameters["version"]);

                switch (strVersion)
                {
                    default:
                        dtModuleData = GetPatientPictureData(parameters);
                        break;
                }
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
        /// <param name="connection">Source Database connection</param>
        /// <param name="parameters">number of parameters which are have version,modulename etc...</param>
        /// <returns>Return the datasetof 2 tables,one for module data and another is for errors or debug log or any validation message</returns>
        private DataTable GetPatientPictureData(Hashtable parameters)
        {
            try
            {
                string strModuleName = string.Empty;
                strModuleName = "PatientPicture";

                DataSet dsModuleData = new DataSet(strModuleName);
                DataTable dtClinicaldata = new DataTable();

                // Do data processing here.
                APIConnection api = (APIConnection)parameters["athenaapiobject"];
                List<patientpicture> objPatientPicture = new List<patientpicture>();

                //ArrayList arydept = new ArrayList();
                //arydept = (ArrayList)parameters["api_departmentid"];

                //foreach (var value in arydept)
                //{

                Dictionary<string, string> dirlst = new Dictionary<string, string>()
                     {
                        {"jpegoutput", "false"}
                    };

                JObject jobj = (JObject)api.GET("patients/" + parameters["patientid"].ToString() + "/photo", dirlst);
                JToken jtobj = jobj["image"];

                if (jtobj != null)
                {
                    if (jtobj.HasValues && jtobj.SelectToken("error") != null)
                    {
                        throw new Exception(jtobj["error"].ToString());
                    }

                    //List<JToken> jtobjlst = new List<JToken>();
                    //foreach (var item in jtobj)
                    //{
                    //        jtobjlst.Add(item);
                    //}

                    patientpicture objclinical = new patientpicture();
                    objclinical.Image = jobj["image"].ToString();
                    objPatientPicture.Add(objclinical);
                }

                dtClinicaldata = GetModuleDataBase.ConvertToDataTable(objPatientPicture);

                if (!dtClinicaldata.Columns.Contains("patientid"))
                {
                    dtClinicaldata.Columns.Add("patientid");
                }

                dtClinicaldata = dtClinicaldata.DefaultView.ToTable(true);
                //dsModuleData.Tables.Add(dtValidation);
                //dsModuleData.Tables.Add(dtClinicaldata);

                foreach (DataRow dRow in dtClinicaldata.Rows)
                {
                    dRow["patientid"] = parameters["patientid"];
                }

                return dtClinicaldata;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GenerateAPIJSONString(DataTable dtPictures, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                string JSONString = string.Empty;
                DataTable dtFinalData;
                dtFinalData = dtPictures.Copy();

                JSONString = DataTableToJSONPictures(dtFinalData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string DataTableToJSONPictures(DataTable table, string emrpatientid, string requestid, string emrid, string moduleid, string userid)
        {
            try
            {
                var JSONString = new StringBuilder();

                JSONString.Append("{");
                JSONString.Append("\"EMRPatientId\":" + "\"" + emrpatientid + "\",");
                JSONString.Append("\"EMRId\":" + "\"" + emrid + "\",");
                JSONString.Append("\"ModuleId\":" + "\"" + moduleid + "\",");
                JSONString.Append("\"RequestId\":" + "\"" + requestid + "\",");
                JSONString.Append("\"CreatedBy\":" + "\"\",");

                if (table.Rows.Count == 0)
                {
                    JSONString.Append("\"Error\":" + "\"No Picture Found\",");
                    JSONString.Append("\"Pictures\":{}");
                    JSONString.Append("}");
                    return JSONString.ToString();
                }

                JSONString.Append("\"Error\":" + "\"\",");
                JSONString.Append("\"Pictures\":");
                JSONString.Append("[");

                int counter = 0;
                foreach (DataRow drow in table.Rows)
                {
                    counter++;

                    JSONString.Append("{");

                    JSONString.Append("\"Base64\":" + "\"" + drow["image"].ToString() + "\"");


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
            catch (Exception)
            {
                throw;
            }
        }

        internal class patientpicture
        {
            [JsonProperty("image")]
            public string Image { get; set; }
        }
    }
}
