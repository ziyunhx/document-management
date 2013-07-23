namespace Model
{
    using System;

    public class Users
    {
        private string _Email;
        private string _ID;
        private string _IsState;
        private string _Password;
        private string _QQ;
        private string _Telephone;
        private string _UserID;
        private string _UserName;
        private string _OID;


        public string Email
        {
            get
            {
                return this._Email;
            }
            set
            {
                this._Email = value;
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

        public string Password
        {
            get
            {
                return this._Password;
            }
            set
            {
                this._Password = value;
            }
        }

        public string QQ
        {
            get
            {
                return this._QQ;
            }
            set
            {
                this._QQ = value;
            }
        }

        public string Telephone
        {
            get
            {
                return this._Telephone;
            }
            set
            {
                this._Telephone = value;
            }
        }

        public string UserID
        {
            get
            {
                return this._UserID;
            }
            set
            {
                this._UserID = value;
            }
        }

        public string UserName
        {
            get
            {
                return this._UserName;
            }
            set
            {
                this._UserName = value;
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
    }
}

