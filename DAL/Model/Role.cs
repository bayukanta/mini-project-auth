using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Model
{
    public class Role : ICloneable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RoleId { get; set; }

        [Required]
        [StringLength(250)]
        public string RoleCode { get; set; }

        [Required]
        [StringLength(250)]
        public string RoleDescription { get; set; }

        public bool Status { get; set; }

        [StringLength(250)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        [StringLength(250)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public Role()
        {
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}