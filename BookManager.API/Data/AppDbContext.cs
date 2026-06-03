using BookManager.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookManager.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50).HasColumnName("name");
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150).HasColumnName("email");
            entity.Property(e => e.Password).IsRequired().HasColumnName("password");
            
            entity.HasIndex(e => e.Email).IsUnique();
        });


        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("books");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100).HasColumnName("title");
            entity.Property(e => e.Author).IsRequired().HasMaxLength(50).HasColumnName("author");
            entity.Property(e => e.PublishDate).IsRequired().HasColumnName("publish_date");
            entity.Property(e => e.UserId).IsRequired().HasColumnName("user_id");


            entity.HasOne(d => d.User)
                .WithMany(p => p.Books)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                ; 
        });
    }
}