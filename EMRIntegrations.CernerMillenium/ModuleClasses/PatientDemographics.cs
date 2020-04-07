using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Text;

namespace EMRIntegrations.CernerMillenium.ModuleClasses
{
    class PatientDemographics
    {
        public DataTable GetData(Hashtable parameters)
        {
            try
            {
                DataTable dtModuleData = new DataTable();
                dtModuleData = GetPatientDemographicsData(parameters);
                return dtModuleData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DataTable GetPatientDemographicsData(Hashtable parameters)
        {
            try
            {
                DataTable dtPatientDemographics = new DataTable();
                dtPatientDemographics.Columns.Add("Title");
                dtPatientDemographics.Columns.Add("Suffix");
                dtPatientDemographics.Columns.Add("FirstName");
                dtPatientDemographics.Columns.Add("MiddleName");
                dtPatientDemographics.Columns.Add("LastName");
                dtPatientDemographics.Columns.Add("Email");
                dtPatientDemographics.Columns.Add("DateOfBirth");
                dtPatientDemographics.Columns.Add("Gender");
                dtPatientDemographics.Columns.Add("Phone");
                dtPatientDemographics.Columns.Add("EthnicityCode");
                dtPatientDemographics.Columns.Add("RaceCode");
                dtPatientDemographics.Columns.Add("Address1");
                dtPatientDemographics.Columns.Add("Address2");
                dtPatientDemographics.Columns.Add("City");
                dtPatientDemographics.Columns.Add("State");
                dtPatientDemographics.Columns.Add("ZipCode");
                dtPatientDemographics.Columns.Add("SSN");

                // Set up FHIR Client for Cerner Sandbox

                //var fhirClient = new FhirClient("https://fhir-open.sandboxcernerpowerchart.com/may2015/d075cf8b-3261-481d-97e5-ba6c48d3b41f");

                var fhirClient = new FhirClient("https://fhir-open.sandboxcerner.com/dstu2/0b8a0111-e8e6-4c26-a91c-5069cbc6b1ca");

                fhirClient.UseFormatParam = true;

                fhirClient.PreferredFormat = ResourceFormat.Json;

                //fhirClient.Search<MedicationOrder>(SearchParams.d)

                //var mySearch = new SearchParams();
                //mySearch.Parameters.Add(new Tuple<string, string>("patient", "4342010"));

                // Read Patient #1
                Patient objPatient = new Patient();
                var currentPatient = objPatient;
                try
                {
                    currentPatient = fhirClient.Read<Patient>("Patient/" + parameters["patientid"].ToString() + "");//1316024
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Operation was unsuccessful because of a client error (NotFound)."))
                        return dtPatientDemographics;
                    else
                        throw ex;
                }

                DataRow dRowPatientDemographics = dtPatientDemographics.NewRow();

                foreach (Hl7.Fhir.Model.HumanName Name in currentPatient.Name)
                {
                    if (Name.Use.Value.ToString().ToLower() == "official")
                    {
                        foreach (Hl7.Fhir.Model.FhirString item in Name.PrefixElement)
                        {
                            dRowPatientDemographics["Title"] += item.Value + " ";
                        }
                        dRowPatientDemographics["Title"] = dRowPatientDemographics["Title"].ToString().Trim();

                        foreach (Hl7.Fhir.Model.FhirString item in Name.FamilyElement)
                        {
                            dRowPatientDemographics["LastName"] += item.Value + " ";
                        }
                        dRowPatientDemographics["LastName"] = dRowPatientDemographics["LastName"].ToString().Trim();

                        foreach (Hl7.Fhir.Model.FhirString item in Name.GivenElement)
                        {
                            dRowPatientDemographics["FirstName"] += item.Value + " ";
                        }
                        dRowPatientDemographics["FirstName"] = dRowPatientDemographics["FirstName"].ToString().Trim();

                        foreach (Hl7.Fhir.Model.FhirString item in Name.SuffixElement)
                        {
                            dRowPatientDemographics["Suffix"] += item.Value + " ";
                        }
                        dRowPatientDemographics["Suffix"] = dRowPatientDemographics["Suffix"].ToString().Trim();
                    }
                }

                foreach (Hl7.Fhir.Model.ContactPoint item in currentPatient.Telecom)
                {
                    if (item.SystemElement.Value.ToString().ToLower() == "email")
                    {
                        dRowPatientDemographics["Email"] = item.Value;
                    }
                    else if (item.SystemElement.Value.ToString().ToLower() == "phone")
                    {
                        if (item.Use.ToString().ToLower() == "home")
                        {
                            dRowPatientDemographics["Phone"] = item.Value;
                        }
                    }
                }

                if (currentPatient.BirthDate != null)
                {
                    string res = currentPatient.BirthDate.Replace("-", string.Empty);
                    DateTime d = DateTime.ParseExact(res, "yyyyMMdd", CultureInfo.InvariantCulture);
                    //Console.WriteLine(d.ToString("MM/dd/yyyy"));

                    dRowPatientDemographics["DateOfBirth"] = d.ToString("MM/dd/yyyy").Replace("-", "/");
                }

                if (currentPatient.Gender != null)
                {
                    dRowPatientDemographics["Gender"] = currentPatient.Gender;
                }

                foreach (Hl7.Fhir.Model.Extension extension in currentPatient.Extension)
                {
                    if (extension.Url == "http://fhir.org/guides/argonaut/StructureDefinition/argo-race")
                    {
                        foreach (Hl7.Fhir.Model.Extension item in extension.Extension)
                        {
                            if (item.Url == "text")
                            {
                                dRowPatientDemographics["RaceCode"] = item.Value;
                            }
                        }
                    }
                    else if (extension.Url == "http://fhir.org/guides/argonaut/StructureDefinition/argo-ethnicity")
                    {
                        foreach (Hl7.Fhir.Model.Extension item in extension.Extension)
                        {
                            if (item.Url == "text")
                            {
                                dRowPatientDemographics["EthnicityCode"] = item.Value;
                            }
                        }
                    }
                }

                int AddressCounter = 0;
                foreach (Hl7.Fhir.Model.Address address in currentPatient.Address)
                {
                    foreach (Hl7.Fhir.Model.FhirString item in address.LineElement)
                    {
                        AddressCounter++;
                        if (AddressCounter == 1)
                        {
                            dRowPatientDemographics["Address1"] = item.Value;
                        }
                        else if (AddressCounter == 2)
                        {
                            dRowPatientDemographics["Address2"] = item.Value;
                        }
                    }

                    dRowPatientDemographics["City"] = address.CityElement == null ? string.Empty : address.CityElement.Value;
                    dRowPatientDemographics["State"] = address.StateElement == null ? string.Empty : address.StateElement.Value;
                    dRowPatientDemographics["ZipCode"] = address.PostalCodeElement == null ? string.Empty : address.PostalCodeElement.Value;
                }

                dtPatientDemographics.Rows.Add(dRowPatientDemographics);

                return dtPatientDemographics;
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
                JSONString = DataTableToJSONPatientDemographics(dtPatientData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string DataTableToJSONPatientDemographics(DataTable table, string emrpatientid, string requestid, string emrid, string moduleid, string userid)
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
                JSONString.Append("\"Title\":" + "\"" + drow["Title"].ToString() + "\",");
                JSONString.Append("\"FirstName\":" + "\"" + drow["FirstName"].ToString() + "\",");
                JSONString.Append("\"MiddleName\":" + "\"" + drow["MiddleName"].ToString() + "\",");
                JSONString.Append("\"LastName\":" + "\"" + drow["LastName"].ToString() + "\",");
                JSONString.Append("\"Suffix\":" + "\"" + drow["Suffix"].ToString() + "\",");
                JSONString.Append("\"Gender\":" + "\"" + drow["Gender"].ToString() + "\",");
                JSONString.Append("\"DateOfBirth\":" + "\"" + drow["DateOfBirth"].ToString() + "\",");
                JSONString.Append("\"Email\":" + "\"" + drow["Email"].ToString() + "\",");
                JSONString.Append("\"Street\":" + "\"\",");
                JSONString.Append("\"City\":" + "\"" + drow["City"].ToString() + "\",");
                JSONString.Append("\"ZipCode\":" + "\"" + drow["ZipCode"].ToString() + "\",");
                JSONString.Append("\"Phone\":" + "\"" + drow["Phone"].ToString() + "\",");
                JSONString.Append("\"Address1\":" + "\"" + drow["Address1"].ToString() + "\",");
                JSONString.Append("\"Address2\":" + "\"" + drow["Address2"].ToString() + "\",");
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
