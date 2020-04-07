using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRIntegrations.Core.Models
{
    public class PullDataModel
    {
        string emrid;
        public string EMRID
        {
            get { return emrid; }
            set { emrid = value; }
        }

        string moduleid;
        public string ModuleID
        {
            get { return moduleid; }
            set { moduleid = value; }
        }

        string patientid;
        public string PatientID
        {
            get { return patientid; }
            set { patientid = value; }
        }
    }
}
