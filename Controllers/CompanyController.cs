using CompanyManagement.ModelHelper;
using CompanyManagement.Models;
using CompanyManagement.ViewModel;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Net.Sockets;
using Dapper;
using CompanyManagement.Helper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Linq;
using System.Security.Cryptography.X509Certificates;


namespace CompanyManagement.Controllers
{
    public class CompanyController : Controller
    {

        public CMScontext _context;

        public CompanyController(CMScontext context)
        {
            _context = context;
        }
        public IActionResult CompanyList()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null){
                return RedirectToAction("Login", "Login");
            }
            var pro = _context.Company.Where(i => i.ActiveCompany == 1).ToList();
            return View(pro);
           
        }
        public IActionResult CompanyProduct()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            using (var con = new SqlConnection(AppSetting.ConnectionString))
            {
                var sql = @"
            SELECT 
                c.Id AS CompanyId,
                c.Name AS CompanyName,
                STRING_AGG(p.ProductName, ', ') AS Products,
                COUNT(cp.ProductId) AS TotalProductCount
            FROM 
                Company c
            INNER JOIN 
                CompanyProduct cp ON c.Id = cp.CompanyId
            INNER JOIN 
                Product p ON cp.ProductId = p.Id
            WHERE 
                c.ActiveCompany = 1  -- Assuming 1 denotes active companies
                AND p.ActiveStatus = 1  -- Assuming 1 denotes active products
            GROUP BY 
                c.Id, c.Name
            ORDER BY 
                c.Name;";

                var results = con.Query<CompanyProductSummaryViewModel>(sql).ToList();
                return View(results);
            }
        }


        [HttpGet]
        public IActionResult AddCompany()
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View(new Company
            {
                ActiveCompany = (int)TicketStatus.Created
            });
        }

        [HttpPost]
        public JsonResult AddCompany(Company company)
        {
            

            // Set the ShortName based on the Name
            company.ShortName = company.Name;

            // Add the company to the context and save
            _context.Company.Add(company);
            _context.SaveChanges();

            return Json(AutoResponse.SuccessMessage("Company Added"));
        }

        [HttpGet]
        public IActionResult EditCompany(int id)
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var company = _context.Company.Find(id);
            if (company == null)
            {
                return NotFound(); // Handle if the company is not found
            }

            return View(company); // Pass the retrieved company to the view
        }

        [HttpPost]
        public JsonResult EditCompany(Company company)
        {
            if (company == null)
            {
                return Json(AutoResponse.ErrorMessage("Invalid company data."));
            }

           // company.ShortName = company.Name;
            company.ActiveCompany = (int)TicketStatus.Created;
            _context.Company.Update(company);
            _context.SaveChanges();
            return Json(AutoResponse.SuccessMessage("Company Updated"));
        }

        public IActionResult CompanyDetails(int id)
        {
            var vmuser = HttpContext.Session.GetComplexData<VmUser>("UserInformation");

            if (vmuser == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var company = _context.Company.FirstOrDefault(c => c.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            var productIds = _context.CompanyProduct
                .Where(cp => cp.CompanyId == company.Id)
                .Select(cp => cp.ProductId)
                .ToList();

            var products = _context.Product
                .Where(p => productIds.Contains(p.Id))
                .ToList();

            var users = _context.User
                .Where(u => u.CompanyId == company.Id) // Fetch users associated with the company
                .ToList();

            var viewModel = new CompanyDetailsViewModel
            {
                Company = company,
                Products = products,
                Users = users // Add the list of users
            };

            return View(viewModel);
        }
        [HttpPost]
        public JsonResult CompanyDelete(int id)
        {
            try
            {
                var company = _context.Company.FirstOrDefault(i => i.Id == id);

                if (company != null)
                {
                    company.ActiveCompany = (int)EnumHelper.PropertyConstant.Deleted;
                    _context.Company.Update(company);
                    _context.SaveChanges();
                    return Json(AutoResponse.SuccessMessage("Company Deleted"));
                }

                return Json(new { success = false, message = "Company not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }
        public JsonResult RestoreCompany(int id)
        {
            try
            {
                var Company = _context.Company.FirstOrDefault(i => i.Id == id);

                if (Company != null)
                {
                    Company.ActiveCompany = (int)EnumHelper.PropertyConstant.Active;
                    _context.Company.Update(Company);
                    _context.SaveChanges();
                    return Json(AutoResponse.SuccessMessage("Company Restore"));
                }

                return Json(new { success = false, message = "Company not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }
        public IActionResult DeletedCompany()
        {
            ViewBag.Company = _context.Company.Where(i => i.ActiveCompany == (int)EnumHelper.PropertyConstant.Deleted).ToList();

            return View();
        }
        public IActionResult ProductManagement()
        {
            var activeCompanies = _context.Company
                .Where(i => i.ActiveCompany == (int)EnumHelper.PropertyConstant.Active)
                .ToList();

            var model = new ProductManagementViewModel
            {
                ActiveCompanies = activeCompanies
            };

            return View(model);
        }

        // GET: /Company/GetProductsByCompany
        [HttpGet]
        public IActionResult GetProductsByCompany(int companyId)
        {
            var assignedProducts = _context.CompanyProduct
                .Where(cp => cp.CompanyId == companyId)
                .Join(_context.Product,
                    cp => cp.ProductId,
                    p => p.Id,
                    (cp, p) => new { p.Id, p.ProductName })
                .ToList();

            return Json(assignedProducts);
        }

        // GET: /Company/GetAvailableProducts
        [HttpGet]
        public IActionResult GetAvailableProducts(int companyId)
        {
            var assignedProductIds = _context.CompanyProduct
                .Where(cp => cp.CompanyId == companyId)
                .Select(cp => cp.ProductId)
                .ToList();

            var availableProducts = _context.Product
                .Where(p => p.ActiveStatus == (int)EnumHelper.PropertyConstant.Active && !assignedProductIds.Contains(p.Id))
                .Select(p => new { p.Id, p.ProductName })
                .ToList();

            return Json(availableProducts);
        }

      
        // POST: /Company/SaveCompanyProducts
        [HttpPost]
        public JsonResult SaveCompanyProducts([FromBody] CompanyProductUpdateModel model)
        {
            if (model.ProductIds == null || !model.ProductIds.Any())
            {
                return Json(new { success = false, message = "No products selected" });
            }

            var currentProducts = _context.CompanyProduct
                .Where(cp => cp.CompanyId == model.CompanyId)
                .Select(cp => cp.ProductId)
                .ToList();

            var newProductIds = model.ProductIds.Except(currentProducts).ToList();
            var removedProductIds = currentProducts.Except(model.ProductIds).ToList();

            try
            {
                // Add new products
                if (newProductIds.Any())
                {
                    var companyProductsToAdd = newProductIds.Select(productId => new CompanyProduct
                    {
                        CompanyId = model.CompanyId,
                        ProductId = productId
                    }).ToList();
                    _context.CompanyProduct.AddRange(companyProductsToAdd);
                }

              

                 _context.SaveChangesAsync();

                return Json(new { success = true, message = "Products updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to update products", exception = ex.Message });
            }
        }
    }
}












