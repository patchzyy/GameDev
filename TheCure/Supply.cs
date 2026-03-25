
using TheCure.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheCure
{
    public enum SupplyType
    {
        WeaponBuff,
        Health,
        Score
    }

    internal class Supply : GameObject
    {
        private RectangleCollider _rectangleCollider;
        private Texture2D _texture;

        public SupplyType Type { get; private set; }

        public Supply()
        {
            Random random = GameManager.GetGameManager().RNG;
            Type = (SupplyType)random.Next(3);
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);

            string textureName = "Crate";
            try
            {
                _texture = content.Load<Texture2D>(textureName);
            }
            catch (ContentLoadException)
            {
                System.Diagnostics.Debug.WriteLine($"Waarschuwing: Kon texture '{textureName}' voor Supply niet laden. Standaardtextuur 'Alien' wordt geladen.");
                _texture = content.Load<Texture2D>("Alien");
            }

            if (_texture != null)
            {
                _rectangleCollider = new RectangleCollider(_texture.Bounds);
                SetCollider(_rectangleCollider);
                System.Diagnostics.Debug.WriteLine($"Supply geladen. RectangleCollider wordt gebruikt.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Fout: Kan textuur voor Supply niet laden. Collider is niet ingesteld.");
            }
        }

        public override void OnCollision(GameObject tmp)
        {
            if (tmp is Player)
            {
                ApplyEffect();
                GameManager.GetGameManager().RemoveGameObject(this);
            }
            else if (tmp is Bullet)
            {
                GameManager.GetGameManager().RemoveGameObject(this);
            }

            base.OnCollision(tmp);
        }

        private void ApplyEffect()
        {
            Player player = GameManager.GetGameManager().Player;
            Random random = GameManager.GetGameManager().RNG;

            switch (Type)
            {
                case SupplyType.WeaponBuff:
                    int weaponChoice = random.Next(2);
                    if (weaponChoice == 0)
                    {
                        player.SetWeapon("Laser");
                        System.Diagnostics.Debug.WriteLine("Supply opgepikt: LaserWeapon buff!");
                    }
                    else
                    {
                        player.SetWeapon("DoubleBarrel");
                        System.Diagnostics.Debug.WriteLine("Supply opgepikt: DoubleBarrelWeapon buff!");
                    }
                    player.SetWeaponBuffTimer(10f);
                    break;
                case SupplyType.Health:
                    player.Heal(30f);
                    System.Diagnostics.Debug.WriteLine("Supply opgepikt: +30 health!");
                    break;
                case SupplyType.Score:
                    GameManager.GetGameManager().AddScore(20);
                    System.Diagnostics.Debug.WriteLine("Supply opgepikt: +20 punten!");
                    break;
            }
        }

        public void RandomMove()
        {
            GameManager game = GameManager.GetGameManager();
            Vector2 randomPosition;

            if (_rectangleCollider == null || game.Player == null)
            {
                System.Diagnostics.Debug.WriteLine("Let op: RandomMove uitgevoerd terwijl collider/speler nog niet klaar is.");
                return;
            }

            randomPosition = game.RandomScreenLocation() - _rectangleCollider.shape.Size.ToVector2() / 2;
            _rectangleCollider.shape.Location = randomPosition.ToPoint();

            Vector2 centerPlayer = game.Player.GetPosition().Center.ToVector2();
            Vector2 center = _rectangleCollider.shape.Center.ToVector2();

            int attempts = 0;

            while (game.Player != null && (center - centerPlayer).Length() < 100 && attempts < 100)
            {
                randomPosition = game.RandomScreenLocation() - _rectangleCollider.shape.Size.ToVector2() / 2;
                _rectangleCollider.shape.Location = randomPosition.ToPoint();
                center = _rectangleCollider.shape.Center.ToVector2();
                attempts++;
            }

            if (attempts >= 100)
            {
                System.Diagnostics.Debug.WriteLine("Waarschuwing: RandomMove kon geen vrije plek vinden voor de voorraad.");
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture != null && _rectangleCollider != null)
            {
                spriteBatch.Draw(_texture, _rectangleCollider.shape, Color.White);
            }

            base.Draw(gameTime, spriteBatch);
        }
    }
}