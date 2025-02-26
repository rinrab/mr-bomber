// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrBoom.NetworkProtocol;

namespace MrBoom.Server
{
    [Route("api/v1/master/join")]
    [ApiController]
    public class MasterServerController : ControllerBase
    {
        private ILogger<MasterServerController> logger;
        private readonly IGameLobby lobby;

        public MasterServerController(ILogger<MasterServerController> logger,
                                      IGameLobby lobby)
        {
            this.logger = logger;
            this.lobby = lobby;
        }

        [HttpPost]
        public ClientJoinResponse Post([FromBody] ClientJoinRequest req)
        {
            var endpoint = new IPEndPoint(Request.HttpContext.Connection.RemoteIpAddress!,
                                          Request.HttpContext.Connection.RemotePort);

            var clientInfo = lobby.ClientJoin(req, endpoint);

            return new ClientJoinResponse
            {
                ClientSecret = clientInfo.ClientSecret,
                LobbyIp = "lobby01._mrboomserver.test.mrbomber.online",
            };
        }
    }
}
