using CompanyManagement.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Drawing;

namespace CompanyManagement.ViewModel
{

    public class CompanyDetailsViewModel
    {
        public Company Company { get; set; }
        public List<Product> Products { get; set; }
        public List<User> Users { get; set; } // Add this line
    }


}