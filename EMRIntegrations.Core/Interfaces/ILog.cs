using EMRIntegrations.Core.Enums;
//using EMRIntegration.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMRIntegrations.Core.Interfaces
{
    public interface ILog
    {
        string LogRequest(EMRIntegrations.Core.Models.ConnectionModel oConn, Logs.Status status, string EMRID, string ModuleID, string PatientID);
        void LogRequest(EMRIntegrations.Core.Models.ConnectionModel oConn, Logs.Status status, string RequestID, string EMRID, string ModuleID, string PatientID);
        void LogStagingUpdated(EMRIntegrations.Core.Models.ConnectionModel oConn, Logs.Status status, string RequestID, string EMRID, string ModuleID, string PatientID);
        void LogMapping(EMRIntegrations.Core.Models.ConnectionModel oConn, Logs.Status status, string RequestID, string EMRID, string ModuleID, string PatientID);
        void LogPushToIntelliH(EMRIntegrations.Core.Models.ConnectionModel oConn, Logs.Status status, string RequestID, string EMRID, string ModuleID, string PatientID);
        void LogError(EMRIntegrations.Core.Models.ConnectionModel oConn, Logs.Status status, string RequestID, string EMRID, string ModuleID, string PatientID);
    }
}
