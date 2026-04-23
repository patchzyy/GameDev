using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheCure.Managers;
using TheCure.PlayerActions;

namespace TheCure;

public class PlayerActionsManager :Manager<PlayerActionsManager>
{
    private int _scale;
    private SpriteFont _font;

    private Keys _shootKey;
    private PlayerAction _shoot;

    private Keys _dashKey;
    private Dash _dash;

    private List<Keys> _actionKeys;
    private List<PlayerAction> _actions;

    private Dictionary<PlayerAction, Keys> _actionKeyMap;


    public PlayerActionsManager()
    {
        _scale = 5;
        _shootKey = Settings.GetValue(SettingsConst.KEY_BINDS.ACTION_1);
        _shoot = new ShootMode("Shoot");

        _dashKey = Settings.GetValue(SettingsConst.KEY_BINDS.ACTION_2);
        _dash = new Dash("Dash");

        _actionKeys = new List<Keys>
        {
            Settings.GetValue(SettingsConst.KEY_BINDS.ACTION_3),
            Settings.GetValue(SettingsConst.KEY_BINDS.ACTION_4),
            Settings.GetValue(SettingsConst.KEY_BINDS.ACTION_5),
            Settings.GetValue(SettingsConst.KEY_BINDS.ACTION_6),
        };

        _actionKeyMap = new Dictionary<PlayerAction, Keys>()
        {
            { _shoot, _shootKey },
            { _dash, _dashKey }
        };

        _actions = new List<PlayerAction>();
    }

    public void Load()
    {
        _font = ContentsManager.Get().HUDFont;
        _shoot.Load();
        _dash.Load();
        foreach (var action in _actions)
        {
            action.Load();
        }
    }

    public void AddAction(PlayerAction action)
    {
        if (_actionKeys.Count == _actions.Count) return;

        _actionKeyMap.Add(action, _actionKeys[_actions.Count]);
        _actions.Add(action);
        action.Load();
    }

    public void Reset()
    {
        _actions.Clear();
        _actionKeyMap = new Dictionary<PlayerAction, Keys>()
        {
            { _shoot, _shootKey },
            { _dash, _dashKey }
        };

        foreach (var action in _actionKeyMap.Keys)
        {
            action.ResetCoolDown();
        }
    }


    public void Update(GameTime gameTime)
    {
        var kbState = InputManager.Get().LastKeyboardState;

        foreach (var pair in _actionKeyMap)
        {
            var action = pair.Key;
            var key = pair.Value;

            if (!kbState.IsKeyDown(key))
                continue;

            action.Execute(gameTime);
            break;
        }

        foreach (var action in _actionKeyMap.Keys)
        {
            action.Update(gameTime);
        }
    }

    public Dash GetDash() => _dash;

    public void Draw(SpriteBatch spriteBatch)
    {
        var gameManager = GameManager.Get();
        if (_actions.Count > 0)
        {
            int screenWidth = gameManager.Game.GraphicsDevice.Viewport.Width;
            var x = screenWidth / 2;

            MainPanel(spriteBatch, gameManager, x, _actions);
        }

        MainPanel(spriteBatch, gameManager, 200, new List<PlayerAction> { _shoot, _dash });
    }

    private void MainPanel(SpriteBatch spriteBatch, GameManager gameManager, int x, List<PlayerAction> actions)
    {
        int panelWidth = 20 * actions.Count * _scale;
        int panelHeight = 20 * _scale;

        int screenHeight = gameManager.Game.GraphicsDevice.Viewport.Height;

        Rectangle panelRect = new Rectangle(
            x - (panelWidth / 2) - 10,
            screenHeight - panelHeight - 10,
            panelWidth,
            panelHeight
        );

        var dummyTexture = ContentsManager.Get().DummyTexture;
        spriteBatch.Draw(dummyTexture, panelRect, new Color(20, 25, 35, 220));

        Color borderColor = new Color(100, 255, 100, 180);
        spriteBatch.Draw(dummyTexture, new Rectangle(panelRect.X, panelRect.Y, panelRect.Width, 2),
            borderColor);
        spriteBatch.Draw(dummyTexture,
            new Rectangle(panelRect.X, panelRect.Y + panelRect.Height - 2, panelRect.Width, 2), borderColor);
        spriteBatch.Draw(dummyTexture, new Rectangle(panelRect.X, panelRect.Y, 2, panelRect.Height),
            borderColor);
        spriteBatch.Draw(dummyTexture,
            new Rectangle(panelRect.X + panelRect.Width - 2, panelRect.Y, 2, panelRect.Height), borderColor);

        for (int i = 0; i < actions.Count; i++)
        {
            IconPanel(spriteBatch, panelRect.X + _scale + (i * 20 * _scale),
                panelRect.Y + _scale, actions[i]);
        }
    }

