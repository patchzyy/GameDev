using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

        public void Load(ContentManager content)
        {
            _iconTexture = content.Load<Texture2D>(_iconName);
        }

        public Texture2D GetIconTexture() => _iconTexture;

        public float GetRemainingCoolDown() => _remainingCoolDown;

        public virtual void Update(GameTime gameTime)
        {
            _remainingCoolDown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            _remainingCoolDown = _remainingCoolDown < 0f ? 0f : _remainingCoolDown;
        }

        public void ResetCoolDown()
        {
            _remainingCoolDown = 0f;
        }

        public void Execute(GameTime gameTime, GameManager gameManager)
        {
            if (_remainingCoolDown > 0f)
                return;

            _remainingCoolDown = CoolDown;
            OnExecute(gameTime, gameManager);
        }

        protected abstract void OnExecute(GameTime gameTime, GameManager gameManager);
    }
}