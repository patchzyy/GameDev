using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheCure.Weapons.Throw;

public class HealBombObject : Throwable
{
    public HealBombObject(Vector2 position, Vector2 target, string textureName) : base(position, target, textureName,
        Color.Green)
    {
    }

    public override void OnImpact()
    {
        HealBombExplosion hbe = new HealBombExplosion(_targetPosition);
        GameManager.Get().AddGameObject(hbe);
        base.OnImpact();
    }
}

class HealBombExplosion : GameObject
{
    private CircleCollider _collider;

    private int healsToGive = 5;

    public HealBombExplosion(Vector2 position)
    {
        _collider = new CircleCollider(position, 50);
        SetCollider(_collider);
    }

    public override void Load()
    {
        SwitchAnimation("Bomb-explosion", 6, 12f, false);

        base.Load();
    }

    public override void Update(GameTime gameTime)
    {
        if (_animatedSprite != null && _animatedSprite.IsFinished)
        {
            Destroy();
        }

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        DrawAnimatedSprite(spriteBatch, Color.White, Vector2.UnitX, 0.6f);

        base.Draw(gameTime, spriteBatch);
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