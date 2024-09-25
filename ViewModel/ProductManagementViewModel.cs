
using CompanyManagement.Models;
using CompanyManagement.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagement.ViewModel
{
    public class ProductManagementViewModel
    {
        public List<Company> ActiveCompanies { get; set; }
    }

    // Model classes
    public class CompanyProductUpdateModel
    {
        public int CompanyId { get; set; }
        public List<int> ProductIds { get; set; }
    }
   
}