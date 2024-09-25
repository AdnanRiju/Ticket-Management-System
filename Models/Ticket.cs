using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Drawing;

namespace CompanyManagement.Models
{
    public class Ticket
    {

        public int Id { get; set; }
        public int TicketNo { get; set; }

        public int IssuedBy { get; set; }

        public DateTime IssuedDate { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int ServiceTypeId { get; set; }

        public int TicketTypeId { get; set; }

        public string TicketSubject { get; set; }

        
        public string TicketDetails { get; set; }
        public int? Assigned { get; set; }

        public int? CompleteBy { get; set; }

        public DateTime? CompleteDate { get; set; }
      
        public int ManagementTicketStatus { get; set; }

        public int CustomerStatus { get; set; }


        public int ActiveStatus { get; set; }

        public virtual List<TicketUser> TicketUsers { get; set; }
        public virtual List<TicketChat> TicketChats { get; set; }

    }
}