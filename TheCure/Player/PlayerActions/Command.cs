using System;
using Microsoft.Xna.Framework;

namespace TheCure.PlayerActions
{
    public class Command : PlayerAction
    {
        public Command(string iconName) : base(iconName)
        {
            CoolDown = 5f;
        }

        protected override void OnExecute(GameTime gameTime, GameManager gameManager)
        {
            Console.WriteLine("Command");
        }
    }
}