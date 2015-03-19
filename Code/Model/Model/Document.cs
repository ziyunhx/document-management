using System;

namespace Model
{
    public class Document
	{
        private string _ID;
        private string _Name;
        private string _URL;
        private string _Remark;
        private string _WID;
        private string _WStep;
        private string _Result;
        private Guid _FlowInstranceID;
        private string _UID;

        public string ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }

        public string URL
        {
            get
            {
                return this._URL;
            }
            set
            {
                this._URL = value;
            }
        }

        public string Remark
        {
            get
            {
                return this._Remark;
            }
            set
            {
                this._Remark = value;
            }
        }

        public string WID
        {
            get
            {
                return this._WID;
            }
            set
            {
                this._WID = value;
            }
        }

        public string WStep
        {
            get
            {
                return this._WStep;
            }
            set
            {
                this._WStep = value;
            }
        }

        public string Result
        {
            get
            {
                return this._Result;
            }
            set
            {
                this._Result = value;
            }
        }

        public Guid FlowInstranceID
        {
            get
            {
                return this._FlowInstranceID;
            }
            set
            {
                this._FlowInstranceID = value;
            }
        }

        public string UID
        {
            get
            {
                return this._UID;
            }
            set
            {
                this._UID = value;
            }
        }
	}
}
