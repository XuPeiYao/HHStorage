using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HHStorage.Models.EF {
    public partial class HHStorageContext : DbContext {
        public virtual DbSet<File> File { get; set; }
        public virtual DbSet<Origin> Origin { get; set; }
        public virtual DbSet<Repository> Repository { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<WebHook> WebHook { get; set; }

        public HHStorageContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<File>(entity => {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AccessModifier)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.ContentType)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.Repository)
                    .WithMany(p => p.File)
                    .HasForeignKey(d => d.RepositoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Repository_File");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.File)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_File");
            });

            modelBuilder.Entity<Origin>(entity => {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Origin1)
                    .IsRequired()
                    .HasColumnName("Origin")
                    .HasMaxLength(128);

                entity.Property(e => e.UserId).HasMaxLength(128);

                entity.HasOne(d => d.Repository)
                    .WithMany(p => p.Origin)
                    .HasForeignKey(d => d.RepositoryId)
                    .HasConstraintName("FK_Repository_Origin");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Origin)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_User_Origin");
            });

            modelBuilder.Entity<Repository>(entity => {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AccessModifier)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Repository)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Repository");
            });

            modelBuilder.Entity<User>(entity => {
                entity.Property(e => e.Id)
                    .HasMaxLength(128)
                    .ValueGeneratedNever();

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("nchar(32)");
            });

            modelBuilder.Entity<WebHook>(entity => {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Url).IsRequired();

                entity.Property(e => e.UserId).HasMaxLength(128);

                entity.HasOne(d => d.Repository)
                    .WithMany(p => p.WebHook)
                    .HasForeignKey(d => d.RepositoryId)
                    .HasConstraintName("FK_Repository_WebHook");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.WebHook)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_User_WebHook");
            });
        }
    }
}
