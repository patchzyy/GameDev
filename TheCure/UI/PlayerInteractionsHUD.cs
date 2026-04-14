using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheCure.PlayerActions;

namespace TheCure;

public class PlayerInteractionsHUD
{
    private int _scale;
    private SpriteFont _font;
    private int _activeIndex = 0;

    private List<Keys> _actionKeys;
    private List<PlayerAction> _actions;
    private List<Texture2D> _icons;
    private Dash _dash;

    public PlayerInteractionsHUD(SpriteFont font)
    {
        _scale = 8;
        _font = font;
        _actionKeys = new List<Keys>
        {
            Settings.GetValue(SettingsConst.KEYBINDS.ACTION_1),
            Settings.GetValue(SettingsConst.KEYBINDS.ACTION_2),
            Settings.GetValue(SettingsConst.KEYBINDS.ACTION_3),
            Settings.GetValue(SettingsConst.KEYBINDS.ACTION_4),
            Settings.GetValue(SettingsConst.KEYBINDS.ACTION_5),
            Settings.GetValue(SettingsConst.KEYBINDS.ACTION_6),
        };

        _dash = new Dash();
        _actions = new List<PlayerAction>
        {
            new ShootMode(),
            new ThrowMode(),
            _dash,
            new Build(),
            new Command(),
            new Boost(),
        };
    }

    public void Load(ContentManager content)
    {
        _icons = new List<Texture2D>
        {
            content.Load<Texture2D>("Shoot"),
            content.Load<Texture2D>("Throw"),
            content.Load<Texture2D>("Dash"),
            content.Load<Texture2D>("Build"),
            content.Load<Texture2D>("Command"),
            content.Load<Texture2D>("Boost"),
        };
    }


    public void Update(GameTime gameTime)
    {
        var gm = GameManager.GetGameManager();
        var kbState = gm.InputManager.LastKeyboardState;

        for (int i = 0; i < _actionKeys.Count; i++)
        {
            if (kbState.IsKeyDown(_actionKeys[i]))
            {
                if (i < 2) _activeIndex = i;
                _actions[i]?.Execute(gameTime, gm);
                break;
            }
        }
    }

    public Dash GetDash() => _dash;

    public void Draw(SpriteBatch spriteBatch, GameManager gameManager)
    {
        MainPanel(spriteBatch, gameManager);
    }

    private void MainPanel(SpriteBatch spriteBatch, GameManager gameManager)
    {
        int panelWidth = 120 * _scale;
        int panelHeight = 20 * _scale;

        int screenWidth = gameManager.Game.GraphicsDevice.Viewport.Width;
        int screenHeight = gameManager.Game.GraphicsDevice.Viewport.Height;

        Rectangle panelRect = new Rectangle(
            (screenWidth / 2) - (panelWidth / 2) - 10,
            screenHeight - panelHeight - 10,
            panelWidth,
            panelHeight
        );

        spriteBatch.Draw(gameManager.DummyTexture, panelRect, new Color(20, 20, 20, 100));

        for (int i = 0; i < _actionKeys.Count; i++)
        {
            var active = i == _activeIndex;
            IconPanel(spriteBatch, gameManager, panelRect.X + _scale + (i * 20 * _scale),
                panelRect.Y + _scale, i, active);
        }
    }

    private void IconPanel(SpriteBatch spriteBatch, GameManager gameManager, int x, int y, int index,
        bool isActive = false)
    {
        int panelWidth = 18 * _scale;
        int panelHeight = 18 * _scale;

        Rectangle panelRect = new Rectangle(
            x,
            y,
            panelWidth,
            panelHeight
        );

        var color = isActive ? new Color(255, 255, 20, 255) : new Color(20, 20, 20, 255);
        spriteBatch.Draw(gameManager.DummyTexture, panelRect, color);


        DrawIcon(spriteBatch, gameManager, panelRect.X, panelRect.Y, panelWidth, panelHeight,
            _icons[index]);

        InputPanel(spriteBatch, gameManager, panelRect.X + panelWidth - 4 * _scale,
            panelRect.Y + _scale, _actionKeys[index]);

        CooldownPanel(spriteBatch, gameManager, panelRect.X, panelRect.Y, panelWidth, panelHeight, _actions[index]);
    }

    private void InputPanel(SpriteBatch spriteBatch, GameManager gameManager, int x, int y, Keys key)
    {
        int panelWidth = 3 * _scale;
        int panelHeight = 3 * _scale;

        Rectangle panelRect = new Rectangle(
            x,
            y,
            panelWidth,
            panelHeight
        );

        spriteBatch.Draw(gameManager.DummyTexture, panelRect, new Color(0, 50, 50, 200));
        var position = new Vector2(panelRect.X + _scale, panelRect.Y + (_scale / 2));

        string text = FormatKey(key);
        spriteBatch.DrawString(_font, text, position, Color.White);
    }

    private void CooldownPanel(SpriteBatch spriteBatch, GameManager gameManager, int x, int y, int panelWidth,
        int panelHeight, PlayerAction action)
    {

        float remaining = action.GetRemainingCooldown(gameManager);
        if (remaining > 0)
        {
            var panelRect = new Rectangle(
                x,
                y,
                panelWidth,
                panelHeight
            );
            string cooldownText = remaining.ToString("0.0");

            spriteBatch.Draw(gameManager.DummyTexture, panelRect, new Color(0, 50, 50, 200));
            spriteBatch.DrawString(_font, cooldownText, panelRect.Center.ToVector2(), Color.White);
        }
    }

    private void DrawIcon(SpriteBatch spriteBatch, GameManager gameManager, int x, int y, int panelWidth,
        int panelHeight, Texture2D icon)
    {
        int iconWidth = (int)(panelWidth * 0.9);
        int iconHeight = (int)(panelHeight * 0.9);

        var iconRect = new Rectangle(
            x + (panelWidth - iconWidth) / 2,
            y + (panelHeight - iconHeight) / 2,
            iconWidth,
            iconHeight
        );
        spriteBatch.Draw(icon, iconRect, Color.White);
    }


    private string FormatKey(Keys key)
    {
        string keyName = key.ToString();
        return keyName.StartsWith("D") ? keyName[1..] : keyName;
    }
}