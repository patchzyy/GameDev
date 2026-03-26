using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TheCure.Mobs
{
    public class Mob : GameObject
    {
        public CircleCollider _collider;
        public string _textureName;
        public Texture2D _texture;
        public SpriteFont _font;
        public float _speed;
        public int _maxHealth;
        public int _startHealth;

        public Mob(string textureName, float speed, int startHealth, int maxHealth)
        {
            _textureName = textureName;
            _speed = speed;
            _maxHealth = maxHealth;
            _startHealth = startHealth;
        }

        public override void Load(ContentManager content)
        {
            _font = content.Load<SpriteFont>("HudFont");
            _texture = content.Load<Texture2D>(_textureName);
            _collider = new CircleCollider(Vector2.Zero, _texture.Width / 2);

            SetCollider(_collider);
        }
    }
}