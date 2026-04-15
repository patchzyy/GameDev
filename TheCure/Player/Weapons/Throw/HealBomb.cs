using Microsoft.Xna.Framework;

namespace TheCure.Weapons.Throw;

public class HealBomb : Throwable
{
    public HealBomb(Vector2 position, Vector2 target, string textureName) : base(position, target, textureName,
        Color.Green)
    {
    }

    public override void OnImpact()
    {
        HealBombExplosion hbe = new HealBombExplosion(_targetPosition);
        GameManager.GetGameManager().AddGameObject(hbe);
        base.OnImpact();
    }
}

class HealBombExplosion : GameObject
{
    private CircleCollider _collider;
    private float _timer = 0;

    private int healsToGive = 5;

    public HealBombExplosion(Vector2 position)
    {
        _collider = new CircleCollider(position, 50);
        SetCollider(_collider);
    }

    public override void OnCollision(GameObject other)
    {
        if (other is Friendly || other is Zombie)
        {
            if (healsToGive > 0)
            {
                other.GainHealth(1);
                healsToGive--;
            }
            else
            {
                Destroy();
            }
        }
    }
}