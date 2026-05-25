using System;
using Microsoft.Xna.Framework;
using TheCure.Managers;

namespace TheCure.PlayerActions;

public class Build : PlayerAction
{
    private const float TrapPlacementDistance = 80f;
    private readonly Type _trapType;
    private readonly float _trapDuration;

    public Build(string iconName, Type trapType, float trapDuration) : base(iconName)
    {
        _trapType = trapType;
        _trapDuration = trapDuration;
        CoolDown = 5f;
    }

    protected override void OnExecute(GameTime gameTime)
    {
        Player player = PlayerManager.Get().Player;
        if (player == null)
            return;

        Vector2 playerPos = player.GetPosition();

        Point mousePosition = InputManager.Get().CurrentMouseState.Position;
        Vector2 worldMousePosition = GameManager.Get().ScreenToWorld(mousePosition.ToVector2());

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

        BaseObjects.Traps.Trap trap = (BaseObjects.Traps.Trap)Activator.CreateInstance(_trapType, new object[] { trapPosition, _trapDuration });

        GameManager.Get().AddGameObject(trap);

        System.Diagnostics.Debug.WriteLine($"Built {trap.GetType().Name} at position {trapPosition}");
    }
}