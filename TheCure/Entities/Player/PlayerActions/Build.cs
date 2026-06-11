using System;
using Microsoft.Xna.Framework;
using TheCure.BaseObjects.Traps;
using TheCure.Managers;

namespace TheCure.PlayerActions;

public class Build : PlayerAction
{
    private const float TrapPlacementDistance = 80f;
    private readonly TrapType _trapType;
    private readonly float _trapDuration;

    public FreezeTrapStats FreezeTrapStats { get; set; } = new FreezeTrapStats();
    public ElectricTrapStats ElectricTrapStats { get; set; } = new ElectricTrapStats();
    public HealBombStats HealBombStats { get; set; } = new HealBombStats();
    public BombTrapStats BombTrapStats { get; set; } = new BombTrapStats();
    public SpikeTrapStats SpikeTrapStats { get; set; } = new SpikeTrapStats();

    public Build(string iconName, TrapType trapType, float trapDuration) : base(iconName)
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

        Trap trap;
        switch (_trapType)
        {
            case TrapType.Bomb:
                trap = new BombTrap(BombTrapStats, trapPosition, _trapDuration);
                break;
            case TrapType.Freeze:
                trap = new FreezeTrap(FreezeTrapStats, trapPosition, _trapDuration);
                break;
            case TrapType.HealBomb:
                trap = new HealBombTrap(HealBombStats, trapPosition, _trapDuration);
                break;
            case TrapType.Spike:
                trap = new SpikeTrap(SpikeTrapStats, trapPosition, _trapDuration);
                break;
            case TrapType.Electric:
                trap = new ElectricTrap(ElectricTrapStats, trapPosition, _trapDuration);
                break;
            default:
                return;
        }

        GameManager.Get().AddGameObject(trap);

        System.Diagnostics.Debug.WriteLine($"Built {trap.GetType().Name} at position {trapPosition}");
    }
}