using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Drawing;

namespace CompanyManagement.ViewModel
{
    public class TicketDetailsVm
    {
        public int Id { get; set; }
        public int TicketNo { get; set; }
        public string IssuedByName { get; set; }
        public DateTime IssuedDate { get; set; }
        public int ProductId { get; set; }
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
    }
}