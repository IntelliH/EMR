using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace EMRIntegrations.CernerMillenium.ModuleClasses
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
            var mySearch = new SearchParams();
            mySearch.Parameters.Add(new Tuple<string, string>("patient", parameters["patientid"].ToString()));

            DataTable dtMedications = new DataTable();
            dtMedications.Columns.Add("Createdby");
            dtMedications.Columns.Add("Medicationid");
            dtMedications.Columns.Add("Medication");
            dtMedications.Columns.Add("Direction");
            dtMedications.Columns.Add("Dosagefrequencyvalue");
            dtMedications.Columns.Add("doseroute");
            dtMedications.Columns.Add("Dosageadditionalinstructions");
            dtMedications.Columns.Add("Dosagefrequencyunit");
            dtMedications.Columns.Add("DoseUnit");
            dtMedications.Columns.Add("Dosequantity");
            dtMedications.Columns.Add("Dosagefrequencydescription");
            dtMedications.Columns.Add("Dosagedurationunit");
            dtMedications.Columns.Add("Stopdate");
            dtMedications.Columns.Add("Startdate");
            dtMedications.Columns.Add("Enterdate");

            try
            {
                var fhirClient = new FhirClient("https://fhir-open.sandboxcerner.com/dstu2/0b8a0111-e8e6-4c26-a91c-5069cbc6b1ca");

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
                        var medOrders = fhirClient.Read<MedicationOrder>("MedicationOrder/" + entry.Resource.Id);
                        //var safeCast = (medOrders?.Medication as ResourceReference)?.Reference;
                        //if (string.IsNullOrWhiteSpace(safeCast))
                        //{
                        //    //return null;
                        //}
                        //else
                        //{
                        DataRow dRowMedication = dtMedications.NewRow();
                        dRowMedication["Createdby"] = medOrders.Prescriber.Display;
                        dRowMedication["Medicationid"] = medOrders.Id;

                        try
                        {
                            dRowMedication["Medication"] = ((Hl7.Fhir.Model.CodeableConcept)medOrders.Medication).Text;
                        }
                        catch (Exception)
                        {
                            dRowMedication["Medication"] = ((Hl7.Fhir.Model.ResourceReference)medOrders.Medication).Display;
                        }

                        dRowMedication["Direction"] = medOrders.DosageInstruction[0].Text;

                        if (medOrders.DosageInstruction[0].Timing.Repeat != null)
                        {
                            dRowMedication["Dosagefrequencyvalue"] = medOrders.DosageInstruction[0].Timing.Repeat.Duration.ToString();
                            dRowMedication["Dosagefrequencyunit"] = medOrders.DosageInstruction[0].Timing.Repeat.DurationUnits.ToString();
                        }

                        if (medOrders.DosageInstruction[0].Route != null)
                        {
                            dRowMedication["doseroute"] = medOrders.DosageInstruction[0].Route.Coding[0].Display;
                        }

                        if (medOrders.DosageInstruction[0].AdditionalInstructions != null)
                        {
                            dRowMedication["Dosageadditionalinstructions"] = medOrders.DosageInstruction[0].AdditionalInstructions.Text;
                        }

                        if ((Hl7.Fhir.Model.Quantity)medOrders.DosageInstruction[0].Dose != null)
                        {
                            dRowMedication["DoseUnit"] = ((Hl7.Fhir.Model.Quantity)medOrders.DosageInstruction[0].Dose).Unit;
                            dRowMedication["Dosequantity"] = ((Hl7.Fhir.Model.Quantity)medOrders.DosageInstruction[0].Dose).Value;
                        }

                        dRowMedication["Dosagefrequencydescription"] = string.Empty;
                        dRowMedication["Dosagedurationunit"] = medOrders.DispenseRequest.ExpectedSupplyDuration;
                        dRowMedication["Stopdate"] = medOrders.DateEnded == null ? string.Empty : DateFormatMMDDYYYY(medOrders.DateEnded);
                        dRowMedication["Startdate"] = medOrders.DateWritten == null ? string.Empty : DateFormatMMDDYYYY(medOrders.DateWritten);
                        dRowMedication["Enterdate"] = medOrders.DateWritten == null ? string.Empty : DateFormatMMDDYYYY(medOrders.DateWritten);

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
                JSONString.Append("\"MedicationName\":" + "\"" + drow["Medication"].ToString() + "\",");
                JSONString.Append("\"Direction\":" + "\"" + drow["Direction"].ToString() + "\",");
                JSONString.Append("\"DosageFrequencyValue\":" + "\"" + drow["Dosagefrequencyvalue"].ToString() + "\",");
                JSONString.Append("\"DosageRoute\":" + "\"" + drow["doseroute"].ToString() + "\",");
                JSONString.Append("\"DosageAdditionalInstructions\":" + "\"" + drow["Dosageadditionalinstructions"].ToString() + "\",");
                JSONString.Append("\"DosageFrequencyUnit\":" + "\"" + drow["Dosagefrequencyunit"].ToString() + "\",");
                JSONString.Append("\"DosageUnit\":" + "\"" + drow["DoseUnit"].ToString() + "\",");
                JSONString.Append("\"DosageQuantity\":" + "\"" + drow["Dosequantity"].ToString() + "\",");
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
