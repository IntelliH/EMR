using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace EMRIntegrations.Epic.ModuleClasses
{
    class Medications
    {
        public DataTable GetData(Hashtable parameters)
        {
            try
            {
                DataTable dtModuleData = new DataTable();

                //List<Medication> objList = new List<Medication>();
                dtModuleData = GetMedicationsData(parameters);// 4342010, 4342008

                return dtModuleData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //private static async Task<List<Medication>> GetMedicationsData(Hashtable parameters, string patientId)
        //{
        //    var mySearch = new SearchParams();
        //    mySearch.Parameters.Add(new Tuple<string, string>("patient", patientId));

        //    try
        //    {
        //        var fhirClient = new FhirClient("https://fhir-open.sandboxcerner.com/dstu2/0b8a0111-e8e6-4c26-a91c-5069cbc6b1ca");

        //        //Query the fhir server with search parameters, we will retrieve a bundle
        //        var searchResultResponse = await Task.Run(() => fhirClient.Search<Hl7.Fhir.Model.MedicationOrder>(mySearch));
        //        //There is an array of "entries" that can return. Get a list of all the entries.
        //        return
        //            searchResultResponse
        //                .Entry
        //                    .AsParallel() //as parallel since we are making network requests
        //                    .Select(entry =>
        //                    {
        //                        var medOrders = fhirClient.Read<MedicationOrder>("MedicationOrder/" + entry.Resource.Id);
        //                        var safeCast = (medOrders?.Medication as ResourceReference)?.Reference;
        //                        if (string.IsNullOrWhiteSpace(safeCast))
        //                        {
        //                            return null;
        //                        }

        //                        return fhirClient.Read<Medication>(safeCast);
        //                    })
        //                    .Where(a => a != null)
        //                    .ToList(); //tolist to force the queries to occur now
        //    }
        //    catch (AggregateException e)
        //    {
        //        throw e.Flatten();
        //    }
        //    catch (FhirOperationException)
        //    {
        //        // if we have issues we likely got a 404 and thus have no medication orders...
        //        return new List<Medication>();
        //    }
        //}

        private DataTable GetMedicationsData(Hashtable parameters)
        {
            try
            {

                var mySearch = new SearchParams();
                mySearch.Parameters.Add(new Tuple<string, string>("patient", parameters["patientid"].ToString()));

                DataTable dtMedications = new DataTable();
                dtMedications.Columns.Add("CreatedBy");
                dtMedications.Columns.Add("MedicationId");
                dtMedications.Columns.Add("MedicationName");
                dtMedications.Columns.Add("Direction");
                dtMedications.Columns.Add("DosageFrequencyValue");
                dtMedications.Columns.Add("DosageRoute");
                dtMedications.Columns.Add("DosageAdditionalInstructions");
                dtMedications.Columns.Add("DosageFrequencyUnit");
                dtMedications.Columns.Add("DosageUnit");
                dtMedications.Columns.Add("DosageQuantity");
                dtMedications.Columns.Add("DosageFrequencyDescription");
                dtMedications.Columns.Add("DosageDurationUnit");
                dtMedications.Columns.Add("StopDate");
                dtMedications.Columns.Add("StartDate");
                dtMedications.Columns.Add("EnterDate");

                try
                {
                    #region DSTU2
                    var fhirClient = new FhirClient("https://open-ic.epic.com/FHIR/api/FHIR/DSTU2/");

                    fhirClient.UseFormatParam = true;

                    fhirClient.PreferredFormat = ResourceFormat.Json;

                    //Query the fhir server with search parameters, we will retrieve a bundle
                    var searchResultResponse = fhirClient.Search<MedicationOrder>(mySearch);

                    //var searchResultResponse = fhirClient.Read<MedicationOrder>("MedicationOrder/4342010");

                    //return new List<Medication>();

                    List<Medication> objlist = new List<Medication>();

                    foreach (Bundle.EntryComponent entry in searchResultResponse.Entry)
                    {
                        try
                        {
                            if (entry.FullUrl == null)
                            {
                                return dtMedications;
                            }

                            var medOrders = fhirClient.Read<MedicationOrder>("MedicationOrder/" + entry.Resource.Id);
                            //var safeCast = (medOrders?.Medication as ResourceReference)?.Reference;
                            //if (string.IsNullOrWhiteSpace(safeCast))
                            //{
                            //    //return null;
                            //}
                            //else
                            //{
                            DataRow dRowMedication = dtMedications.NewRow();
                            dRowMedication["CreatedBy"] = medOrders.Prescriber.Display;
                            dRowMedication["MedicationId"] = medOrders.Id;

                            try
                            {
                                dRowMedication["MedicationName"] = ((Hl7.Fhir.Model.CodeableConcept)medOrders.Medication).Text;
                            }
                            catch (Exception)
                            {
                                dRowMedication["MedicationName"] = ((Hl7.Fhir.Model.ResourceReference)medOrders.Medication).Display;
                            }

                            dRowMedication["Direction"] = medOrders.DosageInstruction[0].Text;

                            if (medOrders.DosageInstruction[0].Timing.Repeat != null)
                            {
                                dRowMedication["DosageFrequencyValue"] = medOrders.DosageInstruction[0].Timing.Repeat.Period.ToString();
                                dRowMedication["DosageFrequencyUnit"] = medOrders.DosageInstruction[0].Timing.Repeat.PeriodUnits.ToString();
                            }

                            if (medOrders.DosageInstruction[0].Route != null)
                            {
                                dRowMedication["DosageRoute"] = medOrders.DosageInstruction[0].Route.Coding[0].Display;
                            }

                            if (medOrders.DosageInstruction[0].AdditionalInstructions != null)
                            {
                                dRowMedication["DosageAdditionalInstructions"] = medOrders.DosageInstruction[0].AdditionalInstructions.Text;
                            }

                            if ((Hl7.Fhir.Model.Quantity)medOrders.DosageInstruction[0].Dose != null)
                            {
                                dRowMedication["DosageUnit"] = ((Hl7.Fhir.Model.Quantity)medOrders.DosageInstruction[0].Dose).Unit;
                                dRowMedication["DosageQuantity"] = ((Hl7.Fhir.Model.Quantity)medOrders.DosageInstruction[0].Dose).Value;
                            }

                            dRowMedication["DosageFrequencyDescription"] = string.Empty;
                            dRowMedication["DosageDurationUnit"] = medOrders.DispenseRequest.ExpectedSupplyDuration;

                            if (medOrders.DosageInstruction[0].Timing.Repeat != null)
                            {
                                dRowMedication["StopDate"] = ((Hl7.Fhir.Model.Period)(medOrders.DosageInstruction[0].Timing.Repeat.Bounds)).End == null ? string.Empty : DateFormatMMDDYYYY(((Hl7.Fhir.Model.Period)(medOrders.DosageInstruction[0].Timing.Repeat.Bounds)).End);
                                dRowMedication["StartDate"] = ((Hl7.Fhir.Model.Period)(medOrders.DosageInstruction[0].Timing.Repeat.Bounds)).Start == null ? string.Empty : DateFormatMMDDYYYY(((Hl7.Fhir.Model.Period)(medOrders.DosageInstruction[0].Timing.Repeat.Bounds)).Start);
                            }

                            dRowMedication["EnterDate"] = medOrders.DateWritten == null ? string.Empty : DateFormatMMDDYYYY(medOrders.DateWritten);

                            dtMedications.Rows.Add(dRowMedication);

                            //string MedicationName = medOrders.Medication.ToString();

                            //var Medication = fhirClient.Read<Medication>("Medication/" + safeCast);
                            //objlist.Add(Medication);
                            //}
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }

                    return dtMedications;
                    #endregion

                    //return
                    //    searchResultResponse
                    //        .Entry
                    //            .Select(ResourceEntry =>
                    //            {
                    //                var medOrders = fhirClient.Read<MedicationOrder>("MedicationOrder/" + ResourceEntry.Resource.Id);
                    //                var safeCast = (medOrders?.Medication as ResourceReference)?.Reference;
                    //                if (string.IsNullOrWhiteSpace(safeCast))
                    //                {
                    //                    return null;
                    //                }
                    //                return fhirClient.Read<Medication>(safeCast);
                    //            })
                    //            .Where(a => a != null)
                    //            .ToList(); //tolist to force the queries to occur now
                }
                catch (AggregateException e)
                {
                    throw e.Flatten();
                }
                catch (FhirOperationException)
                {
                    // if we have issues we likely got a 404 and thus have no medication orders...
                    return new DataTable();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string DateFormatMMDDYYYY(string date)
        {
            try
            {
                string truncatedDate = date.Substring(0, 10).Replace("-", string.Empty);
                DateTime d = DateTime.ParseExact(truncatedDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                //Console.WriteLine(d.ToString("MM/dd/yyyy"));

                string finalDate = d.ToString("MM/dd/yyyy").Replace("-", "/");
                return finalDate;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GenerateAPIJSONString(DataTable table, string emrpatientid, string requestid, string emrid, string moduleid, string userid)
        {
            var JSONString = new StringBuilder();

            JSONString.Append("{");
            JSONString.Append("\"EMRPatientId\":" + "\"" + emrpatientid + "\",");
            JSONString.Append("\"EMRId\":" + "\"" + emrid + "\",");
            JSONString.Append("\"ModuleId\":" + "\"" + moduleid + "\",");
            JSONString.Append("\"RequestId\":" + "\"" + requestid + "\",");
            JSONString.Append("\"CreatedBy\":" + "\"\",");
            JSONString.Append("\"Medications\":");

            if (table.Rows.Count == 0)
            {
                JSONString.Append("\"No Medication Found\"");
                JSONString.Append("}");
                return JSONString.ToString();
            }

            JSONString.Append("[");

            int counter = 0;
            foreach (DataRow drow in table.Rows)
            {
                counter++;

                JSONString.Append("{");

                JSONString.Append("\"CreatedBy\":" + "\"" + drow["Createdby"].ToString() + "\",");
                JSONString.Append("\"MedicationID\":" + "\"" + drow["Medicationid"].ToString() + "\",");
                JSONString.Append("\"MedicationName\":" + "\"" + drow["MedicationName"].ToString() + "\",");
                JSONString.Append("\"Direction\":" + "\"" + drow["Direction"].ToString() + "\",");
                JSONString.Append("\"DosageFrequencyValue\":" + "\"" + drow["Dosagefrequencyvalue"].ToString() + "\",");
                JSONString.Append("\"DosageRoute\":" + "\"" + drow["dosageroute"].ToString() + "\",");
                JSONString.Append("\"DosageAdditionalInstructions\":" + "\"" + drow["Dosageadditionalinstructions"].ToString() + "\",");
                JSONString.Append("\"DosageFrequencyUnit\":" + "\"" + drow["Dosagefrequencyunit"].ToString() + "\",");
                JSONString.Append("\"DosageUnit\":" + "\"" + drow["DosageUnit"].ToString() + "\",");
                JSONString.Append("\"DosageQuantity\":" + "\"" + drow["Dosagequantity"].ToString() + "\",");
                JSONString.Append("\"DosageFrequencyDescription\":" + "\"" + drow["Dosagefrequencydescription"].ToString() + "\",");
                JSONString.Append("\"DosageDurationUnit\":" + "\"" + drow["Dosagedurationunit"].ToString() + "\",");
                JSONString.Append("\"StopDate\":" + "\"" + drow["Stopdate"].ToString() + "\",");
                JSONString.Append("\"StartDate\":" + "\"" + drow["Startdate"].ToString() + "\",");
                JSONString.Append("\"EnterDate\":" + "\"" + drow["Enterdate"].ToString() + "\"");

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
    }
}
