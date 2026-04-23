using Microsoft.Xna.Framework;
using TheCure.Managers;
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


    protected override void OnExecute(GameTime gameTime)
    {
        var gameManager = GameManager.Get();
        Point mousePosition = InputManager.Get().CurrentMouseState.Position;
        var position = PlayerManager.Get().Player.GetPosition().Center.ToVector2();
        Vector2 worldMousePosition = gameManager.ScreenToWorld(mousePosition.ToVector2());

        _bomb.Fire(position, worldMousePosition);
    }
}