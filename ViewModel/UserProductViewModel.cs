
using CompanyManagement.Models;
using CompanyManagement.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagement.ViewModel
{

    public class UserManagementViewModel
    {
        public List<User> ActiveUsers { get; set; }
    }

    // Model classes
    public class UserProductViewModel
    {
        public int UserId { get; set; }
        public List<int> ProductId { get; set; }
    }
}