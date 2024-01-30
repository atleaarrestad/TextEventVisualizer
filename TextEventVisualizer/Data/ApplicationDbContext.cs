﻿using Microsoft.EntityFrameworkCore;
using TextEventVisualizer.Models;

namespace TextEventVisualizer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
    }
}
