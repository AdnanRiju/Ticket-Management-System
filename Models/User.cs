using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyManagement.Models
{
    public class User
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserTitle { get; set; }
        public long UserRoleId { get; set; }
        public long CompanyId { get; set; }
        public bool AdminAccess { get; set; }
        public int ActiveStatus { get; set; }

        [ForeignKey("UserRoleId")]
        public virtual UserRole UserRole { get; set; }
      
    }

}
