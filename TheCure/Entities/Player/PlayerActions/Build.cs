using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheCure.BaseObjects.Traps;
using TheCure.Managers;

namespace TheCure.PlayerActions;

public class Build : PlayerAction
{
    private int _trapIndex = 0;
    private const float TrapPlacementDistance = 80f;
    private List<Type> _availableTrapTypes = new List<Type> { typeof(SpikeTrap) }; // default
    private Dictionary<Type, float> _trapDurations = new Dictionary<Type, float>
    {
        { typeof(SpikeTrap), 8f },
        { typeof(FreezeTrap), 10f },
        { typeof(BombTrap), 12f },
        { typeof(ElectricTrap), 10f },
        { typeof(HealBombTrap), 15f }
    };

    public Build() : base("Build")
    {
        CoolDown = 5f;
    }

    public void AddTrapType(Type trapType)
    {
        if (!_availableTrapTypes.Contains(trapType))
        {
            _availableTrapTypes.Add(trapType);
        }
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

        if (_availableTrapTypes.Count == 0)
        {
            return; // no traps available
        }

        Type trapType = _availableTrapTypes[_trapIndex % _availableTrapTypes.Count];
        float duration = _trapDurations[trapType];
        BaseObjects.Traps.Trap trap = (BaseObjects.Traps.Trap)Activator.CreateInstance(trapType, new object[] { trapPosition, duration });

        _trapIndex++;

        GameManager.Get().AddGameObject(trap);

        System.Diagnostics.Debug.WriteLine($"Built {trap.GetType().Name} at position {trapPosition}");
    }
}