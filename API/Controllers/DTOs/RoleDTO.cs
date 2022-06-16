using System;

namespace API.Auth.DTO
{
    public class RoleDTO
    {
        public Guid? RoleId { get; set; }

        public string RoleCode { get; set; }

        public bool Status { get; set; }
    }
}
