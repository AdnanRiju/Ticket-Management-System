using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Drawing;

namespace CompanyManagement.ViewModel
{

    public class TicketChatVm
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string ChatMessage { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
    }
}