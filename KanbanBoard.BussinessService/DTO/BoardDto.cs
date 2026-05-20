namespace KanbanBoard.BussinessService.DTO
{
    public class BoardDto
    {
       public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } 
        public List<ColumnDto> Columns { get; set; }
        public List<string> OnlineUsers { get; set; }
    }

    public class ColumnDto
    {
        public int Id { get; set; }
        public int  BoardId { get; set; }
        public string  Title { get; set; }
        public int Order { get; set; }
        public List<CardDto> Cards { get; set; }
    }

    public class CardDto
    {
        public int Id { get; set; }
        public int ColumnId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }

        public string AssignedTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    public class CreateBoardDto
    {      
        public string Name { get; set; }
    }

    public class CreateColumnDto
    {
        public int BoardId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
    }

    public class CreateCardDto
        {
            public int ColumnId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string AssignedTo { get; set; }
    }

    public class  MoveCardDto
    {
        public int CardId { get; set; }
        public int NewColumnId { get; set; }
        public int NewOrder { get; set; }
    }
}
