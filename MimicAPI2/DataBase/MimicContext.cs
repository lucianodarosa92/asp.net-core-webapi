using Microsoft.EntityFrameworkCore;
using MimicAPI.V1.Models;

namespace MimicAPI.DataBase
{
    public class MimicContext : DbContext
    {
        public MimicContext(DbContextOptions<MimicContext> options) : base(options)
        {

        }

        public DbSet<Palavra> Palavras { get; set; }
    }
}