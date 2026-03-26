using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheCure
{
    internal class Alien : GameObject
    {
        private CircleCollider _collider;
        private Texture2D _texture;
        private SpriteFont _font;
        private float _speed = 60f;
        private int _maxHealth = 5;
        private int _startHealth = 3;

        private bool _isFriendly = false;
        public bool IsFriendly => _isFriendly;
        private float _followDistance = 60f;

        public override void Load(ContentManager content)
        {
            base.Load(content);

            _texture = content.Load<Texture2D>("Alien");
            _font = content.Load<SpriteFont>("TitleFont");

            SetHealthBar(_texture, _maxHealth, _startHealth, BecomeFriendly, null);

            _collider = new CircleCollider(Vector2.Zero, _texture.Width / 2);
            SetCollider(_collider);

            RandomMove();
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 playerPosition = GameManager.GetGameManager().Player.GetPosition().Center.ToVector2();
            Vector2 direction = playerPosition - _collider.Center;
            float distance = Vector2.Distance(_collider.Center, playerPosition);

            if (_isFriendly)
            {
                if (distance > _followDistance)
                {
                    direction.Normalize();
                    _collider.Center += direction * (_speed + 20f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
            else
            {
                direction.Normalize();
                _collider.Center += direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (distance < 40)
                {
                    GameManager.GetGameManager().Player.TakeDamage(20f);
                    RandomMove();
                }
            }

            base.Update(gameTime);
        }

            public void BecomeFriendly()
            {
                if (!_isFriendly)
                {
                    _isFriendly = true;
                    _speed = 80f;

                    _healthBar?.IncreaseHealth(_maxHealth);
                }
            }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Alien otherAlien)
            {
                if (this._isFriendly && !otherAlien.IsFriendly)
                {
                    otherAlien.LoseHealth(1);

                    Vector2 pushDir = otherAlien._collider.Center - this._collider.Center;

                    pushDir.Normalize();
                    otherAlien._collider.Center += pushDir * 5;
                }
            }

            if (!_isFriendly && (tmp is Bullet || tmp is Laser))
            {
                LoseHealth(1);
            }

            base.OnCollision(tmp);
        }

        public void RandomMove()
        {
            GameManager game = GameManager.GetGameManager();
            _collider.Center = game.RandomLocationOutsideView();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tint = _isFriendly ? Color.LightBlue : Color.White;
            spriteBatch.Draw(_texture, _collider.GetBoundingBox(), tint);

            if (_isFriendly && _font != null)
            {
                string text = "Friendly";
                Vector2 textSize = _font.MeasureString(text);
                Vector2 textPos = new Vector2(_collider.Center.X - (textSize.X / 2), _collider.Center.Y - (_texture.Height / 2) - 20);
                spriteBatch.DrawString(_font, text, textPos, Color.LimeGreen);
            }

            base.Draw(gameTime, spriteBatch);
        }
    }
}