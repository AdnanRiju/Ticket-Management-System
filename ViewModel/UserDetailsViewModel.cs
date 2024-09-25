using CompanyManagement.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Drawing;

namespace CompanyManagement.ViewModel
{
    public class UserDetailsViewModel
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string UserTitle { get; set; }
        public string ShortName { get; set; }
        public bool AdminAccess { get; set; }
        public int ActiveStatus { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public int CompanyPhoneNumber { get; set; }
        public string CompanyAddress { get; set; }
        public List<Product> Products { get; set; }
        public string UserRoleName { get; set; } // For displaying user role
    }
}