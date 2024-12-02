using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Lab2MPA.Models;
using LibraryModel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Lab2MPA.Data
{
    public class Lab2MPAContext : IdentityDbContext<IdentityUser>
    {
        public Lab2MPAContext(DbContextOptions<Lab2MPAContext> options) : base(options)
        {
        }

        public DbSet<Lab2MPA.Models.Book> Book { get; set; } = default!;
        //public DbSet<Lab2MPA.Models.Customer> Customer { get; set; } = default!;
        public DbSet<Lab2MPA.Models.Genre> Genre { get; set; } = default!;
        public DbSet<Lab2MPA.Models.Order> Order { get; set; } = default!;
        public DbSet<Lab2MPA.Models.Author> Author { get; set; } = default!;
        public DbSet<City> City { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<IdentityUser> User { get; set; } = default!;
        public DbSet<Publisher> Publisher { get; set; } = default!;
        public DbSet<PublishedBook> PublishedBook { get; set; } = default!;
    }
}

