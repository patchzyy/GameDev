using Microsoft.Xna.Framework;

namespace TheCure.PlayerActions
{
    public abstract class PlayerAction
    {
        public float Cooldown { get; protected set; }
        private float _lastUsedTime = -100f;

        public float GetRemainingCooldown(GameManager gameManager)
        {
            float currentTime = gameManager.GetGameTime();
            ;
            float remaining = Cooldown - (currentTime - _lastUsedTime);
            return remaining < 0f ? 0f : remaining;
        }

        public bool CanExecute(GameTime gameTime)
        {
            float currentTime = (float)gameTime.TotalGameTime.TotalSeconds;
            return currentTime - _lastUsedTime >= Cooldown;
        }

        public void Execute(GameTime gameTime, GameManager gameManager)
        {
            if (!CanExecute(gameTime))
                return;

            _lastUsedTime = (float)gameTime.TotalGameTime.TotalSeconds;
            OnExecute(gameTime, gameManager);
        }

        protected abstract void OnExecute(GameTime gameTime, GameManager gameManager);
    }
}