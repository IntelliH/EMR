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
    class Vitals
    {
        public DataTable GetData(Hashtable parameters)
        {
            try
            {
                DataTable dtModuleData = new DataTable();

                //List<Vital> objList = new List<Vital>();
                dtModuleData = GetVitalsData(parameters);// 4342010, 4342008

                return dtModuleData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DataTable GetVitalsData(Hashtable parameters)
        {
            try
            {

                var mySearch = new SearchParams();
                mySearch.Parameters.Add(new Tuple<string, string>("patient", parameters["patientid"].ToString()));
                mySearch.Parameters.Add(new Tuple<string, string>("code", "8302-2,29463-7"));
                //mySearch.Parameters.Add(new Tuple<string, string>("code", "29463-7"));

                DataTable dtVitals = new DataTable();
                dtVitals.Columns.Add("EffectiveDate");
                dtVitals.Columns.Add("Vital");
                dtVitals.Columns.Add("Value");
                dtVitals.Columns.Add("Unit");

                try
                {
                    #region DSTU2
                    var fhirClient = new FhirClient("https://open-ic.epic.com/FHIR/api/FHIR/DSTU2/");

                    fhirClient.UseFormatParam = true;

                    fhirClient.PreferredFormat = ResourceFormat.Json;

                    //Query the fhir server with search parameters, we will retrieve a bundle
                    var searchResultResponse = fhirClient.Search<Observation>(mySearch);

                    //var searchResultResponse = fhirClient.Read<VitalOrder>("VitalOrder/4342010");

                    //return new List<Vital>();

                    List<Observation> objlist = new List<Observation>();

                    foreach (Bundle.EntryComponent entry in searchResultResponse.Entry)
                    {
                        try
                        {
                            if (entry.FullUrl == null)
                            {
                                return dtVitals;
                            }

                            var vitalOrders = fhirClient.Read<Observation>("Observation/" + entry.Resource.Id);

                            DataRow dRowVital = dtVitals.NewRow();
                            dRowVital["EffectiveDate"] = DateFormatMMDDYYYY(vitalOrders.Effective.ToString());
                            dRowVital["Vital"] = vitalOrders.Code.Text;
                            dRowVital["Value"] = ((Hl7.Fhir.Model.Quantity)vitalOrders.Value).Value;
                            dRowVital["Unit"] = ((Hl7.Fhir.Model.Quantity)vitalOrders.Value).Unit;

                            dtVitals.Rows.Add(dRowVital);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }

                    return dtVitals;
                    #endregion
                }
                catch (AggregateException e)
                {
                    throw e.Flatten();
                }
                catch (FhirOperationException)
                {
                    // if we have issues we likely got a 404 and thus have no Vital orders...
                    return new DataTable();
                }
                catch (Exception)
                {
                    throw;
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
                    JSONString.Append("\"Error\":" + "\"No Vital Found\",");
                    JSONString.Append("\"Vitals\":{}");
                    JSONString.Append("}");
                    return JSONString.ToString();
                }

                JSONString.Append("\"Error\":" + "\"\",");
                JSONString.Append("\"Vitals\":");
                JSONString.Append("[");

                int counter = 0;
                foreach (DataRow drow in table.Rows)
                {
                    counter++;

                    JSONString.Append("{");

                    JSONString.Append("\"EffectiveDate\":" + "\"" + drow["EffectiveDate"].ToString() + "\",");
                    JSONString.Append("\"Vital\":" + "\"" + drow["Vital"].ToString() + "\",");
                    JSONString.Append("\"Value\":" + "\"" + drow["Value"].ToString() + "\",");
                    JSONString.Append("\"Unit\":" + "\"" + drow["Unit"].ToString() + "\"");

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
    }
}
