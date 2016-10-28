namespace NancyApplication
{
    using Nancy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using MySQL.Data.EntityFrameworkCore.Extensions;
    using System;
    using System.IO;

    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            string hoge = null;

            Console.WriteLine(hoge?.ToString() + Hoge());

            var currentDir = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .AddJsonFile(currentDir + "/Configs/appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();

            string connectionString = configuration.GetConnectionString("SampleConnection");

            // Create an employee instance and save the entity to the database
            var entry = new Employee() { Name = "John", LastName = "Winston" };

            using (var context = EmployeesContextFactory.Create(connectionString))
            {
                context.Add(entry);
                context.SaveChanges();
            }

            Console.WriteLine($"Employee was saved in the database with id: {entry.Id}");

            Get("/", _ => "Hello World");
        }

        int Hoge() => 10;
    }

    /// <summary>
    /// The entity framework context with a Employees DbSet
    /// </summary>
    public class EmployeesContext : DbContext
    {
        public EmployeesContext(DbContextOptions<EmployeesContext> options)
        : base(options)
        { }

        public DbSet<Employee> Employees { get; set; }
    }

    /// <summary>
    /// Factory class for EmployeesContext
    /// </summary>
    public static class EmployeesContextFactory
    {
        public static EmployeesContext Create(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EmployeesContext>();
            optionsBuilder.UseMySQL(connectionString);

            //Ensure database creation
            var context = new EmployeesContext(optionsBuilder.Options);
            context.Database.EnsureCreated();

            return context;
        }
    }

    /// <summary>
    /// A basic class for an Employee
    /// </summary>
    public class Employee
    {
        public Employee()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }
    }
}
