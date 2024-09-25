using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyManagement.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int ProductTypeId { get; set; }
        public int? ActiveStatus { get; set; }
        public DateTime CreateDate { get; set; }
        
    }

}
