using System;
using Microsoft.Xna.Framework;
using TheCure.BaseObjects.Traps;

namespace TheCure.PlayerActions;

public class Build : PlayerAction
{
    private int _trapIndex = 0;
    private const float TrapPlacementDistance = 80f;

    public Build()
    {
        CoolDown = 5f;
    }

    protected override void OnExecute(GameTime gameTime, GameManager gameManager)
    {
        Player player = gameManager.Player;
        if (player == null)
            return;

        Vector2 playerPos = player.GetPosition().Center.ToVector2();

        Point mousePosition = gameManager.InputManager.CurrentMouseState.Position;
        Vector2 worldMousePosition = gameManager.ScreenToWorld(mousePosition.ToVector2());

        Vector2 direction = worldMousePosition - playerPos;
        if (direction.LengthSquared() > 100)
        {
            direction.Normalize();
        }
        else
        {
            direction = new Vector2((float)Math.Cos(player._rotation), (float)Math.Sin(player._rotation));
        }

        Vector2 trapPosition = playerPos + direction * TrapPlacementDistance;

        BaseObjects.Traps.Trap trap = (_trapIndex % 5) switch
        {
            0 => new SpikeTrap(trapPosition),
            1 => new FreezeTrap(trapPosition),
            2 => new BombTrap(trapPosition),
            3 => new ElectricTrap(trapPosition),
            4 => new HealBombTrap(trapPosition),
            _ => new SpikeTrap(trapPosition)
        };

        _trapIndex++;

        gameManager.AddGameObject(trap);

        System.Diagnostics.Debug.WriteLine($"Built {trap.GetType().Name} at position {trapPosition}");
    }
}