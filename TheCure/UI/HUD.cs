using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheCure.Managers;

namespace TheCure
{
    public class HUD
    {
        private SpriteFont _font;
        private Texture2D _dummyTexture;
        private Button _menuButton;
        private Rectangle _statsPanelRect;
        private const int HudVerticalOffset = 20;

        public void Load()
        {
            var content = ContentsManager.Get();
            _font = content.HUDFont;
            _dummyTexture = content.DummyTexture;

            _menuButton = new Button(
                new Rectangle(10, 55, 100, 40),
                "Menu",
                _font
            );

            _menuButton.SetAction(() => GameManager.Get().SetGameState(GameState.Paused));
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
            int barWidth = 200;
            int barHeight = 25;

            int spacing = 25;

            Vector2 basePosition = new Vector2(
                _statsPanelRect.X + (_statsPanelRect.Width - barWidth) / 2,
                _statsPanelRect.Y - barHeight - spacing
            );

            if (gameManager.CurrentGameState == GameState.Paused)
            {
                basePosition = new Vector2(15, 15);
            }

            Rectangle barRect = new Rectangle(
                (int)basePosition.X,
                (int)basePosition.Y,
                barWidth,
                barHeight
            );
            
            Rectangle panelRect = new Rectangle(
                barRect.X - 10,
                barRect.Y - 25,
                barRect.Width + 20,
                barRect.Height + 40
            );

            spriteBatch.Draw(_dummyTexture, panelRect, new Color(20, 30, 20, 170));

            Color borderColor = new Color(100, 255, 100, 200);

            spriteBatch.Draw(_dummyTexture, new Rectangle(panelRect.X, panelRect.Y, panelRect.Width, 2), borderColor);
            spriteBatch.Draw(_dummyTexture, new Rectangle(panelRect.X, panelRect.Y + panelRect.Height - 2, panelRect.Width, 2), borderColor);
            spriteBatch.Draw(_dummyTexture, new Rectangle(panelRect.X, panelRect.Y, 2, panelRect.Height), borderColor);
            spriteBatch.Draw(_dummyTexture, new Rectangle(panelRect.X + panelRect.Width - 2, panelRect.Y, 2, panelRect.Height), borderColor);

            spriteBatch.DrawString(
                _font,
                "HEALTH",
                new Vector2(panelRect.X + 10, panelRect.Y + 5),
                new Color(100, 255, 100, 255)
            );

            spriteBatch.Draw(
                _dummyTexture,
                barRect,
                new Color(50, 50, 60, 185)
            );

            float healthRatio =
                PlayerManager.Get().Player.CurrentHealth() /
                PlayerManager.Get().Player._maxHealth;

            Color healthColor =
                healthRatio > 0.5f ? new Color(34, 177, 76, 175) :
                healthRatio > 0.2f ? new Color(255, 193, 7, 185) :
                                     new Color(244, 67, 54, 185);
            spriteBatch.Draw(
                _dummyTexture,
                new Rectangle(
                    barRect.X,
                    barRect.Y,
                    (int)(barRect.Width * healthRatio),
                    barRect.Height
                ),
                healthColor
            );

            string healthText =
                $"{PlayerManager.Get().Player.CurrentHealth():F0} / {PlayerManager.Get().Player._maxHealth:F0}";

            Vector2 textSize = _font.MeasureString(healthText);

            spriteBatch.DrawString(
                _font,
                healthText,
                new Vector2(
                    barRect.X + barRect.Width / 2 - textSize.X / 2,
                    barRect.Y + barRect.Height / 2 - textSize.Y / 2
                ),
                Color.White
            );
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

            spriteBatch.Draw(_dummyTexture,
                new Rectangle(panelX, panelY, panelWidth, panelHeight),
                new Color(20, 20, 30, 220));

            Color borderColor = new Color(255, 200, 0, 200);
            spriteBatch.Draw(_dummyTexture, new Rectangle(panelX, panelY, panelWidth, 2), borderColor);
            spriteBatch.Draw(_dummyTexture, new Rectangle(panelX, panelY + panelHeight - 2, panelWidth, 2), borderColor);
            spriteBatch.Draw(_dummyTexture, new Rectangle(panelX, panelY, 2, panelHeight), borderColor);
            spriteBatch.Draw(_dummyTexture, new Rectangle(panelX + panelWidth - 2, panelY, 2, panelHeight), borderColor);

            Vector2 position = new Vector2(panelX + panelPadding, panelY + panelPadding / 2);
            spriteBatch.DrawString(_font, text, position, new Color(255, 200, 0, 255));
        }

