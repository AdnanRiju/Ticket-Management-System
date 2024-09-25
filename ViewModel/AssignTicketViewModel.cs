using CompanyManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CompanyManagement.ViewModel
{

    public class AssignTicketViewModel
    {
        public Ticket Ticket { get; set; }
        public IEnumerable<SelectListItem> Users { get; set; }
    }
}
