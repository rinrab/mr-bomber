// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<IGameLobby, GameLobby>();

            builder.Services.AddSingleton<IUdpServer, UdpServer>();
            builder.Services.AddHostedService(serviceProvider => (UdpServer)serviceProvider.GetRequiredService<IUdpServer>());

            builder.Services.AddControllers();

            var app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}
