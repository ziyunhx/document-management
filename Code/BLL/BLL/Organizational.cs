using DAL;
using Model;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace BLL
{
    public class Organizational
    {
        public static string BindToDropDownList(string selectValue)
        {
            string str = "";
            Model.SelectRecord record2 = new Model.SelectRecord
            {
                Irecord = "",
                Scolumnlist = "ID,Name",
                Stablename = "Organizational",
                Scondition = "where PID='0'"
            };
            Model.SelectRecord selectRecord = record2;
            DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
            if ((table != null) && (table.Rows.Count != 0))
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string str2 = str;
                    str = str2 + " <asp:ListItem Text='" + table.Rows[i]["Name"].ToString() + "' Value='" + table.Rows[i]["ID"].ToString() + "'></asp:ListItem>\n\t\t";
                    selectRecord.Scondition = "where PID='" + table.Rows[i]["ID"].ToString() + "'";
                    DataTable table2 = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
                    if ((table2 != null) && (table2.Rows.Count != 0))
                    {
                        for (int j = 0; j < table2.Rows.Count; j++)
                        {
                            string str3 = str;
                            str = str3 + " <asp:ListItem Text='     " + table2.Rows[j]["Name"].ToString() + "' Value='" + table2.Rows[j]["ID"].ToString() + "'></asp:ListItem>\n\t\t";
                            selectRecord.Scondition = "where PID='" + table2.Rows[j]["ID"].ToString() + "'";
                            DataTable table3 = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
                            if ((table3 != null) && (table3.Rows.Count != 0))
                            {
                                for (int k = 0; k < table3.Rows.Count; k++)
                                {
                                    string str4 = str;
                                    str = str4 + " <asp:ListItem Text='        " + table3.Rows[k]["Name"].ToString() + "' Value='" + table3.Rows[k]["ID"].ToString() + "'></asp:ListItem>\n\t\t";
                                }
                                table3.Clear();
                                table3.Dispose();
                            }
                        }
                        table2.Clear();
                        table2.Dispose();
                    }
                }
                table.Clear();
                table.Dispose();
            }
            if (selectValue == "0")
            {
                str = str + " <asp:ListItem Text='请选择岗位信息' Value='0' Selected='True'></asp:ListItem>\n\t\t";
            }
            return str;
        }

        public static void BindToDropDownList(DropDownList ddlID, string selectValue)
        {
            Model.SelectRecord selectRecord = new Model.SelectRecord
            {
                Irecord = "",
                Scolumnlist = "ID,Name",
                Stablename = "Organizational",
                Scondition = "where PID='0'"
            };
            DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
            if ((table != null) && (table.Rows.Count != 0))
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    ListItem item6 = new ListItem
                    {
                        Value = table.Rows[i]["ID"].ToString(),
                        Text = table.Rows[i]["Name"].ToString()
                    };
                    ListItem item = item6;
                    ddlID.Items.Add(item);
                    selectRecord.Scondition = "where PID='" + item.Value + "'";
                    DataTable table2 = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
                    if ((table2 != null) && (table2.Rows.Count != 0))
                    {
                        for (int j = 0; j < table2.Rows.Count; j++)
                        {
                            ListItem item5 = new ListItem
                            {
                                Value = table2.Rows[j]["ID"].ToString(),
                                Text = "　　" + table2.Rows[j]["Name"].ToString()
                            };
                            ListItem item2 = item5;
                            ddlID.Items.Add(item2);
                            selectRecord.Scondition = "where PID='" + item2.Value + "'";
                            DataTable table3 = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
                            if ((table3 != null) && (table3.Rows.Count != 0))
                            {
                                for (int k = 0; k < table3.Rows.Count; k++)
                                {
                                    ListItem item4 = new ListItem
                                    {
                                        Value = table3.Rows[k]["ID"].ToString(),
                                        Text = "　　　　" + table3.Rows[k]["Name"].ToString()
                                    };
                                    ListItem item3 = item4;
                                    ddlID.Items.Add(item3);
                                }
                                table3.Clear();
                                table3.Dispose();
                            }
                        }
                        table2.Clear();
                        table2.Dispose();
                    }
                }
                table.Clear();
                table.Dispose();
            }

            ListItem item8 = new ListItem
            {
                Text = "请选择岗位信息",
                Value = "0"
            };
            ddlID.Items.Add(item8);

            ddlID.DataBind();
            ddlID.Items.FindByValue(selectValue).Selected = true;
        }

        public static void BindToListBox(ListBox ddlID, string selectValue)
        {
            Model.SelectRecord selectRecord = new Model.SelectRecord
            {
                Irecord = "",
                Scolumnlist = "ID,Name",
                Stablename = "Organizational",
                Scondition = "where PID='0'"
            };
            DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
            if ((table != null) && (table.Rows.Count != 0))
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    ListItem item6 = new ListItem
                    {
                        Value = table.Rows[i]["ID"].ToString(),
                        Text = table.Rows[i]["Name"].ToString()
                    };
                    ListItem item = item6;
                    ddlID.Items.Add(item);
                    selectRecord.Scondition = "where PID='" + item.Value + "'";
                    DataTable table2 = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
                    if ((table2 != null) && (table2.Rows.Count != 0))
                    {
                        for (int j = 0; j < table2.Rows.Count; j++)
                        {
                            ListItem item5 = new ListItem
                            {
                                Value = table2.Rows[j]["ID"].ToString(),
                                Text = "　　" + table2.Rows[j]["Name"].ToString()
                            };
                            ListItem item2 = item5;
                            ddlID.Items.Add(item2);
                            selectRecord.Scondition = "where PID='" + item2.Value + "'";
                            DataTable table3 = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
                            if ((table3 != null) && (table3.Rows.Count != 0))
                            {
                                for (int k = 0; k < table3.Rows.Count; k++)
                                {
                                    ListItem item4 = new ListItem
                                    {
                                        Value = table3.Rows[k]["ID"].ToString(),
                                        Text = "　　　　" + table3.Rows[k]["Name"].ToString()
                                    };
                                    ListItem item3 = item4;
                                    ddlID.Items.Add(item3);
                                }
                                table3.Clear();
                                table3.Dispose();
                            }
                        }
                        table2.Clear();
                        table2.Dispose();
                    }
                }
                table.Clear();
                table.Dispose();
            }
            ddlID.DataBind();
            if (selectValue != null && selectValue != "")
            {
                string[] s = selectValue.Split(new char[] { ',' });
                for (int i = 0; i < s.Length; i++)
                {
                    for (int j = 0; j < ddlID.Items.Count; j++)
                    {
                        if (ddlID.Items[j].Value == s[i])
                        {
                            ddlID.Items[j].Selected = true;
                        }
                    }
                }
            }
        }


        public static string BindToListBox(string selectValue)
        {
            string str = "";
            Model.SelectRecord record2 = new Model.SelectRecord
            {
                Irecord = "",
                Scolumnlist = "ID,Name",
                Stablename = "Organizational",
                Scondition = "where PID='0'"
            };
            Model.SelectRecord selectRecord = record2;
            DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
            if ((table != null) && (table.Rows.Count != 0))
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string str2 = str;
                    str = str2 + " <option Value='" + table.Rows[i]["ID"].ToString() + "'>" + table.Rows[i]["Name"].ToString() + "</option>\n\t\t";
                    selectRecord.Scondition = "where PID='" + table.Rows[i]["ID"].ToString() + "'";
                    DataTable table2 = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
                    if ((table2 != null) && (table2.Rows.Count != 0))
                    {
                        for (int j = 0; j < table2.Rows.Count; j++)
                        {
                            string str3 = str;
                            str = str3 + " <option Value='" + table2.Rows[j]["ID"].ToString() + "'>&nbsp;&nbsp;&nbsp;&nbsp;" + table2.Rows[j]["Name"].ToString() + "</option>\n\t\t";
                            selectRecord.Scondition = "where PID='" + table2.Rows[j]["ID"].ToString() + "'";
                            DataTable table3 = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
                            if ((table3 != null) && (table3.Rows.Count != 0))
                            {
                                for (int k = 0; k < table3.Rows.Count; k++)
                                {
                                    string str4 = str;
                                    str = str4 + " <option Value='" + table3.Rows[k]["ID"].ToString() + "'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + table3.Rows[k]["Name"].ToString() + "</option>\n\t\t";
                                }
                                table3.Clear();
                                table3.Dispose();
                            }
                        }
                        table2.Clear();
                        table2.Dispose();
                    }
                }
                table.Clear();
                table.Dispose();
            }
            if (selectValue == "0")
            {
                str = str + " <option Value='0' selected='True'>请选择岗位信息</option>\n\t\t";
            }
            return str;
        }

        public static int OrganAdd(Model.Organizational Organ)
        {
            return DAL.Organizational.OrganAdd(Organ);
        }

        public static int OrganDelete(string ID)
        {
            return DAL.Organizational.OrganDelete(ID);
        }

        public static int OrganUpdate(Model.Organizational Organ)
        {
            return DAL.Organizational.OrganUpdate(Organ);
        }
    }
}

