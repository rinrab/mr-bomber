// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Threading.Tasks.Dataflow;

namespace MrBoom.Server.MatchMaking
{
    internal class MatchMakingRequest
    {
        private WriteOnceBlock<Guid> request;

        public MatchMakingRequest()
        {
            request = new WriteOnceBlock<Guid>(null);
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
        private readonly BufferBlock<MatchMakingRequest> matchMakingQueue;

        public MatchMaker(ILobbyProvider lobbyProvider)
        {
            this.lobbyProvider = lobbyProvider;

            matchMakingQueue = new BufferBlock<MatchMakingRequest>();
        }

        public async Task<Guid> AssignLobbyAsync(CancellationToken cancellationToken)
        {
            var request = new MatchMakingRequest();

            matchMakingQueue.Post(request);

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

                request.CompleteTask(await assignLobbyInternal(stoppingToken));
            }
        }
    }
}
