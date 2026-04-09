using System;
using Microsoft.Xna.Framework;

namespace TheCure.PlayerActions
{
    public class Command : PlayerAction
    {
        public Command()
        {
            Cooldown = 5f;
        }

        protected override void OnExecute(GameTime gameTime, GameManager gameManager)
        {
            Console.WriteLine("Command");
        }
    }
}