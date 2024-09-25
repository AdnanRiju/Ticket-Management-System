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


namespace CompanyManagement.Controllers
{
    public class AdminController : Controller
    {

        public CMScontext _context;

        public AdminController(CMScontext context)
        {
            _context = context;
        }
        public IActionResult RoleList()
        {

            var emp = _context.UserRole.ToList();
            return View(emp);
        }
        public IActionResult Dashboard()
        {

            var emp = _context.Company.ToList();
            return View(emp);
        }
        public IActionResult RoleDelete(UserRole a)
        {

            _context.UserRole.Remove(a);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public IActionResult Role()
        {
            return View(new UserRole());
        }

        [HttpPost]
        public IActionResult Role(UserRole u)
        {
            if (ModelState.IsValid)
            {
                _context.UserRole.Add(u);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }

            return View(u);
        }


        public ActionResult Create()
        {
           
            return View();


        }
        [HttpPost]
        public ActionResult Create(Company p)
        {
            if (ModelState.IsValid)
            {
                _context.Company.Add(p);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }

            return View(p);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var data = _context.Company.FirstOrDefault(i => i.Id == id);
            return View(data);
        }


        [HttpPost]
        public IActionResult Edit(Company d)
        {
            _context.Company.Update(d);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        public IActionResult Delete(Company a)
        {

            _context.Company.Remove(a);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
       
    }
}

       
    



