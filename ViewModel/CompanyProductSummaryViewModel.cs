
using CompanyManagement.Models;
using CompanyManagement.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagement.ViewModel
{
    public class CompanyProductSummaryViewModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Products { get; set; } // Concatenated product names
        public int TotalProductCount { get; set; }
        public List<Product> ProductDetails { get; set; } // Detailed product list
        public List<Company> CompanyDetails { get; set; }
    }

   
}