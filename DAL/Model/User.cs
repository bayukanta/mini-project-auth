using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Model
{
    public class User : ICloneable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(250)]
        public string UserName { get; set; }

        [StringLength(250)]
        public string Email { get; set; }

        [StringLength(250)]
        public string Firstname { get; set; }

        [StringLength(250)]
        public string LastName { get; set; }

        [StringLength(250)]
        public string Gender { get; set; }

        [StringLength(250)]
        public string Password { get; set; }

        [StringLength(250)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        [StringLength(250)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public User()
        {
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }

        [JsonIgnore]
        public List<UserRole> UserRoles { get; set; }


        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
