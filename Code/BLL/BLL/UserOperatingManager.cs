namespace BLL
{
    using DAL;
    using System;

    public class UserOperatingManager
    {
        public static int InputUserOperating(string uid, string actionType, string Descriptions)
        {
            return DAL.UserOperatingManager.InputUserOperating(uid, actionType, Descriptions);
        }
    }
}

