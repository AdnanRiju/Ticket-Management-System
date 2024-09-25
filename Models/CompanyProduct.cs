using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CompanyManagement.Models
{
    public class CompanyProduct
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int ProductId { get; set; }
        public virtual Company Company { get; set; }
        public virtual Product Product { get; set; }
    }
}
