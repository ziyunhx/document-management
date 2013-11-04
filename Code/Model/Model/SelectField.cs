namespace Model
{
    using System;

    public class SelectField
    {
        private string condition;
        private string field;
        private string newfield;
        private string tablename;

        public SelectField(string tablename, string field, string newfield, string condition)
        {
            this.tablename = tablename;
            this.field = field;
            this.newfield = newfield;
            this.condition = condition;
        }

        public string Condition
        {
            get
            {
                return this.condition;
            }
            set
            {
                this.condition = value;
            }
        }

        public string Field
        {
            get
            {
                return this.field;
            }
            set
            {
                this.field = value;
            }
        }

        public string NewField
        {
            get
            {
                return this.newfield;
            }
            set
            {
                this.newfield = value;
            }
        }

        public string TableName
        {
            get
            {
                return this.tablename;
            }
            set
            {
                this.tablename = value;
            }
        }
    }
}

