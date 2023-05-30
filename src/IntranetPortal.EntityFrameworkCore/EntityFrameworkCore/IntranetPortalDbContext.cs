
using IntranetPortal.AppEntities;
using IntranetPortal.AppEntities.Documents;
using IntranetPortal.AppEntities.UserProfiles;
using IntranetPortal.InternalApplications;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace IntranetPortal.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class IntranetPortalDbContext :
    AbpDbContext<IntranetPortalDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public DbSet<InternalApplication> InternalApplications { get; set; }
    public DbSet<DocumentStatus> DocumentStatuses { get; set; }
    public DbSet<Document> Documents { get; set; }
    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion
    public DbSet<DocumentAcknowledgementRequestStatuses> DocumentAcknowledgementRequestStatus { get; set; }
    public DbSet<DocumentAcknowledgementRequests> DocumentAcknowledgementRequests { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Designation> Designation { get; set; }
    public DbSet<UserProfileStatus> UserProfileStatuses { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }


    public IntranetPortalDbContext(DbContextOptions<IntranetPortalDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        builder.Entity<InternalApplication>(b =>
        {
            b.ToTable("InternalApplications");
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(100);
            b.Property(x => x.Description).HasMaxLength(int.MaxValue);
            b.Property(x => x.LogoUrl).HasMaxLength(int.MaxValue);
            b.Property(x => x.ApplicationUrl).HasMaxLength(int.MaxValue);
        });

        builder.Entity<DocumentStatus>(b =>
        {
            b.ToTable("DocumentStatuses");
            b.ConfigureByConvention();

            b.Property(x => x.SystemName).IsRequired().HasMaxLength(100);
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(100);
        });

        builder.Entity<Document>(b =>
        {
            b.ToTable("Documents");
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
            b.Property(x => x.Description).HasMaxLength(int.MaxValue);
            b.Property(x => x.DocumentUrl).HasMaxLength(int.MaxValue);
            b.Property(x => x.AcknowledgementDeadlineInDays).HasColumnType("int");
            b.HasOne<DocumentStatus>().WithMany().HasForeignKey(x => x.DocumentStatusId).IsRequired();
        });

        builder.Entity<DocumentAcknowledgementRequestStatuses>(b =>
        {
            b.ToTable("DocumentAcknowledgementRequestStatus");
            b.ConfigureByConvention();

            b.Property(x => x.SystemName).IsRequired().HasMaxLength(100);
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(100);
        });

        builder.Entity<DocumentAcknowledgementRequests>(b =>
        {
            b.ToTable("DocumentAcknowledgementRequests");
            b.ConfigureByConvention();

            b.Property(x => x.AbpUserId);
            b.Property(x => x.AcknowledgedDateTime).IsRequired();
            b.Property(x => x.DueDateTime).IsRequired();
            b.HasOne<Document>().WithMany().HasForeignKey(x => x.DocumentId).IsRequired();
            b.HasOne<DocumentAcknowledgementRequestStatuses>().WithMany().HasForeignKey(x => x.DocumentAcknowledgementRequestStatusId).IsRequired();
        });

        builder.Entity<Department>(b =>
        {
            b.ToTable("Departments");
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.ReferenceId).IsRequired().HasMaxLength(200);
            b.Property(x => x.Code).HasMaxLength(200);
            b.Property(x => x.IsActive);
        });

        builder.Entity<Designation>(b =>
        {
            b.ToTable("Designations");
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.ReferenceId).IsRequired().HasMaxLength(200);
            b.Property(x => x.Code).HasMaxLength(200);
            b.Property(x => x.IsActive);
        });

        builder.Entity<UserProfileStatus>(b =>
        {
            b.ToTable("UserProfileStatuses");
            b.ConfigureByConvention();

            b.Property(x => x.SystemName).IsRequired();
            b.Property(x => x.DisplayName).IsRequired();
        });

        builder.Entity<UserProfile>(b =>
        {
            b.ToTable("UserProfiles");
            b.ConfigureByConvention();

            b.HasOne<IdentityUser>().WithOne().HasForeignKey("UserProfile", "AbpUserId");
            b.HasOne<Designation>().WithMany().HasForeignKey(x => x.DesignationId).IsRequired();
            b.HasOne<Department>().WithMany().HasForeignKey(x => x.DepartmentId).IsRequired();
            b.Property(x => x.DateOfBirth).IsRequired();
            b.Property(x => x.ProfilePictureUrl);
            b.Property(x => x.HiredDate).IsRequired();
            b.Property(x => x.ReferenceId).IsRequired().HasMaxLength(200);
            b.Property(x => x.MiddleName);
            b.HasOne<UserProfileStatus>().WithMany().HasForeignKey(x => x.UserProfileStatusId).IsRequired();
        });


        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(IntranetPortalConsts.DbTablePrefix + "YourEntities", IntranetPortalConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
    }
}
