using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace APL.Entities
{
    [Table("tbl_frequency_months_mapping")]
    public class FrequencyMonthsMapping
    {
        [Key]
        public int id { get; set; }
        public int frequencyid { get; set; }
        public int monthno { get; set; }
        public bool isactive { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
        public ObjectMaster? tbl_object_master { get; set; }


    }

}