    private void IconPanel(SpriteBatch spriteBatch, int x, int y, PlayerAction action)
    {
        int panelWidth = 18 * _scale;
        int panelHeight = 18 * _scale;

        Rectangle panelRect = new Rectangle(
            x,
            y,
            panelWidth,
            panelHeight
        );

        var bgColor = new Color(40, 45, 60, 255);
        var dummyTexture = ContentsManager.Get().DummyTexture;
        spriteBatch.Draw(dummyTexture, panelRect, bgColor);

        Color borderColor = new Color(100, 255, 100, 200);
        spriteBatch.Draw(dummyTexture, new Rectangle(x, y, panelWidth, 1), borderColor);
        spriteBatch.Draw(dummyTexture, new Rectangle(x, y + panelHeight - 1, panelWidth, 1), borderColor);
        spriteBatch.Draw(dummyTexture, new Rectangle(x, y, 1, panelHeight), borderColor);
        spriteBatch.Draw(dummyTexture, new Rectangle(x + panelWidth - 1, y, 1, panelHeight), borderColor);

        DrawIcon(spriteBatch,  panelRect.X, panelRect.Y, panelWidth, panelHeight,
            action.GetIconTexture());

        InputPanel(spriteBatch,  panelRect.X + panelWidth - 4 * _scale,
            panelRect.Y + _scale, _actionKeyMap[action]);

        CoolDownPanel(spriteBatch,  panelRect.X, panelRect.Y, panelWidth, panelHeight, action);
    }

    private void InputPanel(SpriteBatch spriteBatch, int x, int y, Keys key)
    {
        int panelWidth = 3 * _scale;
        int panelHeight = 3 * _scale;

        Rectangle panelRect = new Rectangle(
            x,
            y,
            panelWidth,
            panelHeight
        );

        var dummyTexture = ContentsManager.Get().DummyTexture;
        spriteBatch.Draw(dummyTexture,panelRect, new Color(100, 255, 100, 220));

        spriteBatch.Draw(dummyTexture, new Rectangle(x, y, panelWidth, 1), new Color(200, 255, 200, 255));
        spriteBatch.Draw(dummyTexture, new Rectangle(x, y + panelHeight - 1, panelWidth, 1),
            new Color(200, 255, 200, 255));
        spriteBatch.Draw(dummyTexture, new Rectangle(x, y, 1, panelHeight), new Color(200, 255, 200, 255));
        spriteBatch.Draw(dummyTexture, new Rectangle(x + panelWidth - 1, y, 1, panelHeight),
            new Color(200, 255, 200, 255));

        var position = new Vector2(panelRect.X + _scale, panelRect.Y + (_scale / 2));

        string text = FormatKey(key);
        spriteBatch.DrawString(_font, text, position, new Color(0, 0, 0, 255));
    }

    private void CoolDownPanel(SpriteBatch spriteBatch, int x, int y, int panelWidth,
        int panelHeight, PlayerAction action)
    {
        float remaining = action.GetRemainingCoolDown();

        if (remaining > 0)
        {
            spriteBatch.Draw(ContentsManager.Get().DummyTexture,
                new Rectangle(x, y, panelWidth, panelHeight),
                new Color(100, 50, 50, (int)(100 * (remaining / 10f))));

            string coolDownText = remaining.ToString("0.0");
            Vector2 textSize = _font.MeasureString(coolDownText);
            Vector2 textPos = new Vector2(x + panelWidth / 2 - textSize.X / 2, y + panelHeight / 2 - textSize.Y / 2);

            spriteBatch.DrawString(_font, coolDownText, textPos, new Color(255, 100, 100, 255));
        }
    }

    private void DrawIcon(SpriteBatch spriteBatch, int x, int y, int panelWidth,
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