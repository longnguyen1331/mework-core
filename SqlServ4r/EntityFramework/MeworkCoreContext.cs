using Domain.AppConfigs;
using Domain.AppHistories;
using Domain.BackupDetails;
using Domain.Backups;
using Domain.Departments;
using Domain.Identity.RoleClaims;
using Domain.Identity.Roles;
using Domain.Identity.UserClaim;
using Domain.Identity.UserLogins;
using Domain.Identity.UserRoles;
using Domain.Identity.Users;
using Domain.Identity.UserTokens;
using Domain.Positions;
using Domain.PostCategories;
using Domain.Posts;
using Domain.Services;
using Domain.ServiceTypes;
using Domain.StaticFiles;
using Domain.UserDepartments;
using Domain.WebBanners;
using Domain.WebMenus;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SqlServ4r.EntityFramework
{
    public class MeworkCoreContext  : IdentityDbContext<User,Role,Guid,UserClaim,UserRole,UserLogin,RoleClaim,UserToken>
    {
        public DbSet<AppHistory> AppHistories { get; set;}
        public DbSet<Backup> Backups { get; set;}
        public DbSet<BackupDetail> BackupDetails { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Department> Departments { get; set;}
        public DbSet<UserDepartment> UserDepartments { get; set;}
        public DbSet<AppConfig> AppConfigs { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<WebBanner> WebBanners { get; set; }
        public DbSet<WebMenu> WebMenus { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<StaticFile> StaticFiles { get; set; }
        public MeworkCoreContext(DbContextOptions<MeworkCoreContext> options):base(options)
        {
        }
        
        protected override void OnModelCreating (ModelBuilder builder) {

            base.OnModelCreating (builder); 

            //default ondelete cascade
            //get rid of prefix Asp
            foreach (var entityType in builder.Model.GetEntityTypes ()) {
                var tableName = entityType.GetTableName ();
                if (tableName.StartsWith ("AspNet")) {
                    entityType.SetTableName (tableName.Substring (6));
                }
            }

            builder.Entity<AppHistory>().HasOne<User>(x => x.User)
                .WithMany(x => x.AppHistoryUsers)
                .HasForeignKey(x => x.UserId);


            builder.Entity<BackupDetail>().HasOne<User>(x => x.User)
                .WithMany(x => x.BackupDetailUsers)
                .HasForeignKey(x => x.UserId); 

            builder.Entity<Backup>().HasOne<User>(x => x.User)
                .WithMany(x => x.BackupUsers)
                .HasForeignKey(x => x.UserId);


            builder.Entity<PostCategory>().HasOne<User>(x => x.CreatedUser)
                .WithMany(x => x.CreatedUserPostCategories)
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PostCategory>().HasOne<User>(x => x.ModifiedUser)
                .WithMany(x => x.ModifiedUserPostCategories)
                .HasForeignKey(x => x.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Post>().HasOne<User>(x => x.CreatedUser)
                .WithMany(x => x.CreatedUserPosts)
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Post>().HasOne<User>(x => x.ModifiedUser)
                .WithMany(x => x.ModifiedUserPosts)
                .HasForeignKey(x => x.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Post>().HasOne<PostCategory>(x => x.PostCategory)
                .WithMany(x => x.Posts)
                .HasForeignKey(x => x.PostCategoryId)
                .OnDelete(DeleteBehavior.Restrict);



            //================SS2

            //relative 1-n : position  - user
            builder.Entity<User>().HasOne<Position>(x => x.Position)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.PositionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // relative n-n : user - user_department -  department
            // when you delete department then it delete userdepartment record correspond 
            builder.Entity<UserDepartment>().HasKey(sc => new { sc.Id,sc.DepartmentId,sc.UserId });
            builder.Entity<UserDepartment>().HasOne<User>(x => x.User)
                .WithMany(x => x.UserDepartments)
                .HasForeignKey(x=>x.UserId);
            
            builder.Entity<UserDepartment>().HasOne<Department>(x => x.Department)
                .WithMany(x => x.UserDepartments)
                .HasForeignKey(x=>x.DepartmentId);

         

			builder.Entity<WebBanner>().HasOne<User>(x => x.CreatedUser)
                .WithMany(x => x.CreatedUserWebBanners)
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WebBanner>().HasOne<User>(x => x.ModifiedUser)
                .WithMany(x => x.ModifiedUserWebBanners)
                .HasForeignKey(x => x.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WebMenu>().HasOne<User>(x => x.CreatedUser)
                .WithMany(x => x.CreatedUserWebMenus)
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WebMenu>().HasOne<User>(x => x.ModifiedUser)
                .WithMany(x => x.ModifiedUserWebMenus)
                .HasForeignKey(x => x.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);



            //service 
            builder.Entity<Service>().HasKey(sc => new { sc.Id });
            builder.Entity<Service>().HasOne<StaticFile>(x => x.ImageFile)
                .WithMany(x => x.Services)
                .HasForeignKey(x => x.ImageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Service>().HasKey(sc => new { sc.Id });
            builder.Entity<Service>().HasOne<ServiceType>(x => x.ServiceType)
                .WithMany(x => x.Services)
                .HasForeignKey(x => x.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Service>().HasKey(sc => new { sc.Id });
            builder.Entity<Service>().HasOne<User>(x => x.CreatedUser)
                .WithMany(x => x.CreatedUserServices)
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Service>().HasKey(sc => new { sc.Id });
            builder.Entity<Service>().HasOne<User>(x => x.ModifiedUser)
                .WithMany(x => x.ModifiedUserServices)
                .HasForeignKey(x => x.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            //service type
            builder.Entity<ServiceType>().HasKey(sc => new { sc.Id });
            builder.Entity<ServiceType>().HasOne<StaticFile>(x => x.ImageFile)
                .WithMany(x => x.ServiceTypes)
                .HasForeignKey(x => x.ImageId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ServiceType>().HasOne<User>(x => x.CreatedUser)
                .WithMany(x => x.CreatedUserServiceTypes)
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ServiceType>().HasOne<User>(x => x.ModifiedUser)
                .WithMany(x => x.ModifiedUserServiceTypes)
                .HasForeignKey(x => x.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            SetUniqueForProperties(builder);
        }

        public void SetUniqueForProperties(ModelBuilder builder)
        {
            builder.Entity<Role>(entity => {

                entity.HasIndex(p => p.Name)     
                    .IsUnique(true);
                entity.HasIndex(p => p.Code)     
                    .IsUnique(true);
            });
            
            builder.Entity<User>(entity => {
                entity.HasIndex(p => p.PhoneNumber)     
                    .IsUnique(true);
                entity.HasIndex(p => p.UserCode)     
                    .IsUnique(true);
                entity.HasIndex(p => p.Email)     
                    .IsUnique(true);
            });
            
            
            builder.Entity<Position>(entity => {
                entity.HasIndex(p => p.Name)     
                    .IsUnique(true);
                entity.HasIndex(p => p.Code)     
                    .IsUnique(true);
            });
            
            builder.Entity<Department>(entity => {
                entity.HasIndex(p => p.Name)     
                    .IsUnique(true);
                entity.HasIndex(p => p.Code)     
                    .IsUnique(true);
            });
            
            
            builder.Entity<AppConfig>().HasOne<StaticFile>(x => x.IconFile)
               .WithMany(x => x.AppConfigIcons)
               .HasForeignKey(x => x.IconId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppConfig>().HasOne<StaticFile>(x => x.LogoFile)
                .WithMany(x => x.AppConfigLogos)
                .HasForeignKey(x => x.LogoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppConfig>(entity =>
            {
                entity.HasIndex(p => p.Code)
                    .IsUnique(true);
            });
        }
    }
}