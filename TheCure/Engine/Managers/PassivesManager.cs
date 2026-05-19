using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Managers;
using TheCure.Passives;
using TheCure.Upgrades;

namespace TheCure;

public class PassivesManager:Manager<PassivesManager>
{
    private List<Upgrade> _availableUpgrades;

    private List<Upgrade> _unlockedUpgrades;
    private List<Upgrade> _selectedUpgrades;

    private UpgradesUI _upgradesUI;


    public void Load()
    {
        _upgradesUI = new UpgradesUI("Upgrades");
        _upgradesUI.Load();

        Reset();
        PickRandomUpgrade();
    }

    public void Reset()
    {

        _availableUpgrades = new List<Upgrade>
        {
            new HealthPassiveUpgrade(),
            new HealingPassiveUpgrade(),
        };

        _unlockedUpgrades = new List<Upgrade>();
        _selectedUpgrades = new List<Upgrade>();
        _upgradesUI.Reset();
    }

    public void PickRandomUpgrade()
    {
        _selectedUpgrades.Clear();
        _upgradesUI.Reset();

        var random = new Random();
        var selectableUpgrades = _availableUpgrades.FindAll(upgrade =>
            upgrade.RequiredUpgrade == null || _unlockedUpgrades.Contains(upgrade.RequiredUpgrade));

        while (selectableUpgrades.Count < 4)
        {
            selectableUpgrades.Add(new HealthPassiveUpgrade());
        }

        for (int i = 0; i < 4; i++)
        {
            var upgrade = selectableUpgrades[random.Next(0, selectableUpgrades.Count)];
            _selectedUpgrades.Add(upgrade);
        }
    }

    public void Draw(SpriteBatch spriteBatch, GameManager gameManager)
    {
        _upgradesUI.Draw(spriteBatch, gameManager, _selectedUpgrades, HandleUpgradePicked);
    }

    private void HandleUpgradePicked(Upgrade upgrade)
    {
        upgrade.Unlock(_unlockedUpgrades);
        _selectedUpgrades.Clear();

        if (!_unlockedUpgrades.Contains(upgrade))
        {
            _unlockedUpgrades.Add(upgrade);
        }

        GameManager.Get().SetGameState(GameState.GameOver);
    }

    public void UpdateButtons(GameTime gameTime)
    {
        _upgradesUI.UpdateButtons(gameTime);
    }
}