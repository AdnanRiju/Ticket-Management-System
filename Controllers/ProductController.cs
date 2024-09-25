using CompanyManagement.ModelHelper;
using CompanyManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CompanyManagement.Helper;
using Microsoft.Data.SqlClient;
using Dapper;
using CompanyManagement.ViewModel;


namespace CompanyManagement.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public CMScontext _context;

        public ProductController(CMScontext context)
        {
            _context = context;
        }

        public IActionResult ProductList()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            using (var con = new SqlConnection(AppSetting.ConnectionString))
            {
            var sql = $@"SELECT p.*, pt.Product_Type 
                FROM Product p
                INNER JOIN ProductType pt ON p.ProductTypeId = pt.Id
                WHERE p.ActiveStatus = 1;";
                ViewBag.Product = con.Query<VmProduct>(sql).ToList();
                return View();
            }
            
        }
        public IActionResult DeletedProduct()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.Product = _context.Product.Where(i => i.ActiveStatus == (int)EnumHelper.PropertyConstant.Deleted).ToList();
            ViewBag.ProductType = _context.ProductType.ToList();
            return View();
        }
        public IActionResult ProductTypeList()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.ProductType = _context.ProductType.ToList();
            return View();
        }
        
        [HttpGet]
        public IActionResult AddProduct()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.ProductType = new SelectList(_context.ProductType.ToList(), "Id", "Product_Type");
            return View(new Models.Product { ActiveStatus = (int)TicketStatus.Created, CreateDate = DateTime.Now, });
        }
        [HttpPost]
        public JsonResult AddProduct(Models.Product Product)
        {
            _context.Product.Add(Product);
            _context.SaveChanges();
            return Json(AutoResponse.SuccessMessage("Product Added"));
        }
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.ProductType = new SelectList(_context.ProductType.ToList(), "Id", "Product_Type");
            var Product = _context.Product.Find(id);
            return View(Product);
        }

        [HttpPost]
        public JsonResult EditProduct(Models.Product Product)
        {
            _context.Product.Update(Product);
            _context.SaveChanges();
            return Json(AutoResponse.SuccessMessage("Product Updated"));
        }
       
        [HttpPost]
        public JsonResult ProductDelete(int id)
        {
            try
            {
                var Product = _context.Product.FirstOrDefault(i => i.Id == id);

                if (Product != null)
                {
                    Product.ActiveStatus = (int)EnumHelper.PropertyConstant.Deleted;
                    _context.Product.Update(Product);
                    _context.SaveChanges();
                    return Json(AutoResponse.SuccessMessage("Product Deleted"));
                }

                return Json(new { success = false, message = "Product not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }
        [HttpPost]
        public JsonResult RestoreProduct(int id)
        {
            try
            {
                var Product = _context.Product.FirstOrDefault(i => i.Id == id);

                if (Product != null)
                {
                    Product.ActiveStatus = (int)EnumHelper.PropertyConstant.Active;
                    _context.Product.Update(Product);
                    _context.SaveChanges();
                    return Json(AutoResponse.SuccessMessage("Product Restore"));
                }

                return Json(new { success = false, message = "Product not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult AddProductType()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AddProductType(ProductType ProductType)
        {
            _context.ProductType.Add(ProductType);
            _context.SaveChanges();
            return Json(AutoResponse.SuccessMessage("Product Type Added"));
        }
    }
}
