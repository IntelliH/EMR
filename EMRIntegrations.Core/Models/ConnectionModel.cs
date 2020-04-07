using System.Data.SqlClient;

namespace EMRIntegrations.Core.Models
{
    public class ConnectionModel
    {
        string emrintegrationmasterconstr;
        public string EMRIntegrationMasterConStr
        {
            get { return emrintegrationmasterconstr; }
            set { emrintegrationmasterconstr = value; }
        }

        string emrstagingdbconstr;
        public string EmrStagingDBConStr
        {
            get { return emrstagingdbconstr; }
            set { emrstagingdbconstr = value; }
        }

        SqlConnection oconnemrmaster;
        public SqlConnection oConnEmrMaster
        {
            get { return oconnemrmaster; }
            set { oconnemrmaster = value; }
        }

        SqlConnection oconnemrstagingdb;
        public SqlConnection oConnEmrStagingDB
        {
            get { return oconnemrstagingdb; }
            set { oconnemrstagingdb = value; }
        }
    }
}
