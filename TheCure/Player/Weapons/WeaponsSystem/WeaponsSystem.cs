using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Weapons.Throw;

namespace TheCure.Weapons;

public class WeaponsSystem
{
    private WeaponMode _currentWeaponMode;

    private BaseWeapon CurrentWeapon { get; set; }

    private readonly ThrowWeapon _throwWeapon = new ThrowWeapon();

    private readonly SingleBulletWeapon _singleBulletWeapon = new SingleBulletWeapon();

    public WeaponsSystem()
    {
        SetShootWeapon(ShootWeapons.SingleBullet);
    }

    public void Update(GameTime gameTime)
    {
        CurrentWeapon?.UpdateCoolDown(gameTime);
    }

    public void SetShootWeapon(ShootWeapons weapon)
    {
        _currentWeaponMode = WeaponMode.Shoot;
        switch (weapon)
        {
            case ShootWeapons.SingleBullet:
                CurrentWeapon = _singleBulletWeapon;
                break;
        }
    }

    public void SetThrowWeapon(ThrowWeapons weapon)
    {
        _currentWeaponMode = WeaponMode.Throw;
        switch (weapon)
        {
            case ThrowWeapons.HealBomb:
                _throwWeapon.SetCurrentThrowWeapon(ThrowWeapons.HealBomb);
                CurrentWeapon = _throwWeapon;
                break;
        }
    }

    public void Fire(InputManager inputManager)
    {
        Point mousePosition = inputManager.CurrentMouseState.Position;
        var gameManager = GameManager.GetGameManager();
        Vector2 worldMousePosition = gameManager.ScreenToWorld(mousePosition.ToVector2());

        if (CurrentWeapon != null && CurrentWeapon.CanFire)
        {
            Vector2 position = gameManager.Player.GetPosition().Center.ToVector2();

            if (_currentWeaponMode == WeaponMode.Throw)
            {
                CurrentWeapon.Fire(position, worldMousePosition);
            }

            if (_currentWeaponMode == WeaponMode.Shoot)
            {
                Vector2 aimDirection =
                    LinePieceCollider.GetDirection(position, worldMousePosition);
                CurrentWeapon.Fire(position, aimDirection);
            }
        }
    }
}