using Microsoft.EntityFrameworkCore;
using MimicAPI2.Models;

namespace MimicAPI2.DataBase
{
    public class MimicContext : DbContext
    {
        public MimicContext(DbContextOptions<MimicContext> options) : base(options)
        {

        }

        public DbSet<Palavra> Palavras { get; set; }
    }
}