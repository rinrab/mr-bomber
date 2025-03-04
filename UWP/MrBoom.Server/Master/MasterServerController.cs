// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        private readonly Settings settings;

        public MasterServerController(ILogger<MasterServerController> logger,
                                      IMatchMakingProvider matchMakingProvider,
                                      IOptions<Settings> options)
        {
            this.logger = logger;
            this.matchMakingProvider = matchMakingProvider;
            settings = options.Value;
        }

        [HttpPost]
        public async Task<ClientJoinResponse> PostAsync([FromBody] ClientJoinRequest req)
        {
            //var endpoint = new IPEndPoint(Request.HttpContext.Connection.RemoteIpAddress!,
            //                              Request.HttpContext.Connection.RemotePort);

            // var clientInfo = lobby.ClientJoin(req, endpoint);

            Guid clientId = Guid.NewGuid();
            Guid lobby = await matchMakingProvider.AssignLobbyAsync(clientId, default);

            return new ClientJoinResponse
            {
                //ClientSecret = clientInfo.ClientSecret,
                ClientSecret = clientId,
                LobbyIp = settings.LobbyIp,
                LobbyPort = settings.LobbyPort,
                LobbyId = lobby,
            };
        }
    }
}
