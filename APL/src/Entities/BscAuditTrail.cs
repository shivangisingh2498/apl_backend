using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace APL.Entities
{
    [Table("tbl_bsc_audit_trail")]
    public class BscAuditTrail
    {
        [Key]
        public int id { get; set; }
        public int bscformid { get; set; }
        public int userid { get; set; }
        public string? comments { get; set; }
        public int formstatusid { get; set; }
        public bool isactive { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
        public BscFormHeader? tbl_bsc_form_header { get; set; }
        public ObjectMaster? tbl_object_master { get; set; }
        public UserManagement? tbl_user_management { get; set; }

    }

}
