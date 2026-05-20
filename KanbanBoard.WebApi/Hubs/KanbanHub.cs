using KanbanBoard.BussinessService.DTO;
using KanbanBoard.BussinessService.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace KanbanBoard.WebApi.Hubs
{
    public class KanbanHub : Hub
    {
        private readonly IBoardService _boardService;

        public KanbanHub(IBoardService boardService)
        {
            _boardService = boardService;
        }

        private static string GetGroupName(int boardId)
        {
            return $"Board-{boardId}";
        }

        // ===== User joins a board room =====
        public async Task JoinBoard(int boardId, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new HubException("Username cannot be empty");
            }

            var groupName = GetGroupName(boardId);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await _boardService.AddBoardUserAsync(boardId, userName, Context.ConnectionId);

            //send the full board state to the  joining client only
            var board = await _boardService.GetBoardAsync(boardId);
            await Clients.Caller.SendAsync("BoardLoaded", board);

            //Notify others in the group
            await Clients.OthersInGroup(groupName).SendAsync("UserJoined", userName);

            //Broadcast online users to entire group
            var onlineUsers = await _boardService.GetOnlineUsersAsync(boardId);
            await Clients.Group(groupName).SendAsync("OnlineUsersUpdated", onlineUsers);
        }

        public async Task LeaveBoard(int boardId)
        {
            var groupName = GetGroupName(boardId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            var user = await _boardService.GetBoardUserByConnectionIdAsync(Context.ConnectionId);
            if (user != null)
            {
                await _boardService.RemoveBoardUserAsync(Context.ConnectionId);
                await Clients.Group(groupName).SendAsync("UserLeft", user.Username);
            }

            var onlineUsers = await _boardService.GetOnlineUsersAsync(boardId);
            await Clients.Group(groupName).SendAsync("OnlineUsersUpdated", onlineUsers);
        }

        // ===== Card moved =====
        public async Task MoveCard(int boardId, MoveCardDto moveCardDto)
        {
            var moved = await _boardService.MoveCardAsync(moveCardDto);
            if (moved == null)
            {
                return;
            }

            await Clients.Group(GetGroupName(boardId)).SendAsync("CardMoved", moved);
        }

        //====Card created ====
        public async Task CreateCard(int boardId, CreateCardDto createCardDto)
        {
            var created = await _boardService.CreateCardAsync(createCardDto);
            await Clients.Group(GetGroupName(boardId)).SendAsync("CardCreated", created);
        }

        // ===== Card updated =====
        public async Task UpdateCard(int boardId, CardDto card)
        {
            var updatedCard = await _boardService.UpdateCardAsync(card);
            if (updatedCard == null)
            {
                return;
            }
            await Clients.Group(GetGroupName(boardId)).SendAsync("CardUpdated", updatedCard);
        }

        // ===== Card deleted =====
        public async Task DeleteCard(int boardId, int cardId)
        {
            var deleted = await _boardService.DeleteCardAsync(cardId);
            if (!deleted)
            {
                return;
            }
            await Clients.Group(GetGroupName(boardId)).SendAsync("CardDeleted", cardId);

        }

        //===== Column created =====
        public async Task CreateColumn(int boardId, CreateColumnDto createColumnDto)
        {
            createColumnDto.BoardId = boardId;
            var newColumn = await _boardService.CreateColumnAsync(createColumnDto);
            await Clients.Group(GetGroupName(boardId)).SendAsync("ColumnCreated", newColumn);
        }

        //===== Column deleted =====
        public async Task DeleteColumn(int boardId, int columnId)
        {
            var deleted = await _boardService.DeleteColumnAsync(columnId);
            if (!deleted)
            {
                return;
            }
            await Clients.Group(GetGroupName(boardId)).SendAsync("ColumnDeleted", columnId);
        }

        //===== Typing/activity indicator =====

        public async Task SendTypingIndicator(int boardId, string userName, int? cardId)
        {
            await Clients.OthersInGroup(GetGroupName(boardId)).SendAsync("UserTyping", new { UserName = userName, CardId = cardId });
        }


        //====connection lifecycle ====
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var boardUser = await _boardService.GetBoardUserByConnectionIdAsync(Context.ConnectionId);
                if(boardUser != null)
                {
                    var groupName = GetGroupName(boardUser.BoardId);
                   await _boardService.RemoveBoardUserAsync(Context.ConnectionId);

                    await Clients.Group(groupName).SendAsync("UserLeft", boardUser.Username);
                    var onlineUsers = await _boardService.GetOnlineUsersAsync(boardUser .BoardId);
                    await Clients.Group(groupName).SendAsync("OnlineUsersUpdated", onlineUsers);
                }
            }
           catch(Exception ex)
            {
                Console.WriteLine($"Error in OnDisconnectedAsync: {ex.Message}");
            }

            await base.OnDisconnectedAsync(exception);
        }

    }
}
