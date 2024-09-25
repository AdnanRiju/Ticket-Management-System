


using CompanyManagement.Helper;
using CompanyManagement.Models;

namespace CompanyManagement.Services
{
    public class RoleMenuService : GenericService<RoleMenu>
    {
        private readonly CMScontext _context;
        public RoleMenuService(CMScontext context) : base(context)
        {
            _context = context;
        }

        public ResponseMessage AddRoleMenu(RoleMenu roleMenu)
        {
            try
            {
                _context.RoleMenu.Add(roleMenu);
                _context.SaveChanges();
                return AutoResponse.SuccessMessage("Permission Added", roleMenu);
            }
            catch (Exception e)
            {
                var exmsg = e.Message.ToString() + ((e.InnerException != null) ? " ; " + e.InnerException.Message.ToString() : "");
                return AutoResponse.ExceptionOccuredMessage(exmsg);
            }
        }

        public ResponseMessage DeleteRoleMenu(RoleMenu roleMenu)
        {
            try
            {
                _context.RoleMenu.Update(roleMenu);
                _context.SaveChanges();
                return AutoResponse.SuccessMessage("Permission Updated");
            }
            catch (Exception e)
            {
                var exmsg = e.Message.ToString() + ((e.InnerException != null) ? " ; " + e.InnerException.Message.ToString() : "");
                return AutoResponse.ExceptionOccuredMessage(exmsg);
            }
        }

        public ResponseMessage UpdateRoleMenu(List<RoleMenu> roleMenu)
        {
            try
            {
                roleMenu = roleMenu.Where(i => i.MenuItemId != 0).ToList();
                var menuids = roleMenu.Select(i => i.MenuItemId).ToList();
                var existingaccess = _context.RoleMenu.Where(i => i.UserRoleId == roleMenu.First().UserRoleId).ToList();
                // find menus that need to revoke
                var existingremovemenus = existingaccess.Where(i => !menuids.Contains(i.MenuItemId)).ToList();
                // remove existing access from newly added menus
                var existingaccessmenuids = existingaccess.Select(i => i.MenuItemId).ToList();
                roleMenu = roleMenu.Where(i => !existingaccessmenuids.Contains(i.MenuItemId)).ToList();
                _context.RoleMenu.RemoveRange(existingremovemenus);
                _context.RoleMenu.UpdateRange(roleMenu);
                _context.SaveChanges();
                return AutoResponse.SuccessMessage("Access Updated");
            }
            catch (Exception e)
            {
                var exmsg = e.Message.ToString() + ((e.InnerException != null) ? " ; " + e.InnerException.Message.ToString() : "");
                return AutoResponse.ExceptionOccuredMessage(exmsg);
            }
        }

        public ResponseMessage GetbyRoleId(long id)
        {
            try
            {
                //_context.RoleMenu.Remove(roleMenu);
                //_context.SaveChanges();
                return AutoResponse.SuccessMessage("Permission Deleted");
            }
            catch (Exception e)
            {
                var exmsg = e.Message.ToString() + ((e.InnerException != null) ? " ; " + e.InnerException.Message.ToString() : "");
                return AutoResponse.ExceptionOccuredMessage(exmsg);
            }
        }
    }
}
