using System;
using Microsoft.Xna.Framework;
using TheCure.Managers;

namespace TheCure.Weapons;

public class WeaponsSystem
{
    public WeaponMode CurrentWeaponMode;

    public BaseWeapon CurrentWeapon { get; set; }

    private SingleBulletWeapon _singleBulletWeapon = new SingleBulletWeapon();

    public WeaponsSystem()
    {
        SetShootWeapon(ShootWeapons.SingleBullet);
    }

    public void Reset()
    {
        SetShootWeapon(ShootWeapons.SingleBullet);
    }

    public void Reload()
    {
        _singleBulletWeapon = new SingleBulletWeapon();
    }

    public void Update(GameTime gameTime)
    {
        CurrentWeapon?.UpdateCoolDown(gameTime);
    }

    public void SetShootWeapon(ShootWeapons weapon)
    {
        CurrentWeaponMode = WeaponMode.Shoot;
        switch (weapon)
        {
            case ShootWeapons.SingleBullet:
                Console.WriteLine("Single bullet weapon selected");
                CurrentWeapon = _singleBulletWeapon;
                break;
        }
    }

    public float GetFireRate()
    {
        return CurrentWeapon.FireRate;
    }

    public void Fire()
    {
        var inputManager = InputManager.Get();
        Point mousePosition = inputManager.CurrentMouseState.Position;
        var gameManager = GameManager.Get();
        Vector2 worldMousePosition = gameManager.ScreenToWorld(mousePosition.ToVector2());

        if (CurrentWeapon != null && CurrentWeapon.CanFire)
        {
            Vector2 position = PlayerManager.Get().Player.GetPosition();

            if (CurrentWeaponMode == WeaponMode.Throw)
                CurrentWeapon.Fire(position, worldMousePosition);

            if (CurrentWeaponMode == WeaponMode.Shoot)
            {
                Vector2 aimDirection = LinePieceCollider.GetDirection(position, worldMousePosition);
                CurrentWeapon.Fire(position, aimDirection);
            }
        }
    }
}