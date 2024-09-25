using CompanyManagement.Helper;
using CompanyManagement.ModelHelper;
using CompanyManagement.Models;
using CompanyManagement.Services;
using CompanyManagement.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;

namespace CompanyManagement.Controllers
{
    public class MenuController : Controller
    {
        private readonly MenuItemService _menuservice;
        public MenuController(IHttpContextAccessor httpContextAccessor, MenuItemService menuservice)
        {
            _menuservice = menuservice;
        }
        public IActionResult MenuSetup()
        {
            ViewBag.Title = "App Menu";
            var menuItems = _menuservice.GetMenuItems();
            return View(menuItems);
        }
        [HttpGet]
        public IActionResult FontAwesomeIcons()
        {
            ViewBag.PageTitle = "Font Awesome Icons";
            return View();
        }
        public IActionResult Add()
        {
            var menuItemViewModel = new VmMenuItem();
            ViewBag.TitleName = "Add Menu";
            ViewBag.IsEdit = false;
            ViewBag.Action = "Add";
            menuItemViewModel.MenuOrder = _menuservice.GetLastMenuOrder() + 1;
            menuItemViewModel.ParentMenus = new SelectList(_menuservice.GetParentMenuItems(), "Id", "MenuName");
            menuItemViewModel.Modules = new SelectList(GetModules(), "Value", "Text");
            return View("~/Views/Menu/Partial/_AddMenuItem.cshtml", menuItemViewModel);
        }

        private List<SelectListItem> GetModules()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value="Developer", Text = "Developer"},
                new SelectListItem { Value="Administration", Text = "Administration"},
                new SelectListItem { Value="TicketingSystem", Text = "TicketingSystem"},
                new SelectListItem { Value="Report", Text = "Report"},
            };
        }

        [HttpPost]
        public JsonResult Add(VmMenuItem menuItemViewModel)
        {
            MenuItem menuItem = GenericMapper<VmMenuItem, MenuItem>.GetDestination(menuItemViewModel);
            if (menuItem.IsParent)
            {
                menuItem.ControllerName = " ";
                menuItem.ActionName = " ";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(menuItem.ControllerName))
                {
                    return Json(AutoResponse.InvalidRequestFormatMessage("Enter controller name"));
                }
                if (string.IsNullOrWhiteSpace(menuItem.ActionName))
                {
                    return Json(AutoResponse.InvalidRequestFormatMessage("Enter action name"));
                }
            }
            menuItem.ParentId = (menuItem.ParentId == 0) ? null : menuItem.ParentId;
            var response = _menuservice.AddMenuItem(menuItem);
            return Json(response);
        }
        public IActionResult Edit(long id)
        {
            ViewBag.TitleName = "Edit Menu";
            ViewBag.IsEdit = true;
            ViewBag.Action = "Edit";
            var menuItem = _menuservice.GetById(id);
            VmMenuItem menuItemViewModel = GenericMapper<MenuItem, VmMenuItem>.GetDestination(menuItem);
            menuItemViewModel.ParentMenus = new SelectList(_menuservice.GetParentMenuItems(), "Id", "MenuName");
            menuItemViewModel.Modules = new SelectList(GetModules(), "Value", "Text");
            return View("~/Views/Menu/Partial/_AddMenuItem.cshtml", menuItemViewModel);
        }
        [HttpPost]
        public JsonResult Edit(VmMenuItem menuItemViewModel)
        {
            MenuItem menuItem = GenericMapper<VmMenuItem, MenuItem>.GetDestination(menuItemViewModel);
            menuItem.ParentId = (menuItem.ParentId == 0) ? null : menuItem.ParentId;
            var response = _menuservice.UpdateMenuItem(menuItem);
            return Json(response);
        }
        public JsonResult GetMenuItems()
        {
            var userMenuItems = _menuservice.GetMenuItems();
            return Json(userMenuItems);
        }
        public JsonResult GetParentMenuItems()
        {
            var userMenuItems = _menuservice.GetParentMenuItems();
            return Json(userMenuItems);
        }
        [HttpPost]
        public JsonResult SaveMenuItem(MenuItem menuItem)
        {
            if (string.IsNullOrWhiteSpace(menuItem.AreaName)) return Json(AutoResponse.InvalidRequestFormatMessage("Add area name"));
            if (string.IsNullOrWhiteSpace(menuItem.ActiveLinkName)) return Json(AutoResponse.InvalidRequestFormatMessage("Add active link name"));
            var response = _menuservice.AddMenuItem(menuItem);
            return Json(response);
        }

        [HttpPost]
        public JsonResult UpdateMenuItem(MenuItem menuItem)
        {
            var response = _menuservice.UpdateMenuItem(menuItem);
            return Json(response);
        }

        public JsonResult DeleteMenuItem(MenuItem menuItem)
        {
            var response = _menuservice.DeleteMenuItem(menuItem);
            return Json(response);
        }

        [HttpGet]
        public JsonResult ListMenuItemsForSideBar()
        {
            List<MenuItem> menuItems = HttpContext.Session.GetComplexData<List<MenuItem>>("MenuItems");
            return Json(menuItems);
        }

    }
}

