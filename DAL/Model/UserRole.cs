using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Model
{
    public class UserRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserRoleId { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid RoleId { get; set; }
        public Role Role { get; set; }

        [StringLength(250)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public UserRole()
        {
            CreatedDate = DateTime.UtcNow;
        }
    }
}