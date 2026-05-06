using Microsoft.Xna.Framework;
using TheCure.Weapons;
using TheCure.Weapons.Throw;

namespace TheCure.PlayerActions;

public class Throw : PlayerAction
{
    private BaseWeapon _bomb;

    public Throw(string iconName, ThrowWeapons weapon) : base(iconName)
    {
        switch (weapon)
        {
            case ThrowWeapons.HealBomb:
                _bomb = new HealBomb();
                break;
        }

        CoolDown = _bomb.FireRate;
    }


    protected override void OnExecute(GameTime gameTime, GameManager gameManager)
    {
        Point mousePosition = gameManager.InputManager.CurrentMouseState.Position;
        var position = gameManager.Player.GetPosition().Center.ToVector2();
        Vector2 worldMousePosition = gameManager.ScreenToWorld(mousePosition.ToVector2());

        _bomb.Fire(position, worldMousePosition);
    }
}