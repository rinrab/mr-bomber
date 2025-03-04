// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Threading.Tasks.Dataflow;

namespace MrBoom.Server.MatchMaking
{
    internal class MatchMakingRequest
    {
        private WriteOnceBlock<Guid> request;
        public Guid ClientId { get; }

        public MatchMakingRequest(Guid clientId)
        {
            request = new WriteOnceBlock<Guid>(null);
            ClientId = clientId;
        }

        public async Task<Guid> WaitForCompletionAsync(CancellationToken cancellationToken)
        {
            return await request.ReceiveAsync(cancellationToken);
        }

        public void CompleteTask(Guid response)
        {
            request.Post(response);
        }
    }

    public class MatchMaker : BackgroundService, IMatchMakingProvider
    {
        private readonly ILobbyProvider lobbyProvider;
        private readonly ILogger<MatchMaker> logger;
        private readonly BufferBlock<MatchMakingRequest> matchMakingQueue;

        public MatchMaker(ILobbyProvider lobbyProvider, ILogger<MatchMaker> logger)
        {
            this.lobbyProvider = lobbyProvider;
            this.logger = logger;
            matchMakingQueue = new BufferBlock<MatchMakingRequest>();
        }

        public async Task<Guid> AssignLobbyAsync(Guid clientId, CancellationToken cancellationToken)
        {
            var request = new MatchMakingRequest(clientId);

            matchMakingQueue.Post(request);
            logger.LogInformation("Scheduling matchmaking request for client {clientId}...", clientId);

            return await request.WaitForCompletionAsync(cancellationToken);
        }

        private async Task<Guid> assignLobbyInternal(CancellationToken cancellationToken)
        {
            foreach (var lobby in lobbyProvider.EnumerateLobbies())
            {
                if (!lobby.IsFull)
                {
                    return lobby.Key;
                }
            }

            return lobbyProvider.CreateLobby();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var request = await matchMakingQueue.ReceiveAsync(stoppingToken);

                var lobby = await assignLobbyInternal(stoppingToken);
                logger.LogInformation("Client {clientId} assigned to lobby {lobby}",
                                      request.ClientId, lobby);

                request.CompleteTask(lobby);
            }
        }
    }
}
