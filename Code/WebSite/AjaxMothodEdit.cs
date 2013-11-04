using Ajax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

public class AjaxMothodEdit : Page
{
    private StringBuilder text = new StringBuilder();

    [AjaxMethod(HttpSessionStateRequirement.Read)]
    public string DelAuthorize(string id)
    {
        if (BLL.GeneralMethods.GeneralDelDB("Role", "where id=" + id).ToString() == "1")
        {
            return "操作成功";
        }
        return "操作失败";
    }

    [AjaxMethod(HttpSessionStateRequirement.Read)]
    public string GeneralDelDB(string id)
    {
        if (BLL.GeneralMethods.GeneralDelDB("RoleUSER", "where userid=" + id).ToString() == "1")
        {
            return "操作成功";
        }
        return "操作失败";
    }

    [AjaxMethod(HttpSessionStateRequirement.Read)]
    public string UpdateUserisStates(string id, string isState)
    {
        string filevalue = (isState == "0") ? "1" : "0";
        if (BLL.GeneralMethods.GeneralUPdateDB("RoleUSER", " isState", filevalue, "userid=" + id).ToString() == "1")
        {
            return "操作成功";
        }
        return "操作失败";
    }
}
