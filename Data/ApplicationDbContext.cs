using BooksOnLoan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BooksOnLoan.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        #region "Seed Data"

        builder.Entity<IdentityRole>().HasData(
            new { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new { Id = "2", Name = "Member", NormalizedName = "MEMBER" }
        );

        builder.Entity<Book>().ToTable("Book");
        builder.Entity<Transaction>().ToTable("Transaction");
        builder.Entity<TransactionType>().ToTable("TransactionType");
        
        // data will be added here.
        builder.Entity<Book>().HasData(SampleData.GetBooks());
        builder.Entity<TransactionType>().HasData(SampleData.GetTransactionTypes());

        #endregion
    }

    public DbSet<Book>? Inventory { get; set; }
    public DbSet<TransactionType>? TransactionTypes { get; set; }
    public DbSet<Transaction>? Transactions { get; set; }
}
