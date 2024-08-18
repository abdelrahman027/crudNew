using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class PersonsDbContext:DbContext
    {
        public PersonsDbContext(DbContextOptions options):base(options) 
        {

        }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> Persons { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            string CountriesJson = System.IO.File.ReadAllText("countries.json");

            List<Country> countries =System.Text.Json.JsonSerializer.Deserialize<List<Country>>(CountriesJson);

            foreach (Country country in countries)
            {

            modelBuilder.Entity<Country>().HasData(country);
            }

            string PersonsJson = System.IO.File.ReadAllText("persons.json");

            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(PersonsJson);

            foreach (var person in persons)
            {

                modelBuilder.Entity<Person>().HasData(person);
            };

            //fluent apai
            modelBuilder.Entity<Person>().Property(temp => temp.TIN)
                .HasColumnName("TaxIdentficationNummber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC12345");
        }
        public List<Person> sp_GetAllPersons()
        {
          return  Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }
    }
}
