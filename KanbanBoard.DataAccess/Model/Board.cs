using System.ComponentModel.DataAnnotations;

namespace KanbanBoard.DataAccess.Model
{
    public class Board
    {
        public int BoardId { get; set; }
        [Required]
        [MaxLength(200)]
        public string BoardName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //Navigation properties
        public ICollection<Column> Columns { get; set; }
        public ICollection<BoardUser> BoardUsers { get; set; }
    }
}
