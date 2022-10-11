using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSXDownloader.MVVM.Models
{
    public class PSXDataContext : DbContext
    {
        public DbSet<PSXDatabase>? PSXDatabases { get; set; }

        public PSXDataContext()
        {
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!Directory.Exists("Database"))
            {
                Directory.CreateDirectory("Database");
            }
            optionsBuilder.UseSqlite(@"Data Source=Database\\PSXDatabase.db");
        }
    }
}
