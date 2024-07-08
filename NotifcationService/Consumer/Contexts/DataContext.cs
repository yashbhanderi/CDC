using Consumer.Models;
using Microsoft.EntityFrameworkCore;

namespace Consumer.Contexts;

public class DataContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>().ToTable("Person");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(Common.Global.Constants.connString);
    }
}