using System;
using TheCure.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TheCure
{
    public abstract class GameObject
    {
        protected Collider collider;
        protected HealthBar _healthBar;

        public void SetCollider(Collider collider)
        {
            this.collider = collider;
        }

        public virtual void Load(ContentManager content)
        {
        }

        public virtual void HandleInput(InputManager inputManager)
        {
        }

        public bool CheckCollision(GameObject other)
        {
            if (collider == null)
                return false;

            return collider.CheckIntersection(other.collider);
        }

        public virtual void OnCollision(GameObject other)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            if (_healthBar != null)
            {
                _healthBar.UpdateHealthBar(collider.GetBoundingBox().Center, collider.GetBoundingBox().Height);
            }
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
            GameManager.GetGameManager().RemoveGameObject(this);
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

        public virtual void LoseHealth(int amount)
        {
            if (_healthBar != null)
            {
                _healthBar.DecreaseHealth(amount);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No healthbar set");
            }
        }

        public void GainHealth(int amount)
        {
            if (_healthBar != null)
            {
                _healthBar.IncreaseHealth(amount);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No healthbar set");
            }
        }

        public void ResetHealth()
        {
            if (_healthBar != null)
            {
                _healthBar.ResetHealth();
            }
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