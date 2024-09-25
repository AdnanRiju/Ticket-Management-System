using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CompanyManagement.Models
{
    public class TicketUser
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }

    
    }
}
