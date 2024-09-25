
namespace CompanyManagement.ViewModel
{
    public class UserTicketSummaryVm
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int CreatedCount { get; set; }
        public int ActiveCount { get; set; }
        public int DelegateCount { get; set; }
        public int DiscardCount { get; set; }

        public int ProcessingCount { get; set; }
        public int ClosedCount { get; set; }
        public int AssignedCount { get; set; }
        public int TotalCount { get; set; }
        public int TotalTicketCount { get; set; }
    }

}