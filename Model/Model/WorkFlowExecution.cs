namespace Model
{
    public class WorkFlowExecution
	{
        private string _ID;
        private string _DID;
        private string _UID;
        private string _step;
        private string _Remark;
        private string _Result;

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

        public string DID
        {
            get
            {
                return this._DID;
            }
            set
            {
                this._DID = value;
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

        public string step
        {
            get
            {
                return this._step;
            }
            set
            {
                this._step = value;
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
	}
}
