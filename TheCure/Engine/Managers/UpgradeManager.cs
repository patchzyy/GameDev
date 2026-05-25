using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheCure.Managers;
using TheCure.Upgrades;

namespace TheCure;

public class UpgradeManager : Manager<UpgradeManager>
{
    private List<Upgrade> _availableUpgrades;
    private List<Upgrade> _availableActions;

    private List<Upgrade> _unlockedUpgrades;
    private List<Upgrade> _unlockedActions;
    private List<Upgrade> _selectedUpgrades;

    private int _lastScore = 0;

    private UpgradesUI _upgradesUI;


    public void Load()
    {
        _upgradesUI = new UpgradesUI("Upgrades");
        _upgradesUI.Load();

        Reset();
    }

    public void Reset()
    {
        var boostUnlock = new BoostUnlockUpgrade();

        _availableActions = new List<Upgrade>
        {
            new HealthBombUnlockUpgrade(),
            new CommandUnlockUpgrade(),
            new SpikeTrapUnlockUpgrade(),
            new FreezeTrapUnlockUpgrade(),
            new BombTrapUnlockUpgrade(),
            new ElectricTrapUnlockUpgrade(),
            new HealBombTrapUnlockUpgrade(),
            boostUnlock,
        };

        _availableUpgrades = new List<Upgrade>
        {
            new BoostPowerUpgrade(boostUnlock),
        };

        _unlockedUpgrades = new List<Upgrade>();
        _unlockedActions = new List<Upgrade>();
        _selectedUpgrades = new List<Upgrade>();
        _lastScore = 0;
        _upgradesUI.Reset();
    }

    public void PickRandomUpgrade()
    {
        _selectedUpgrades.Clear();
        _upgradesUI.Reset();


        var selection = new List<Upgrade>();
        if (_unlockedActions.Count >= 5)
        {
            selection = _availableUpgrades;
        }
        else
        {
            selection = _availableUpgrades.Concat(_availableActions).ToList();
        }

        var random = new Random();
        var selectableUpgrades = selection.FindAll(upgrade =>
            upgrade.RequiredUpgrade == null || _unlockedUpgrades.Contains(upgrade.RequiredUpgrade) ||
            _unlockedActions.Contains(upgrade.RequiredUpgrade));

        while (selectableUpgrades.Count < 4)
        {
            selectableUpgrades.Add(new GainHealthUpgrade());
        }

        for (int i = 0; i < 4; i++)
        {
            var randomIndex = random.Next(0, selectableUpgrades.Count);
            var upgrade = selectableUpgrades[randomIndex];

            _selectedUpgrades.Add(upgrade);
            selectableUpgrades.RemoveAt(randomIndex);
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

        if (_availableActions.Contains(upgrade))
        {
            _availableActions.Remove(upgrade);
            _unlockedActions.Add(upgrade);
            GameManager.Get().SetGameState(GameState.Playing);
            return;
        }

        if (_availableUpgrades.Contains(upgrade))
        {
            if (upgrade.UnlockedOnce)
            {
                _availableUpgrades.Remove(upgrade);
            }

            _unlockedUpgrades.Add(upgrade);
            GameManager.Get().SetGameState(GameState.Playing);
            return;
        }

        // Heal fallback
        _unlockedUpgrades.Add(upgrade);
        GameManager.Get().SetGameState(GameState.Playing);
    }

    public void Update(GameTime gameTime)
    {
        if (_selectedUpgrades.Count == 0)
        {
            PickRandomUpgrade();
        }

        var score = ScoreManager.Get().GetScore();

        if (score > _lastScore & score % 100 == 0 && GameManager.Get().CurrentGameState != GameState.Upgrade)
        {
            _lastScore = score;
            GameManager.Get().SetGameState(GameState.Upgrade);
            SoundManager.Get().PlayUpgradeUnlock();
        }
    }

    public void UpdateButtons(GameTime gameTime)
    {
        _upgradesUI.UpdateButtons(gameTime);
    }
}