using dashboardtask.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dashboardtask.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // =====================================================
        // ASSET HIERARCHY
        // =====================================================
        public DbSet<AssetType> AssetType { get; set; }
        public DbSet<Line> Line { get; set; }
        public DbSet<Machine> Machine { get; set; }
        public DbSet<Unit> Unit { get; set; }
        public DbSet<Part> Part { get; set; }

        // =====================================================
        // DYNAMIC ATTRIBUTES
        // =====================================================
        public DbSet<AttributeDefinition> AttributeDefinition { get; set; }
        public DbSet<LineAttribute> LineAttribute { get; set; }
        public DbSet<MachineAttribute> MachineAttribute { get; set; }
        public DbSet<UnitAttribute> UnitAttribute { get; set; }
        public DbSet<PartAttribute> PartAttribute { get; set; }

        // =====================================================
        // MAINTENANCE MANAGEMENT
        // =====================================================
        public DbSet<MaintenanceStandard> MaintenanceStandard { get; set; }
        public DbSet<MaintenanceStandardType> MaintenanceStandardType { get; set; }
        public DbSet<MaintenanceIndicatorUnit> MaintenanceIndicatorUnit { get; set; }
        public DbSet<MaintenanceIndicator> MaintenanceIndicator { get; set; }
        public DbSet<MaintenanceControl> MaintenanceControl { get; set; }
        public DbSet<MaintenanceSchedule> MaintenanceSchedule { get; set; }

        // =====================================================
        // MAINTENANCE LINKS
        // =====================================================
        public DbSet<MaintenanceLink> MaintenanceLink { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================================
            // TABLE NAME MAPPINGS (ให้ตรงกับชื่อจริงในฐานข้อมูล)
            // =====================================================
            modelBuilder.Entity<AssetType>().ToTable("AssetType");
            modelBuilder.Entity<Line>().ToTable("Line");
            modelBuilder.Entity<Machine>().ToTable("Machine");
            modelBuilder.Entity<Unit>().ToTable("Unit");
            modelBuilder.Entity<Part>().ToTable("Part");

            modelBuilder.Entity<AttributeDefinition>().ToTable("AttributeDefinition");
            modelBuilder.Entity<LineAttribute>().ToTable("LineAttribute");
            modelBuilder.Entity<MachineAttribute>().ToTable("MachineAttribute");
            modelBuilder.Entity<UnitAttribute>().ToTable("UnitAttribute");
            modelBuilder.Entity<PartAttribute>().ToTable("PartAttribute");

            modelBuilder.Entity<MaintenanceStandard>().ToTable("MaintenanceStandard");
            modelBuilder.Entity<MaintenanceStandardType>().ToTable("MaintenanceStandardType");
            modelBuilder.Entity<MaintenanceIndicatorUnit>().ToTable("MaintenanceIndicatorUnit");
            modelBuilder.Entity<MaintenanceIndicator>().ToTable("MaintenanceIndicator");
            modelBuilder.Entity<MaintenanceControl>().ToTable("MaintenanceControl");
            modelBuilder.Entity<MaintenanceSchedule>().ToTable("MaintenanceSchedule");
            modelBuilder.Entity<MaintenanceLink>().ToTable("MaintenanceLink");

            // =====================================================
            // ASSET TYPE RELATIONSHIPS
            // =====================================================
            modelBuilder.Entity<AssetType>()
                .HasMany(at => at.AttributeDefinitions)
                .WithOne(ad => ad.AssetType)
                .HasForeignKey(ad => ad.AssetTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssetType>()
                .HasMany(at => at.MaintenanceControls)
                .WithOne(mc => mc.AssetType)
                .HasForeignKey(mc => mc.AssetTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); // ✅ เพิ่ม

            modelBuilder.Entity<AssetType>()
                .HasMany(at => at.MaintenanceLinks)
                .WithOne(ml => ml.AssetType)
                .HasForeignKey(ml => ml.AssetTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            // =====================================================
            // LINE RELATIONSHIPS
            // =====================================================
            modelBuilder.Entity<Line>()
                .HasMany(l => l.Machines)
                .WithOne(m => m.Line)
                .HasForeignKey(m => m.LineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Line>()
                .HasMany(l => l.LineAttributes)
                .WithOne(la => la.Line)
                .HasForeignKey(la => la.LineId)
                .OnDelete(DeleteBehavior.Cascade);

            // =====================================================
            // MACHINE RELATIONSHIPS
            // =====================================================
            modelBuilder.Entity<Machine>()
                .HasMany(m => m.Units)
                .WithOne(u => u.Machine)
                .HasForeignKey(u => u.MachineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Machine>()
                .HasMany(m => m.MachineAttributes)
                .WithOne(ma => ma.Machine)
                .HasForeignKey(ma => ma.MachineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Machine>()
                .HasMany(m => m.MaintenanceControls)
                .WithOne(mc => mc.Machine)
                .HasForeignKey(mc => mc.MachineId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); // ✅ เพิ่ม

            // =====================================================
            // UNIT RELATIONSHIPS
            // =====================================================
            modelBuilder.Entity<Unit>()
                .HasMany(u => u.Parts)
                .WithOne(p => p.Unit)
                .HasForeignKey(p => p.UnitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Unit>()
                .HasMany(u => u.UnitAttributes)
                .WithOne(ua => ua.Unit)
                .HasForeignKey(ua => ua.UnitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Unit>()
                .HasMany(u => u.MaintenanceControls)
                .WithOne(mc => mc.Unit)
                .HasForeignKey(mc => mc.UnitId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); // ✅ เพิ่ม

            // =====================================================
            // PART RELATIONSHIPS
            // =====================================================
            modelBuilder.Entity<Part>()
                .HasMany(p => p.PartAttributes)
                .WithOne(pa => pa.Part)
                .HasForeignKey(pa => pa.PartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Part>()
                .HasMany(p => p.MaintenanceControls)
                .WithOne(mc => mc.Part)
                .HasForeignKey(mc => mc.PartId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); // ✅ เพิ่ม

            // =====================================================
            // ATTRIBUTE DEFINITION RELATIONSHIPS
            // =====================================================
            modelBuilder.Entity<AttributeDefinition>()
                .HasMany(ad => ad.LineAttributes)
                .WithOne(la => la.AttributeDefinition)
                .HasForeignKey(la => la.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AttributeDefinition>()
                .HasMany(ad => ad.MachineAttributes)
                .WithOne(ma => ma.AttributeDefinition)
                .HasForeignKey(ma => ma.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AttributeDefinition>()
                .HasMany(ad => ad.UnitAttributes)
                .WithOne(ua => ua.AttributeDefinition)
                .HasForeignKey(ua => ua.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AttributeDefinition>()
                .HasMany(ad => ad.PartAttributes)
                .WithOne(pa => pa.AttributeDefinition)
                .HasForeignKey(pa => pa.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);

            // =====================================================
            // MAINTENANCE CONTROL RELATIONSHIPS ✅ แก้ไขตรงนี้
            // =====================================================
            modelBuilder.Entity<MaintenanceControl>(entity =>
            {
                // ✅ เชื่อมกับ Standard


                // ✅ เชื่อมกับ AssetType
                entity.HasOne(d => d.AssetType)
                      .WithMany(p => p.MaintenanceControls)
                      .HasForeignKey(d => d.AssetTypeId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false); // ✅ เพิ่มบรรทัดนี้

                // ✅ เชื่อมกับ Machine
                entity.HasOne(d => d.Machine)
                      .WithMany(p => p.MaintenanceControls)
                      .HasForeignKey(d => d.MachineId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false); // ✅ เพิ่มบรรทัดนี้

                // ✅ เชื่อมกับ Unit
                entity.HasOne(d => d.Unit)
                      .WithMany(p => p.MaintenanceControls)
                      .HasForeignKey(d => d.UnitId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false); // ✅ เพิ่มบรรทัดนี้

                // ✅ เชื่อมกับ Part
                entity.HasOne(d => d.Part)
                      .WithMany(p => p.MaintenanceControls)
                      .HasForeignKey(d => d.PartId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false); // ✅ เพิ่มบรรทัดนี้


            });
            // ✅ แยกออกมาเฉพาะ relationship ระหว่าง Control ↔ Schedule
            modelBuilder.Entity<MaintenanceControl>()
                .HasMany(c => c.Schedules)
                .WithOne(s => s.ControlItem)
                .HasForeignKey(s => s.ControlItemId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MaintenanceSchedule>(entity =>
            {
                entity.HasOne(d => d.ControlItem)
                      .WithMany(p => p.Schedules)
                      .HasForeignKey(d => d.ControlItemId)
                      .OnDelete(DeleteBehavior.NoAction); // 🔒 ป้องกันการ cascade ทั้งคู่

                entity.HasOne(s => s.Standard)
                      .WithMany(st => st.Schedules)
                      .HasForeignKey(s => s.StandardId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Indicator)
                      .WithMany(i => i.Schedules)
                      .HasForeignKey(s => s.IndicatorID)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            // =====================================================
            // SEED DATA
            // =====================================================
            modelBuilder.Entity<AssetType>().HasData(
                new AssetType { AssetTypeId = 1, TypeName = "Line", CreatedTime = DateTime.Now },
                new AssetType { AssetTypeId = 2, TypeName = "Machine", CreatedTime = DateTime.Now },
                new AssetType { AssetTypeId = 3, TypeName = "Unit", CreatedTime = DateTime.Now },
                new AssetType { AssetTypeId = 4, TypeName = "Part", CreatedTime = DateTime.Now }
            );

            // Set Default Value for CreatedTime
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var CreatedTimeProperty = entityType.FindProperty("CreatedTime");
                if (CreatedTimeProperty != null && CreatedTimeProperty.ClrType == typeof(DateTime))
                {
                    CreatedTimeProperty.SetDefaultValueSql("GETDATE()");
                }
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveColumnType("datetime2");
        }
    }
}
