
namespace CompanyManagement.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        public int ActiveCompany { get; set; }
        public string ShortName { get; set; }
    }
}
