using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Collision;

namespace TheCure.Mobs;

public class Mob : GameObject
{
    public CircleCollider _collider;
    public Texture2D _texture;
    public AnimatedSprite _animatedSprite;
    public SpriteFont _font;
    public float _speed;
    public int _maxHealth;
    public int _startHealth;
    protected readonly string _textureName;
    protected readonly int _frameCount;
    protected readonly float _frameRate;
    protected readonly bool _isLooping;
    protected readonly float _scale;

    public Mob(string textureName, float speed, int startHealth, int maxHealth, int frameCount = 1,
        float frameRate = 1f, bool isLooping = true, float scale = 1f)
    {
        _textureName = textureName;
        _speed = speed;
        _maxHealth = maxHealth;
        _startHealth = startHealth;
        _frameCount = frameCount;
        _frameRate = frameRate;
        _isLooping = isLooping;
        _scale = scale;
    }

    public override void Load(ContentManager content)
    {
        _texture = content.Load<Texture2D>(_textureName);
        int frameWidth = _texture.Width / _frameCount;
        _animatedSprite = new AnimatedSprite(_texture, frameWidth, _texture.Height, _frameCount, _frameRate, _isLooping);
        _font = content.Load<SpriteFont>("HudFont");
        _collider = new CircleCollider(Vector2.Zero, _animatedSprite.FrameWidth * _scale / 2f);
        SetCollider(_collider);
    }

    public override void Update(GameTime gameTime)
    {
        _animatedSprite?.Update(gameTime);
        base.Update(gameTime);
    }

    protected Rectangle GetAnimatedSpriteDestinationRectangle()
    {
        int scaledWidth = (int)(_animatedSprite.FrameWidth * _scale);
        int scaledHeight = (int)(_animatedSprite.FrameHeight * _scale);

        return new Rectangle(
            (int)(_collider.X - scaledWidth / 2),
            (int)(_collider.Y - scaledHeight / 2),
            scaledWidth,
            scaledHeight
        );
    }

    protected void DrawAnimatedSprite(SpriteBatch spriteBatch, Color color)
    {
        _animatedSprite?.Draw(spriteBatch, GetAnimatedSpriteDestinationRectangle(), color);
    }
}
