// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.AspNetCore.Mvc.RazorPages;
using MrBoom.Server.Lobby;
using MrBoom.Server.MatchMaking;

namespace MrBoom.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSystemd();

            builder.Services.AddSingleton<IUdpServer, UdpServer>();
            builder.Services.AddHostedService(serviceProvider => (UdpServer)serviceProvider.GetRequiredService<IUdpServer>());

            builder.Services.AddSingleton<ILobbyProvider, LobbyServer>();
            builder.Services.AddHostedService(serviceProvider => (LobbyServer)serviceProvider.GetRequiredService<ILobbyProvider>());

            builder.Services.AddSingleton<IMatchMakingProvider, MatchMakingService>();
            builder.Services.AddHostedService(serviceProvider => (MatchMakingService)serviceProvider.GetRequiredService<IMatchMakingProvider>());

            builder.Services.AddControllers();

            builder.Services.AddRazorPages(options =>
            {
                options.RootDirectory = "/Admin";
            });

            builder.Services.AddSingleton<IMetrics, Metrics>();

            var app = builder.Build();

            app.MapControllers();

            app.MapRazorPages();

            app.Run();
        }
    }
}
