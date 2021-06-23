using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Text;

namespace EMRIntegrations.Epic.ModuleClasses
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
                dtPatientDemographics.Columns.Add("Ethnicity");
                dtPatientDemographics.Columns.Add("RaceCode");
                dtPatientDemographics.Columns.Add("Race");
                dtPatientDemographics.Columns.Add("Address1");
                dtPatientDemographics.Columns.Add("Address2");
                dtPatientDemographics.Columns.Add("City");
                dtPatientDemographics.Columns.Add("State");
                dtPatientDemographics.Columns.Add("ZipCode");
                dtPatientDemographics.Columns.Add("SSN");

                var fhirClient = new FhirClient("https://open-ic.epic.com/FHIR/api/FHIR/DSTU2/");
                //var fhirClient = new FhirClient("https://open-ic.epic.com/argonaut/api/FHIR/Argonaut/");


                fhirClient.UseFormatParam = true;
                fhirClient.PreferredFormat = ResourceFormat.Json;

                // Read Patient #1
                Patient objPatient = new Patient();
                var currentPatient = objPatient;
                currentPatient = fhirClient.Read<Patient>("Patient/" + parameters["patientid"].ToString() + "");//Tbt3KuCY0B5PSrJvCu2j-PlK.aiHsu2xUjUM8bWpetXoB

                DataRow dRowPatientDemographics = dtPatientDemographics.NewRow();

                foreach (Hl7.Fhir.Model.HumanName Name in currentPatient.Name)
                {
                    if (Name.Use.Value.ToString().ToLower() == "usual")
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
                    if (extension.Url == "http://hl7.org/fhir/StructureDefinition/us-core-race")
                    {
                        if (((Hl7.Fhir.Model.CodeableConcept)extension.Value).Coding != null && ((Hl7.Fhir.Model.CodeableConcept)extension.Value).Coding.Count > 0)
                        {
                            foreach (var item in ((Hl7.Fhir.Model.CodeableConcept)extension.Value).Coding)
                            {
                                dRowPatientDemographics["RaceCode"] = item.Code;
                                dRowPatientDemographics["Race"] = item.Display;
                            }
                        }
                    }
                    else if (extension.Url == "http://hl7.org/fhir/StructureDefinition/us-core-ethnicity")
                    {
                        if (((Hl7.Fhir.Model.CodeableConcept)extension.Value).Coding != null && ((Hl7.Fhir.Model.CodeableConcept)extension.Value).Coding.Count > 0)
                        {
                            foreach (var item in ((Hl7.Fhir.Model.CodeableConcept)extension.Value).Coding)
                            {
                                dRowPatientDemographics["EthnicityCode"] = item.Code;
                                dRowPatientDemographics["Ethnicity"] = item.Display;
                            }
                        }
                    }
                }

                int AddressCounter = 0;
                foreach (Hl7.Fhir.Model.Address address in currentPatient.Address)
                {
                    if (address.Use.Value.ToString().ToLower() == "home")
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
                    JSONString.Append("\"Error\":" + "\"No Patient Found\",");
                    JSONString.Append("\"EMRUserExtensionLogDetails\":{}");
                    JSONString.Append("}");
                    return JSONString.ToString();
                }

                JSONString.Append("\"Error\":" + "\"\",");
                JSONString.Append("\"EMRUserExtensionLogDetails\":");

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
                    JSONString.Append("\"MRN\":" + "\"" + emrpatientid + "\",");
                    JSONString.Append("\"State\":" + "\"" + drow["State"].ToString() + "\",");
                    JSONString.Append("\"Password\":" + "\"\",");
                    JSONString.Append("\"Role\":" + "\"\",");
                    JSONString.Append("\"heightFeet\":" + "\"\",");
                    JSONString.Append("\"heightInch\":" + "\"\",");
                    JSONString.Append("\"weight\":" + "\"\",");
                    JSONString.Append("\"FacilityId\":" + "\"" + requestid + "\",");
                    JSONString.Append("\"StateId\":" + "\"\",");
                    JSONString.Append("\"EthnicityCode\":" + "\"" + drow["EthnicityCode"].ToString() + "\",");
                    JSONString.Append("\"Ethnicity\":" + "\"" + mapEthnicity(drow["EthnicityCode"].ToString()) + "\",");
                    JSONString.Append("\"RaceCode\":" + "\"" + drow["RaceCode"].ToString() + "\",");
                    JSONString.Append("\"Race\":" + "\"" + mapRace(drow["RaceCode"].ToString()) + "\",");
                    JSONString.Append("\"NPI\":" + "\"\",");
                    JSONString.Append("\"Status\":" + "\"\",");
                    JSONString.Append("\"POSCode\":" + "\"\"");

                    JSONString.Append("}");
                    JSONString.Append("}");
                }
                return JSONString.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        string mapRace(string race)
        {
            try
            {
                switch (race)
                {
                    case "1002-5":
                        return "American Indian or Alaska Native";
                    case "2028-9":
                        return "Asian";
                    case "2054-5":
                        return "Black or African American";
                    case "2076-8":
                        return "Native Hawaiian or Other Pacific Islander";
                    case "2106-3":
                        return "White";
                    default:
                        return string.Empty;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        string mapEthnicity(string ethnicity)
        {
            try
            {
                switch (ethnicity)
                {
                    case "2135-2":
                        return "Hispanic";
                    case "2186-5":
                        return "Non-hispanic";
                    default:
                        return string.Empty;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
