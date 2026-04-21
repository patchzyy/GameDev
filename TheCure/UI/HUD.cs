using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheCure
{
    public class HUD
    {
        private SpriteFont _font;
        private Button _menuButton;

        public void Load(ContentManager content)
        {
            _font = content.Load<SpriteFont>("HudFont");

            _menuButton = new Button(
                new Rectangle(10, 55, 100, 40),
                "Menu",
                _font
            );
            
            _menuButton.SetAction(() => GameManager.GetGameManager().SetGameState(GameState.Paused));
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            _menuButton.Update(mouse);
        }
        
        public void Draw(SpriteBatch spriteBatch, GameManager gameManager)
        {
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
            Rectangle playerBounds = gameManager.Player.GetPosition();
            Vector2 playerCenter = playerBounds.Center.ToVector2();

            Rectangle cameraBounds = gameManager.Camera.GetViewBounds();

            Vector2 barPosition = new Vector2(
                playerCenter.X - cameraBounds.X - 100,
                playerCenter.Y - cameraBounds.Y - 80
            );

            int barWidth = 200;
            int barHeight = 25;
            barPosition.X = Math.Max(10, Math.Min(barPosition.X, gameManager.Game.GraphicsDevice.Viewport.Width - barWidth - 20));
            barPosition.Y = Math.Max(10, Math.Min(barPosition.Y, gameManager.Game.GraphicsDevice.Viewport.Height - 100));

            int panelWidth = barWidth + 20;
            int panelHeight = barHeight + 40;

            if (gameManager.CurrentGameState == GameState.Paused)
            {
                barPosition = new Vector2(15, 15);
            }

            spriteBatch.Draw(gameManager.DummyTexture,
                new Rectangle((int)barPosition.X - 10, (int)barPosition.Y - 25, panelWidth, panelHeight),
                new Color(20, 30, 20, 220));

            int panelX = (int)barPosition.X - 10;
            int panelY = (int)barPosition.Y - 25;
            Color borderColor = new Color(100, 255, 100, 200);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX, panelY, panelWidth, 2), borderColor);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX, panelY + panelHeight - 2, panelWidth, 2), borderColor);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX, panelY, 2, panelHeight), borderColor);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX + panelWidth - 2, panelY, 2, panelHeight), borderColor);

            Vector2 labelSize = _font.MeasureString("HEALTH");
            spriteBatch.DrawString(_font, "HEALTH", new Vector2(panelX + 10, panelY + 5), new Color(100, 255, 100, 255));

            spriteBatch.Draw(gameManager.DummyTexture,
                new Rectangle((int)barPosition.X, (int)barPosition.Y, barWidth, barHeight),
                new Color(50, 50, 60, 255));

            float healthRatio = gameManager.Player.CurrentHealth() / gameManager.Player.MaxHealth;
            Color healthColor = healthRatio > 0.5f ? new Color(34, 177, 76, 255) :
                                healthRatio > 0.2f ? new Color(255, 193, 7, 255) :
                                new Color(244, 67, 54, 255);

            spriteBatch.Draw(gameManager.DummyTexture,
                new Rectangle((int)barPosition.X, (int)barPosition.Y, (int)(barWidth * healthRatio), barHeight),
                healthColor);

            string healthText = $"{gameManager.Player.CurrentHealth():F0} / {gameManager.Player.MaxHealth:F0}";
            Vector2 healthTextSize = _font.MeasureString(healthText);
            spriteBatch.DrawString(_font, healthText,
                new Vector2(barPosition.X + barWidth / 2 - healthTextSize.X / 2, barPosition.Y + barHeight / 2 - healthTextSize.Y / 2),
                Color.White);
        }

        private void DrawTimer(SpriteBatch spriteBatch, GameManager gameManager)
        {
            float time = gameManager.GetGameTime();

            int minutes = (int)time / 60;
            int seconds = (int)time % 60;

            string text = $"{minutes:00}:{seconds:00}";
            Vector2 size = _font.MeasureString(text);

            int panelPadding = 15;
            int panelX = (int)(gameManager.Game.GraphicsDevice.Viewport.Width / 2 - (size.X + panelPadding * 2) / 2);
            int panelY = 10;
            int panelWidth = (int)(size.X + panelPadding * 2);
            int panelHeight = (int)(size.Y + panelPadding);

            spriteBatch.Draw(gameManager.DummyTexture,
                new Rectangle(panelX, panelY, panelWidth, panelHeight),
                new Color(20, 20, 30, 220));

            Color borderColor = new Color(255, 200, 0, 200);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX, panelY, panelWidth, 2), borderColor);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX, panelY + panelHeight - 2, panelWidth, 2), borderColor);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX, panelY, 2, panelHeight), borderColor);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX + panelWidth - 2, panelY, 2, panelHeight), borderColor);

            Vector2 position = new Vector2(panelX + panelPadding, panelY + panelPadding / 2);
            spriteBatch.DrawString(_font, text, position, new Color(255, 200, 0, 255));
        }

        private void DrawScore(SpriteBatch spriteBatch, GameManager gameManager)
        {
            string text = $"Score: {gameManager.GetScore()}";
            Vector2 size = _font.MeasureString(text);

            int panelPadding = 15;
            int panelWidth = (int)(size.X + panelPadding * 2);
            int panelHeight = (int)(size.Y + panelPadding);
            int panelX = gameManager.Game.GraphicsDevice.Viewport.Width - panelWidth - 15;
            int panelY = 10;

            spriteBatch.Draw(gameManager.DummyTexture,
                new Rectangle(panelX, panelY, panelWidth, panelHeight),
                new Color(20, 30, 20, 220));

            Color borderColor = new Color(100, 255, 100, 200);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX, panelY, panelWidth, 2), borderColor);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX, panelY + panelHeight - 2, panelWidth, 2), borderColor);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX, panelY, 2, panelHeight), borderColor);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX + panelWidth - 2, panelY, 2, panelHeight), borderColor);

            Vector2 position = new Vector2(panelX + panelPadding, panelY + panelPadding / 2);
            spriteBatch.DrawString(_font, text, position, new Color(100, 255, 100, 255));
        }

        private void DrawScorePopups(SpriteBatch spriteBatch, GameManager gameManager)
        {
            var popups = gameManager.GetScorePopups();

            int startY = 80;
            int spacing = 25;

            for (int i = 0; i < popups.Count; i++)
            {
                var popup = popups[i];

                float alpha = popup.TimeLeft / 1.5f;

                Color color = Color.Lerp(new Color(255, 100, 100), new Color(255, 255, 100), alpha);
                Vector2 textSize = _font.MeasureString(popup.Text) * 0.95f;

                int popupX = gameManager.Game.GraphicsDevice.Viewport.Width - (int)textSize.X - 25;
                int popupY = startY + i * spacing;
                int popupWidth = (int)textSize.X + 20;
                int popupHeight = (int)textSize.Y + 10;

                Color bgColor = new Color(40, 20, 20, (int)(150 * alpha));
                spriteBatch.Draw(gameManager.DummyTexture,
                    new Rectangle(popupX - 10, popupY - 5, popupWidth, popupHeight),
                    bgColor);

                Color borderColor = Color.Lerp(new Color(255, 100, 100, 0), new Color(255, 200, 100, 255), alpha);
                spriteBatch.Draw(gameManager.DummyTexture,
                    new Rectangle(popupX - 10, popupY - 5, popupWidth, 1), borderColor);
                spriteBatch.Draw(gameManager.DummyTexture,
                    new Rectangle(popupX - 10, popupY - 5 + popupHeight - 1, popupWidth, 1), borderColor);
                spriteBatch.Draw(gameManager.DummyTexture,
                    new Rectangle(popupX - 10, popupY - 5, 1, popupHeight), borderColor);
                spriteBatch.Draw(gameManager.DummyTexture,
                    new Rectangle(popupX - 11 + popupWidth, popupY - 5, 1, popupHeight), borderColor);

                Vector2 position = new Vector2(popupX, popupY);
                spriteBatch.DrawString(_font, popup.Text, position, color, 0f, Vector2.Zero, 0.95f, SpriteEffects.None, 0f);
            }
        }

        private void DrawStatsPanel(SpriteBatch spriteBatch, GameManager gameManager)
        {
            var stats = gameManager.GetStats();

            int panelPadding = 12;
            int lineHeight = 22;
            int panelWidth = 250;

            Vector2 titleSize = _font.MeasureString("STATS");

            int titleHeight = (int)titleSize.Y + 8;
            int panelHeight = stats.Count * lineHeight + panelPadding * 2 + titleHeight + 5;

            int screenWidth = gameManager.Game.GraphicsDevice.Viewport.Width;
            int screenHeight = gameManager.Game.GraphicsDevice.Viewport.Height;

            int panelX = screenWidth - panelWidth - 15;
            int panelY = screenHeight - panelHeight - 15;

            Rectangle panelRect = new Rectangle(panelX, panelY, panelWidth, panelHeight);

            spriteBatch.Draw(gameManager.DummyTexture, panelRect, new Color(20, 30, 40, 220));

            Color borderColor = new Color(100, 255, 100, 200);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX, panelY, panelWidth, 2), borderColor);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX, panelY + panelHeight - 2, panelWidth, 2), borderColor);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX, panelY, 2, panelHeight), borderColor);
            spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelX + panelWidth - 2, panelY, 2, panelHeight), borderColor);

            spriteBatch.DrawString(_font, "STATS",
                new Vector2(panelX + panelPadding, panelY + 5),
                new Color(100, 255, 100, 255));

            spriteBatch.Draw(gameManager.DummyTexture,
                new Rectangle(panelX + panelPadding, panelY + titleHeight - 2, panelWidth - panelPadding * 2, 1),
                new Color(100, 255, 100, 150));

            int offsetY = titleHeight + 5;
            foreach (var stat in stats)
            {
                spriteBatch.DrawString(
                    _font,
                    stat.Label,
                    new Vector2(panelX + panelPadding, panelY + offsetY),
                    new Color(180, 180, 180, 255));

                spriteBatch.DrawString(
                    _font,
                    stat.Value,
                    new Vector2(panelX + panelWidth - panelPadding - _font.MeasureString(stat.Value).X, panelY + offsetY),
                    new Color(100, 255, 200, 255));
                offsetY += lineHeight;
            }
        }
    }
}