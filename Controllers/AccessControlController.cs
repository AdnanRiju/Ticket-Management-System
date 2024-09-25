using CompanyManagement.Constrains;
using CompanyManagement.ModelHelper;
using CompanyManagement.Models;
using CompanyManagement.Services;
using CompanyManagement.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace CompanyManagement.Controllers
{
    public class AccessControlController : Controller
    {        
        private readonly RoleService _roleService;
        private readonly MenuItemService _menuService;
        private readonly RoleMenuService _rolemenuService;
        public AccessControlController (RoleService roleService, MenuItemService menuService, RoleMenuService rolemenuService)
        {
            _roleService = roleService;
            _menuService = menuService;
            _rolemenuService = rolemenuService;
        }
        public IActionResult RoleSetting()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");
            ViewBag.RoleType = vmuser.RoleType;            
            var roles = _roleService.FindAll(i => i.RoleType != 0);
            ViewBag.MenuItems = _menuService.GetParentWithChild();
            ViewBag.TitleName = "Access Control";
            ViewBag.RoleViewModels = roles;
            return View();
        }

        private void SetupRoleViewData(string title, string action)
        {
            ViewBag.TitleName = title;
            ViewBag.Action = action;
        }
        public IActionResult AddRole()
        {
            var userRole = new UserRole
            {
                RoleType = RoleTypeConstant.CompanyUser
            };
            SetupRoleViewData("Add Role", "SaveRole");
            return View("~/Views/AccessControl/Partial/_AddRole.cshtml", userRole);
        }
        [HttpPost]
        public JsonResult SaveRole(UserRole userRole)
        {
            var response = _roleService.AddRole(userRole);
            return Json(response);
        }
        [HttpGet]
        public IActionResult EditRole(long id)
        {
            var userRole = _roleService.GetById(id) ?? new UserRole();
            userRole.RoleType = RoleTypeConstant.CompanyUser;
            SetupRoleViewData("Edit Role", "EditRole");
            return View("~/Views/AccessControl/Partial/_AddRole.cshtml", userRole);
        }
        [HttpPost]
        public JsonResult EditRole(UserRole userRole)
        {
            var response = _roleService.UpdateRole(userRole);
            return Json(response);
        }
        [HttpGet]
        public JsonResult LoadDetails(long roleId)
        {
            var rolemenu = new VmRoleMenu();
            var menuAccess = _menuService.GetRoleMenuItems(roleId) ?? new List<MenuItem>();
            rolemenu.UserRole = _roleService.GetById(roleId) ?? new UserRole();
            rolemenu.MenuItems = GenericMapper<MenuItem, VmMenuItem>.GetDestinationList(menuAccess);
            return Json(rolemenu);
        }
        [HttpPost]
        public JsonResult UpdateMenuAccess(List<RoleMenu> rolemenu)
        {
            var response = _rolemenuService.UpdateRoleMenu(rolemenu);
            return Json(response);
        }
        public JsonResult DeleteRole(long roleId)
        {
            var response = _roleService.DeleteRole(roleId);
            return Json(response);
        }
    }
}
