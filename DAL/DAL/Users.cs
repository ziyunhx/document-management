namespace DAL
{
    using DBAccess;
    using Model;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class Users
    {
        public static int CheckEmail(string Email)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@UserEmail", SqlDbType.VarChar, 50) };
            pars[0].Value = Email;
            return SqlHelper.ExecuteProcess("checkUserEmail", pars);
        }

        public static int CheckUserID(string userID)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.VarChar, 50) };
            pars[0].Value = userID;
            return SqlHelper.ExecuteProcess("checkUserID", pars);
        }

        public static int DelUesrsInfo(string uid, string isState)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@uid", SqlDbType.Int), new SqlParameter("@isState", SqlDbType.Int) };
            pars[0].Value = uid;
            pars[1].Value = isState;
            return SqlHelper.ExecuteProcess("pro_DelUserInfo", pars);
        }

        public static int EditPwd(int uid, string oldpwd, string newpwd)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@uid", SqlDbType.Int), new SqlParameter("@oldpwd", SqlDbType.NVarChar, 50), new SqlParameter("@pwd", SqlDbType.VarChar, 50) };
            pars[0].Value = uid;
            pars[1].Value = oldpwd;
            pars[2].Value = newpwd;
            return SqlHelper.ExecuteProcess("proc_EditPwd", pars);
        }

        public static int Login(string UserID, string Password)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@userid", SqlDbType.VarChar, 50), new SqlParameter("@userpass", SqlDbType.VarChar, 50) };
            pars[0].Value = UserID;
            pars[1].Value = Password;
            return SqlHelper.ExecuteProcess("CheckUser", pars);
        }

        public static DataSet SelectForUserID(string ID)
        {
            return SqlHelper.GetAllInfo("select * from Users where ID=" + ID);
        }

        public static int TelOrEmail_Verf(int uid, int verf)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@uid", SqlDbType.Int), new SqlParameter("@verf", SqlDbType.Int) };
            pars[0].Value = uid;
            pars[1].Value = verf;
            return SqlHelper.ExecuteProcess("pro_TelOrEmail_Verf", pars);
        }

        public static int UpdateLoginCount(string UID)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@UID", SqlDbType.Int) };
            pars[0].Value = UID;
            return SqlHelper.ExecuteProcess("UpdateLoginCount", pars);
        }

        public static int UserAdd(Model.Users userInfo)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.VarChar, 50), new SqlParameter("@Email", SqlDbType.VarChar, 50), new SqlParameter("@Password", SqlDbType.VarChar, 50), new SqlParameter("@UserName", SqlDbType.VarChar, 30), new SqlParameter("@Telephone", SqlDbType.VarChar, 50), new SqlParameter("@QQ", SqlDbType.VarChar, 30), new SqlParameter("@OID", SqlDbType.VarChar, 50) };
            pars[0].Value = userInfo.UserID;
            pars[1].Value = userInfo.Email;
            pars[2].Value = userInfo.Password;
            pars[3].Value = userInfo.UserName;
            pars[4].Value = userInfo.Telephone;
            pars[5].Value = userInfo.QQ;
            pars[6].Value = userInfo.OID;
            return SqlHelper.ExecuteProcess("pro_User_Add", pars);
        }

        public static int UserDel(string ID)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int) };
            pars[0].Value = ID;
            return SqlHelper.ExecuteProcess("pro_User_Del", pars);
        }

        public static DataSet UserLogin(string UserID, string Password)
        {
            return SqlHelper.GetAllInfo(string.Format("select * from Users where (UserID='{0}' or Email='{0}') and Password='{1}'", UserID, Password));
        }

        public static int UsersUpdateForIsState(string ID, string IsState)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int), new SqlParameter("@IsState", SqlDbType.Int) };
            pars[0].Value = ID;
            pars[1].Value = IsState;
            return SqlHelper.ExecuteProcess("pro_User_stauts_Edit", pars);
        }

        public static int UserUpdate(Model.Users userInfo)
        {
            if (userInfo.Password == null)
            {
                SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int), new SqlParameter("@UserID", SqlDbType.VarChar, 50), new SqlParameter("@Email", SqlDbType.VarChar, 50), new SqlParameter("@UserName", SqlDbType.VarChar, 30), new SqlParameter("@Telephone", SqlDbType.VarChar, 50), new SqlParameter("@QQ", SqlDbType.VarChar, 30), new SqlParameter("@OID", SqlDbType.VarChar, 50) };
                pars[0].Value = userInfo.ID;
                pars[1].Value = userInfo.UserID;
                pars[2].Value = userInfo.Email;
                pars[3].Value = userInfo.UserName;
                pars[4].Value = userInfo.Telephone;
                pars[5].Value = userInfo.QQ;
                pars[6].Value = userInfo.OID;
                return SqlHelper.ExecuteProcess("pro_User_NoPasswordUpdate", pars);
            }
            else
            {
                SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int), new SqlParameter("@UserID", SqlDbType.VarChar, 50), new SqlParameter("@Email", SqlDbType.VarChar, 50), new SqlParameter("@Password", SqlDbType.VarChar, 50), new SqlParameter("@UserName", SqlDbType.VarChar, 30), new SqlParameter("@Telephone", SqlDbType.VarChar, 50), new SqlParameter("@QQ", SqlDbType.VarChar, 30), new SqlParameter("@OID", SqlDbType.VarChar, 50) };
                pars[0].Value = userInfo.ID;
                pars[1].Value = userInfo.UserID;
                pars[2].Value = userInfo.Email;
                pars[3].Value = userInfo.Password;
                pars[4].Value = userInfo.UserName;
                pars[5].Value = userInfo.Telephone;
                pars[6].Value = userInfo.QQ;
                pars[7].Value = userInfo.OID;
                return SqlHelper.ExecuteProcess("pro_User_Update", pars);
            }
        }

        public static int UserRoleAdd(string uid, string rid)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@roleid", SqlDbType.Int), new SqlParameter("@userid", SqlDbType.Int) };
            pars[0].Value = rid;
            pars[1].Value = uid;
            return SqlHelper.ExecuteProcess("pro_Add_RoleUsers", pars);
        }

        public static int UserRoleEdit(string uid, string rid)
        {
            SqlParameter[] pars = new SqlParameter[] { new SqlParameter("@roleid", SqlDbType.Int), new SqlParameter("@userid", SqlDbType.Int) };
            pars[0].Value = rid;
            pars[1].Value = uid;
            return SqlHelper.ExecuteProcess("pro_Edit_RoleUsers", pars);
        }
    }
}

