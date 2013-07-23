using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace BLL
{
    public class GeneralMethods
    {
        private const string StrKeyWord = @"<|>|select|insert|update|delete\(|drop table|truncate\(|mid\(|char\(|xp_cmdshell|exec master|netlocalgroup|administrators|net user";
        private const string StrRegex = @"[;|\(|)|[|]|}|{|']";

        public static bool CheckKeyWord(string _sWord)
        {
            if (!(Regex.IsMatch(_sWord, @"<|>|select|insert|update|delete\(|drop table|truncate\(|mid\(|char\(|xp_cmdshell|exec master|netlocalgroup|administrators|net user", RegexOptions.IgnoreCase) || Regex.IsMatch(_sWord, @"[;|\(|)|[|]|}|{|']")))
            {
                return false;
            }
            return true;
        }

        private static string Format_HashTable(Hashtable hastb)
        {
            string str = "";
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            foreach (string str2 in hastb.Keys)
            {
                builder.Append(str2.ToString());
                builder.Append(" & ");
                object obj2 = hastb[str2];
                if (obj2.GetType().Name.Equals(string.Empty.GetType().Name))
                {
                    builder2.Append("'" + obj2 + "'");
                    builder2.Append(" & ");
                }
                else if (obj2.GetType().Name.Equals(DateTime.Now.GetType().Name))
                {
                    builder2.Append("'" + obj2 + "'");
                    builder2.Append(" & ");
                }
                else
                {
                    try
                    {
                        builder2.Append(obj2.ToString());
                        builder2.Append(" & ");
                    }
                    catch (Exception)
                    {
                        builder2.Append("");
                        builder2.Append(" & ");
                    }
                }
            }
            str = builder.ToString().Trim().TrimEnd(new char[] { '&' }) + "#" + builder2.ToString().Trim().Trim(new char[] { '&' });
            hastb.Clear();
            return str;
        }

        public static int GeneralDelDB(string tablename, string where)
        {
            try
            {
                if (CheckKeyWord(tablename) && CheckKeyWord(where))
                {
                    return -1;
                }
                return DAL.GeneralMethods.GeneralDelDB(tablename, where);
            }
            catch (Exception)
            {
                return -2;
            }
        }

        public static string GeneralEditDB_Hash(string tablename, Hashtable hastb, string SqlWhere)
        {
            string str = Format_HashTable(hastb);
            try
            {
                if ((CheckKeyWord(tablename) && CheckKeyWord(str.Split(new char[] { '#' })[0].ToString())) && (CheckKeyWord(str.Split(new char[] { '#' })[1].ToString()) && CheckKeyWord(SqlWhere)))
                {
                    return "-1";
                }
                return DAL.GeneralMethods.GeneralUPdateDB(tablename, str.Split(new char[] { '#' })[0].ToString(), str.Split(new char[] { '#' })[1].ToString(), SqlWhere).ToString();
            }
            catch (Exception exception)
            {
                return exception.ToString();
            }
        }

        public static int GeneralInsertDB(string tablename, string filename, string filevale)
        {
            try
            {
                if ((CheckKeyWord(tablename) && CheckKeyWord(filename)) && CheckKeyWord(filevale))
                {
                    return -1;
                }
                return DAL.GeneralMethods.GeneralInsertDB(tablename, filename, filevale);
            }
            catch (Exception)
            {
                return -2;
            }
        }

        public static string GeneralInsertDB_Array(string tablename, ArrayList list)
        {
            StringBuilder builder = new StringBuilder();
            foreach (object obj2 in list)
            {
                if (obj2.GetType().Name.Equals(string.Empty.GetType().Name))
                {
                    builder.Append("'" + obj2.ToString() + "'");
                    builder.Append(" , ");
                }
                else
                {
                    try
                    {
                        builder.Append(obj2.ToString());
                        builder.Append(" , ");
                    }
                    catch (Exception)
                    {
                        builder.Append("");
                        builder.Append(" , ");
                    }
                }
            }
            try
            {
                if (CheckKeyWord(tablename) && CheckKeyWord(builder.ToString()))
                {
                    return "-1";
                }
                return DAL.GeneralMethods.GeneralInsertDB(tablename, "", builder.ToString().Trim().TrimEnd(new char[] { ',' })).ToString();
            }
            catch (Exception)
            {
                return "-2";
            }
        }

        public static string GeneralInsertDB_Hash(string tablename, Hashtable hastb)
        {
            string str = Format_HashTable(hastb);
            try
            {
                if ((CheckKeyWord(tablename) && CheckKeyWord(str.Split(new char[] { '&' })[0].ToString())) && CheckKeyWord(str.Split(new char[] { '&' })[1].ToString()))
                {
                    return "-1";
                }
                return DAL.GeneralMethods.GeneralInsertDB(tablename, str.Split(new char[] { '&' })[0].ToString(), str.Split(new char[] { '&' })[0].ToString()).ToString();
            }
            catch (Exception)
            {
                return "-1";
            }
        }

        public static int GeneralUPdateDB(string tablename, string filename, string filevalue, string where)
        {
            try
            {
                if ((CheckKeyWord(tablename) && CheckKeyWord(filename)) && (CheckKeyWord(filevalue) && CheckKeyWord(where)))
                {
                    return -1;
                }
                return DAL.GeneralMethods.GeneralUPdateDB(tablename, filename, filevalue, where);
            }
            catch (Exception)
            {
                return -2;
            }
        }

        public static bool GetPermissions(string url, string uid)
        {
            string str = ModuleName(url);
            if ((str != null) && (str != ""))
            {
                Model.SelectRecord selectRecord = new Model.SelectRecord("view_UserPermissions", "", "*", "where uid=" + uid + " and url like '%" + str.ToLower() + "%'");
                DataTable table = BLL.SelectRecord.SelectRecordData(selectRecord).Tables[0];
                if (table.Rows.Count > 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static string ModuleName(string path)
        {
            string[] strArray = path.Split(new char[] { '/' });
            if (strArray.Length > 0)
            {
                return strArray[strArray.Length - 1].Trim().Split(new char[] { '?' })[0].Trim();
            }
            return "fjoefjj";
        }
    }
}

