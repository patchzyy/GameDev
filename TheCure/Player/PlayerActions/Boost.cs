using System;
using Microsoft.Xna.Framework;

namespace TheCure.PlayerActions;

public class Boost : PlayerAction
{
    public Boost(string iconName) : base(iconName)
    {
        CoolDown = 10f;
    }


    protected override void OnExecute(GameTime gameTime, GameManager gameManager)
    {
        Console.WriteLine("Boost");
    }
}