using Microsoft.Xna.Framework;
using TheCure.Engine.Managers;

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
            Point mousePosition = InputManager.Get().CurrentMouseState.Position;
            Vector2 worldMousePosition = GameManager.Get().ScreenToWorld(mousePosition.ToVector2());

            CommandManager.Get().ActivateFriendlyCommand(worldMousePosition, CommandDuration, HoldDuration);
        }
    }
}