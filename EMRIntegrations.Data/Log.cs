using EMRIntegrations.Core.Enums;
using EMRIntegrations.Core.Interfaces;
using EMRIntegrations.Core.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace EMRIntegrations.Data
{
    public class Log : ILog
    {
        public void LogError(ConnectionModel oConn, Logs.Status status, string RequestID, string EMRID, string ModuleID, string PatientID)
        {
            throw new NotImplementedException();
        }

        public void LogMapping(ConnectionModel oConn, Logs.Status status, string RequestID, string EMRID, string ModuleID, string PatientID)
        {
            SqlParameter[] parameters =
                {
                  new SqlParameter("@PatientID", SqlDbType.VarChar, 100) { Value = PatientID },
                  new SqlParameter("@EMRID", SqlDbType.VarChar, 100) { Value = EMRID },
                  new SqlParameter("@ModuleID", SqlDbType.VarChar, 100) { Value = ModuleID },
                  new SqlParameter("@Status", SqlDbType.VarChar, 100) { Value = status.ToString() },
                  new SqlParameter("@RequestID", SqlDbType.VarChar, 100) { Value = RequestID }
                };

            SqlHelper.ExecuteNonQuery(oConn.oConnEmrMaster, "LogRequest", parameters).ToString();

        }

        public void LogPushToIntelliH(ConnectionModel oConn, Logs.Status status, string RequestID, string EMRID, string ModuleID, string PatientID)
        {
            SqlParameter[] parameters =
                {
                  new SqlParameter("@PatientID", SqlDbType.VarChar, 100) { Value = PatientID },
                  new SqlParameter("@EMRID", SqlDbType.VarChar, 100) { Value = EMRID },
                  new SqlParameter("@ModuleID", SqlDbType.VarChar, 100) { Value = ModuleID },
                  new SqlParameter("@Status", SqlDbType.VarChar, 100) { Value = status.ToString() },
                  new SqlParameter("@RequestID", SqlDbType.VarChar, 100) { Value = RequestID }
                };

            SqlHelper.ExecuteNonQuery(oConn.oConnEmrMaster, "LogRequest", parameters).ToString();

        }

        public string LogRequest(ConnectionModel oConn, Logs.Status status, string EMRID, string ModuleID, string PatientID)
        {
            try
            {
                SqlParameter[] parameters =
                {
                  new SqlParameter("@PatientID", SqlDbType.VarChar, 100) { Value = PatientID },
                  new SqlParameter("@EMRID", SqlDbType.VarChar, 100) { Value = EMRID },
                  new SqlParameter("@ModuleID", SqlDbType.VarChar, 100) { Value = ModuleID },
                  new SqlParameter("@Status", SqlDbType.VarChar, 100) { Value = status.ToString() },
                  new SqlParameter("@RequestID", SqlDbType.VarChar, 100) { Value = string.Empty }
                };

                string RequestID = SqlHelper.ExecuteScalar(oConn.oConnEmrMaster, "LogRequest", parameters).ToString();
                return RequestID;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void LogRequest(ConnectionModel oConn, Logs.Status status, string RequestID, string EMRID, string ModuleID, string PatientID)
        {
            SqlParameter[] parameters =
                {
                  new SqlParameter("@PatientID", SqlDbType.VarChar, 100) { Value = PatientID },
                  new SqlParameter("@EMRID", SqlDbType.VarChar, 100) { Value = EMRID },
                  new SqlParameter("@ModuleID", SqlDbType.VarChar, 100) { Value = ModuleID },
                  new SqlParameter("@Status", SqlDbType.VarChar, 100) { Value = status.ToString() },
                  new SqlParameter("@RequestID", SqlDbType.VarChar, 100) { Value = RequestID.ToString() }
                };

            SqlHelper.ExecuteNonQuery(oConn.oConnEmrMaster, "LogRequest", parameters).ToString();
        }

        public void LogStagingUpdated(ConnectionModel oConn, Logs.Status status, string RequestID, string EMRID, string ModuleID, string PatientID)
        {
            SqlParameter[] parameters =
                {
                  new SqlParameter("@PatientID", SqlDbType.VarChar, 100) { Value = PatientID },
                  new SqlParameter("@EMRID", SqlDbType.VarChar, 100) { Value = EMRID },
                  new SqlParameter("@ModuleID", SqlDbType.VarChar, 100) { Value = ModuleID },
                  new SqlParameter("@Status", SqlDbType.VarChar, 100) { Value = status.ToString() },
                  new SqlParameter("@RequestID", SqlDbType.VarChar, 100) { Value = RequestID }
                };

            SqlHelper.ExecuteNonQuery(oConn.oConnEmrMaster, "LogRequest", parameters).ToString();
        }
    }
}
