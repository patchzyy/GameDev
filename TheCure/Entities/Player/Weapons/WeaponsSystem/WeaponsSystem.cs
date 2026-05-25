using System;
using Microsoft.Xna.Framework;
using TheCure.Managers;
using TheCure.Weapons.Throw;

namespace TheCure.Weapons;

public class WeaponsSystem
{
    public WeaponMode CurrentWeaponMode;

    public BaseWeapon CurrentWeapon { get; set; }

    private SingleBulletWeapon _singleBulletWeapon = new SingleBulletWeapon();

    private HealBomb _healBombWeapon = new HealBomb();

    private MovementWeapon _movementWeapon = new MovementWeapon();

    private InstantWeapon _instantWeapon = new InstantWeapon();

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
        _healBombWeapon = new HealBomb();
        _instantWeapon = new InstantWeapon();
        _movementWeapon = new MovementWeapon();
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
            case ShootWeapons.Movement:
                Console.WriteLine("Movement weapon selected");
                CurrentWeapon = _movementWeapon;
                break;
            case ShootWeapons.Instant:
                Console.WriteLine("Instant weapon selected");
                CurrentWeapon = _instantWeapon;
                break;
        }
    }

    public void SetThrowWeapon(ThrowWeapons weapon)
    {
        CurrentWeaponMode = WeaponMode.Throw;
        switch (weapon)
        {
            case ThrowWeapons.HealBomb:
                Console.WriteLine("Heal bomb weapon selected");
                CurrentWeapon = _healBombWeapon;
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
            {
                CurrentWeapon.Fire(position, worldMousePosition);
            }

            if (CurrentWeaponMode ==
                WeaponMode.Shoot)
            {
                Vector2 aimDirection =
                    LinePieceCollider.GetDirection(position, worldMousePosition);
                CurrentWeapon.Fire(position, aimDirection);
            }
        }
    }
}