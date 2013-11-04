namespace Commons
{
    using System;
    using System.Configuration;

    public class GetEmailInfo
    {
        private string adminEmail;
        private string emailName;
        private string emailpassword;
        private string projects;
        private string username;

        public GetEmailInfo()
        {
            this.EmailName = ConfigurationSettings.AppSettings["EmailName"].ToString();
            this.Emailpassword = ConfigurationSettings.AppSettings["EmailPassword"].ToString();
            this.Username = ConfigurationSettings.AppSettings["UserName"].ToString();
            this.AdminEmail = ConfigurationSettings.AppSettings["adminemail"].ToString();
        }

        public string AdminEmail
        {
            get
            {
                return this.adminEmail;
            }
            set
            {
                this.adminEmail = value;
            }
        }

        public string EmailName
        {
            get
            {
                return this.emailName;
            }
            set
            {
                this.emailName = value;
            }
        }

        public string Emailpassword
        {
            get
            {
                return this.emailpassword;
            }
            set
            {
                this.emailpassword = value;
            }
        }

        public string Projects
        {
            get
            {
                return this.projects;
            }
            set
            {
                this.projects = value;
            }
        }

        public string Username
        {
            get
            {
                return this.username;
            }
            set
            {
                this.username = value;
            }
        }
    }
}

