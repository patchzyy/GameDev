using TheCure.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TheCure
{
    public abstract class GameObject
    {
        protected Collider collider;

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
            {
                return false;
            }

            return collider.CheckIntersection(other.collider);
        }

        public virtual void OnCollision(GameObject other)
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }

        public virtual void Destroy()
        {

        }
    }
}
