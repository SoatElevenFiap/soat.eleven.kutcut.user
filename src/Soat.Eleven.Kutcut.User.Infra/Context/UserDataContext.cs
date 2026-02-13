using Microsoft.EntityFrameworkCore;
using Soat.Eleven.Kutcut.Users.Domain.Entities;
using Soat.Eleven.Kutcut.Users.Domain.Enums;

namespace Soat.Eleven.Kutcut.Users.Infra.Context;

public class UserDataContext(DbContextOptions<UserDataContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
                    .HasKey(u => u.Id);
        
        modelBuilder.Entity<User>()
                    .Property(u => u.Id)
                    .HasDefaultValue("gen_random_uuid()");
        
        modelBuilder.Entity<User>()
                    .Property(u => u.Name)
                    .IsRequired()
                    .HasMaxLength(100);
        
        modelBuilder.Entity<User>()
                    .Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);
        
        modelBuilder.Entity<User>()
                    .Property(u => u.Password)
                    .IsRequired()
                    .HasMaxLength(255);
        
        modelBuilder.Entity<User>()
                    .Property(u => u.Active)
                    .HasConversion<string>()
                    .IsRequired()
                    .HasDefaultValue(StatusUser.Active);
        
        modelBuilder.Entity<User>()
                    .Property(u => u.CreatedAt)
                    .HasColumnType("timestamp")
                    .HasDefaultValue("NOW()");
        
        modelBuilder.Entity<User>()
                    .Property(u => u.UpdatedAt)
                    .HasColumnType("timestamp")
                    .HasDefaultValue("NOW()");
    }
}
