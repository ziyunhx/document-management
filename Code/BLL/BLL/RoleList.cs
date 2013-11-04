using System;
namespace BLL
{


    public class RoleList
    {
        public static int DelMenuRIGHT(string roleid)
        {
            return DAL.RoleList.DelMenuRIGHT(roleid);
        }

        public static int setRole(string name, string isstate)
        {
            return DAL.RoleList.setRole(name, isstate);
        }

        public static int setMenuRIGHT(string flowid, string roleid)
        {
            return DAL.RoleList.setMenuRIGHT(flowid, roleid);
        }

        public static int UpdateRole(string id, string name, string isstate)
        {
            return DAL.RoleList.UpdateRole(id, name, isstate);
        }
    }
}

