using CompanyManagement.Helper;
using CompanyManagement.ModelHelper;
using CompanyManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompanyManagement.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public CMScontext _context;
        public ServiceController(CMScontext context)
        {
            _context = context;
        }
        public IActionResult ServiceList()
        {
            
            ViewBag.Services = _context.ServiceType.Where(i => i.ActiveStatus == (int)EnumHelper.PropertyConstant.Active).ToList();


            return View();
        }
        //Adding a company
        [HttpGet]
        public IActionResult AddService()
        {
            ViewBag.TitleName = "Add Service";
            ViewBag.Action = "AddService";
            return View("~/Views/Service/AddService.cshtml", new ServiceType());
        }
        [HttpPost]
        public JsonResult AddService(ServiceType addServices)
        {

            addServices.ActiveStatus = (int)EnumHelper.PropertyConstant.Active;

            _context.ServiceType.Add(addServices);
            _context.SaveChanges();
            return Json(AutoResponse.SuccessMessage("Services Added"));
        }
        //Edit Project
        [HttpGet]
        public IActionResult EditService(int id)
        {
            var editService = _context.ServiceType.Find(id);
            return View(editService);
        }
        [HttpPost]
        public JsonResult EditService(ServiceType editService)
        {
            editService.ActiveStatus = (int)EnumHelper.PropertyConstant.Active;

            _context.ServiceType.Update(editService);
            _context.SaveChanges();
            return Json(AutoResponse.SuccessMessage("Service Updated"));
        }
        //Delete a ticket
        [HttpPost]
        public JsonResult Delete(int id)
        {

            try
            {
                var Service = _context.ServiceType.FirstOrDefault(i => i.Id == id);

                if (Service != null)
                {
                    Service.ActiveStatus = (int)EnumHelper.PropertyConstant.Deleted;
                    _context.ServiceType.Update(Service);
                    _context.SaveChanges();
                    return Json(AutoResponse.SuccessMessage("Service Deleted"));
                }

                return Json(new { success = false, message = "Service not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
