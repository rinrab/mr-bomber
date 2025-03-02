// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrBoom.NetworkProtocol;
using MrBoom.Server.Lobby;

namespace MrBoom.Server.Master
{
    [Route("api/v1/master/join")]
    [ApiController]
    public class MasterServerController : ControllerBase
    {
        private ILogger<MasterServerController> logger;
        private readonly ILobbyProvider lobbyProvider;

        public MasterServerController(ILogger<MasterServerController> logger, ILobbyProvider lobbyProvider)
        {
            this.logger = logger;
            this.lobbyProvider = lobbyProvider;
        }

        [HttpPost]
        public ClientJoinResponse Post([FromBody] ClientJoinRequest req)
        {
            //var endpoint = new IPEndPoint(Request.HttpContext.Connection.RemoteIpAddress!,
            //                              Request.HttpContext.Connection.RemotePort);

            // var clientInfo = lobby.ClientJoin(req, endpoint);

            return new ClientJoinResponse
            {
                //ClientSecret = clientInfo.ClientSecret,
                ClientSecret = Guid.NewGuid(),
                LobbyIp = "lobby01._mrboomserver.test.mrbomber.online",
                LobbyPort = 5297,
                LobbyId = lobbyProvider.AssignLobby(),
            };
        }
    }
}
