using CompanyManagement.Helper;
using CompanyManagement.ModelHelper;
using CompanyManagement.Models;
using CompanyManagement.ViewModel;

namespace CompanyManagement.Services
{
    public class RoleService : GenericService<UserRole>
    {
        private readonly CMScontext _context;
        public RoleService(CMScontext context) : base(context)
        {
            _context = context;
        }
        public ResponseMessage AddRole(UserRole userRole)
        {
            try
            {
                userRole.RoleName = userRole.RoleName.Trim();
                var isexist = _context.UserRole.Any(i => i.RoleName == userRole.RoleName.Trim());
                if (isexist) return AutoResponse.ExistMessage("Role already in the system.");
                Add(userRole);
                return AutoResponse.SuccessMessage("Role Added");
            }
            catch (Exception e)
            {
                var exmsg = e.Message.ToString() + ((e.InnerException != null) ? " ; " + e.InnerException.Message.ToString() : "");
                return AutoResponse.ExceptionOccuredMessage(exmsg);
            }
        }

        public ResponseMessage UpdateRole(UserRole userRole)
        {
            try
            {
                userRole.RoleName = userRole.RoleName.Trim();
                var rolelist = _context.UserRole.ToList();
                var existrole = rolelist.FirstOrDefault(i => i.Id == userRole.Id);
                if (existrole == null) return AutoResponse.NotFoundMessage("no data found to update");
                var duplicatecheck = rolelist.FirstOrDefault(i => i.RoleName == userRole.RoleName && i.Id != userRole.Id);
                if (duplicatecheck != null) return AutoResponse.ExistMessage("Already role exist in the system");
                existrole.RoleName = userRole.RoleName;
                Update(existrole);
                return AutoResponse.SuccessMessage("Role Updated");
            }
            catch (Exception e)
            {
                var exmsg = e.Message.ToString() + ((e.InnerException != null) ? " ; " + e.InnerException.Message.ToString() : "");
                return AutoResponse.ExceptionOccuredMessage(exmsg);
            }
        }

        public ResponseMessage DeleteRole(long roleId)
        {
            try
            {
                var userrole = GetById(roleId);
                if (!_context.User.Any(i => i.UserRoleId == roleId))
                {
                    var rolemenus = _context.RoleMenu.Where(i => i.UserRoleId == roleId).ToList();
                    Remove(userrole);
                    _context.RoleMenu.RemoveRange(rolemenus);
                    _context.SaveChanges();
                }
                return AutoResponse.ErrorMessage($@"Could not delete {userrole.RoleName}. {userrole.RoleName} linked with one or more users.");
            }
            catch (Exception e)
            {
                var exmsg = e.Message.ToString() + ((e.InnerException != null) ? " ; " + e.InnerException.Message.ToString() : "");
                return AutoResponse.ExceptionOccuredMessage(exmsg);
            }
        }
        public List<VmMenuItem> GetMenuItemForUser(long roleId)
        {
            var menuitems = new List<MenuItem>();
            try
            {
                if (roleId == 1)
                {
                    menuitems = _context.MenuItem.ToList();
                }
                else
                {
                    var menuids = _context.RoleMenu.Where(i => i.UserRoleId == roleId).Select(i => i.MenuItemId).ToList();
                    menuitems = _context.MenuItem.Where(i => menuids.Contains(i.Id)).OrderBy(i => i.MenuOrder).ToList();
                }
                menuitems = menuitems.Where(i => i.ParentId == null).OrderBy(i => i.MenuOrder).ToList();
                foreach (var parentmenu in menuitems)
                {
                    if (parentmenu.IsParent && parentmenu.ChildMenus != null)
                    {
                        parentmenu.ChildMenus = parentmenu.ChildMenus.OrderBy(i => i.MenuOrder).ToList();
                    }
                }
                return GenericMapper<MenuItem, VmMenuItem>.GetDestinationList(menuitems);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<VmMenuItem>();
            }

        }
    }
}
