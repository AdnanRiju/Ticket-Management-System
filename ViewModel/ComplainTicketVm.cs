using System.ComponentModel.DataAnnotations;
using CompanyManagement.Models;

namespace CompanyManagement.ViewModel
{
    public class ComplainTicketVm
    {
        public int Id { get; set; }
        public string TicketNo { get; set; }

        public int IssuedBy { get; set; }

        public DateTime IssuedDate { get; set; }


        public int ProductId { get; set; }

        public int ServiceTypeId { get; set; }

        
        public int TicketTypeId { get; set; }

      
        public string TicketSubject { get; set; }

      
        public string TicketDetails { get; set; }

        public int CompleteBy { get; set; }

        public DateTime? CompleteDate { get; set; }
      
        public int ActiveStatus { get; set; }
        public int ManagementTicketStatus { get; set; }
        public int CustomerStatus { get; set; }

        public int TicketId { get; set; }

        public int UserId { get; set; }

        public string ChatMessage { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string FileUploadPath { get; set; }
        public string CompanyName { get; set; }
        public string ProductName { get; set; }
        public string ServiceTypeName { get; set; }
        public string TicketTypeName { get; set; }
        public string ActiveStatusString { get; set; }



    }
}
