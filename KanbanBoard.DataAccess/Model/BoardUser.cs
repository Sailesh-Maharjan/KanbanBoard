using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanBoard.DataAccess.Model
{
    public class BoardUser
    {
        public int BoardUserId { get; set; }

        [ForeignKey("Board")]
        public int BoardId { get; set; }
        public Board Board { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [MaxLength(200)]
        public string ConnectionId { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public bool IsOnline { get; set; } = true;

    }
}
