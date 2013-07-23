namespace Model
{
    public class Organizational
	{
        private string _ID;
        private string _Name;
        private string _PID;

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

        public string PID
        {
            get
            {
                return this._PID;
            }
            set
            {
                this._PID = value;
            }
        }
	}
}
