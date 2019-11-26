using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using qbService.Models;

namespace qbService.Models
{
    public class ApplicationDbContex : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContex(DbContextOptions<ApplicationDbContex> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(x => x.Tareas)
                .WithOne(x => x.ApplicationUser);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<qbService.Models.Tarea> Tareas { get; set; }

    }
}
