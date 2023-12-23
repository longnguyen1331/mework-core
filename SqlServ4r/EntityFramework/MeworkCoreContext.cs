using Domain.AppConfigs;
using Domain.AppHistories;
using Domain.Departments;
using Domain.DocumentFiles;
using Domain.DocumentTypes;
using Domain.FileFolders;
using Domain.FileVersions;
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
        public DbSet<Position> Positions { get; set;}
        public DbSet<Department> Departments { get; set;}
        public DbSet<UserDepartment> UserDepartments { get; set;}
        public DbSet<AppConfig> AppConfigs { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<WebBanner> WebBanners { get; set; }
        public DbSet<WebMenu> WebMenus { get; set; }
        public DbSet<DocumentFile> DocumentFiles { get; set; }
        public DbSet<FileFolder> FileFolders { get; set; }
        public DbSet<StaticFile> StaticFiles { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
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


            //FILE MANAGER
            // if have a foreign key references to FileFolder then forbidden Deletetion
            builder.Entity<DocumentFile>().HasOne<FileFolder>(x => x.FileFolder)
                .WithMany(x => x.Files)
                .HasForeignKey(x => x.DocumentFolderId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);


            builder.Entity<DocumentFile>().HasOne<User>(x => x.User)
                .WithMany(x => x.CreatedFiles)
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DocumentFile>().HasOne<DocumentType>(x => x.DocumentType)
                .WithMany(x => x.DocumentFiles)
                .HasForeignKey(x => x.DocumentTypeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<FileVersion>().
                HasOne<DocumentFile>(x => x.DocumentFile)
                .WithMany(x => x.FileVersions)
                .HasForeignKey(x => x.FileId)
                .OnDelete(DeleteBehavior.Restrict);



            builder.Entity<FileVersion>().HasOne<User>(x => x.User)
                .WithMany(x => x.EditedFileVersions)
                .HasForeignKey(x => x.EditBy)
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