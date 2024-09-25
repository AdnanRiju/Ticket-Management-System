using System.ComponentModel.DataAnnotations;

namespace CompanyManagement.Models
{
    public class UserRole
    {
        public long Id { get; set; }
        [Required]
        public string RoleName { get; set; }
        public int RoleType { get; set; }
        public virtual List<RoleMenu> RoleMenus { get; set; }
    }
}
