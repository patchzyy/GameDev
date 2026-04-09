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
        base.OnImpact();
    }
}