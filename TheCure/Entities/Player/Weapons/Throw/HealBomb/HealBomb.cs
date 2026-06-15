using Microsoft.Xna.Framework;
using TheCure.Managers;
using TheCure.PlayerActions;

namespace TheCure.Weapons.Throw;

public class HealBomb : PlayerAction
{
    private const float FireRate = 5f;

    public float HealingAmount;
    public int Radius;
    public int Ticks;

    public HealBomb(float healingAmount, int radius, int ticks) : base("Throw")
    {
        HealingAmount = healingAmount;
        Radius = radius;
        Ticks = ticks;
        CoolDown = FireRate;
    }

    protected override void OnExecute(GameTime gameTime)
    {
        var gameManager = GameManager.Get();
        Point mousePosition = InputManager.Get().CurrentMouseState.Position;
        var position = PlayerManager.Get().Player.GetPosition();
        Vector2 worldMousePosition = gameManager.ScreenToWorld(mousePosition.ToVector2());

        HealBombObject healBombObject = new HealBombObject(HealingAmount, Radius, Ticks, position, worldMousePosition, "Bullet");

        gameManager.AddGameObject(healBombObject);
    }

    public void UpgradeHealingAmount(float amount)
    {
        HealingAmount += amount;
    }

    public void UpgradeRadius(int amount)
    {
        Radius += amount;
    }

    public void UpgradeTicks(int amount)
    {
        Ticks += amount;
    }
}