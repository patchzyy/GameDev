using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheCure.Managers;

namespace TheCure.Engine.Managers;

public class CommandManager : Manager<CommandManager>
{
    private float _friendlyCommandTimer = 0f;
    private float _friendlyCommandHoldTimer = 0f;
    private Vector2 _friendlyCommandTarget = Vector2.Zero;
    private readonly Dictionary<Friendly, Vector2> _friendlyCommandHoldPositions = new();

    private const float FriendlyCommandBaseRadius = 52f;
    private const float FriendlyCommandRingSpacing = 48f;

    public void Reset()
    {
        _friendlyCommandTimer = 0f;
        _friendlyCommandHoldTimer = 0f;
        _friendlyCommandTarget = Vector2.Zero;
        _friendlyCommandHoldPositions.Clear();
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        UpdateFriendlyCommand(deltaTime);
    }

    public void ActivateFriendlyCommand(Vector2 target, float commandDuration, float holdDuration)
    {
        var gameManager = GameManager.Get();

        _friendlyCommandTarget = gameManager.ClampToPlayableBounds(target, 48f);
        _friendlyCommandTimer = commandDuration;
        _friendlyCommandHoldTimer = holdDuration;
        _friendlyCommandHoldPositions.Clear();
    }

    private void UpdateFriendlyCommand(float deltaTime)
    {
        var gameManager = GameManager.Get();

        if (_friendlyCommandTimer > 0f)
        {
            Point mousePosition = InputManager.Get().CurrentMouseState.Position;
            _friendlyCommandTarget = gameManager.ClampToPlayableBounds(
                gameManager.ScreenToWorld(mousePosition.ToVector2()),
                48f
            );

            _friendlyCommandTimer = Math.Max(0f, _friendlyCommandTimer - deltaTime);

            if (_friendlyCommandTimer <= 0f)
            {
                CaptureFriendlyHoldPositions();
            }

            return;
        }

        if (_friendlyCommandHoldTimer > 0f)
        {
            _friendlyCommandHoldTimer = Math.Max(0f, _friendlyCommandHoldTimer - deltaTime);

            if (_friendlyCommandHoldTimer <= 0f)
            {
                _friendlyCommandHoldPositions.Clear();
            }
        }
    }

    private void CaptureFriendlyHoldPositions()
    {
        _friendlyCommandHoldPositions.Clear();

        foreach (var friendly in GameManager.Get().Friendlies)
        {
            _friendlyCommandHoldPositions[friendly] = ((CircleCollider)friendly.collider).Center;
        }
    }

    public bool TryGetFriendlyCommandPosition(Friendly friendly, out Vector2 position)
    {
        if (_friendlyCommandTimer > 0f)
        {
            position = GetFriendlyCommandFormationPosition(friendly);
            return true;
        }

        if (_friendlyCommandHoldTimer > 0f && _friendlyCommandHoldPositions.TryGetValue(friendly, out position))
        {
            return true;
        }

        position = Vector2.Zero;
        return false;
    }

    private Vector2 GetFriendlyCommandFormationPosition(Friendly friendly)
    {
        var gameManager = GameManager.Get();
        var friendlies = gameManager.Friendlies;

        int index = friendlies.IndexOf(friendly);
        if (index < 0)
            return _friendlyCommandTarget;

        if (index == 0)
            return _friendlyCommandTarget;

        index--;

        int ring = 0;
        int spots = 6;
        int start = 0;

        while (index >= start + spots)
        {
            start += spots;
            ring++;
            spots += 6;
        }

        int slot = index - start;

        float angleStep = MathHelper.TwoPi / spots;
        float angle = slot * angleStep - MathHelper.PiOver2;
        float radius = FriendlyCommandBaseRadius + ring * FriendlyCommandRingSpacing;

        return gameManager.ClampToPlayableBounds(
            _friendlyCommandTarget + new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle)
            ) * radius,
            32f
        );
    }

    public bool IsFriendlyCommandActive()
    {
        return _friendlyCommandTimer > 0f;
    }

    public bool IsFriendlyCommandHolding()
    {
        return _friendlyCommandTimer <= 0f && _friendlyCommandHoldTimer > 0f;
    }

    public Vector2 GetFriendlyCommandTarget()
    {
        return _friendlyCommandTarget;
    }
}