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
                   .HasForeignKey(x => x.departmentid)
                   .OnDelete(DeleteBehavior.Restrict);

            // FK: typeid -> tbl_object_master.id
            modelBuilder.Entity<UserManagement>()
             .HasOne(x => x.tbl_roles_master)
             .WithMany(o => o.tbl_user_management)    
             .HasForeignKey(x => x.typeid)
             .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<UserManagement>()
             .HasOne(x => x.tbl_station_master)
             .WithMany(o => o.tbl_user_management)
             .HasForeignKey(x => x.stationid)
             .OnDelete(DeleteBehavior.Restrict);

        }

        public DbSet<FormMaster> tbl_form_master { get; set; }

        public DbSet<Perspective> tbl_perspective { get; set; }
        public DbSet<StrategicObjective> tbl_strategic_objective { get; set; }
        public DbSet<DepartmentMaster> tbl_department_master{ get; set; }

        public DbSet<ObjectMaster> tbl_object_master { get; set; }
        public DbSet<UserManagement> tbl_user_management { get; set; }
        public DbSet<RolesMaster> tbl_roles_master { get; set; }
        public DbSet<StationMaster> tbl_station_master { get; set; }
    }

}
