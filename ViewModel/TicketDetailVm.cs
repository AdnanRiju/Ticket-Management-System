using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Drawing;

namespace CompanyManagement.ViewModel
{
    public class TicketDetailVm
    {
        public int TicketId { get; set; }
        public int TicketNo { get; set; }
        public string TicketSubject { get; set; }
        public int ActiveStatus { get; set; }
        public string StatusString => GetStatusString(ActiveStatus);

        private string GetStatusString(int status)
        {
            return status switch
            {
                1 => "Created",
                2 => "Processing",
                3 => "Closed",
                _ => "Unknown"
            };
        }
    }
}