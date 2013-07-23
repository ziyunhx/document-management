namespace BLL
{
    using DAL;
    using Model;
    using System;
    using System.Data;

    public class Users
    {
        public static int CheckEmail(string Email)
        {
            return DAL.Users.CheckEmail(Email);
        }

        public static int CheckUserID(string userID)
        {
            return DAL.Users.CheckUserID(userID);
        }

        public static int DelUesrsInfo(string uid, string isState)
        {
            return DAL.Users.DelUesrsInfo(uid, isState);
        }

        public static int EditPwd(int uid, string oldpwd, string newpwd)
        {
            return DAL.Users.EditPwd(uid, oldpwd, newpwd);
        }

        public static DataSet SelectForUserID(string ID)
        {
            return DAL.Users.SelectForUserID(ID);
        }

        public static int TelOrEmail_Verf(int uid, int verf)
        {
            return DAL.Users.TelOrEmail_Verf(uid, verf);
        }

        public static int UpdateLoginCount(string UID)
        {
            return DAL.Users.UpdateLoginCount(UID);
        }

        public static int UserAdd(Model.Users userInfo)
        {
            return DAL.Users.UserAdd(userInfo);
        }

        public static int UserDel(string ID)
        {
            return DAL.Users.UserDel(ID);
        }

        public static DataSet UserLogin(string UserID, string Password)
        {
            return DAL.Users.UserLogin(UserID, Password);
        }

        public static int UsersUpdateForIsState(string ID, string IsState)
        {
            return DAL.Users.UsersUpdateForIsState(ID, IsState);
        }

        public static int UserUpdate(Model.Users userInfo)
        {
            return DAL.Users.UserUpdate(userInfo);
        }

        public static int UserRoleAdd(string uid, string rid)
        {
            return DAL.Users.UserRoleAdd(uid, rid);
        }

        public static int UserRoleEdit(string uid, string rid)
        {
            return DAL.Users.UserRoleEdit(uid, rid);
        }
    }
}

