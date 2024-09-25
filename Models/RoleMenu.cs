using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyManagement.Models
{
    public class RoleMenu
    {
        public long Id { get; set; }
        public long UserRoleId { get; set; }
        public long MenuItemId { get; set; }
        [ForeignKey("UserRoleId")]
        public virtual UserRole UserRole { get; set; }

        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }
    }
}
