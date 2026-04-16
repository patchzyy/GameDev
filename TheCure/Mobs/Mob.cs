using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Collision;

namespace TheCure.Mobs;

public class Mob : GameObject
{
    public CircleCollider _collider;
    public string _textureName;
    public Texture2D _texture;
    public AnimatedSprite _animatedSprite;
    public SpriteFont _font;
    public float _speed;
    public float _maxHealth;
    public float _startHealth;
    protected readonly int _frameCount;
    protected readonly float _frameRate;
    protected readonly bool _isLooping;
    protected readonly float _scale;

    protected Vector2 _knockBackVelocity = Vector2.Zero;
    protected float _knockBackDuration = 0f;

    public Mob(string textureName, float speed, float startHealth, float maxHealth, int frameCount = 1,
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

        _animatedSprite =
            new AnimatedSprite(_texture, frameWidth, _texture.Height, _frameCount, _frameRate, _isLooping);
        _font = content.Load<SpriteFont>("HudFont");
        _collider = new CircleCollider(Vector2.Zero, _animatedSprite.FrameWidth * _scale / 2f);

        SetCollider(_collider);
    }

    public override void Update(GameTime gameTime)
    {
        _animatedSprite?.Update(gameTime);

        base.Update(gameTime);
    }

    public void ApplyKnockBack(Vector2 direction, float force, float duration)
    {
        direction.Normalize();
        _knockBackVelocity = direction * force;
        _knockBackDuration = duration;
    }

    protected void UpdateKnockBack(float deltaTime, float knockBackDamping = 0.85f)
    {
        if (_knockBackDuration > 0f)
        {
            _collider.Center += _knockBackVelocity * deltaTime;
            _knockBackVelocity *= knockBackDamping;
            _knockBackDuration -= deltaTime;
        }
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

    protected void DrawShadow(SpriteBatch spriteBatch, Rectangle destRect, float coreAlpha = 0.14f,
        float softAlpha = 0.08f)
    {
        Rectangle shadowCore = new Rectangle(
            destRect.X + destRect.Width / 8,
            destRect.Y + destRect.Height - 6,
            destRect.Width - destRect.Width / 4,
            4
        );

        Rectangle shadowSoft = new Rectangle(
            destRect.X + destRect.Width / 6,
            destRect.Y + destRect.Height - 4,
            destRect.Width - destRect.Width / 3,
            2
        );

        spriteBatch.Draw(GameManager.GetGameManager().DummyTexture, shadowCore, Color.Black * coreAlpha);
        spriteBatch.Draw(GameManager.GetGameManager().DummyTexture, shadowSoft, Color.Black * softAlpha);
    }

    protected void DrawAnimatedSprite(SpriteBatch spriteBatch, Color color)
    {
        _animatedSprite?.Draw(spriteBatch, GetAnimatedSpriteDestinationRectangle(), color);
    }

    protected void DrawAnimatedSprite(SpriteBatch spriteBatch, Color color, Vector2 facingDirection)
    {
        if (_animatedSprite == null)
            return;

        SpriteEffects effects = facingDirection.X < 0f ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        var destinationRectangle = GetAnimatedSpriteDestinationRectangle();
        var position = destinationRectangle.Center.ToVector2();

        _animatedSprite.Draw(spriteBatch, position, color, 0f, _scale, effects, 0f);
    }
}