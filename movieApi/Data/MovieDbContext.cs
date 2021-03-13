using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using movieApi.Models;

namespace movieApi.Data
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
        {

        }

        // These properties determine the names of the tables that will be created in my database
        public DbSet<Movie> Movies { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<Reservation> Reservations { get; set; }
    }
}
