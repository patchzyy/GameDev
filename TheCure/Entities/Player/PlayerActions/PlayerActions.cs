using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Managers;

namespace TheCure.PlayerActions
{
    public abstract class PlayerAction
    {
        protected float CoolDown { get; set; }
        private float _remainingCoolDown;
        public string _iconName;
        public Texture2D _iconTexture;


        public PlayerAction(string iconName)
        {
            _iconName = iconName;
        }

        public virtual void Load()
        {
            var content = ContentsManager.Get().GetContent();
            _iconTexture = content.Load<Texture2D>(_iconName);
        }

        public Texture2D GetIconTexture() => _iconTexture;

        public virtual float GetRemainingCoolDown() => _remainingCoolDown;

        public virtual void Update(GameTime gameTime)
        {
            if (GameManager.Get().CurrentGameState != GameState.Playing)
                return;
            _remainingCoolDown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            _remainingCoolDown = _remainingCoolDown < 0f ? 0f : _remainingCoolDown;
        }

        public void ResetCoolDown()
        {
            _remainingCoolDown = 0f;
        }

        public virtual void Execute(GameTime gameTime)
        {
            if (GameManager.Get().CurrentGameState != GameState.Playing)
                return;

            if (_remainingCoolDown > 0f)
                return;

            _remainingCoolDown = CoolDown;
            OnExecute(gameTime);
        }

        protected abstract void OnExecute(GameTime gameTime);
    }
}