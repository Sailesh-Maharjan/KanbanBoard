using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanBoard.DataAccess.Model
{
    public class Column
    {
        public  int ColumnId { get; set; }

        [ForeignKey("Board")]
        public int BoardId { get; set; }
        public Board Board { get; set; }

        [Required]
        [MaxLength(200)]
        public  string Title { get; set; }
        public int Order { get; set; }

        public ICollection<Card> Cards { get; set; }    
    }
}
