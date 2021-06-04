using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EMRIntegrations.Epic.ModuleClasses
{
    class Allergy
    {
        public DataTable GetData(Hashtable parameters)
        {
            try
            {
                DataTable dtModuleData = new DataTable();

                //List<Allergy> objList = new List<Allergy>();
                dtModuleData = GetAllergyData(parameters);// 4342010, 4342008

                return dtModuleData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DataTable GetAllergyData(Hashtable parameters)
        {
            var mySearch = new SearchParams();
            mySearch.Parameters.Add(new Tuple<string, string>("patient", parameters["patientid"].ToString()));

            DataTable dtAllergy = new DataTable();
            dtAllergy.Columns.Add("Allergicto");
            dtAllergy.Columns.Add("Allergenid");
            dtAllergy.Columns.Add("Reaction");
            dtAllergy.Columns.Add("Severity");
            dtAllergy.Columns.Add("Note");
            dtAllergy.Columns.Add("Onsetdate");
            dtAllergy.Columns.Add("admitflag");
            dtAllergy.Columns.Add("daterecorded");
            dtAllergy.Columns.Add("patientid");

            try
            {
                var fhirClient = new FhirClient("https://open-ic.epic.com/FHIR/api/FHIR/DSTU2/");

                fhirClient.UseFormatParam = true;

                fhirClient.PreferredFormat = ResourceFormat.Json;

                //Query the fhir server with search parameters, we will retrieve a bundle
                var searchResultResponse = fhirClient.Search<AllergyIntolerance>(mySearch);

                //var searchResultResponse = fhirClient.Read<AllergyOrder>("AllergyOrder/4342010");

                //return new List<Allergy>();

                List<Allergy> objlist = new List<Allergy>();

                foreach (Bundle.EntryComponent entry in searchResultResponse.Entry)
                {
                    try
                    {
                        if (entry.FullUrl == null)
                        {
                            return dtAllergy;
                        }

                        var allergyIntolerance = fhirClient.Read<AllergyIntolerance>("AllergyIntolerance/" + entry.Resource.Id);

                        if (allergyIntolerance.Reaction != null && allergyIntolerance.Reaction.Count > 0)
                        {
                            foreach (var item in allergyIntolerance.Reaction)
                            {
                                DataRow dRowAllergy = dtAllergy.NewRow();

                                dRowAllergy["Allergicto"] = allergyIntolerance.Substance.Text;
                                dRowAllergy["Allergenid"] = allergyIntolerance.Id;
                                dRowAllergy["Reaction"] = item.Substance == null ? (item.Manifestation[0].Text == null ? string.Empty : item.Manifestation[0].Text) : item.Substance.Text;
                                dRowAllergy["Note"] = allergyIntolerance.Note == null ? string.Empty : allergyIntolerance.Note.Text;
                                dRowAllergy["Onsetdate"] = allergyIntolerance.RecordedDate;
                                dRowAllergy["PatientID"] = parameters["patientid"].ToString();
                                dRowAllergy["admitflag"] = "1";
                                dRowAllergy["daterecorded"] = allergyIntolerance.RecordedDate;

                                if (item.Severity != null)
                                {
                                    dRowAllergy["Severity"] = item.Severity.Value;
                                }

                                dtAllergy.Rows.Add(dRowAllergy);
                            }
                        }
                        else
                        {
                            DataRow dRowAllergy = dtAllergy.NewRow();

                            dRowAllergy["Allergicto"] = allergyIntolerance.Substance.Text;
                            dRowAllergy["Allergenid"] = allergyIntolerance.Id;
                            dRowAllergy["Note"] = allergyIntolerance.Note == null ? string.Empty : allergyIntolerance.Note.Text;
                            dRowAllergy["Onsetdate"] = allergyIntolerance.RecordedDate;
                            dRowAllergy["PatientID"] = parameters["patientid"].ToString();
                            dRowAllergy["admitflag"] = "1";
                            dRowAllergy["daterecorded"] = allergyIntolerance.RecordedDate;

                            dtAllergy.Rows.Add(dRowAllergy);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                return dtAllergy;
            }
            catch (AggregateException e)
            {
                throw e.Flatten();
            }
            catch (FhirOperationException)
            {
                // if we have issues we likely got a 404 and thus have no Allergy orders...
                return new DataTable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string DataTableToJSONAllergy(DataTable table, string emrpatientid, string requestid, string emrid, string moduleid, string userid)
        {
            var JSONString = new StringBuilder();

            JSONString.Append("{");
            JSONString.Append("\"EMRPatientId\":" + "\"" + emrpatientid + "\",");
            JSONString.Append("\"EMRId\":" + "\"" + emrid + "\",");
            JSONString.Append("\"ModuleId\":" + "\"" + moduleid + "\",");
            JSONString.Append("\"RequestId\":" + "\"" + requestid + "\",");
            JSONString.Append("\"CreatedBy\":" + "\"\",");
            JSONString.Append("\"Allergy\":");

            if (table.Rows.Count == 0)
            {
                JSONString.Append("\"No Allergy Found\"");
                JSONString.Append("}");
                return JSONString.ToString();
            }

            JSONString.Append("[");

            int counter = 0;
            foreach (DataRow drow in table.Rows)
            {
                counter++;

                JSONString.Append("{");

                JSONString.Append("\"allergicto\":" + "\"" + drow["Allergicto"].ToString() + "\",");
                JSONString.Append("\"Reaction\":" + "\"" + drow["Reaction"].ToString() + "\",");
                JSONString.Append("\"Reactionsnomedcode\":" + "\"\",");
                JSONString.Append("\"Severity\":" + "\"" + drow["Severity"].ToString() + "\",");
                JSONString.Append("\"Note\":" + "\"" + drow["Note"].ToString() + "\",");
                JSONString.Append("\"Onsetdate\":" + "\"" + drow["Onsetdate"].ToString() + "\",");
                JSONString.Append("\"Severitysnomedcode\":" + "\"\",");
                JSONString.Append("\"Encounterid\":" + "\"\",");
                JSONString.Append("\"Deactivatedate\":" + "\"\",");
                JSONString.Append("\"patientid\":" + "\"" + drow["patientid"].ToString() + "\",");
                JSONString.Append("\"admitflag\":" + "\"" + drow["admitflag"].ToString() + "\",");
                JSONString.Append("\"daterecorded\":" + "\"" + drow["daterecorded"].ToString() + "\",");

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

        public string GenerateAPIJSONString(DataTable dtAllergy, string EMRPatientID, string RequestID, string EMRID, string ModuleID, string UserID)
        {
            try
            {
                string JSONString = string.Empty;
                DataTable dtFinalData;
                dtFinalData = dtAllergy.Copy();

                JSONString = DataTableToJSONAllergy(dtFinalData, EMRPatientID, RequestID, EMRID, ModuleID, UserID);
                return JSONString;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
