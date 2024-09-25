using CompanyManagement.ModelHelper;
using CompanyManagement.Models;
using Microsoft.AspNetCore.Mvc;
using CompanyManagement.Helper;
using Microsoft.AspNetCore.Authentication;
using CompanyManagement.ViewModel;
using System.Data.SqlClient;
using Dapper;
using CompanyManagement.Services;

namespace CompanyManagement.Controllers
{
    public class LoginController : Controller
    {
        public CMScontext _context;
        private readonly CompanyService _companyService;
        private readonly RoleService _roleService;
        public LoginController(CMScontext context, CompanyService companyService, RoleService roleService)
        {
            _context = context;
            _companyService = companyService;
            _roleService = roleService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.LoginInfo = AutoResponse.SuccessMessage("Login initiated");
            var vmUser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");
            if (vmUser != null && vmUser.Id > 0)
            {
                return (vmUser.RoleType == 1) ? RedirectToAction("Index", "Dashboard") : RedirectToAction("Index", "AdminDashboard");
            }
            return View(new User());
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            try
            {
                // verify user
                var registeredUser = LoggedInUser(user.UserName, user.UserPassword);
                if (registeredUser != null)
                {
                    // check user status for access
                    if (registeredUser.ActiveStatus == (int)PropertyConstant.Blocked)
                    {
                        ViewBag.LoginInfo = AutoResponse.ErrorMessage("Account blocked. Contact with administrator");
                        return View(user);
                    }
                    if (registeredUser.ActiveStatus == (int)PropertyConstant.Inactive)
                    {
                        ViewBag.LoginInfo = AutoResponse.ErrorMessage("Account inactive. Contact with administrator");
                        return View(user);
                    }
                    // get company information
                    var companyInfo = _companyService.GetById(registeredUser.CompanyId); // 2 = company information
                    // get menuitems
                    var menuItems = _roleService.GetMenuItemForUser(registeredUser.UserRoleId).ToList();
                    // set session variables
                    SetSession(registeredUser, companyInfo, menuItems);
                    // redirect to home if access granted

                    return (registeredUser.RoleType == 1) ? RedirectToAction("Index", "Dashboard") : RedirectToAction("Index", "AdminDashboard");
                }
                ViewBag.LoginInfo = AutoResponse.NotFoundMessage("Username or Password not matched");
                ModelState.Clear();
                return View(user);
            }
            catch (Exception)
            {
                HttpContext.Session.Clear();
                ModelState.Clear();
                ViewBag.LoginInfo = AutoResponse.NotFoundMessage("Contact with system administration");
                return View();
            }
        }

        private VmUser? LoggedInUser(string userName, string password)
        {
            try
            {
                using (var con = new SqlConnection(AppSetting.ConnectionString))
                {
                    var sql = $@"select u.*,ur.RoleName from [User] u inner join UserRole ur on ur.Id = u.UserRoleId where u.Username = '{userName}' and UserPassword = '{password}' and ActiveStatus= {(int)PropertyConstant.Active}";
                    return con.Query<VmUser>(sql).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        private void SetSession(VmUser registeredUser, Company companyInfo, List<VmMenuItem> menuItems)
        {
            companyInfo.ShortName ??= "WS";
            HttpContext.Session.SetInt32("islogin", 1);
            HttpContext.Session.SetComplexData("UserInformation", registeredUser);
            HttpContext.Session.SetComplexData("CompanyInfo", companyInfo);
            HttpContext.Session.SetComplexData("MenuItems", menuItems);
        }
        
        private bool VerifyPassword(User user, string password)
        {
            return user.UserPassword == password;
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();

            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Login");
        }        
    }
}

  

