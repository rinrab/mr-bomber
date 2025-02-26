// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrBoom.Common;

namespace MrBoom.Server
{
    [Route("api/v1/game")]
    [ApiController]
    public class JoinController : ControllerBase
    {
        private readonly IGameLobby lobby;

        public JoinController(IGameLobby lobby)
        {
            this.lobby = lobby;
        }

        [HttpPost]
        public PlayerInfo Post([FromBody] PlayerJoinInfo player)
        {
            return lobby.PlayerJoin(player);
        }

        [HttpGet]
        public LobbyInfo Get()
        {
            return lobby.GetLobbyInfo();
        }
    }
}
