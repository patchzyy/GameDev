#nullable enable
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheCure
{
    public class HealthBar
    {
        private float _maxHealth;
        private float _startHealth;
        private float _currentHealth;
        private Action _onDeath;
        private Action? _onMaxHealth;
        private bool _hideHealthBar;

        private Vector2 _barPosition;
        private int _objectHeight;

        public Texture2D _texture;
        public bool IsMaxHealth => _currentHealth >= _maxHealth;

        public float CurrentHealth => _currentHealth;

        public HealthBar(Texture2D texture, float maxHealth, float startHealth, Action onDeath, Action? onMaxHealth, bool hide = false)
        {
            _maxHealth = maxHealth;
            _startHealth = startHealth;
            _currentHealth = startHealth;
            _onDeath = onDeath;
            _onMaxHealth = onMaxHealth;
            _texture = texture;
            _hideHealthBar = hide;
            _objectHeight = 0;
        }

        public void IncreaseHealth(int health)
        {
            _currentHealth += health;

            if (_currentHealth >= _maxHealth)
            {
                _currentHealth = _maxHealth;

                if (_onMaxHealth != null)
                {
                    _onMaxHealth.Invoke();
                }
            }
        }

        public void DecreaseHealth(int health)
        {
            _currentHealth -= health;

            if (_currentHealth <= 0)
            {
                _onDeath.Invoke();
            }
        }

        public void ResetHealth()
        {
            _currentHealth = _startHealth;
        }

        public void UpdateHealthBar(Point position, int objectHeight)
        {
            _barPosition = new Vector2(position.X, position.Y);
            _objectHeight = objectHeight;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_hideHealthBar)
                return;

            var dummyTexture = GameManager.GetGameManager().DummyTexture;

            if (_texture == null)
                return;

            int objectHeight = Math.Max(1, _objectHeight);
            int barWidth = Math.Clamp(objectHeight * 2, 44, 72);
            int barHeight = 10;
            int xOffset = barWidth / 2;
            int yOffset = (objectHeight / 2) + barHeight + 4;

            int drawX = (int)_barPosition.X - xOffset;
            int drawY = (int)_barPosition.Y - yOffset;

            var borderRectangle = new Rectangle(drawX - 1, drawY - 1, barWidth + 2, barHeight + 2);
            var backgroundRectangle = new Rectangle(drawX, drawY, barWidth, barHeight);
            var fillRect = new Rectangle(drawX + 1, drawY + 1, Math.Max(1, (int)((barWidth - 2) * (_currentHealth / _maxHealth))), barHeight - 2);

            spriteBatch.Draw(dummyTexture, borderRectangle, Color.Black * 0.9f);
            spriteBatch.Draw(dummyTexture, backgroundRectangle, new Color(30, 30, 30, 220));

            double healthRatio = _currentHealth / (double)_maxHealth;
            Color healthColor = Color.Lerp(Color.Red, Color.Green, (float)healthRatio);

            spriteBatch.Draw(dummyTexture, fillRect, healthColor);
        }
    }
}