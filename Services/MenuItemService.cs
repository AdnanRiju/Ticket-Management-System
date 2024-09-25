using Dapper;
using System.Data.SqlClient;
using CompanyManagement.Models;
using CompanyManagement.Helper;
using CompanyManagement.ViewModel;

namespace CompanyManagement.Services
{
    public class MenuItemService : GenericService<MenuItem>
    {
        private readonly CMScontext _context;
        public MenuItemService(CMScontext context) : base(context)
        {
            _context = context;
        }
        public ResponseMessage AddMenuItem(MenuItem menuItem)
        {
            try
            {
                var othermenus = _context.MenuItem.OrderBy(i => i.MenuOrder).ToList();
                if (othermenus.Any(i => i.ParentId == menuItem.ParentId && i.ActionName == menuItem.ActionName && i.AreaName == menuItem.AreaName)) return AutoResponse.ExistMessage("This menu already exist");
                var resetMenus = othermenus.Where(i => i.MenuOrder > menuItem.MenuOrder).ToList();
                _context.MenuItem.Add(menuItem);
                // get menu list to reorder
                if(resetMenus.Count > 0)
                {
                    int increaseOrder = menuItem.MenuOrder;
                    foreach (var item in resetMenus)
                    {
                        item.MenuOrder = increaseOrder + 1;
                        increaseOrder++;
                    }
                    _context.MenuItem.UpdateRange(resetMenus);
                }
                _context.SaveChanges();
                return AutoResponse.SuccessMessage("Menu Added");
            }
            catch (Exception e)
            {
                var exmsg = e.Message.ToString() + ((e.InnerException != null) ? " ; " + e.InnerException.Message.ToString() : "");
                return AutoResponse.ExceptionOccuredMessage(exmsg);
            }
        }

        public ResponseMessage UpdateMenuItem(MenuItem menuItem)
        {
            try
            {
                var allMenus = _context.MenuItem.Where(i => i.Id != menuItem.Id).OrderBy(i => i.MenuOrder).ToList();
                if (allMenus.Any(i => i.ParentId == menuItem.ParentId && i.ActionName == menuItem.ActionName && !i.IsParent)) return AutoResponse.ExistMessage("This menu already exist");
                int increaseOrder = menuItem.MenuOrder;
                var othermenus = allMenus.Where(i => i.MenuOrder >= menuItem.MenuOrder).OrderBy(i => i.MenuOrder).ToList();
                foreach (var item in othermenus)
                {
                    item.MenuOrder = increaseOrder + 1;
                    increaseOrder++;
                }
                _context.MenuItem.Update(menuItem);
                _context.MenuItem.UpdateRange(othermenus);
                _context.SaveChanges();
                return AutoResponse.SuccessMessage("Menu Updated");
            }
            catch (Exception e)
            {
                var exmsg = e.Message.ToString() + ((e.InnerException != null) ? " ; " + e.InnerException.Message.ToString() : "");
                return AutoResponse.ExceptionOccuredMessage(exmsg);
            }
        }

        public ResponseMessage DeleteMenuItem(MenuItem menuItem)
        {
            try
            {
                _context.MenuItem.Remove(menuItem);
                _context.SaveChanges();
                return AutoResponse.SuccessMessage("Menu Deleted");
            }
            catch (Exception e)
            {
                var exmsg = e.Message.ToString() + ((e.InnerException != null) ? " ; " + e.InnerException.Message.ToString() : "");
                return AutoResponse.ExceptionOccuredMessage(exmsg);
            }
        }

        public int GetLastMenuOrder()
        {
            return _context.MenuItem.Max(i => i.MenuOrder);
        }
        public List<VmMenuItem>? GetMenuItems()
        {
            try
            {
                string sql = $@"SELECT mi.*,imi.MenuName as ParentMenuName FROM [MenuItem] mi 
                            left join MenuItem imi on imi.Id = mi.ParentId order by mi.MenuOrder";

                using (var connection = new SqlConnection(AppSetting.ConnectionString))
                {
                    var menuitems = connection.Query<VmMenuItem>(sql).ToList();
                    return menuitems;
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<MenuItem>? GetParentMenuItems()
        {
            try
            {
                string sql = $@"SELECT Id, MenuName FROM [MenuItem] where IsParent=1";

                using (var connection = new SqlConnection(AppSetting.ConnectionString))
                {
                    var parentMenus = connection.Query<MenuItem>(sql).ToList();
                    return parentMenus;
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<MenuItem>? GetParentWithChild()
        {
            try
            {
                var parentMenus = _context.MenuItem.Where(i => i.IsParent == true && i.MenuVisibility).ToList();
                var childMenus = _context.MenuItem.Where(i => i.IsParent == false && i.MenuVisibility).ToList();
                return parentMenus;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public List<MenuItem>? GetRoleMenuItems(long roleid)
        {
            try
            {
                var menuitems = _context.RoleMenu.Where(i => i.UserRoleId == roleid).Select(i => i.MenuItem).ToList();
                menuitems = menuitems.Where(i => i.IsParent == true && i.MenuVisibility).ToList();
                return menuitems;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
