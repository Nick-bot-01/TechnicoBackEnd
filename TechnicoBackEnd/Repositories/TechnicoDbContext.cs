using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicoBackEnd.Models;

namespace TechnicoBackEnd.Repositories;

public class TechnicoDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Repair> Repairs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = "Data Source=(local);Initial Catalog=Technico; Integrated Security = True;TrustServerCertificate=True;";
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder){

        modelBuilder.Entity<User>().HasData(
                    new User()
                    {
                        Id = -10,
                        Name = "Mike",
                        Surname = "Tyson",
                        Email = "m.tyson@aol.com",
                        Phone = "3423423423",
                        Address = "p0k3b4ll 43",
                        Password = "Str0ngP4ssw0rd",
                        VATNum = "M1K3_TYS0N",
                    },
                    new User()
                    {
                        Id = -9,
                        Name = "Mikri",
                        Surname = "Gorgona",
                        Email = "ariel_onlyfans@gmail.com",
                        Phone = "34545343423",
                        Address = "zei sti thalassa se anana 85",
                        Password = "NotFound",
                        VATNum = "Sp0ng3B0b",
                    }

            );

        base.OnModelCreating(modelBuilder);
    }
}
