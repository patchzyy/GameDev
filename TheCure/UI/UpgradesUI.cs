using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Managers;
using TheCure.Upgrades;

namespace TheCure;

public class UpgradesUI
{
    private readonly string _title;

    private SpriteFont _font;
    private List<Button> _upgradeButtons;
    private bool _upgradePicked;

    public UpgradesUI(string title)
    {
        _title = title;
    }

    public void Load()
    {
        var content = ContentsManager.Get();
        _font = content.HUDFont;
        _upgradeButtons = new List<Button>(4)
        {
            new Button(new Rectangle(10, 50, 200, 50), "Upgrade 1", _font),
            new Button(new Rectangle(10, 100, 200, 50), "Upgrade 2", _font),
            new Button(new Rectangle(10, 150, 200, 50), "Upgrade 3", _font),
            new Button(new Rectangle(10, 200, 200, 50), "Upgrade 4", _font),
        };

        Reset();
    }

    public void Reset()
    {
        _upgradePicked = false;
    }

    public void Draw(SpriteBatch spriteBatch, GameManager gameManager, List<Upgrade> selectedUpgrades, Action<Upgrade> onUpgradePicked)
    {
        var screenWidth = gameManager.Game.GraphicsDevice.Viewport.Width;
        var screenHeight = gameManager.Game.GraphicsDevice.Viewport.Height;

        var mainPanelWidth = 1000;
        var mainPanelHeight = 600;

        var mainRect = new Rectangle(screenWidth / 2 - mainPanelWidth / 2, screenHeight / 2 - mainPanelHeight / 2, mainPanelWidth, mainPanelHeight);

        var dummyTexture = ContentsManager.Get().DummyTexture;

        spriteBatch.Draw(dummyTexture, mainRect, new Color(100, 255, 100, 200));
        spriteBatch.DrawString(_font, _title, new Vector2(mainRect.X + 10, mainRect.Y + 10), Color.White);

        int buttonHeight = 50;
        int buttonY = mainRect.Y + mainRect.Height - buttonHeight - 20;
        var count = _upgradeButtons.Count;

        if (selectedUpgrades.Count < count)
            return;

        for (int i = 0; i < count; i++)
        {
            var button = _upgradeButtons[i];
            var upgrade = selectedUpgrades[i];

            var buttonPanel = new Rectangle(mainRect.X + i * (mainRect.Width / count) + 10, mainRect.Y + 10, (mainRect.Width / count) - 20, mainRect.Height - 20);

            spriteBatch.Draw(dummyTexture, buttonPanel, new Color(20, 30, 20, 255));

            // Active/passive
            var color = upgrade.Type == UpgradeType.Passive ? Color.Green : Color.Red;
            var typeRect = new Rectangle(buttonPanel.X + buttonPanel.Width - 30, buttonPanel.Y + 10, 20, 20);

            spriteBatch.Draw(dummyTexture, typeRect, color);
            spriteBatch.DrawString(_font, upgrade.Type.ToString().Substring(0, 1), new Vector2(typeRect.X + 5, typeRect.Y + 2), Color.White);
            spriteBatch.DrawString(_font, upgrade.Name, new Vector2(buttonPanel.X + 10, buttonPanel.Y + 10), Color.White);

            DrawWrappedText(spriteBatch, _font, upgrade.Description, new Vector2(buttonPanel.X + 10, buttonPanel.Y + 40), Color.White, buttonPanel.Width - 20, 20);

            button.SetAction(() =>
            {
                if (_upgradePicked)
                {
                    return;
                }

                _upgradePicked = true;
                onUpgradePicked(upgrade);
            });
            button.SetPosition(buttonPanel.X + 15, buttonY);
            button.Draw(spriteBatch);
        }
    }

    private void DrawWrappedText(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color, float maxLineWidth, float lineHeight)
    {
        var words = text.Split(' ');
        string currentLine = string.Empty;
        float y = position.Y;

        foreach (var word in words)
        {
            string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
            Vector2 size = font.MeasureString(testLine);
            if (size.X > maxLineWidth && !string.IsNullOrEmpty(currentLine))
            {
                spriteBatch.DrawString(font, currentLine, new Vector2(position.X, y), color);
                currentLine = word;
                y += lineHeight;
            }
            else
                currentLine = testLine;
        }

        if (!string.IsNullOrEmpty(currentLine))
            spriteBatch.DrawString(font, currentLine, new Vector2(position.X, y), color);
    }

    public void UpdateButtons(GameTime gameTime)
    {
        var mouseState = InputManager.Get().CurrentMouseState;

        foreach (var button in _upgradeButtons)
            button.Update(mouseState);
    }
}