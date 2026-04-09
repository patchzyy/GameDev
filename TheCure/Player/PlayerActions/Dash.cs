using System;
using Microsoft.Xna.Framework;

namespace TheCure.PlayerActions;

public class Dash : PlayerAction
{
    public Dash()
    {
        Cooldown = 10f;
    }

    protected override void OnExecute(GameTime gameTime, GameManager gameManager)
    {
        Console.WriteLine("Dash");
    }
}