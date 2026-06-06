using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Managers;

namespace TheCure.Weapons.Throw;

public abstract class Throwable : GameObject
{
    //private Texture2D _texture;
    private Vector2 _startPosition;
    public Vector2 _targetPosition;
    private Vector2 _position;
    private string _textureName;
    private float _rotation;
    private Color _color;

    private float _speed = 1.5f;
    private float _maxThrowHeight = 1000f;
    private float _elapsedTime;
    private float _maxThrowDistance = 500f;

    private Shadow _shadow;


    public Throwable(Vector2 position, Vector2 target, string textureName, Color color)
    {
        _startPosition = position;
        _targetPosition = SetTargetPosition(target, position);
        _position = position;
        _rotation = 0f;
        _textureName = textureName;
        _color = color;
        _shadow = new Shadow(_targetPosition);
    }

    public override void Load()
    {
        SwitchAnimation(_textureName, 6, 12f, true);

        _shadow.Load();

        base.Load();
    }

    public override void Update(GameTime gameTime)
    {
        _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        var travelTime = MathHelper.Clamp(_elapsedTime / _speed, 0f, 1f);

        var x = MathHelper.Lerp(_startPosition.X, _targetPosition.X, travelTime);

        // Bounce
        var extraY = (float)Math.Sin(_elapsedTime) * _maxThrowHeight * (1 - travelTime);
        var y = MathHelper.Lerp(_startPosition.Y, _targetPosition.Y, travelTime) - extraY;

        _position = new Vector2(x, y);

        _rotation += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (travelTime >= 0.95f)
        {
            OnImpact();
            _shadow.Destroy();
        }

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (_animatedSprite != null)
        {
            _animatedSprite.Draw(
                spriteBatch,
                _position,
                _color,
                _rotation,
                0.5f
            );
        }

        _shadow.Draw(gameTime, spriteBatch);

        base.Draw(gameTime, spriteBatch);
    }
    
    public virtual void OnImpact()
    {
        Destroy();
    }

    public Vector2 SetTargetPosition(Vector2 target, Vector2 position)
    {
        var throwOffset = target - position;
        if (throwOffset.LengthSquared() > _maxThrowDistance * _maxThrowDistance)
        {
            throwOffset.Normalize();
            throwOffset *= _maxThrowDistance;
        }

        return position + throwOffset;
    }
}

class Shadow : GameObject
{
    private CircleCollider _collider;
    private Texture2D _texture;

    public Shadow(Vector2 position)
    {
        _collider = new CircleCollider(position, 5);
        SetCollider(_collider);
    }

    public override void Load()
    {
        _texture = ContentsManager.Get().GetContent().Load<Texture2D>("Bullet");
        base.Load();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        var rect = _collider.GetBoundingBox();

        rect.Width = 12;
        rect.Height = 6;

        var color = Color.Black * 0.25f;

        spriteBatch.Draw(_texture, rect, color);
    }
}
