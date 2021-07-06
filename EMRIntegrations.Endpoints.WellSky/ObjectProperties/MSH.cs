using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRIntegrations.Endpoints.WellSky
{
    public class MSH
    {
        private string _fieldseparator;
        public string fieldseparator
        {
            get => _fieldseparator;
            set => _fieldseparator = value;
        }

        private string _encodingcharactors;
        public string encodingcharactors
        {
            get => _encodingcharactors;
            set => _encodingcharactors = value;
        }

        private string _sendingapplication;
        public string sendingapplication
        {
            get => _sendingapplication;
            set => _sendingapplication = value;
        }

        private string _sendingfacility;
        public string sendingfacility
        {
            get => _sendingfacility;
            set => _sendingfacility = value;
        }

        private string _receivingapplication;
        public string receivingapplication
        {
            get => _receivingapplication;
            set => _receivingapplication = value;
        }

        private string _receivingfacility;
        public string receivingfacility
        {
            get => _receivingfacility;
            set => _receivingfacility = value;
        }

        private string _creationdatetime;
        public string creationdatetime
        {
            get => _creationdatetime;
            set => _creationdatetime = value;
        }

        private string _security;
        public string security
        {
            get => _security;
            set => _security = value;
        }

        private string _messagetype;
        public string messagetype
        {
            get => _messagetype;
            set => _messagetype = value;
        }

        private string _triggerevent;
        public string triggerevent
        {
            get => _triggerevent;
            set => _triggerevent = value;
        }

        private string _messagecontrolid;
        public string messagecontrolid
        {
            get => _messagecontrolid;
            set => _messagecontrolid = value;
        }

        private string _processingid;
        public string processingid
        {
            get => _processingid;
            set => _processingid = value;
        }

        private string _versionid;
        public string versionid
        {
            get => _versionid;
            set => _versionid = value;
        }

        private string _sequencenumber;
        public string sequencenumber
        {
            get => _sequencenumber;
            set => _sequencenumber = value;
        }
    }
}
