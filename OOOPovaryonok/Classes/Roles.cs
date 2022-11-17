using System;
using System.Linq;

namespace OOOPovaryonok.Classes
{
    public class Roles
    {
        public int? roleID;
        public string nameRole;

        
        public int? GetRoleID(string login, string secret)
        {
            try
            {
                roleID = SQLDatabaseHelper.DatabaseContext.User.
                   Where(x => x.UserLogin == login && x.UserPassword == secret).FirstOrDefault().UserRole;     

                nameRole = SQLDatabaseHelper.DatabaseContext.Role.
                    Where(x => x.RoleID == roleID).FirstOrDefault().RoleName;
            }
            catch (Exception)
            {
                roleID = -1;

            }
            return roleID;
        }
    }
}
