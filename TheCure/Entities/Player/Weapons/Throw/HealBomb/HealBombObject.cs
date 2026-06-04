using Microsoft.Xna.Framework;

namespace TheCure.Weapons.Throw;

public class HealBombObject : Throwable
{
    private float healingAmount;
    private int radius;
    private int ticks;

    public HealBombObject(float healingAmount, int radius, int ticks, Vector2 position, Vector2 target,
        string textureName) : base(position, target, textureName,
        Color.Green)
    {
        this.healingAmount = healingAmount;
        this.radius = radius;
        this.ticks = ticks;
    }

    public override void OnImpact()
    {
        HealBombExplosion hbe = new HealBombExplosion(healingAmount, radius, ticks, _targetPosition);
        GameManager.Get().AddGameObject(hbe);
        base.OnImpact();
    }
}

class HealBombExplosion : GameObject
{
    private CircleCollider _collider;

    private int healsToGive;
    private float healingAmount;

    public HealBombExplosion(float healingAmount, int radius, int ticks, Vector2 position)
    {
        this.healingAmount = healingAmount;
        healsToGive = ticks;
        _collider = new CircleCollider(position, radius);
        SetCollider(_collider);
    }

    public override void OnCollision(GameObject other)
    {
        if (other is Friendly || other is Zombie)
        {
            if (healsToGive > 0)
            {
                other.GainHealth(healingAmount);
                healsToGive--;
            }
            else
            {
                Destroy();
            }
        }
    }
}