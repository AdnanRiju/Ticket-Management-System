using CompanyManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace CompanyManagement.ViewModel
{
    public class TicketVm
    {
        public string Id { get; set; }
        public int TicketId { get; set; }
        public int TicketNo { get; set; }
        public int IssuedBy { get; set; }
        public DateTime IssuedDate { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ServiceTypeId { get; set; }
        public string ServiceTypeName { get; set; }
        public int TicketTypeId { get; set; }
        public string TicketTypeName { get; set; }
        public string TicketSubject { get; set; }
        public string TicketDetails { get; set; }
        public int? CompleteBy { get; set; }
        public DateTime? CompleteDate { get; set; }
        public int ManagementTicketStatus { get; set; }
        public int CustomerStatus { get; set; }
        public int ActiveStatus { get; set; }
        public string IssuedByName { get; set; }
       
        public int Assigned {  get; set; }
        public string AssignedName { get; set; }
        public virtual User AssignedUser { get; set; }
        public string CompleteByName { get; set; }
        public string StatusString
        {
            get
            {
                switch (ActiveStatus)
                {
                    case 1:
                        return "Active";
                    case 2:
                        return "Processing";
                    case 3:
                        return "Closed";
                    case 4:
                        return "Discarded";

                    default:
                        return "Active";
                }
            }
            set { }
        }
    }

}
