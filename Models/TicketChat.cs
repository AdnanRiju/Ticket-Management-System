using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyManagement.Models
{
    public class TicketChat
    {
        public int Id { get; set; }
        public int TicketId { get; set; }

        public int UserId { get; set; }

        public string? ChatMessage { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? FileUploadPath { get; set; }
    }
}
