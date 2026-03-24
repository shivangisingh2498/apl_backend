using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace APL.Entities
{
    [Table("tbl_bsc_form_header")]
    public class BscFormHeader
    {
        [Key]
        public int id { get; set; }
        public int departmentid { get; set; }
        public int stationid { get; set; }
        public bool issharedbyadmin { get; set; }
        public int statusid { get; set; }
        public bool issubmittedbyspoc { get; set; }
        public bool isactive { get; set; }
        public DateTime? templatecreatedate { get; set; }
        public DateTime? spoctemplatesubmitdate { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string? updatedby { get; set; }
        public DateTime? updatedon { get; set; }
        public DepartmentMaster? tbl_department_master { get; set; }
        public StationMaster? tbl_station_master { get; set; }
        public ICollection<BscPerspective>? tbl_bsc_perspective { get; set; } = new List<BscPerspective>();
        public ICollection<BscStrategicObjective>? tbl_bsc_strategic_objective { get; set; } = new List<BscStrategicObjective>();
        public ICollection<BscKpi>? tbl_bsc_kpi { get; set; } = new List<BscKpi>();
    }

}
