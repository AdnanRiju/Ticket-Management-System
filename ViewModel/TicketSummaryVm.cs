using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Drawing;

namespace CompanyManagement.ViewModel
{
    public class TicketSummaryVm
    {
        public int CreatedCount { get; set; }
        public int ProcessingCount { get; set; }
        public int ClosedCount { get; set; }
        public int AssignedCount { get; set; }
        public int TotalCount { get; set; }
        public List<TicketDetailVm> TicketDetails { get; set; }
    }
}