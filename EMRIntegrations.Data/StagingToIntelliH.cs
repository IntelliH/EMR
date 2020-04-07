using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace EMRIntegrations.Data
{
    public class StagingToIntelliH
    {
        public DataTable MapEMRColumns(string FileName, DataTable dtData)
        {
            try
            {
                // Add code to read JSON file and return updated datatable
            }
            catch (Exception)
            {
                throw;
            }
            return new DataTable();
        }

        public string DataTableToJSONPatientDemographics(DataTable table, string emrid, string moduleid, string userid)
        {
            var JSONString = new StringBuilder();

            foreach (DataRow drow in table.Rows)
            {
                JSONString.Append("{");
                JSONString.Append("\"EMRPatientId\":" + "\"" + drow["Patientid"].ToString() + "\",");
                JSONString.Append("\"EMRId\":" + "\"" + emrid + "\",");
                JSONString.Append("\"ModuleId\":" + "\"" + moduleid + "\",");
                JSONString.Append("\"RequestId\":" + "\"" + drow["RequestID"].ToString() + "\",");
                JSONString.Append("\"CreatedBy\":" + "\"\",");
                JSONString.Append("\"EMRUserExtensionLogDetails\": {");

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
                JSONString.Append("\"FacilityId\":" + "\"" + drow["RequestID"].ToString() + "\",");
                JSONString.Append("\"StateId\":" + "\"\",");
                JSONString.Append("\"EthnicityCode\":" + "\"" + drow["EthnicityCode"].ToString() + "\",");
                JSONString.Append("\"RaceCode\":" + "\"" + drow["RaceCode"].ToString() + "\",");
                JSONString.Append("\"NPI\":" + "\"\",");
                JSONString.Append("\"Status\":" + "\"\",");
                JSONString.Append("\"POSCode\":" + "\"\",");

                JSONString.Append("}");
                JSONString.Append("}");
            }
            return JSONString.ToString();
        }
    }
}
