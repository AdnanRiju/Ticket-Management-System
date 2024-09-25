using Azure.Identity;
using CompanyManagement.Helper;
using CompanyManagement.ModelHelper;
using CompanyManagement.Models;
using CompanyManagement.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace CompanyManagement.Controllers
{
    public class UserController : Controller
    {

        public CMScontext _context;

        public UserController(CMScontext context)
        {
            _context = context;
        }
        public IActionResult UserList()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");
            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var users = from u in _context.User
                        join ur in _context.UserRole on u.UserRoleId equals ur.Id
                        join c in _context.Company on u.CompanyId equals c.Id
                        select new
                        {
                            u.Id,
                            u.UserName,
                            UserRoleName = ur.RoleName, // assuming RoleName is the property name in UserRole
                            CompanyName = c.Name // assuming Name is the property name in Company
                        };

            var userList = users.ToList();

            return View(userList);
        }

        //public IActionResult TicketList()
        //{
        //    var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

        //    if (vmuser == null)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }

        //    var emp = _context.Ticket.ToList();
        //    return View(emp);
        //}
        [HttpPost]
        public JsonResult UserDelete(int id)
        {
            try
            {
                var user = _context.User
                    .FirstOrDefault(a => a.Id == id);

                if (user != null)
                {
                    
                    _context.User.Remove(user);
                    _context.SaveChanges();
                    return Json(AutoResponse. SuccessMessage("User deleted successfully"));

                }
                return Json(AutoResponse.ErrorMessage("User not found"));


            }
            catch (Exception ex)
            {
                return Json((AutoResponse.SuccessMessage($"An error occurred: {ex.Message}")));

            }
        }
        [HttpGet]
        public IActionResult AddUser()
        {
            ViewBag.UserRole = new SelectList(_context.UserRole.ToList(), "Id", "RoleName");
            ViewBag.Company = new SelectList(_context.Company.ToList(), "Id", "Name");

            return View(new User
            {
                ActiveStatus = (int)TicketStatus.Created,
                UserTitle = PropertyConstant.Active.ToString(),
                AdminAccess=false
            });
        }

        [HttpPost]
        public JsonResult AddUser(User user)
        {
                user.UserTitle = user.UserName;
                _context.User.Add(user);
                _context.SaveChanges();
                return Json(AutoResponse.SuccessMessage("User Added"));
        }
        [HttpGet]
        public IActionResult UserEdit(long id)
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.UserRole = new SelectList(_context.UserRole.ToList(), "Id", "RoleName");
            ViewBag.Company = new SelectList(_context.Company.ToList(), "Id", "Name");

            var user = _context.User.Find(id);
            if (user == null)
            {
                return NotFound(); // Handle user not found
            }

            return View(user);
        }

        [HttpPost]
        public JsonResult UserEdit(User user)
        {
            if (user.Id <= 0)
            {
                return Json(AutoResponse.ErrorMessage("Select a user."));
            }

            if (user.CompanyId <= 0)
            {
                return Json(AutoResponse.ErrorMessage("Select a company."));
            }

            // Update other fields as necessary
            user.ActiveStatus = (int)TicketStatus.Created;
            user.UserTitle = user.UserName;
            _context.User.Update(user);
            _context.SaveChanges();
            return Json(AutoResponse.SuccessMessage("User updated successfully."));
        }
        [HttpGet]
        public IActionResult UpdatePassword()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.TitleName = "Update Password";
            return View();
        }

        [HttpPost]
        public JsonResult UpdatePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return Json(new { success = false, message = "User not found. Please log in again." });
            }

            var existUser = _context.User.FirstOrDefault(u => u.Id == vmuser.Id);
            if (existUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            if (!VerifyPassword(existUser, oldPassword))
            {
                 return Json(AutoResponse.ErrorMessage ("Old password is incorrect."));
            }

            if (newPassword != confirmPassword)
            {
                return Json(AutoResponse.ErrorMessage ("New password and confirm password do not match." ));
            }

            if (newPassword == oldPassword || oldPassword == confirmPassword)
            {
                return Json(AutoResponse.ErrorMessage("The new password must be different from the old one."));
            }

            existUser.UserPassword = newPassword.Trim();
            _context.User.Update(existUser);
            _context.SaveChanges();

            return Json(AutoResponse.SuccessMessage("User updated successfully."));
        }

        // Method to verify password (make sure to implement this correctly)
        private bool VerifyPassword(User user, string password)
        {
            // Implement your password verification logic here
            return user.UserPassword == password; // Placeholder logic
        }



        [HttpPost]
        public JsonResult ResetPassword(int id)
        {
            try
            {
                var user = _context.User.FirstOrDefault(i => i.Id == id);

                if (user != null)
                {
                    user.UserPassword = "1234";
                    _context.User.Update(user);
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Password has been reset to '1234'." });
                }

                return Json(new { success = false, message = "User not found." });
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }
        public IActionResult ProductManagement()
        {
            var activeUsers = _context.User
                              .ToList();
            return View(activeUsers);
        }

        // GET: /ProductManagement/GetProductsByUser
        [HttpGet]
        public IActionResult GetProductsByUser(int userId)
        {
            var assignedProducts = _context.UserProduct
                .Where(up => up.UserId == userId)
                .Join(_context.Product,
                    up => up.ProductId,
                    p => p.Id,
                    (up, p) => new { p.Id, p.ProductName })
                .ToList();

            return Json(assignedProducts);
        }

        // GET: /ProductManagement/GetAvailableProducts
        [HttpGet]
        public IActionResult GetAvailableProducts(int userId)
        {
            var assignedProductIds = _context.UserProduct
                .Where(up => up.UserId == userId)
                .Select(up => up.ProductId)
                .ToList();

            var availableProducts = _context.Product
                .Where(p => p.ActiveStatus == (int)EnumHelper.PropertyConstant.Active && !assignedProductIds.Contains(p.Id))
                .Select(p => new { p.Id, p.ProductName })
                .ToList();

            return Json(availableProducts);
        }

        // POST: /ProductManagement/SaveUserProducts
        [HttpPost]
        public async Task<IActionResult> SaveUserProducts([FromBody] CompanyProductUpdateModel model)
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (model.ProductIds == null || !model.ProductIds.Any())
            {
                return Json(new { success = false, message = "No products selected" });
            }

            var currentProducts = _context.UserProduct
                .Where(up => up.UserId == model.CompanyId) // Note: Using CompanyId here, but it represents UserId
                .Select(up => up.ProductId)
                .ToList();

            var newProductIds = model.ProductIds.Except(currentProducts).ToList();
            var removedProductIds = currentProducts.Except(model.ProductIds).ToList();

            try
            {
                // Add new products
                if (newProductIds.Any())
                {
                    var userProductsToAdd = newProductIds.Select(productId => new UserProduct
                    {
                        UserId = model.CompanyId, // Note: Using CompanyId here, but it represents UserId
                        ProductId = productId
                    }).ToList();
                    _context.UserProduct.AddRange(userProductsToAdd);
                }
               await _context.SaveChangesAsync();
               return Json(new { success = true, message = "Products updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to update products", exception = ex.Message });
            }
        }
        public async Task<IActionResult> UserDetails()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");
            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var userId = vmuser.Id;
            var user = await _context.User
                .Include(u => u.UserRole)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var productIds = await _context.UserProduct
                .Where(up => up.UserId == userId)
                .Select(up => up.ProductId)
                .ToListAsync();

            var products = await _context.Product
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            var userDetails = new UserDetailsViewModel
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                UserTitle = user.UserTitle ?? string.Empty,
                AdminAccess = user.AdminAccess,
                ActiveStatus = user.ActiveStatus,
                Products = products,
                UserRoleName = user.UserRole?.RoleName ?? string.Empty
            };

            return View(userDetails);
        }

    }
}


