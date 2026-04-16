using System;
using Microsoft.Xna.Framework;

namespace TheCure.Weapons.Throw;

public class ThrowWeapon : BaseWeapon
{
    private ThrowWeapons _currentThrowWeapon;

    public void Aim(Vector2 start, Vector2 aim)
    {
        Vector2 distance = aim - start;
    }

    public override float FireRate => 1f;

    public override void Fire(Vector2 position, Vector2 direction)
    {
        switch (_currentThrowWeapon)
        {
            case ThrowWeapons.HealBomb:
                HealBomb healBomb = new HealBomb(position, direction, "Bullet");
                GameManager.GetGameManager().AddGameObject(healBomb);
                Console.WriteLine($"Fired HealBomb from {position} in direction {direction}");
                break;
        }

        ResetCoolDown();
    }

    public void SetCurrentThrowWeapon(ThrowWeapons weapon)
    {
        _currentThrowWeapon = weapon;
    }
}