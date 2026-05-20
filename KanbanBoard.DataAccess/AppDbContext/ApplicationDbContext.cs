using KanbanBoard.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace KanbanBoard.DataAccess.AppDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {

        }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Column> Columns { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<BoardUser> BoardUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Column>()
                .HasIndex(c => new { c.BoardId, c.Order });

            modelBuilder.Entity<Card>()
                .HasIndex(c => new { c.ColumnId, c.Order });

            modelBuilder.Entity<BoardUser>()
                .HasIndex(bu => bu.ConnectionId);
        }
    }
}
