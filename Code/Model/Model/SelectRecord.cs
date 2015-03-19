namespace Model
{
    using System;

    public class SelectRecord
    {
        private string irecord;
        private string scolumnlist;
        private string scondition;
        private string stablename;

        public SelectRecord()
        {
        }

        public SelectRecord(string tablename, string record, string columnlist, string condition)
        {
            this.stablename = tablename;
            this.irecord = record;
            this.scolumnlist = columnlist;
            this.scondition = condition;
        }

        public string Irecord
        {
            get
            {
                return this.irecord;
            }
            set
            {
                this.irecord = value;
            }
        }

        public string Scolumnlist
        {
            get
            {
                return this.scolumnlist;
            }
            set
            {
                this.scolumnlist = value;
            }
        }

        public string Scondition
        {
            get
            {
                return this.scondition;
            }
            set
            {
                this.scondition = value;
            }
        }

        public string Stablename
        {
            get
            {
                return this.stablename;
            }
            set
            {
                this.stablename = value;
            }
        }
    }
}

