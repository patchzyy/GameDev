using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheCure.PlayerActions;

namespace TheCure
{
    public class HUD
    {
        private SpriteFont _font;
        private Button _menuButton;
        private PlayerInteractionsHUD _playerInteractionsHUD;

        public void Load(ContentManager content)
        {
            _font = content.Load<SpriteFont>("HudFont");

            _menuButton = new Button(
                new Rectangle(10, 10, 100, 40),
                "Menu",
                _font
            );

            _playerInteractionsHUD = new PlayerInteractionsHUD(_font);
            _playerInteractionsHUD.Load(content);
            _menuButton.Clicked += (s, e) => { GameManager.GetGameManager().SetGameState(GameState.Paused); };
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            _menuButton.Update(mouse);
            _playerInteractionsHUD.Update(gameTime);
        }

        public Dash GetDash() => _playerInteractionsHUD.GetDash();

        public void Draw(SpriteBatch spriteBatch, GameManager gameManager)
        {
            _playerInteractionsHUD.Draw(spriteBatch, gameManager);
            if (gameManager.CurrentGameState == GameState.Playing)
            {
                _menuButton.Draw(spriteBatch);
            }

            if (gameManager.CurrentGameState == GameState.Playing || gameManager.CurrentGameState == GameState.Paused)
            {
                DrawHealthBar(spriteBatch, gameManager);
                DrawTimer(spriteBatch, gameManager);
                DrawScore(spriteBatch, gameManager);
            }

            DrawStatsPanel(spriteBatch, gameManager);
            DrawScorePopups(spriteBatch, gameManager);
        }

        private void DrawHealthBar(SpriteBatch spriteBatch, GameManager gameManager)
        {
            Vector2 barPosition = new Vector2(10, 60);
            int barWidth = 150;
            int barHeight = 15;

            if (gameManager.CurrentGameState == GameState.Paused)
            {
                barPosition = new Vector2(10, 10);
            }

            spriteBatch.Draw(gameManager.DummyTexture,
                new Rectangle((int)barPosition.X, (int)barPosition.Y, barWidth, barHeight), Color.Gray);
            float healthRatio = gameManager.Player.CurrentHealth() / gameManager.Player.MaxHealth;

            spriteBatch.Draw(gameManager.DummyTexture,
                new Rectangle((int)barPosition.X, (int)barPosition.Y, (int)(barWidth * healthRatio), barHeight),
                Color.Green);
        }

        private void DrawTimer(SpriteBatch spriteBatch, GameManager gameManager)
        {
            float time = gameManager.GetGameTime();

            int minutes = (int)time / 60;
            int seconds = (int)time % 60;

            string text = $"{minutes:00}:{seconds:00}";
            Vector2 size = _font.MeasureString(text);

            Vector2 position = new Vector2(gameManager.Game.GraphicsDevice.Viewport.Width / 2 - size.X / 2, 10);

            spriteBatch.DrawString(_font, text, position, Color.White);
        }

        private void DrawScore(SpriteBatch spriteBatch, GameManager gameManager)
        {
            string text = $"Score: {gameManager.GetScore()}";
            Vector2 size = _font.MeasureString(text);

            Vector2 position = new Vector2(gameManager.Game.GraphicsDevice.Viewport.Width - size.X - 10, 10);

            spriteBatch.DrawString(_font, text, position, Color.White);
        }

        private void DrawScorePopups(SpriteBatch spriteBatch, GameManager gameManager)
        {
            var popups = gameManager.GetScorePopups();

            int startY = 40;
            int spacing = 20;

            for (int i = 0; i < popups.Count; i++)
            {
                var popup = popups[i];

                float alpha = popup.TimeLeft / 1.5f;

                Color color = Color.Lerp(Color.Transparent, Color.Red, alpha);
                Vector2 textSize = _font.MeasureString(popup.Text) * 0.9f;

                Vector2 position = new Vector2(gameManager.Game.GraphicsDevice.Viewport.Width - textSize.X - 10,
                    startY + i * spacing
                );

                spriteBatch.DrawString(_font, popup.Text, position, color, 0f, Vector2.Zero, 0.9f, SpriteEffects.None,
                    0f);
            }
        }

        private void DrawStatsPanel(SpriteBatch spriteBatch, GameManager gameManager)
        {
            var stats = gameManager.GetStats();

            int panelWidth = 200;
            int padding = 10;
            int lineHeight = 25;
            int offsetY = 0;

            int panelHeight = stats.Count * lineHeight + padding * 2;

            int screenWidth = gameManager.Game.GraphicsDevice.Viewport.Width;
            int screenHeight = gameManager.Game.GraphicsDevice.Viewport.Height;

            Rectangle panelRect = new Rectangle(
                screenWidth - panelWidth - 10,
                screenHeight - panelHeight - 10,
                panelWidth,
                panelHeight
            );

            spriteBatch.Draw(gameManager.DummyTexture, panelRect, new Color(20, 20, 20, 200));

            foreach (var stat in stats)
            {
                spriteBatch.DrawString(
                    _font,
                    stat.Label,
                    new Vector2(panelRect.X + padding, panelRect.Y + offsetY),
                    Color.LightGray);

                spriteBatch.DrawString(
                    _font,
                    stat.Value,
                    new Vector2(panelRect.X + panelWidth - _font.MeasureString(stat.Value).X, panelRect.Y + offsetY),
                    Color.White);
                offsetY += lineHeight;
            }
        }
    }
}