using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Collision;
using TheCure.Managers;

namespace TheCure
{
    public abstract class GameObject
    {
        public Collider collider;
        protected AnimatedSprite _animatedSprite;
        protected HealthBar _healthBar;
        protected float _flashTimer = 0f;
        protected const float _flashDuration = 0.15f;
        protected Color _flashColor;
        protected bool _isFlashing => _flashTimer > 0f;
        private float _aliveTimer = 0f;

        public float LastHealed;

        public void SetCollider(Collider collider)
        {
            this.collider = collider;
        }

        public virtual void Load()
        {
        }

        public virtual void HandleInput()
        {
        }

        public bool CheckCollision(GameObject other)
        {
            if (collider == null || other.collider == null)
                return false;

            return collider.CheckIntersection(other.collider);
        }

        public virtual void OnCollision(GameObject other)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            if (_aliveTimer < 2f)
                _aliveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (_isFlashing)
                _flashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_healthBar != null && collider != null)
                _healthBar.UpdateHealthBar(collider.GetBoundingBox().Center, collider.GetBoundingBox().Height);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_healthBar != null)
            {
                _healthBar.Draw(spriteBatch);
            }
        }

        public virtual void Destroy()
        {
            GameManager.Get().RemoveGameObject(this);
        }

        protected void SwitchAnimation(string name, int frames, float fps, bool loop, bool reverse = false)
        {
            var texture = ContentsManager.Get().GetContent().Load<Texture2D>(name);
            int frameWidth = texture.Width / frames;

            _animatedSprite = new AnimatedSprite(texture, frameWidth, texture.Height, frames, fps, loop, reverse);
        }

        public void SetHealthBar(Texture2D texture, float maxHealth, float startHealth, Action onDeath,
            Action onMaxHealth, bool hide = false)
        {
            _healthBar = new HealthBar(texture, maxHealth, startHealth, onDeath, onMaxHealth, hide);
        }

        public void SetHealthBar(Texture2D texture, float maxHealth, float startHealth, Action onDeath)
        {
            SetHealthBar(texture, maxHealth, startHealth, onDeath, null);
        }

        public void SetHealthBar(Texture2D texture, float maxHealth, float startHealth, Action onDeath, bool hide)
        {
            SetHealthBar(texture, maxHealth, startHealth, onDeath, null, hide);
        }

        protected void SyncHealthBarPosition()
        {
            if (_healthBar != null && collider != null)
                _healthBar.UpdateHealthBar(collider.GetBoundingBox().Center, collider.GetBoundingBox().Height);
        }

        public virtual void LoseHealth(float amount)
        {
            if (_healthBar != null)
            {
                if (_aliveTimer < 2f)
                {
                    return;
                }

                _healthBar.DecreaseHealth(amount);

                _flashTimer = _flashDuration;
                _flashColor = Color.Red * 0.9f;
            }
            else
                System.Diagnostics.Debug.WriteLine("No health bar set");
        }

        public void GainHealth(float amount)
        {
            if (_healthBar != null)
            {
                _healthBar.IncreaseHealth(amount);
                SoundManager.Get().PlayHeal();
                LastHealed = 0f;

                _flashTimer = _flashDuration;
                _flashColor = Color.Green * 0.9f;
            }
            else
                System.Diagnostics.Debug.WriteLine("No health bar set");
        }

        public void ResetHealth()
        {
            if (_healthBar != null)
                _healthBar.ResetHealth();
        }

        public float CurrentHealth()
        {
            return _healthBar?.CurrentHealth ?? 0;
        }

        public Collider GetCollider()
        {
            return collider;
        }
    }
}