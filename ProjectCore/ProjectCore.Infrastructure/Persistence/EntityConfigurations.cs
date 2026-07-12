using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectCore.Domain.Entities;
using ProjectCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Infrastructure.Persistence;

public abstract class BaseEntityConfiguration<TEntity, TKey>
    : IEntityTypeConfiguration<TEntity>
    where TEntity : DomainEntity<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);

        // ===== AUDIT =====
        builder.Property(x => x.CreatedDate)
               .IsRequired();

        builder.Property(x => x.CreatedBy)
               .IsRequired();

        builder.Property(x => x.UpdatedDate);
        builder.Property(x => x.UpdatedBy);

        builder.Property(x => x.Status)
               .IsRequired();
    }
}

public sealed class RoleConfiguration
    : BaseEntityConfiguration<Role, Guid>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);
        // ===== TABLE =====
        builder.ToTable("Roles");
        // ===== ID =====
        builder.Property(x => x.Id)
               .ValueGeneratedNever();
        // ===== RoleName =====
        builder.OwnsOne(x => x.Name, u =>
        {
            u.Property(p => p.Value)
             .HasColumnName("Name")
             .IsRequired()
             .HasMaxLength(100);
            u.HasIndex(p => p.Value).IsUnique();
        });
        // ===== Description =====
        builder.Property(x => x.Description)
               .HasMaxLength(500)
               .IsRequired(false);

        // ===== RELATIONSHIP =====
        builder.HasMany(x => x.RolePermissions)
               .WithOne()
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class RolePermissionConfiguration
    : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");
        builder.HasKey(x => new { x.RoleId, x.PermissionId });
     
    }
}

public sealed class PermissionConfiguration
    : BaseEntityConfiguration<Permission, Guid>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);
        // ===== TABLE =====
        builder.ToTable("Permissions");
        // ===== ID =====
        builder.Property(x => x.Id)
               .ValueGeneratedNever();
        // ===== PermissionCode =====
        builder.OwnsOne(x => x.Code, u =>
        {
            u.Property(p => p.Value)
             .HasColumnName("Code")
             .IsRequired()
             .HasMaxLength(100);
        });

        // ===== Module =====
        builder.Property(x => x.Module)
               .IsRequired()
               .HasMaxLength(50);

        // ===== Action =====
        builder.Property(x => x.Action)
               .IsRequired()
               .HasMaxLength(50);

        // ===== Description =====
        builder.Property(x => x.Description)
               .HasMaxLength(500)
               .IsRequired(false);
    }
}

public sealed class UserConfiguration
    : BaseEntityConfiguration<User, Guid>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        // ===== TABLE =====
        builder.ToTable("Users");

        // ===== ID =====
        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // ===== PASSWORD =====
        builder.Property(x => x.PasswordHash)
               .IsRequired()
               .HasMaxLength(500);

        // ===== VALUE OBJECTS =====

        // UserName
        builder.OwnsOne(x => x.UserName, u =>
        {
            u.Property(p => p.Value)
             .HasColumnName("UserName")
             .IsRequired()
             .HasMaxLength(100);
        });

        // Email
        builder.OwnsOne(x => x.Email, e =>
        {
            e.Property(p => p.Value)
             .HasColumnName("Email")
             .IsRequired()
             .HasMaxLength(150);

            e.HasIndex(p => p.Value).IsUnique();
        });

        // FullName (nullable)
        builder.OwnsOne(x => x.FullName, f =>
        {
            f.Property(p => p.Value)
             .HasColumnName("FullName")
             .HasMaxLength(150);
        });

        // PhoneNumber (nullable)
        builder.OwnsOne(x => x.PhoneNumber, p =>
        {
            p.Property(x => x.Value)
             .HasColumnName("PhoneNumber")
             .HasMaxLength(20);
        });

        // Avatar (nullable)
        builder.OwnsOne(x => x.Avatar, a =>
        {
            a.Property(p => p.Value)
             .HasColumnName("Avatar")
             .HasMaxLength(500);
        });

        // ===== ENUM =====
        builder.Property(x => x.Gender)
               .HasConversion<string>()
               .HasMaxLength(20);

        // ===== SIMPLE FIELDS =====
        builder.Property(x => x.Address)
               .HasMaxLength(255);

        builder.Property(x => x.DateOfBirth)
               .HasColumnType("date");

        // ===== RELATIONSHIP =====
        builder.HasMany(x => x.UserRoles)
               .WithOne()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // ===== GLOBAL FILTER (OPTIONAL) =====
        builder.HasQueryFilter(x => x.Status == EntityStatus.Active);
    }
}

public sealed class UserRoleConfiguration
    : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasKey(x => new { x.UserId, x.RoleId });

        builder.Property(x => x.AssignedAt)
               .IsRequired();

        builder.Property(x => x.AssignedBy)
               .IsRequired();
    }
}

