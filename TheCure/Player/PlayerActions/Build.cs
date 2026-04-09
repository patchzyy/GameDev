using System;
using Microsoft.Xna.Framework;

namespace TheCure.PlayerActions;

public class Build : PlayerAction
{
    public Build()
    {
        Cooldown = 15f;
    }

    protected override void OnExecute(GameTime gameTime, GameManager gameManager)
    {
        Console.WriteLine("Build");
    }
}