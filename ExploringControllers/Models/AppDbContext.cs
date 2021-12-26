using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExploringControllers
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :
            base(options)
        { }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Activity> Activities { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>(eb=> {
                eb.HasKey(e => e.Id);
                eb.HasData(
                        new Person[]
                        {
                            new Person {Id = 1, FirstName = "George", LastName = "Washington", SysUser="system"},
                            new Person {Id = 2, FirstName = "John", LastName = "Adams", SysUser="system"},
                            new Person {Id = 3, FirstName = "Thomas", LastName = "Jefferson", SysUser="system"}
                        }
                    );
            });

            modelBuilder.Entity<Activity>(eb => {
                eb.HasKey(e => e.Id);
                eb.HasData(
                        new Activity[]
                        {
                            new Activity {Id = 1, Name = "Walk", SysUser="system"},
                            new Activity {Id = 2, Name = "Sleep", SysUser="system"},
                            new Activity {Id = 3, Name = "Talk", SysUser="system"},
                            new Activity {Id = 4, Name = "Govern", SysUser="system"}
                        }
                    );
            });


        }


    }
}
