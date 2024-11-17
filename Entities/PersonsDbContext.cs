using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Entities
{
    public class PersonsDbContext : DbContext
    {
        public PersonsDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            string countriesJson = File.ReadAllText("countries.json");
            List<Country>? countries = JsonSerializer.Deserialize<List<Country>>(countriesJson);

            foreach (var country in countries ?? [])
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            string personsJson = File.ReadAllText("persons.json");
            List<Person>? persons = JsonSerializer.Deserialize<List<Person>>(personsJson);

            foreach (var person in persons ?? [])
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

            modelBuilder.Entity<Person>().Property(person => person.TIN)
            .HasColumnName("TaxIdentificationNumber")
            .HasColumnType("varchar(8)")
            .HasDefaultValue("ABC12345");

            // modelBuilder.Entity<Person>().HasIndex(person => person.TIN).IsUnique();

        }

        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = [
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters),
            ];
            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPersons] @PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @Address, @CountryID, @ReceiveNewsLetters", parameters);
        }
    }
}