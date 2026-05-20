using KanbanBoard.BussinessService.DTO;
using KanbanBoard.BussinessService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KanbanBoard.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : ControllerBase
    {

        private readonly IBoardService _boardService;

        public BoardController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BoardDto>>> GetAllBoards()
        {
            
            
            var boards = await _boardService.GetAllBoardsAsync();
            return Ok(boards);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BoardDto>> GetBoard(int id)
        {
            var board = await _boardService.GetBoardAsync(id);
            if (board == null)
            {
                return NotFound();
            }
            return Ok(board);
        }

        [HttpPost]
        public async Task<ActionResult<BoardDto>> CreateBoard([FromBody] CreateBoardDto createBoardDto)
        {
            if (string.IsNullOrWhiteSpace(createBoardDto.Name))
            {
                return BadRequest("Board name is required");
            }
            var newBoard = await _boardService.CreateBoardAsync(createBoardDto);
            return Ok(newBoard);
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteBoard(int id)
        {
            var success = await _boardService.DeleteBoardAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("columns")]
        public async Task<ActionResult<ColumnDto>> CreateColumn([FromBody] CreateColumnDto createColumnDto)
        {
            var newColumn = await _boardService.CreateColumnAsync(createColumnDto);
            return Ok(newColumn);
        }

        [HttpPost("cards")]
        public async Task<ActionResult<CardDto>> CreateCard([FromBody] CreateCardDto createCardDto)
        {
            var newCard = await _boardService.CreateCardAsync(createCardDto);
            return Ok(newCard);
        }
    }
}
