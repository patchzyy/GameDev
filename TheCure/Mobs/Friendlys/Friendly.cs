using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Mobs;
using TheCure.Weapons;

namespace TheCure
{
    internal class Friendly : Mob
    {
        private float _followDistance = 60f;
        private Vector2 _startPosition;

        private BaseWeapon _weapon;

        public Friendly(FriendlyWeapons friendlyWeapon) : base("Alien", 80f, 5, 5)
        {
            switch (friendlyWeapon)
            {
                case FriendlyWeapons.HandGun:
                    _weapon = new Handgun();
                    break;
            }
        }

        public Friendly(FriendlyWeapons friendlyWeapons, Vector2 position) : this(friendlyWeapons)
        {
            _startPosition = position;
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            _collider.Center = _startPosition;

            SetHealthBar(_texture, _maxHealth, _startHealth, Destroy, null);
        }

        public override void Update(GameTime gameTime)
        {
            Move(gameTime);
            Attack(gameTime);
            base.Update(gameTime);
        }


        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Bullet && tmp is Bullet bullet && bullet.IsHealing)
            {
                if (!_healthBar.IsMaxHealth)
                {
                    GainHealth(1);
                    tmp.Destroy();
                }
            }

            base.OnCollision(tmp);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tint = Color.LightBlue;
            spriteBatch.Draw(_texture, _collider.GetBoundingBox(), tint);


            string text = "Friendly";
            Vector2 textSize = _font.MeasureString(text);
            Vector2 textPos = new Vector2(_collider.Center.X - (textSize.X / 2),
                _collider.Center.Y - (_texture.Height / 2) - 20);
            spriteBatch.DrawString(_font, text, textPos, Color.LimeGreen);

            base.Draw(gameTime, spriteBatch);
        }

        private void Move(GameTime gameTime)
        {
            Vector2 playerPosition = GameManager.GetGameManager().Player.GetPosition().Center.ToVector2();
            Vector2 direction = playerPosition - _collider.Center;
            float distance = Vector2.Distance(_collider.Center, playerPosition);

            if (distance > _followDistance)
            {
                direction.Normalize();
                _collider.Center += direction * (_speed + 20f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        private void Attack(GameTime gameTime)
        {
            if (_weapon.CanFire)
            {
                Vector2 nearestZombiePosition = GetNearestZombiePosition();
                Vector2 aimDirection =
                    LinePieceCollider.GetDirection(_collider.Center,
                        nearestZombiePosition);

                float distance = Vector2.Distance(nearestZombiePosition, _collider.Center);
                if (distance < 300f)
                {
                    _weapon.Fire(_collider.Center, aimDirection, this);
                }
            }

            _weapon.UpdateCoolDown(gameTime);
        }

        private Vector2 GetNearestZombiePosition()
        {
            var zombies = GameManager.GetGameManager().Zombies;

            Zombie closest = null;
            float closestDistance = float.MaxValue;

            foreach (var zombie in zombies)
            {
                if (zombie.LastHealed < 3f) continue;

                var zombieLocation = zombie._collider.Center;
                var distance = Vector2.Distance(zombieLocation, _collider.Center);
                if (distance < closestDistance)
                {
                    closest = zombie;
                    closestDistance = distance;
                }
            }

            if (closest == null) return Vector2.Zero;

            return closest._collider.Center;
        }
    }
}