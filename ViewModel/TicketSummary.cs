
using CompanyManagement.Models;
using CompanyManagement.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagement.ViewModel
{

    public class TicketSummary
    {
        public int StatusCount { get; set; }
        public string ActiveStatusString { get; set; }
    }


}


