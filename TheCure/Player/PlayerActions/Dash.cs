using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheCure.Collision;
using TheCure.Mobs;

namespace TheCure.PlayerActions;

public class Dash : PlayerAction
{
    private const float DashSpeed = 800f;
    private const float DashDuration = 0.4f;

    private float _dashTimer = 0f;
    private bool _isDashing = false;
    private Vector2 _dashDirection = Vector2.Zero;
    private HashSet<Mob> _hitEnemies = new HashSet<Mob>();

    public Dash()
    {
        Cooldown = 10f;
    }

    protected override void OnExecute(GameTime gameTime, GameManager gameManager)
    {
        if (_isDashing)
            return;

        Player player = gameManager.Player;
        if (player == null)
            return;

        Point mousePosition = gameManager.InputManager.CurrentMouseState.Position;
        Vector2 worldMousePosition = gameManager.ScreenToWorld(mousePosition.ToVector2());
        Vector2 playerCenter = player.GetPosition().Center.ToVector2();

        Vector2 directionToMouse = worldMousePosition - playerCenter;
        if (directionToMouse.LengthSquared() < 10000)
        {
            _dashDirection = new Vector2((float)Math.Cos(player._rotation), (float)Math.Sin(player._rotation));
        }
        else
        {
            directionToMouse.Normalize();
            _dashDirection = directionToMouse;
        }

        _isDashing = true;
        _dashTimer = DashDuration;
        _hitEnemies.Clear();

        System.Diagnostics.Debug.WriteLine($"Dash started! Direction: {_dashDirection}");
    }

    public void UpdateDash(GameTime gameTime, GameManager gameManager)
    {
        if (!_isDashing)
            return;

        Player player = gameManager.Player;
        if (player == null)
            return;

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _dashTimer -= deltaTime;

        player._velocity = _dashDirection * DashSpeed;

        CheckEnemyCollisions(gameManager, player);

        if (_dashTimer <= 0)
        {
            _isDashing = false;
            player._velocity = Vector2.Zero;
            System.Diagnostics.Debug.WriteLine("Dash ended");
        }
    }

    private void CheckEnemyCollisions(GameManager gameManager, Player player)
    {
        if (gameManager.Enemies == null)
            return;

        foreach (Mob enemy in gameManager.Enemies)
        {
            if (_hitEnemies.Contains(enemy))
                continue;

            if (player.CheckCollision(enemy))
            {
                System.Diagnostics.Debug.WriteLine($"Dash hit enemy!");
            }
        }
    }

    public bool IsDashing => _isDashing;
}