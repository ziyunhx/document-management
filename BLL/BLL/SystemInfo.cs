using System;
namespace BLL
{


    public class SystemInfo
    {
        public static int UpdateInfo(string id, string value)
        {
            return DAL.SystemInfo.UpdateInfo(id, value);
        }
    }
}

