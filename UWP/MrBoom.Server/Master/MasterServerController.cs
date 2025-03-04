// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrBoom.NetworkProtocol;
using MrBoom.Server.Lobby;
using MrBoom.Server.MatchMaking;

namespace MrBoom.Server.Master
{
    [Route("api/v1/master/join")]
    [ApiController]
    public class MasterServerController : ControllerBase
    {
        private ILogger<MasterServerController> logger;
        private readonly IMatchMakingProvider matchMakingProvider;

        public MasterServerController(ILogger<MasterServerController> logger,
                                      IMatchMakingProvider matchMakingProvider)
        {
            this.logger = logger;
            this.matchMakingProvider = matchMakingProvider;
        }

        [HttpPost]
        public async Task<ClientJoinResponse> PostAsync([FromBody] ClientJoinRequest req)
        {
            //var endpoint = new IPEndPoint(Request.HttpContext.Connection.RemoteIpAddress!,
            //                              Request.HttpContext.Connection.RemotePort);

            // var clientInfo = lobby.ClientJoin(req, endpoint);

            Guid lobby = await matchMakingProvider.AssignLobbyAsync(default);

            return new ClientJoinResponse
            {
                //ClientSecret = clientInfo.ClientSecret,
                ClientSecret = Guid.NewGuid(),
                LobbyIp = "eu.mrbomber.online",
                LobbyPort = 5297,
                LobbyId = lobby,
            };
        }
    }
}
