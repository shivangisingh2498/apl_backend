using APL.Entities;
using Microsoft.EntityFrameworkCore;


namespace APL.Data
{
    public class AplDbContext : DbContext
    {
        public AplDbContext(DbContextOptions<AplDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("bsc");
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<UserManagement>()
                   .HasOne(x => x.tbl_department_master)
                   .WithMany(d => d.tbl_user_management)
                   .HasForeignKey(x => x.departmentid);

            // FK: typeid -> tbl_object_master.id
            modelBuilder.Entity<UserManagement>()
             .HasOne(x => x.tbl_roles_master)
             .WithMany(o => o.tbl_user_management)
             .HasForeignKey(x => x.typeid);


            modelBuilder.Entity<UserManagement>()
             .HasOne(x => x.tbl_station_master)
             .WithMany(o => o.tbl_user_management)
             .HasForeignKey(x => x.stationid);

            modelBuilder.Entity<BscFormHeader>()
            .HasOne(x => x.tbl_station_master)
            .WithMany(o => o.tbl_bsc_form_header)
            .HasForeignKey(x => x.stationid);

            modelBuilder.Entity<BscFormHeader>()
           .HasOne(x => x.tbl_department_master)
           .WithMany(o => o.tbl_bsc_form_header)
           .HasForeignKey(x => x.departmentid);

            modelBuilder.Entity<BscPerspective>()
            .HasOne(x => x.tbl_bsc_form_header)
            .WithMany(o => o.tbl_bsc_perspective)
            .HasForeignKey(x => x.bscformid);

            modelBuilder.Entity<BscPerspective>()
            .HasOne(x => x.tbl_perspective)
            .WithMany(o => o.tbl_bsc_perspective)
            .HasForeignKey(x => x.perspectiveid);


            modelBuilder.Entity<BscStrategicObjective>()
    .HasOne(so => so.tbl_bsc_form_header)
    .WithMany(fh => fh.tbl_bsc_strategic_objective)
    .HasForeignKey(so => so.bscformid);


            modelBuilder.Entity<BscStrategicObjective>()
            .HasOne(x => x.tbl_strategic_objective)
            .WithMany(o => o.tbl_strategic_objective)
            .HasForeignKey(x => x.strategicobjectiveid);

            modelBuilder.Entity<BscStrategicObjective>()
            .HasOne(x => x.tbl_bsc_perspective)
            .WithMany(o => o.tbl_bsc_strategic_objective)
            .HasForeignKey(x => x.bscperspectiveid);


            modelBuilder.Entity<BscKpi>()
     .HasOne(x => x.tbl_kpi_master)
     .WithMany(o => o.tbl_bsc_kpi)
     .HasForeignKey(x => x.kpiid);

            modelBuilder.Entity<BscKpi>()
            .HasOne(x => x.tbl_bsc_form_header)
            .WithMany(o => o.tbl_bsc_kpi)
            .HasForeignKey(x => x.bscformid);


            modelBuilder.Entity<BscKpi>()
       .HasOne(k => k.tbl_strategic_objective)
       .WithMany()
       .HasForeignKey(k => k.bscstrategicobjectiveid);

            modelBuilder.Entity<BscKpi>()
                .HasOne(k => k.tbl_bsc_strategic_objective)
                .WithMany(o => o.tbl_bsc_kpi)
                .HasForeignKey(k => k.bscstrategicobjectiveid);

            modelBuilder.Entity<BscKpi>()
            .HasOne(k => k.tbl_strategic_objective)
            .WithMany()
            .HasForeignKey(k => k.bscstrategicobjectiveid);

            modelBuilder.Entity<BscAuditTrail>()
            .HasOne(at => at.tbl_bsc_form_header)
            .WithMany(fh => fh.tbl_bsc_audit_trail)
            .HasForeignKey(at => at.bscformid)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BscAuditTrail>()
            .HasOne(at => at.tbl_user_management)
            .WithMany(fh => fh.tbl_bsc_audit_trail)
            .HasForeignKey(at => at.userid)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BscAuditTrail>()
           .HasOne(at => at.tbl_object_master)
           .WithMany(fh => fh.tbl_bsc_audit_trail)
           .HasForeignKey(at => at.formstatusid);

        }

        public DbSet<Perspective> tbl_perspective { get; set; }
        public DbSet<StrategicObjective> tbl_strategic_objective { get; set; }
        public DbSet<DepartmentMaster> tbl_department_master { get; set; }

        public DbSet<ObjectMaster> tbl_object_master { get; set; }
        public DbSet<UserManagement> tbl_user_management { get; set; }
        public DbSet<RolesMaster> tbl_roles_master { get; set; }
        public DbSet<StationMaster> tbl_station_master { get; set; }
        public DbSet<KpiMaster> tbl_kpi_master { get; set; }
        public DbSet<BscFormHeader> tbl_bsc_form_header { get; set; }
        public DbSet<BscPerspective> tbl_bsc_perspective { get; set; }
        public DbSet<BscStrategicObjective> tbl_bsc_strategic_objective { get; set; }
        public DbSet<BscKpi> tbl_bsc_kpi { get; set; }
        public DbSet<BscAuditTrail> tbl_bsc_audit_trail { get; set; }
    }

}
