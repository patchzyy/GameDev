using Microsoft.Xna.Framework;

namespace TheCure.PlayerActions
{
    public class Command : PlayerAction
    {
        private const float CommandDuration = 3f;
        private const float HoldDuration = 2f;

        public Command(string iconName) : base(iconName)
        {
            CoolDown = 12f;
        }

        protected override void OnExecute(GameTime gameTime)
        {
            Point mousePosition = gameManager.InputManager.CurrentMouseState.Position;
            Vector2 worldMousePosition = gameManager.ScreenToWorld(mousePosition.ToVector2());

            gameManager.ActivateFriendlyCommand(worldMousePosition, CommandDuration, HoldDuration);
        }
    }
}
