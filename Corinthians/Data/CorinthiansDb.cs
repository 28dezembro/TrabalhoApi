using Corinthians.Models;
using Microsoft.EntityFrameworkCore;

namespace Corinthians.Data
{
    public class CorinthiansDb : DbContext
    {
        public DbSet<Jogador> Jogadores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("DataSource=corinthians.sqlite;Cache=Shared");

    }
}
