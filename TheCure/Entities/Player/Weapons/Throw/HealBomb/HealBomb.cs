using Microsoft.Xna.Framework;
using TheCure.Managers;
using TheCure.PlayerActions;

namespace TheCure.Weapons.Throw;

public class HealBomb : PlayerAction
{
    private const float FireRate = 5f;
    
    public float _healingAmount;
    public int _radius;
    public int _ticks;
    
    public HealBomb(float healingAmount, int radius, int ticks) : base("Throw")
    {
        _healingAmount = healingAmount;
        _radius = radius;
        _ticks = ticks;
        CoolDown = FireRate;
    }

    protected override void OnExecute(GameTime gameTime)
    {
        var gameManager = GameManager.Get();
        Point mousePosition = InputManager.Get().CurrentMouseState.Position;
        var position = PlayerManager.Get().Player.GetPosition();
        Vector2 worldMousePosition = gameManager.ScreenToWorld(mousePosition.ToVector2());

        HealBombObject healBombObject =
            new HealBombObject(_healingAmount, _radius, _ticks, position, worldMousePosition, "Bullet");
        gameManager.AddGameObject(healBombObject);
    }
    
    public void UpgradeHealingAmount(float amount)
    {
        _healingAmount += amount;
    }
    
    public void UpgradeRadius(int amount)
    {
        _radius += amount;
    }
    
    public void UpgradeTicks(int amount)
    {
        _ticks += amount;
    }
    
}