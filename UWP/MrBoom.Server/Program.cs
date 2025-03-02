// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Server.Lobby;

namespace MrBoom.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<IUdpServer, UdpServer>();
            builder.Services.AddHostedService(serviceProvider => (UdpServer)serviceProvider.GetRequiredService<IUdpServer>());

            builder.Services.AddHostedService<LobbyServer>();

            builder.Services.AddControllers();

            builder.Services.AddRazorPages(options =>
            {
                options.RootDirectory = "/Admin";
            });

            var app = builder.Build();

            app.MapControllers();

            app.MapRazorPages();

            app.Run();
        }
    }
}
