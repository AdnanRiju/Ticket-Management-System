using CompanyManagement.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Drawing;

namespace CompanyManagement.ViewModel
{
    public class TicketViewModel
    {
        public TicketDetailsVm TicketDetails { get; set; }
        public List<TicketChatVm> TicketChats { get; set; }
    }
}
