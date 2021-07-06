using HL7.Dotnetcore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace EMRIntegrations.Endpoints.WellSky
{
    public class ACK
    {
        public MSH fetchMSHComponents(string msg)
        {
            try
            {
                MSH oMSH = new MSH();


                Message message = new Message(msg);
                message.ParseMessage();

                List<Segment> segList = message.Segments();
                foreach (Segment oSegment in segList)
                {
                    //switch ((GenericFunctions.IsNumeric(oSegment.Name.ToUpper().Substring(2, 1)) == true) ? ((oSegment.Name.ToUpper() == DataFetcherConstants.PV1_Segment || oSegment.Name.ToUpper() == DataFetcherConstants.PV2_Segment) ? oSegment.Name.ToUpper() : oSegment.Name.ToUpper().Substring(0, 2)) : oSegment.Name.ToUpper())
                    switch (oSegment.Name.ToUpper())
                    {
                        case "MSH":
                            List<Field> fieldList = oSegment.GetAllFields();
                            //DataRow dRow = dtData.NewRow();
                            int counter = 0;
                            foreach (Field oField in fieldList)
                            {
                                switch (counter)
                                {
                                    case 0:
                                        oMSH.fieldseparator = oField.Value;
                                        break;
                                    case 2:
                                        oMSH.receivingapplication = oField.Value;
                                        break;
                                    case 3:
                                        oMSH.receivingfacility = oField.Value;
                                        break;
                                    case 9:
                                        oMSH.messagecontrolid = oField.Value;
                                        break;
                                    case 10:
                                        oMSH.processingid = oField.Value;
                                        break;
                                    case 11:
                                        oMSH.versionid = oField.Value;
                                        break;
                                    default:
                                        break;
                                }
                                counter++;
                            }
                            break;
                        default:
                            break;
                    }
                }

                oMSH.encodingcharactors = "^~\\&";
                oMSH.sendingapplication = "IntelliH";
                oMSH.sendingfacility = "IntelliH";
                oMSH.creationdatetime = DateTime.Now.ToString();
                oMSH.security = string.Empty;
                oMSH.messagetype = "ADT";
                oMSH.triggerevent = "A01";

                return oMSH;
            }
            catch (global::System.Exception)
            {
                throw;
            }
        }
    }
}
