namespace Model
{
    using System;

    public class UserFinanceLog
    {
        private string _Amount;
        private string _CreateTime;
        private string _FinanceInfo;
        private string _FinanceType;
        private string _ID;
        private string _IsState = "1";
        private string _UID;

        public string Amount
        {
            get
            {
                return this._Amount;
            }
            set
            {
                this._Amount = value;
            }
        }

        public string CreateTime
        {
            get
            {
                return this._CreateTime;
            }
            set
            {
                this._CreateTime = value;
            }
        }

        public string FinanceInfo
        {
            get
            {
                return this._FinanceInfo;
            }
            set
            {
                this._FinanceInfo = value;
            }
        }

        public string FinanceType
        {
            get
            {
                return this._FinanceType;
            }
            set
            {
                this._FinanceType = value;
            }
        }

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

        public string IsState
        {
            get
            {
                return this._IsState;
            }
            set
            {
                this._IsState = value;
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

