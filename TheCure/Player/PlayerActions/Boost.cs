using System;
using Microsoft.Xna.Framework;

namespace TheCure.PlayerActions;

public class Boost : PlayerAction
{
    public Boost()
    {
        Cooldown = 10f;
    }


    protected override void OnExecute(GameTime gameTime, GameManager gameManager)
    {
        Console.WriteLine("Boost");
    }
}