        private void DrawScore(SpriteBatch spriteBatch, GameManager gameManager)
        {
            string text = $"Score: {ScoreManager.Get().GetScore()}";
            Vector2 size = _font.MeasureString(text);

            int panelPadding = 15;
            int panelWidth = (int)(size.X + panelPadding * 2);
            int panelHeight = (int)(size.Y + panelPadding);
            int panelX = gameManager.Game.GraphicsDevice.Viewport.Width - panelWidth - 15;
            int panelY = 10;

            spriteBatch.Draw(_dummyTexture,
                new Rectangle(panelX, panelY, panelWidth, panelHeight),
                new Color(20, 30, 20, 220));

            Color borderColor = new Color(100, 255, 100, 200);
            spriteBatch.Draw(_dummyTexture, new Rectangle(panelX, panelY, panelWidth, 2), borderColor);
            spriteBatch.Draw(_dummyTexture, new Rectangle(panelX, panelY + panelHeight - 2, panelWidth, 2),
                borderColor);
            spriteBatch.Draw(_dummyTexture, new Rectangle(panelX, panelY, 2, panelHeight), borderColor);
            spriteBatch.Draw(_dummyTexture, new Rectangle(panelX + panelWidth - 2, panelY, 2, panelHeight),
                borderColor);

            Vector2 position = new Vector2(panelX + panelPadding, panelY + panelPadding / 2);
            spriteBatch.DrawString(_font, text, position, new Color(100, 255, 100, 255));
        }

        private void DrawScorePopups(SpriteBatch spriteBatch, GameManager gameManager)
        {
            var popups = ScoreManager.Get().GetScorePopups();

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
                spriteBatch.Draw(_dummyTexture,
                    new Rectangle(popupX - 10, popupY - 5, popupWidth, popupHeight),
                    bgColor);

                Color borderColor = Color.Lerp(new Color(255, 100, 100, 0), new Color(255, 200, 100, 255), alpha);
                spriteBatch.Draw(_dummyTexture,
                    new Rectangle(popupX - 10, popupY - 5, popupWidth, 1), borderColor);
                spriteBatch.Draw(_dummyTexture,
                    new Rectangle(popupX - 10, popupY - 5 + popupHeight - 1, popupWidth, 1), borderColor);
                spriteBatch.Draw(_dummyTexture,
                    new Rectangle(popupX - 10, popupY - 5, 1, popupHeight), borderColor);
                spriteBatch.Draw(_dummyTexture,
                    new Rectangle(popupX - 11 + popupWidth, popupY - 5, 1, popupHeight), borderColor);

                Vector2 position = new Vector2(popupX, popupY);
                spriteBatch.DrawString(_font, popup.Text, position, color, 0f, Vector2.Zero, 0.95f, SpriteEffects.None,
                    0f);
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

            int hudOffsetY = 50;

            int panelX = screenWidth - panelWidth - 15;
            int panelY = screenHeight - panelHeight - 15 - hudOffsetY;

            _statsPanelRect = new Rectangle(panelX, panelY, panelWidth, panelHeight);
            Rectangle panelRect = _statsPanelRect;

            spriteBatch.Draw(_dummyTexture, panelRect, new Color(20, 30, 40, 170));

            Color borderColor = new Color(100, 255, 100, 200);

            spriteBatch.Draw(_dummyTexture, new Rectangle(panelRect.X, panelRect.Y, panelRect.Width, 2), borderColor);
            spriteBatch.Draw(_dummyTexture, new Rectangle(panelRect.X, panelRect.Y + panelRect.Height - 2, panelRect.Width, 2), borderColor);
            spriteBatch.Draw(_dummyTexture, new Rectangle(panelRect.X, panelRect.Y, 2, panelRect.Height), borderColor);
            spriteBatch.Draw(_dummyTexture, new Rectangle(panelRect.X + panelRect.Width - 2, panelRect.Y, 2, panelRect.Height), borderColor);

            spriteBatch.DrawString(
                _font,
                "STATS",
                new Vector2(panelRect.X + panelPadding, panelRect.Y + 5),
                new Color(100, 255, 100, 255)
            );

            spriteBatch.Draw(
                _dummyTexture,
                new Rectangle(panelRect.X + panelPadding, panelRect.Y + titleHeight - 2, panelWidth - panelPadding * 2, 1),
                new Color(100, 255, 100, 150)
            );

            int offsetY = titleHeight + 5;

            foreach (var stat in stats)
            {
                spriteBatch.DrawString(
                    _font,
                    stat.Label,
                    new Vector2(panelRect.X + panelPadding, panelRect.Y + offsetY),
                    new Color(180, 180, 180, 255)
                );

                spriteBatch.DrawString(
                    _font,
                    stat.Value,
                    new Vector2(
                        panelRect.X + panelWidth - panelPadding - _font.MeasureString(stat.Value).X,
                        panelRect.Y + offsetY
                    ),
                    new Color(100, 255, 200, 255)
                );

                offsetY += lineHeight;
            }
        }
    }
}