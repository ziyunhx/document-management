namespace Model
{
    using System;

    public class SelectByPager
    {
        private string col;
        private string columnlist;
        private bool docount;
        private string order;
        private int pageindex;
        private int pagesize;
        private string tabs;
        private string where;

        public SelectByPager()
        {
        }

        public SelectByPager(string tabs, string col, bool docount, string columnlist, int pagesize, int pageindex, string where, string order)
        {
            this.tabs = tabs;
            this.col = col;
            this.order = order;
            this.columnlist = columnlist;
            this.pagesize = pagesize;
            this.pageindex = pageindex;
            this.where = where;
            this.docount = docount;
        }

        public string Col
        {
            get
            {
                return this.col;
            }
            set
            {
                this.col = value;
            }
        }

        public string Columnlist
        {
            get
            {
                return this.columnlist;
            }
            set
            {
                this.columnlist = value;
            }
        }

        public bool DoCount
        {
            get
            {
                return this.docount;
            }
            set
            {
                this.docount = value;
            }
        }

        public string Order
        {
            get
            {
                return this.order;
            }
            set
            {
                this.order = value;
            }
        }

        public int PageIndex
        {
            get
            {
                return this.pageindex;
            }
            set
            {
                this.pageindex = value;
            }
        }

        public int Pagesize
        {
            get
            {
                return this.pagesize;
            }
            set
            {
                this.pagesize = value;
            }
        }

        public string Tabs
        {
            get
            {
                return this.tabs;
            }
            set
            {
                this.tabs = value;
            }
        }

        public string Where
        {
            get
            {
                return this.where;
            }
            set
            {
                this.where = value;
            }
        }
    }
}

