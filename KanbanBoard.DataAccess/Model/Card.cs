using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanBoard.DataAccess.Model
{
    public class Card
    {
        public int CardId { get; set; }

        [ForeignKey("Column")]
        public int ColumnId { get; set; }
        public Column Column { get; set; }


        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }

        [MaxLength(100)]
        public string AssignedTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    }
}
