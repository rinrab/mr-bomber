// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Service;

namespace MrBoom.Core.Terrain.Cheats
{
    public class CheatHost
    {
        private readonly BomberServiceProvider services;

        public CheatHost(BomberServiceProvider services)
        {
            this.services = services;

            services.AddSingleton<DetonateAll>();
            services.AddSingleton<ClearAll>();
            services.AddSingleton<StartApocalypseCheat>();
            services.AddSingleton<GiveAllCheat>();
        }

        private IServerCheatModule GetCheat(int code)
        {
            foreach (var cheat in services.EnumerateServices<IServerCheatModule>())
            {
                if (cheat.GetCheatCode() == code)
                {
                    return cheat;
                }
            }

            return null;
        }

        public bool ApplyCheat(int code)
        {
            var cheat = GetCheat(code);

            if (cheat == null)
            {
                return false;
            }
            else
            {
                cheat.ApplyCheat();
                return true;
            }
        }
    }
}
