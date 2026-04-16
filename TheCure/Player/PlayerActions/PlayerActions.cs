using Microsoft.Xna.Framework;

namespace TheCure.PlayerActions
{
    public abstract class PlayerAction
    {
        protected float Cooldown { get; set; }
        private float _remainingCooldown;

        public float GetRemainingCooldown() => _remainingCooldown;

        public virtual void Update(GameTime gameTime)
        {
            _remainingCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            _remainingCooldown = _remainingCooldown < 0f ? 0f : _remainingCooldown;
        }

        public void ResetCooldown()
        {
            _remainingCooldown = 0f;
        }

        public void Execute(GameTime gameTime, GameManager gameManager)
        {
            if (_remainingCooldown > 0f)
                return;

            _remainingCooldown = Cooldown;
            OnExecute(gameTime, gameManager);
        }

        protected abstract void OnExecute(GameTime gameTime, GameManager gameManager);
    }
}