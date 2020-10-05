using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using RestSharp;
using System.Net.Http;
using System.IO;
using System.Web;
using System.Net;
using System.Net.Http.Headers;

namespace EMRIntegrations.DrChrono
{
    class DocumentUpload
    {
        public DataTable PostData(Hashtable parameters)
        {
            try
            {
                DataTable dtModuleData = new DataTable();
                dtModuleData = PostDocumentUploadData(parameters);
                return dtModuleData;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private DataTable PostDocumentUploadData(Hashtable parameters)
        {
            try
            {

                string strModuleName = "DocumentUpload";

                //DataSet dsModuleData = new DataSet(strModuleName);
                DataTable dtClinicaldata = new DataTable();
                ////var form = new MultipartFormDataContent();

                ////using (form)
                ////using (var stream = File.OpenRead(@"Path\To\Your\File.ext"))
                ////using (var streamContent = new StreamContent(stream))
                ////{
                ////    form.Add(streamContent, "files");
                ////    form.Add(new StringContent("<{\"DocName\": \"Test\"}>"), "Info");
                ////    //response = await client.PostAsync(null, form);
                ////}

                ////RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();

                ////var stream = File.OpenRead(@"E:\IntelliH_Inc\TestPDF.pdf");
                ////var streamContent = new StreamContent(stream);

                ////Byte[] bytes = File.ReadAllBytes(@"E:\IntelliH_Inc\TestPDF.pdf");
                ////String file = Convert.ToBase64String(bytes);



                ////JObject jobj = (JObject)api.POST("patients/" + parameters["patientid"].ToString() + "/documents/clinicaldocument", dirlst);


                ////JToken jtobj = jobj["clinicaldocument"];
                ////if (jobj != null)
                ////{
                ////    if (jobj.HasValues && jobj.SelectToken("error") != null)
                ////    {
                ////        return string.Empty;
                ////        //dtValidation.Rows.Add(new object[] { parameters["patientid"].ToString(), "", "", "", "", strModuleName, string.Empty, jtobj["error"].ToString() + parameters["patientid"].ToString() });
                ////        //{ throw new Exception(jtobj["error"].ToString()); }

                ////    }
                ////}
                ////return jobj.ToString();

                ////Stream oStream = File.OpenRead(@"E:\IntelliH_Inc\TestPDF.pdf");

                //var client = new RestClient("" + parameters["api_baseurl"].ToString() + "api/documents" + "");
                //var request = new RestRequest(Method.POST);
                //request.AddHeader("cache-control", "no-cache");
                //request.AddHeader("authorization", "bearer:" + parameters["access_token"].ToString() + "");
                ////request.AddHeader("Content-Type", "multipart/form-data");
                ////request.AddFile("document", @"E:\IntelliH_Inc\TestPDF.pdf", "multipart/form-data");
                //request.AlwaysMultipartFormData = true;
                ////Byte[] bytes = File.ReadAllBytes(@"E:\IntelliH_Inc\TestPDF.pdf");
                ////String file = Convert.ToBase64String(bytes);
                ////FileInfo oFileInfo = new FileInfo(@"E:\IntelliH_Inc\TestPDF.pdf");

                ////Dictionary<string, string> dirlst = new Dictionary<string, string>()
                ////    {
                ////        { "document", "\"" + file + "\""},
                ////        { "patient", "\"" + parameters["patientid"].ToString() + "\""},
                ////        { "doctor", "269552" },
                ////        { "date", "2014-02-24"},
                ////        { "description", "Test Document 2"}
                ////    };


                //request.RequestFormat = DataFormat.Json;
                ////Stream fileStream = System.IO.File.OpenRead(@"E:\IntelliH_Inc\TestPDF.pdf");

                //byte[] byteArray = Encoding.UTF8.GetBytes(File.ReadAllText(@"E:\IntelliH_Inc\TestPDF.pdf"));

                //request.AddJsonBody(new
                //{
                //    //document = new StreamContent(fileStream), //streamContent,
                //    doctor = "269552",//parameters["doctorid"].ToString(),
                //    patient = parameters["patientid"].ToString(),
                //    description = "Test Document 2",//parameters["description"].ToString()
                //    date = "2014-02-24"//parameters["date"].ToString(),
                //    //document = @"E:\IntelliH_Inc\TestPDF.pdf"
                //    //document = byteArray
                //});

                //request.AddFile("document", @"E:\IntelliH_Inc\TestPDF.pdf", "multipart/form-data");

                //IRestResponse response = client.Execute(request);

                //JToken jtobj = (JToken)JsonConvert.DeserializeObject("[" + response.Content.Replace("[", "\"[").Replace("]", "]\"") + "]");








                //var client = new RestClient("https://drchrono.com/api/documents?access_token="+ parameters["access_token"].ToString() + "");
                var client = new RestClient("https://drchrono.com/api/documents");
                client.Timeout = -1;

                var request = new RestRequest(Method.POST);
                request.AddHeader("authorization", "bearer:" + parameters["access_token"].ToString() + "");

                request.AddFile("document", @"E:\IntelliH_Inc\TestPDF.pdf");
                request.AddParameter("patient", "87734739");
                request.AddParameter("doctor", "269552");
                request.AddParameter("date", "2014-02-24");
                request.AddParameter("description", "Test Document 3");
                IRestResponse response = client.Execute(request);


                int i = 1;







                //using (var client = new HttpClient())
                //{
                //    using (var formData = new MultipartFormDataContent())
                //    {
                //        formData.Add(stringContent1, "P1"); // Le paramètre P1 aura la valeur contenue dans param1String
                //        formData.Add(stringContent2, "P2"); // Le parmaètre P2 aura la valeur contenue dans param2String
                //        formData.Add(fileStreamContent, "FICHIER", "RETURN.xml");
                //        //  formData.Add(bytesContent, "file2", "file2");
                //        try
                //        {
                //            var response = client.PostAsync(actionUrl, formData).Result;
                //            MessageBox.Show(response.ToString());
                //            if (!response.IsSuccessStatusCode)
                //            {
                //                MessageBox.Show("Erreur de réponse");
                //            }
                //        }
                //        catch (Exception Error)
                //        {
                //            MessageBox.Show(Error.Message);
                //        }
                //        finally
                //        {
                //            client.CancelPendingRequests();
                //        }
                //    }
                //}









                //HttpWebResponse response = null;
                //try
                //{
                //    // Create the data to send

                //    StringBuilder stringBuilder = new StringBuilder();

                //    var headers = string.Format("{{'Authorization': 'Bearer {0}',}}", parameters["access_token"].ToString());

                //    stringBuilder.Append("headers=" + Uri.EscapeDataString(headers));

                //    var data = "{'doctor': 269552,'patient': " + parameters["patientid"].ToString() + ",'description': 'Test Document 2','date': '2014-02-24',}";

                //    stringBuilder.Append("&data=" + Uri.EscapeDataString(data));

                //    var pdfFileText = string.Format("{{'document': {0}}}", System.IO.File.ReadAllText(@"E:\IntelliH_Inc\TestPDF.pdf"));

                //    stringBuilder.Append("&files=" + pdfFileText);

                //    // Create a byte array of the data to be sent

                //    byte[] byteArray = Encoding.UTF8.GetBytes(stringBuilder.ToString());

                //    // Setup the Request

                //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(parameters["api_baseurl"].ToString() + "api/documents");

                //    request.Method = "POST";

                //    request.ContentType = "multipart/form-data";

                //    request.ContentLength = byteArray.Length;

                //    // Write data

                //    Stream postStream = request.GetRequestStream();

                //    postStream.Write(byteArray, 0, byteArray.Length);

                //    postStream.Close();

                //    // Send Request & Get Response

                //    response = (HttpWebResponse)request.GetResponse();

                //    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                //    {

                //        // Get the Response Stream

                //        string json = reader.ReadLine();

                //        Console.WriteLine(json);

                //    }

                //}

                //catch (Exception)
                //{
                //    throw;
                //    //The remote server returned an error: (404) Not Found.
                //}













                //List<Patients> Patientdata = jtobj.ToObject<List<Patients>>();

                //dtClinicaldata = GetModuleDataBase.ConvertToDataTable(Patientdata);


                //// Définition des variables qui seront envoyés
                //HttpContent stringContent1 = new StringContent(parameters["patientid"].ToString()); // Le contenu du paramètre P1
                //HttpContent stringContent2 = new StringContent("269552"); // Le contenu du paramètre P2
                //HttpContent stringContent3 = new StringContent("2014-02-24");
                //HttpContent stringContent4 = new StringContent("Test Document 2");

                //Stream fileStream = System.IO.File.OpenRead(@"E:\IntelliH_Inc\TestPDF.pdf");
                ////formContent.Add(new StreamContent(fileStream), fileName, fileName);

                //HttpContent fileStreamContent = new StreamContent(fileStream);
                ////HttpContent bytesContent = new ByteArrayContent(paramFileBytes);

                //using (var client = new HttpClient())
                //using (var formData = new MultipartFormDataContent())
                //{
                //    formData.Add(stringContent1, "patient"); // Le paramètre P1 aura la valeur contenue dans param1String
                //    formData.Add(stringContent2, "doctor"); // Le parmaètre P2 aura la valeur contenue dans param2String
                //    formData.Add(stringContent3, "date");
                //    formData.Add(stringContent4, "description");
                //    formData.Add(fileStreamContent, "document", "TestPDF.pdf");
                //    //  formData.Add(bytesContent, "file2", "file2");
                //    try
                //    {
                //        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + parameters["access_token"].ToString());
                //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
                //        var response = client.PostAsync(parameters["api_baseurl"].ToString() + "api/documents", formData).Result;
                //        //MessageBox.Show(response.ToString());
                //        //if (!response.IsSuccessStatusCode)
                //        //{
                //        //    MessageBox.Show("Erreur de réponse");
                //        //}
                //    }
                //    catch (Exception)
                //    {
                //        throw;
                //    }
                //    finally
                //    {
                //        client.CancelPendingRequests();
                //    }
                //}


                dtClinicaldata.TableName = strModuleName;

                return dtClinicaldata;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GenerateAPIJSONString(DataTable dtPatientData, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                string JSONString = string.Empty;
                DataTable dtFinalData;
                dtFinalData = dtPatientData.Copy();

                foreach (DataColumn dColumnPatDemo in dtFinalData.Columns)
                {
                    if (dColumnPatDemo.ColumnName.ToLower() == "first_name")
                    {
                        dColumnPatDemo.ColumnName = "FirstName";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "middle_name")
                    {
                        dColumnPatDemo.ColumnName = "MiddleName";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "last_name")
                    {
                        dColumnPatDemo.ColumnName = "LastName";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "email")
                    {
                        dColumnPatDemo.ColumnName = "Email";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "date_of_birth")
                    {
                        dColumnPatDemo.ColumnName = "DateOfBirth";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "gender")
                    {
                        dColumnPatDemo.ColumnName = "Gender";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "home_phone")
                    {
                        dColumnPatDemo.ColumnName = "Phone";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "ethnicity")
                    {
                        dColumnPatDemo.ColumnName = "EthnicityCode";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "race")
                    {
                        dColumnPatDemo.ColumnName = "RaceCode";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "address")
                    {
                        dColumnPatDemo.ColumnName = "Address1";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "city")
                    {
                        dColumnPatDemo.ColumnName = "City";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "state")
                    {
                        dColumnPatDemo.ColumnName = "State";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "zip_code")
                    {
                        dColumnPatDemo.ColumnName = "ZipCode";
                    }
                    else if (dColumnPatDemo.ColumnName.ToLower() == "social_security_number")
                    {
                        dColumnPatDemo.ColumnName = "SSN";
                    }
                }

                JSONString = DataTableToJSONDocumentUpload(dtFinalData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string DataTableToJSONDocumentUpload(DataTable table, string emrpatientid, string requestid, string emrid, string moduleid, string userid)
        {
            var JSONString = new StringBuilder();

            JSONString.Append("{");
            JSONString.Append("\"EMRPatientId\":" + "\"" + emrpatientid + "\",");
            JSONString.Append("\"EMRId\":" + "\"" + emrid + "\",");
            JSONString.Append("\"ModuleId\":" + "\"" + moduleid + "\",");
            JSONString.Append("\"RequestId\":" + "\"" + requestid + "\",");
            JSONString.Append("\"CreatedBy\":" + "\"\",");
            JSONString.Append("\"EMRUserExtensionLogDetails\":");

            if (table.Rows.Count == 0)
            {
                JSONString.Append("\"No Patient Found\"");
                JSONString.Append("}");
                return JSONString.ToString();
            }

            foreach (DataRow drow in table.Rows)
            {
                JSONString.Append("{");

                JSONString.Append("\"UserId\":" + "\"" + userid + "\",");
                JSONString.Append("\"Title\":" + "\"\",");
                JSONString.Append("\"FirstName\":" + "\"" + drow["FirstName"].ToString() + "\",");
                JSONString.Append("\"MiddleName\":" + "\"" + drow["MiddleName"].ToString() + "\",");
                JSONString.Append("\"LastName\":" + "\"" + drow["LastName"].ToString() + "\",");
                JSONString.Append("\"Suffix\":" + "\"\",");
                JSONString.Append("\"Gender\":" + "\"" + drow["Gender"].ToString() + "\",");
                JSONString.Append("\"DateOfBirth\":" + "\"" + drow["DateOfBirth"].ToString() + "\",");
                JSONString.Append("\"Email\":" + "\"" + drow["Email"].ToString() + "\",");
                JSONString.Append("\"Street\":" + "\"\",");
                JSONString.Append("\"City\":" + "\"" + drow["City"].ToString() + "\",");
                JSONString.Append("\"ZipCode\":" + "\"" + drow["ZipCode"].ToString() + "\",");
                JSONString.Append("\"Phone\":" + "\"" + drow["Phone"].ToString() + "\",");
                JSONString.Append("\"Address1\":" + "\"" + drow["Address1"].ToString() + "\",");
                JSONString.Append("\"Address2\":" + "\"\",");
                JSONString.Append("\"SSN\":" + "\"" + drow["SSN"].ToString() + "\",");
                JSONString.Append("\"MRN\":" + "\"\",");
                JSONString.Append("\"State\":" + "\"" + drow["State"].ToString() + "\",");
                JSONString.Append("\"Password\":" + "\"\",");
                JSONString.Append("\"Role\":" + "\"\",");
                JSONString.Append("\"heightFeet\":" + "\"\",");
                JSONString.Append("\"heightInch\":" + "\"\",");
                JSONString.Append("\"weight\":" + "\"\",");
                JSONString.Append("\"FacilityId\":" + "\"" + requestid + "\",");
                JSONString.Append("\"StateId\":" + "\"\",");
                JSONString.Append("\"EthnicityCode\":" + "\"" + drow["EthnicityCode"].ToString() + "\",");
                JSONString.Append("\"RaceCode\":" + "\"" + drow["RaceCode"].ToString() + "\",");
                JSONString.Append("\"NPI\":" + "\"\",");
                JSONString.Append("\"Status\":" + "\"\",");
                JSONString.Append("\"POSCode\":" + "\"\"");

                JSONString.Append("}");
                JSONString.Append("}");
            }
            return JSONString.ToString();
        }

    }
}