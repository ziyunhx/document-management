using System;
using System.Data;
using System.IO;
using System.Text;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Session["admin"] != null && this.Session["IsState"] != null)
        {
            try
            {
                //StringBuilder builder = new StringBuilder();
                //builder.Append("");
                //Model.SelectRecord selectRecord = new Model.SelectRecord("GeneralStatistics", "", "*", "where 1=1");
                //DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
                //if (table.Rows.Count > 0)
                //{
                //    for (int i = 0; i < table.Rows.Count; i++)
                //    {
                //        int num = int.Parse(table.Rows[i]["SumAmount"].ToString()) * 3;
                //        int num3 = int.Parse(table.Rows[i]["TranQuantity"].ToString()) * 3;
                //        int num4 = int.Parse(table.Rows[i]["NewUserCount"].ToString()) * 3;
                //        int num5 = int.Parse(table.Rows[i]["WebsiteCount"].ToString());
                //        builder.Append("<ul class='clearfix_gd'>");
                //        builder.AppendFormat("<li class='tle'><strong>{0}：</strong></li>", table.Rows[i]["stattime"].ToString());
                //        builder.AppendFormat("<li class='line'></li><li>交易总额：<em>{0}</em>元</li>", this.GetStr(num));
                //        builder.AppendFormat("<li class='line'></li><li>交易数量：<em>{0}</em>笔</li>", this.GetStr(num3));
                //        builder.AppendFormat("<li class='line'></li><li>新增会员：<em>{0}</em>个</li>", this.GetStr(num4));
                //        builder.AppendFormat("<li class='line'></li><li>网站总计：<em>{0}</em>个</li>", this.GetStr(num5));
                //        builder.Append("</ul><div class='qingchu_gd'></div>");
                //    }
                //    FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                //    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                //    string str3 = reader.ReadToEnd();
                //    reader.Close();
                //    stream.Close();
                //    StreamWriter writer = new StreamWriter(str2, false, Encoding.UTF8);
                //    writer.WriteLine(str3.Replace("$html_statics", builder.ToString()));
                //    writer.Close();
                //}
            }
            catch (Exception exception)
            {
                base.Response.Write(exception.ToString());
            }
        }
    }
}
