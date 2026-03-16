using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APL.Entities
{
    [Table("tbl_form_master")]
    public class FormMaster
    {
        [Key]
        public long id { get; set; }
        public string formtype { get; set; } = null!;
        public bool isactive { get; set; } = true;
        public string? createdby { get; set; }
        public DateTimeOffset? createddttm { get; set; }
        public string? updatedby { get; set; }
        public DateTimeOffset? updateddttm { get; set; }
    }

}
