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
        _scale = 5;
        _font = font;
        _actionKeys = new List<Keys>
        {
            Settings.GetValue(SettingsConst.KEY_BINDS.ACTION_1),
            Settings.GetValue(SettingsConst.KEY_BINDS.ACTION_2),
            Settings.GetValue(SettingsConst.KEY_BINDS.ACTION_3),
            Settings.GetValue(SettingsConst.KEY_BINDS.ACTION_4),
            Settings.GetValue(SettingsConst.KEY_BINDS.ACTION_5),
            Settings.GetValue(SettingsConst.KEY_BINDS.ACTION_6),
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

    public void Reset()
    {
        foreach (var action in _actions)
        {
            action.ResetCoolDown();
        }
    }


    public void Update(GameTime gameTime)
    {
        var gameManager = GameManager.GetGameManager();
        var kbState = gameManager.InputManager.LastKeyboardState;

        for (int i = 0; i < _actionKeys.Count; i++)
        {
            if (kbState.IsKeyDown(_actionKeys[i]))
            {
                if (i < 2) _activeIndex = i;
                _actions[i]?.Execute(gameTime, gameManager);
                break;
            }
        }

        foreach (var action in _actions)
        {
            action.Update(gameTime);
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

        spriteBatch.Draw(gameManager.DummyTexture, panelRect, new Color(20, 25, 35, 220));

        Color borderColor = new Color(100, 255, 100, 180);
        spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelRect.X, panelRect.Y, panelRect.Width, 2), borderColor);
        spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelRect.X, panelRect.Y + panelRect.Height - 2, panelRect.Width, 2), borderColor);
        spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelRect.X, panelRect.Y, 2, panelRect.Height), borderColor);
        spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(panelRect.X + panelRect.Width - 2, panelRect.Y, 2, panelRect.Height), borderColor);

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

        var bgColor = isActive ? new Color(255, 200, 50, 255) : new Color(40, 45, 60, 255);
        spriteBatch.Draw(gameManager.DummyTexture, panelRect, bgColor);

        Color borderColor = isActive ? new Color(255, 255, 150, 255) : new Color(100, 255, 100, 200);
        spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(x, y, panelWidth, 1), borderColor);
        spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(x, y + panelHeight - 1, panelWidth, 1), borderColor);
        spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(x, y, 1, panelHeight), borderColor);
        spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(x + panelWidth - 1, y, 1, panelHeight), borderColor);

        DrawIcon(spriteBatch, gameManager, panelRect.X, panelRect.Y, panelWidth, panelHeight,
            _icons[index]);

        InputPanel(spriteBatch, gameManager, panelRect.X + panelWidth - 4 * _scale,
            panelRect.Y + _scale, _actionKeys[index]);

        CoolDownPanel(spriteBatch, gameManager, panelRect.X, panelRect.Y, panelWidth, panelHeight, _actions[index]);
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

        spriteBatch.Draw(gameManager.DummyTexture, panelRect, new Color(100, 255, 100, 220));

        spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(x, y, panelWidth, 1), new Color(200, 255, 200, 255));
        spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(x, y + panelHeight - 1, panelWidth, 1), new Color(200, 255, 200, 255));
        spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(x, y, 1, panelHeight), new Color(200, 255, 200, 255));
        spriteBatch.Draw(gameManager.DummyTexture, new Rectangle(x + panelWidth - 1, y, 1, panelHeight), new Color(200, 255, 200, 255));

        var position = new Vector2(panelRect.X + _scale, panelRect.Y + (_scale / 2));

        string text = FormatKey(key);
        spriteBatch.DrawString(_font, text, position, new Color(0, 0, 0, 255));
    }

    private void CoolDownPanel(SpriteBatch spriteBatch, GameManager gameManager, int x, int y, int panelWidth,
        int panelHeight, PlayerAction action)
    {
        float remaining = action.GetRemainingCoolDown();

        if (remaining > 0)
        {
            spriteBatch.Draw(gameManager.DummyTexture,
                new Rectangle(x, y, panelWidth, panelHeight),
                new Color(100, 50, 50, (int)(100 * (remaining / 10f))));

            string coolDownText = remaining.ToString("0.0");
            Vector2 textSize = _font.MeasureString(coolDownText);
            Vector2 textPos = new Vector2(x + panelWidth / 2 - textSize.X / 2, y + panelHeight / 2 - textSize.Y / 2);

            spriteBatch.DrawString(_font, coolDownText, textPos, new Color(255, 100, 100, 255));
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