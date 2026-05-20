using KanbanBoard.BussinessService.DTO;
using KanbanBoard.BussinessService.Interfaces;
using KanbanBoard.DataAccess.AppDbContext;
using KanbanBoard.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace KanbanBoard.BussinessService.Services
{
    public class BoardService : IBoardService
    {
        private readonly ApplicationDbContext _context;

        public BoardService(ApplicationDbContext context)
        {
            _context = context;
        }

        private static BoardDto MapBoardToDto(Board board)
        {
            return new BoardDto
            {
                Id = board.BoardId,
                Name = board.BoardName,
                CreatedAt = board.CreatedAt,

                Columns = (board.Columns ?? new List<Column>())
                    .OrderBy(c => c.Order)
                    .Select(c => new ColumnDto
                    {
                        Id = c.ColumnId,
                        BoardId = c.BoardId,
                        Title = c.Title,
                        Order = c.Order,

                        Cards = (c.Cards ?? new List<Card>())
                            .OrderBy(card => card.Order)
                            .Select(MapCardToDto)
                            .ToList()
                    })
                    .ToList(),

                OnlineUsers = (board.BoardUsers ?? new List<BoardUser>())
                    .Where(bu => bu.IsOnline)
                    .Select(bu => bu.Username)
                    .Distinct()
                    .ToList()
            };
        }

        public static CardDto MapCardToDto(Card card)
        {
            return new CardDto
            {
                Id = card.CardId,
                ColumnId = card.ColumnId,
                Title = card.Title,
                Description = card.Description,
                AssignedTo = card.AssignedTo,
                Order = card.Order,
                CreatedAt = card.CreatedAt,
                ModifiedAt = card.ModifiedAt
            };
        }

        public async Task AddBoardUserAsync(int boardId, string username, string connectionId)
        {
            var existingBoardUsers = await _context.BoardUsers.Where(bu => bu.Username == username && bu.BoardId == boardId).ToListAsync();
            foreach (var item in existingBoardUsers)
            {
                item.IsOnline = false;
            }

            var bu = new BoardUser
            {
                BoardId = boardId,
                Username = username,
                ConnectionId = connectionId,
                IsOnline = true
            };

            _context.BoardUsers.Add(bu);
            await _context.SaveChangesAsync();
        }

        public async Task<BoardDto> CreateBoardAsync(CreateBoardDto createBoardDto)
        {
            var board = new Board
            {
                BoardName = createBoardDto.Name,
            };
            _context.Boards.Add(board);
            await _context.SaveChangesAsync();
            return MapBoardToDto(board);
        }
        public async Task<CardDto> CreateCardAsync(CreateCardDto createCardDto)
        {
            var maxOrder = await _context.Cards.Where(c => c.ColumnId == createCardDto.ColumnId).MaxAsync(c => (int?)c.Order) ?? -1;
            var card = new Card
            {
                ColumnId = createCardDto.ColumnId,
                Title = createCardDto.Title,
                Description = createCardDto.Description,
                AssignedTo = createCardDto.AssignedTo,
                Order = maxOrder + 1,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            return MapCardToDto(card);
        }

        public async Task<ColumnDto> CreateColumnAsync(CreateColumnDto createColumnDto)
        {
            var maxOrder = await _context.Columns.Where(c => c.BoardId == createColumnDto.BoardId).MaxAsync(c => (int?)c.Order) ?? -1;
            var column = new Column
            {
                BoardId = createColumnDto.BoardId,
                Title = createColumnDto.Title,
                Order = maxOrder + 1
            };
            _context.Columns.Add(column);
            await _context.SaveChangesAsync();

            return new ColumnDto
            {
                Id = column.ColumnId,
                BoardId = column.BoardId,
                Title = column.Title,
                Order = column.Order
            };
        }


        public async Task<bool> DeleteBoardAsync(int boardId)
        {
            var board = await _context.Boards.FindAsync(boardId);
            if (board == null)
                return false;
            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCardAsync(int cardId)
        {
            var card = await _context.Cards.FindAsync(cardId);
            if (card == null)
            {
                return false;
            }
            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteColumnAsync(int columnId)
        {
            var column = await _context.Columns.FindAsync(columnId);
            if (column == null)
            {
                return false;
            }
            _context.Columns.Remove(column);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<BoardDto>> GetAllBoardsAsync()
        {
            var boards = await _context.Boards
                .Include(b => b.Columns)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return boards.Select(MapBoardToDto).ToList();
        }

        public async Task<BoardDto> GetBoardAsync(int boardId)
        {
            var board = await _context.Boards
                .Where(b => b.BoardId == boardId)
                .Select(b => new BoardDto
                {
                    Id = b.BoardId,
                    Name = b.BoardName,
                    CreatedAt = b.CreatedAt,

                    Columns = b.Columns
                        .OrderBy(c => c.Order)
                        .Select(c => new ColumnDto
                        {
                            Id = c.ColumnId,
                            BoardId = c.BoardId,
                            Title = c.Title,
                            Order = c.Order,

                            Cards = c.Cards
                                .OrderBy(card => card.Order)
                                .Select(card => new CardDto
                                {
                                    Id = card.CardId,
                                    ColumnId = card.ColumnId,
                                    Title = card.Title,
                                    Description = card.Description,
                                    Order = card.Order,
                                    AssignedTo = card.AssignedTo,
                                    CreatedAt = card.CreatedAt,
                                    ModifiedAt = card.ModifiedAt
                                }).ToList()
                        }).ToList(),

                    OnlineUsers = b.BoardUsers
                        .Where(bu => bu.IsOnline)
                        .Select(bu => bu.Username)
                        .Distinct()
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return board;
        }

        public async Task<BoardUser> GetBoardUserByConnectionIdAsync(string connectionId)
        {
            return await _context.BoardUsers.FirstOrDefaultAsync(bu => bu.ConnectionId == connectionId && bu.IsOnline);
        }

        public async Task<List<string>> GetOnlineUsersAsync(int boardId)
        {
            return await _context.BoardUsers
                 .Where(bu => bu.BoardId == boardId && bu.IsOnline)
                 .Select(bu => bu.Username)
                 .Distinct()
                 .ToListAsync();
        }

        public async Task<CardDto> MoveCardAsync(MoveCardDto moveCardDto)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var card = await _context.Cards.FindAsync(moveCardDto.CardId);
                    if (card == null)
                    {
                        return null;
                    }
                    var oldColumnId = card.ColumnId;
                    var oldOrder = card.Order;

                    // If moving to different column ,shofting orders in old column
                    if (oldColumnId != moveCardDto.NewColumnId)
                    {
                        var oldColumnCards = await _context.Cards.Where(c => c.ColumnId == oldColumnId && c.Order > oldOrder).ToListAsync();

                        foreach (var item in oldColumnCards)
                        {
                            item.Order -= 1;
                        }

                        //Make space in new column
                        var newColumnCards = await _context.Cards.Where(c => c.ColumnId == moveCardDto.NewColumnId && c.Order >= moveCardDto.NewOrder).ToListAsync();
                        foreach (var item in newColumnCards)
                        {
                            item.Order += 1;
                        }
                        card.ColumnId = moveCardDto.NewColumnId;
                        card.Order = moveCardDto.NewOrder;
                    }

                    else
                    {
                        // Same Column reorder
                        if (moveCardDto.NewOrder == oldOrder)
                        {
                            // No change
                        }
                        else if (moveCardDto.NewOrder > oldOrder) // case: card down in the same column
                        {
                            var between = await _context.Cards.Where(c => c.ColumnId == oldColumnId && c.Order > oldOrder && c.Order <= moveCardDto.NewOrder && c.CardId != card.CardId).ToListAsync();
                            foreach (var item in between)
                            {
                                item.Order -= 1;
                            }
                            card.Order = moveCardDto.NewOrder;
                        }
                        else // case: card up in the same column
                        {
                            var between = await _context.Cards.Where(c => c.ColumnId == oldColumnId && c.Order < oldOrder && c.Order >= moveCardDto.NewOrder && c.CardId != card.CardId).ToListAsync();
                            foreach (var item in between)
                            {
                                item.Order += 1;
                            }
                            card.Order = moveCardDto.NewOrder;
                        }
                    }
                    card.ModifiedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return MapCardToDto(card);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task RemoveBoardUserAsync(string connectionId)
        {
            var users = await _context.BoardUsers.Where(bu => bu.ConnectionId == connectionId && bu.IsOnline).ToListAsync();
            foreach (var item in users)
            {
                item.IsOnline = false;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<CardDto> UpdateCardAsync(CardDto cardDto)
        {
            var card = _context.Cards.Find(cardDto.Id);
            if (card == null)
            {
                return null;
            }
            card.Title = cardDto.Title;
            card.Description = cardDto.Description;
            card.AssignedTo = cardDto.AssignedTo;
            card.ModifiedAt = DateTime.UtcNow;
            _context.Cards.Update(card);
            await _context.SaveChangesAsync();
            return MapCardToDto(card);
        }
    }
}
