using CompanyManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CompanyManagement.ViewModel
{

    public class TicketChatViewModel
    {

        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }  // Added to show username
        public string? ChatMessage { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? FileUploadPath { get; set; }  // To store the file path
        public IFormFile? FileUpload { get; set; }  // For uploading the file
        public List<TicketChat> PreviousMessages { get; set; } = new List<TicketChat>();
        public List<User> User { get; set; }
    }
}
