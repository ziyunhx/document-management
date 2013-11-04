namespace Model
{
    public class WorkFlowRole
	{
        private string _ID;
        private string _OID;
        private string _WID;
        private string _WSTEP;
        private string _name;
        private string _value;
        private string _State;

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

        public string OID
        {
            get
            {
                return this._OID;
            }
            set
            {
                this._OID = value;
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

        public string WSTEP
        {
            get
            {
                return this._WSTEP;
            }
            set
            {
                this._WSTEP = value;
            }
        }

        public string name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        public string value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        public string State
        {
            get
            {
                return this._State;
            }
            set
            {
                this._State = value;
            }
        }
	}
}
