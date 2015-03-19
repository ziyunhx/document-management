namespace Model
{
	public class WorkFlow
	{
        private string _ID;
        private string _Name;
        private string _URL;
        private string _State;
        private string _Remark;

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
	}
}
