// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Common;

namespace MrBoom.Screens
{
    public class SinglePlayerStartScreen : AbstractStartScreen
    {
        private readonly NameGenerator nameGenerator;

        public SinglePlayerStartScreen(Assets assets, List<Team> teams,
                                       List<IController> controllers, Settings settings)
            : base(assets, teams, controllers, settings)
        {
            nameGenerator = new NameGenerator(Terrain.Random);
        }

        protected override IPlayerState CreatePlayer(int index, IController controller)
        {
            return new HumanPlayerState(controller, index, nameGenerator.GenerateName());
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw(SpriteBatch ctx)
        {
            base.Draw(ctx);
        }
    }
}
