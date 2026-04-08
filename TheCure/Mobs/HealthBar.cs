#nullable disable
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

        public HealthBar(Texture2D texture, int maxHealth, int startHealth, Action onDeath, Action? onMaxHealth,
            bool hide = false)
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
            if (_hideHealthBar) return;

            var dummyTexture = GameManager.GetGameManager().DummyTexture;

            if (_texture == null) return;

            int barWidth = 50;
            int barHeight = 15;

            int xOffset = barWidth / 2;
            int yOffset = (_objectHeight / 2) + barHeight + 5;

            int drawX = (int)_barPosition.X - xOffset;
            int drawY = (int)_barPosition.Y - yOffset;

            spriteBatch.Draw(dummyTexture, new Rectangle(drawX, drawY, barWidth, barHeight),
                Color.Gray);

            double healthRatio = _currentHealth / (double)_maxHealth;

            // scale colour based on ratio from 0-red to 1-green
            Color healthColor = Color.Lerp(Color.Red, Color.Green, (float)healthRatio);

            spriteBatch.Draw(dummyTexture,
                new Rectangle(drawX, drawY, (int)(barWidth * healthRatio), barHeight),
                healthColor);
        }
    }
}