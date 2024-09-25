using CompanyManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CompanyManagement.ViewModel
{

    public class AssignedTicketsViewModel
    {
        public List<Ticket> Tickets { get; set; }
        public List<User> ActiveUsers { get; set; } // Add this property
    }

}
