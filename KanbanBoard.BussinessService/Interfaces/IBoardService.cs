using KanbanBoard.BussinessService.DTO;
using KanbanBoard.DataAccess.Model;

namespace KanbanBoard.BussinessService.Interfaces
{
    public interface IBoardService
    {
        Task<BoardDto> GetBoardAsync (int boardId);
        Task<List<BoardDto>> GetAllBoardsAsync();
        Task<BoardDto> CreateBoardAsync(CreateBoardDto createBoardDto);
        Task<bool> DeleteBoardAsync(int boardId);

        Task<ColumnDto> CreateColumnAsync(CreateColumnDto createColumnDto);
        Task<bool> DeleteColumnAsync(int columnId);


        Task<CardDto>CreateCardAsync(CreateCardDto createCardDto);
        Task<CardDto>UpdateCardAsync(CardDto cardDto);
        Task<bool>DeleteCardAsync(int cardId);
        Task<CardDto> MoveCardAsync(MoveCardDto moveCardDto);

        Task AddBoardUserAsync(int boardId, string username, string connectionId);
        Task RemoveBoardUserAsync(string connectionId);
        Task<List<string>> GetOnlineUsersAsync(int boardId);
        Task<BoardUser>GetBoardUserByConnectionIdAsync(string connectionId);
    }
}
