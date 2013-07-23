using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Commons
{
    public class MessageBox
    {
        public static void ResponseScript(Page page, string script)
        {
            page.RegisterStartupScript("message", "<script language='javascript' defer>" + script + "</script>");
        }

        public static void Show(Page page, string msg)
        {
            page.RegisterStartupScript("message", "<script language='javascript' defer>alert('" + msg.ToString() + "');</script>");
        }

        public static void ShowAndRedirect(Page page, string msg, string url)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<script language='javascript' defer>");
            builder.AppendFormat("alert('{0}');", msg);
            builder.AppendFormat("location.href='{0}'", url);
            builder.Append("</script>");
            page.RegisterStartupScript("message", builder.ToString());
        }

        public static void ShowAndRedirectDome(Page page, string msg, string url)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<script language='javascript' defer>");
            builder.AppendFormat("alert('{0}');", msg);
            builder.AppendFormat("parent.location.href='{0}'", url);
            builder.Append("</script>");
            page.RegisterStartupScript("message", builder.ToString());
        }

        public static void ShowConfirm(WebControl Control, string msg)
        {
            Control.Attributes.Add("onclick", "return confirm('" + msg + "');");
        }
    }
}

