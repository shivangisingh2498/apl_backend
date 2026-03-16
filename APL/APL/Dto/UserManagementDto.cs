using APL.Entities;
using System.ComponentModel.DataAnnotations;

namespace APL.Models
{
    public class UserManagementDto
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? departmentname { get; set; }
        public string? supervisor { get; set; }
        public string? type { get; set; }
        public bool isactive { get; set; } = true;
    }
    public class UserCreateDto
    {
        public int id { get; set; }
        [Required]
        public string? name { get; set; }
        [Required]
        public string? email { get; set; }
        [Required]
        public int departmentid { get; set; }
        [Required]
        public string? supervisor { get; set; }
        [Required]
        public int typeid { get; set; }
    }

}
