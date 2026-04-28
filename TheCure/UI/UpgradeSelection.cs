using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Upgrades;

namespace TheCure;

public class UpgradeSelection
{
    private SpriteFont _font;

    private List<Button> _upgradeButtons;

    private List<Upgrade> _availableUpgrades;

    private List<Upgrade> _unlockedUpgrades;

    private List<Upgrade> _selectedUpgrades;

    private int _lastScore = 0;
    private bool _upgradePicked = false;

    public UpgradeSelection()
    {
    }

    public void Load(ContentManager content)
    {
        _font = content.Load<SpriteFont>("HudFont");
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
        var boostUnlock = new BoostUnlockUpgrade();

        _availableUpgrades = new List<Upgrade>
        {
            new HealthBombUnlockUpgrade(),
            boostUnlock,
            new BoostPowerUpgrade(boostUnlock),
        };

        _unlockedUpgrades = new List<Upgrade>();
        _selectedUpgrades = new List<Upgrade>();
        _lastScore = 0;
        _upgradePicked = false;
    }

    public void PickRandomUpgrade()
    {
        _upgradePicked = false;
        var random = new Random();
        var selectableUpgrades = _availableUpgrades.FindAll(upgrade =>
            upgrade.RequiredUpgrade == null || _unlockedUpgrades.Contains(upgrade.RequiredUpgrade));

        while (selectableUpgrades.Count < _upgradeButtons.Count)
        {
            selectableUpgrades.Add(new GainHealthUpgrade());
        }

        for (int i = 0; i < _upgradeButtons.Count; i++)
        {
            var upgrade = selectableUpgrades[random.Next(0, selectableUpgrades.Count)];
            if (_selectedUpgrades.Contains(upgrade))
            {
                i--;
                continue;
            }

            _selectedUpgrades.Add(upgrade);
        }
    }

    public void Draw(SpriteBatch spriteBatch, GameManager gameManager)
    {
        var screenWidth = gameManager.Game.GraphicsDevice.Viewport.Width;
        var screenHeight = gameManager.Game.GraphicsDevice.Viewport.Height;

        var mainPanelWidth = 1000;
        var mainPanelHeight = 600;

        var mainRect = new Rectangle(screenWidth / 2 - mainPanelWidth / 2, screenHeight / 2 - mainPanelHeight / 2,
            mainPanelWidth, mainPanelHeight);


        spriteBatch.Draw(gameManager.DummyTexture, mainRect, new Color(100, 255, 100, 200));
        spriteBatch.DrawString(_font, "Upgrades", new Vector2(mainRect.X + 10, mainRect.Y + 10), Color.White);

        int buttonWidth = 200;
        int buttonHeight = 50;
        int buttonY = mainRect.Y + mainRect.Height - buttonHeight - 20;
        var count = _upgradeButtons.Count;

        for (int i = 0; i < count; i++)
        {
            var button = _upgradeButtons[i];
            var upgrade = _selectedUpgrades[i];


            var buttonPanel =
                new Rectangle(mainRect.X + i * (mainRect.Width / count) + 10, mainRect.Y + 10,
                    (mainRect.Width / count) - 20,
                    mainRect.Height - 20);
            spriteBatch.Draw(gameManager.DummyTexture, buttonPanel, new Color(20, 30, 20, 255));

            spriteBatch.DrawString(_font, upgrade.Name, new Vector2(buttonPanel.X + 10, buttonPanel.Y + 10),
                Color.White);

            spriteBatch.DrawString(_font, upgrade.Description, new Vector2(buttonPanel.X + 10, buttonPanel.Y + 40),
                Color.White);

            button.SetAction(() =>
            {
                if (_upgradePicked)
                {
                    return;
                }

                _upgradePicked = true;
                upgrade.Unlock(_unlockedUpgrades);
                _selectedUpgrades.Clear();

                if (!_unlockedUpgrades.Contains(upgrade))
                {
                    _unlockedUpgrades.Add(upgrade);
                }

                if (upgrade.UnlockedOnce)
                {
                    _availableUpgrades.Remove(upgrade);
                }

                GameManager.GetGameManager().SetGameState(GameState.Playing);
            });
            button.SetPosition(buttonPanel.X + 15, buttonY);
            button.Draw(spriteBatch);
        }
    }

    public void Update(GameTime gameTime)
    {
        if (_selectedUpgrades.Count == 0)
        {
            PickRandomUpgrade();
        }

        var score = GameManager.GetGameManager().GetScore();

        if (score > _lastScore & score % 100 == 0 && GameManager.GetGameManager().CurrentGameState != GameState.Upgrade)
        {
            _lastScore = score;
            GameManager.GetGameManager().SetGameState(GameState.Upgrade);
        }
    }

    public void UpdateButtons(GameTime gameTime)
    {
        var mouseState = GameManager.GetGameManager().InputManager.CurrentMouseState;
        foreach (var button in _upgradeButtons)
        {
            button.Update(mouseState);
        }
    }
}
