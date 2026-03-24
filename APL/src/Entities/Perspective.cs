using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APL.Entities
{
    [Table("tbl_perspective")]
    public class Perspective
    {
        [Key]
        public int id { get; set; }
        public string? perspective { get; set; } = null!;
        public bool isactive { get; set; } = true;
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
        public ICollection<BscPerspective> tbl_bsc_perspective { get; set; } =  new List<BscPerspective>();
    }

}
