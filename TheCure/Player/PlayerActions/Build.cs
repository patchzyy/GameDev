using System;
using Microsoft.Xna.Framework;

namespace TheCure.PlayerActions;

public class Build : PlayerAction
{
    public Build(string iconName) : base(iconName)
    {
        CoolDown = 15f;
    }

    protected override void OnExecute(GameTime gameTime)
    {
        Console.WriteLine("Build");
    }
}