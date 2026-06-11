using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheCure.Upgrades;
using TheCure.Weapons;
using TheCure.Weapons.Throw;
using System.Collections.Generic;
using System.Linq;

namespace TheCure.Managers;

public class ScreenManager : Manager<ScreenManager>
{
    private GameState _previousState;

    private Button _startButton;
    private Button _quitButton;
    private Button _continueButton;

    private Button _pauseQuitButton;
    private Button _restartButton;

    private Button _settingsButton;
    private Button _pauseSettingsButton;
    private Button _settingsBackButton;

    private Button _healSelectButton2;
    private Button _healSelectButton3;
    private Button _healSelectButton4;

    private Texture2D _tutorialPlayerTexture;
    private Texture2D _tutorialZombieTexture;
    private Texture2D _tutorialFriendlyTexture;
    private Texture2D _tutorialHealTexture;
    private Texture2D _tutorialDashTexture;

    private Button _resolutionDropdownButton;
    private List<Button> _resolutionButtons;
    private bool _resolutionDropdownOpen = false;
    private readonly (int width, int height)[] _resolutions = 
    {
        (1920, 1080),
        (2560, 1440)
    };
    private int _selectedResolutionIndex;

    private DisplayModeSetting _selectedDisplayMode;
    private Button _displayModeDropdownButton;
    private List<Button> _displayModeButtons;
    private bool _displayModeDropdownOpen = false;
    private readonly DisplayModeSetting[] _displayModes =
    {
        DisplayModeSetting.Windowed,
        DisplayModeSetting.Borderless,
        DisplayModeSetting.Fullscreen
    };

    private Button _applySettingsButton;

    public void Load()
    {
        int width = Settings.GetValue(SettingsConst.VIDEO.WIDTH);
        int height = Settings.GetValue(SettingsConst.VIDEO.HEIGHT);

        for (int i = 0; i < _resolutions.Length; i++)
        {
            if (_resolutions[i].width == width && _resolutions[i].height == height)
            {
                _selectedResolutionIndex = i;
                break;
            }
        }
        _selectedDisplayMode = Settings.GetValue(SettingsConst.VIDEO.DISPLAY_MODE);
        LoadTutorialContent();
        CreateButtons();
        UpdateResolutionDropdownText();
        UpdateDisplayModeDropdownText();
        PositionButtons();
    }

    private void LoadTutorialContent()
    {
        var content = ContentsManager.Get().GetContent();

        _tutorialPlayerTexture = content.Load<Texture2D>("Character-Joe-Idle");
        _tutorialZombieTexture = content.Load<Texture2D>("Zombie-Atk");
        _tutorialFriendlyTexture = content.Load<Texture2D>("Character-Unknown-Idle");
        _tutorialHealTexture = content.Load<Texture2D>("Shoot");
        _tutorialDashTexture = content.Load<Texture2D>("Dash");
    }

    public void Update()
    {
        var inputManager = InputManager.Get();
        var mouseState = inputManager.CurrentMouseState;
        var gameManager = GameManager.Get();
        var state = gameManager.CurrentGameState;

        if (state == GameState.StartScreen)
        {
            _startButton.Update(mouseState);
            _settingsButton.Update(mouseState);
            _quitButton.Update(mouseState);

            return;
        }

        if (state == GameState.HealSelection)
        {
            _healSelectButton2.Update(mouseState);
            _healSelectButton3.Update(mouseState);
            _healSelectButton4.Update(mouseState);

            return;
        }

        if (state == GameState.Tutorial)
        {
            if (inputManager.IsKeyPress(Keys.Space))
            {
                gameManager.ResetGame();
                gameManager.SetGameState(GameState.Playing);
            }

            return;
        }

        if (state == GameState.Paused)
        {
            if (inputManager.IsKeyPress(Keys.Space))
                gameManager.ResumeGame();

            _continueButton.Update(mouseState);
            _pauseSettingsButton.Update(mouseState);
            _pauseQuitButton.Update(mouseState);

            return;
        }

        if (state == GameState.Settings)
        {
            _settingsBackButton.Update(mouseState);
            _resolutionDropdownButton.Update(mouseState);
            _displayModeDropdownButton.Update(mouseState);

            if (_resolutionDropdownOpen)
                foreach (var button in _resolutionButtons)
                    button.Update(mouseState);
            
            if (_displayModeDropdownOpen)
                foreach (var button in _displayModeButtons)
                    button.Update(mouseState);

            _applySettingsButton.Update(mouseState);
            return;
        }

        if (state == GameState.GameOver)
        {
            _restartButton.Update(mouseState);
            _quitButton.Update(mouseState);
        }
    }

    private void CreateButtons()
    {
        int buttonWidth = 200;
        int buttonHeight = 50;

        _startButton = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Start Game",
            ContentsManager.Get().ButtonFont);
        _quitButton = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Quit",
            ContentsManager.Get().ButtonFont);
        _continueButton = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Continue",
            ContentsManager.Get().ButtonFont);
    
        _pauseQuitButton = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Quit",
            ContentsManager.Get().ButtonFont);
        _restartButton = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Play Again",
            ContentsManager.Get().ButtonFont);

        _settingsButton = new Button(
            new Rectangle(0, 0, buttonWidth, buttonHeight), "Settings",
            ContentsManager.Get().ButtonFont);

        _pauseSettingsButton = new Button(
            new Rectangle(0, 0, buttonWidth, buttonHeight), "Settings",
            ContentsManager.Get().ButtonFont);

        _settingsBackButton = new Button(
            new Rectangle(0, 0, buttonWidth, buttonHeight), "Back",
            ContentsManager.Get().ButtonFont);
        
        _healSelectButton2 = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Instant heal",
            ContentsManager.Get().ButtonFont);
        _healSelectButton3 = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Heal in movement",
            ContentsManager.Get().ButtonFont);
        _healSelectButton4 = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Heal shoot",
            ContentsManager.Get().ButtonFont);

        _resolutionButtons = new List<Button>();
        _resolutionDropdownButton = new Button( new Rectangle(0,0,180,50), "", ContentsManager.Get().ButtonFont);
        for (int i = 0; i < _resolutions.Length; i++)
        {
            int index = i;

            var res = _resolutions[i];

            var button = new Button(new Rectangle(0,0,300,50), $"{res.width} x {res.height}",
                ContentsManager.Get().ButtonFont);

            button.SetAction(() =>
            {
                _selectedResolutionIndex = index;
                UpdateResolutionDropdownText();
                _resolutionDropdownOpen = false;
            });

            _resolutionButtons.Add(button);
        }

        _displayModeButtons = new List<Button>();
        _displayModeDropdownButton = new Button(new Rectangle(0, 0, 180, 50), "", ContentsManager.Get().ButtonFont);
        for (int i = 0; i < _displayModes.Length; i++)
        {
            int index = i;
            var button = new Button(new Rectangle(0, 0, 300, 50), _displayModes[i].ToString(), ContentsManager.Get().ButtonFont);

            button.SetAction(() =>
            {
                _selectedDisplayMode = _displayModes[index];
                UpdateDisplayModeDropdownText();
                _displayModeDropdownOpen = false;
            });

            _displayModeButtons.Add(button);
        }

        _applySettingsButton = new Button( new Rectangle(0,0,200,50), "Apply", ContentsManager.Get().ButtonFont);

        var gameManager = GameManager.Get();

        _startButton.SetAction(() => gameManager.SetGameState(GameState.Tutorial));
        _quitButton.SetAction(gameManager.Game.Exit);
        _continueButton.SetAction(gameManager.ResumeGame);

        _pauseQuitButton.SetAction(gameManager.Game.Exit);
        _restartButton.SetAction(RestartButtonAction);

        _settingsButton.SetAction(() =>
        {
            _previousState = GameState.StartScreen;
            gameManager.SetGameState(GameState.Settings);
        });

        _pauseSettingsButton.SetAction(() =>
        {
            _previousState = GameState.Paused;
            gameManager.SetGameState(GameState.Settings);
        });
        _settingsBackButton.SetAction(() => gameManager.SetGameState(_previousState, false));


        _healSelectButton2.SetAction(() => HealSelectButtonAction(HealType.Instant));
        _healSelectButton3.SetAction(() => HealSelectButtonAction(HealType.Movement));
        _healSelectButton4.SetAction(() => HealSelectButtonAction(HealType.Shoot));

        _resolutionDropdownButton.SetAction(() =>
        {
            _resolutionDropdownOpen = !_resolutionDropdownOpen;

            if (_resolutionDropdownOpen)
                _displayModeDropdownOpen = false;
        });

        _displayModeDropdownButton.SetAction(() =>
        {
            _displayModeDropdownOpen = !_displayModeDropdownOpen;

            if (_displayModeDropdownOpen)
                _resolutionDropdownOpen = false;
        });

        _applySettingsButton.SetAction(ApplySettings);
    }

    private void HealSelectButtonAction(HealType type)
    {
        switch (type)
        {
            case HealType.Movement:
                PlayerManager.Get().Player.WeaponsSystem.SetShootWeapon(ShootWeapons.Movement);
                break;
            case HealType.Instant:
                PlayerManager.Get().Player.WeaponsSystem.SetShootWeapon(ShootWeapons.Instant);
                break;
            case HealType.Shoot:
                PlayerManager.Get().Player.WeaponsSystem.SetShootWeapon(ShootWeapons.SingleBullet);
                break;
        }

        PlayerActionsManager.Get().ReloadShoot();
        GameManager.Get().SetGameState(GameState.StartScreen);
    }

    private void RestartButtonAction()
    {
        var gameManager = GameManager.Get();
        gameManager.ResetGame();
        gameManager.SetGameState(GameState.Playing);
    }

    private void PositionButtons()
    {
        var game = GameManager.Get().Game;
        int buttonWidth = 200;
        int centerX = game.GraphicsDevice.Viewport.Width / 2;

        _startButton.SetPosition(centerX - buttonWidth / 2, (int)(game.GraphicsDevice.Viewport.Height * 0.54f));
        _quitButton.SetPosition(centerX - buttonWidth / 2, (int)(game.GraphicsDevice.Viewport.Height * 0.68f));
        _continueButton.SetPosition(centerX - buttonWidth / 2, (int)(game.GraphicsDevice.Viewport.Height * 0.5f));

        _restartButton.SetPosition(centerX - buttonWidth / 2, (int)(game.GraphicsDevice.Viewport.Height * 0.5f));
        _pauseQuitButton.SetPosition(centerX - buttonWidth / 2, (int)(game.GraphicsDevice.Viewport.Height * 0.68f));

        _settingsButton.SetPosition(centerX - buttonWidth / 2, (int)(game.GraphicsDevice.Viewport.Height * 0.61f));
        _pauseSettingsButton.SetPosition( centerX - buttonWidth / 2, (int)(game.GraphicsDevice.Viewport.Height * 0.59f));
        _settingsBackButton.SetPosition( centerX - buttonWidth / 2, (int)(game.GraphicsDevice.Viewport.Height * 0.75f));
        
        _healSelectButton2.SetPosition(centerX - buttonWidth / 2,
            (int)(game.GraphicsDevice.Viewport.Height * 0.75f) + 120);
        _healSelectButton3.SetPosition(centerX - buttonWidth / 2,
            (int)(game.GraphicsDevice.Viewport.Height * 0.75f) + 180);
        _healSelectButton4.SetPosition(centerX - buttonWidth / 2, (int)(game.GraphicsDevice.Viewport.Height * 0.75f));

        var resolutionLabel = "Resolution: ";
        var resolutionLabelSize = ContentsManager.Get().ButtonFont.MeasureString(resolutionLabel);

        int resolutionLabelX = centerX - 150;
        int buttonX = resolutionLabelX + (int)resolutionLabelSize.X + 10;
        int buttonY = 400 - (_resolutionDropdownButton.Rectangle.Height / 2) + (int)resolutionLabelSize.Y / 2;

        _resolutionDropdownButton.SetPosition(buttonX, buttonY);
        for (int i = 0; i < _resolutionButtons.Count; i++)
            _resolutionButtons[i].SetPosition(buttonX, buttonY + 60 + i * 60);

        var displayModeLabel = "Display Mode: ";
        var displayLabelSize = ContentsManager.Get().ButtonFont.MeasureString(displayModeLabel);

        int displayButtonX = resolutionLabelX + (int)displayLabelSize.X + 10;
        int displayButtonY = 500 - (_displayModeDropdownButton.Rectangle.Height / 2)
                            + (int)displayLabelSize.Y / 2;

        _displayModeDropdownButton.SetPosition(displayButtonX, displayButtonY);

        for (int i = 0; i < _displayModeButtons.Count; i++)
            _displayModeButtons[i].SetPosition(displayButtonX, displayButtonY + 60 + i * 60);

        _applySettingsButton.SetPosition(centerX - 100, (int)(game.GraphicsDevice.Viewport.Height * 0.65f));
    }

    public void DrawHealSelectScreen(SpriteBatch spriteBatch)
    {
        var game = GameManager.Get().Game;
        spriteBatch.Begin();
        var content = ContentsManager.Get();

        spriteBatch.Draw(content.BackgroundTexture,
            new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height),
            Color.White);

        string title = "Choose your healing type";
        Vector2 titleSize = content.TitleFont.MeasureString(title);
        spriteBatch.DrawString(content.TitleFont, title,
            new Vector2(game.GraphicsDevice.Viewport.Width / 2 - titleSize.X / 2, 150),
            Color.White);

        _healSelectButton2.Draw(spriteBatch);
        _healSelectButton3.Draw(spriteBatch);
        _healSelectButton4.Draw(spriteBatch);

        spriteBatch.End();
    }

    public void DrawGameOver(SpriteBatch spriteBatch)
    {
        var game = GameManager.Get().Game;
        spriteBatch.Begin();
        var content = ContentsManager.Get();

        spriteBatch.Draw(content.BackgroundGameOverTexture,
            new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height),
            Color.White);

        spriteBatch.Draw(content.DummyTexture,
            new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height),
            new Color(0, 0, 0, 100));

        var gameOverText = "Game Over";
        var gameOverTextSize = content.TitleFont.MeasureString(gameOverText);
        var gameOverTextPosition =
            new Vector2(game.GraphicsDevice.Viewport.Width / 2 - gameOverTextSize.X / 2,
                game.GraphicsDevice.Viewport.Height / 8f);

        spriteBatch.DrawString(content.TitleFont, gameOverText, gameOverTextPosition, Color.Red);

        string scoreText = $"Final Score: {ScoreManager.Get().GetScore()}";
        Vector2 scoreTextSize = content.TitleFont.MeasureString(scoreText);
        float scale = 0.5f;
        Vector2 scoreTextPosition =
            new Vector2(game.GraphicsDevice.Viewport.Width / 2 - (scoreTextSize.X * scale) / 2,
                game.GraphicsDevice.Viewport.Height / 10f);

        spriteBatch.DrawString(content.TitleFont, scoreText, scoreTextPosition, Color.White, 0f, Vector2.Zero,
            scale,
            SpriteEffects.None, 0f);

        _restartButton.Draw(spriteBatch);
        _quitButton.Draw(spriteBatch);

        spriteBatch.End();
    }

    public void DrawPauseMenu(SpriteBatch spriteBatch)
    {
        var game = GameManager.Get().Game;
        spriteBatch.Begin();
        var content = ContentsManager.Get();

        spriteBatch.Draw(content.BackgroundPauseTexture,
            new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height),
            Color.White);

        spriteBatch.Draw(content.DummyTexture,
            new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height),
            new Color(0, 0, 0, 100));

        string pauseText = "Game Paused";
        Vector2 pauseTextSize = content.TitleFont.MeasureString(pauseText);
        float scale = 0.6f;
        Vector2 pauseTextPosition =
            new Vector2(game.GraphicsDevice.Viewport.Width / 2 - (pauseTextSize.X * scale) / 2,
                game.GraphicsDevice.Viewport.Height / 8f);

        spriteBatch.DrawString(content.TitleFont, pauseText, pauseTextPosition, Color.White, 0f, Vector2.Zero,
            scale,
            SpriteEffects.None, 0f);

        DrawPauseControls(spriteBatch);

        _continueButton.Draw(spriteBatch);
        _pauseSettingsButton.Draw(spriteBatch);
        _pauseQuitButton.Draw(spriteBatch);

        spriteBatch.End();
    }

    private void DrawPauseControls(SpriteBatch spriteBatch)
    {
        var game = GameManager.Get().Game;
        var content = ContentsManager.Get();

        int centerXPaused = game.GraphicsDevice.Viewport.Width / 4;

        spriteBatch.Draw(_tutorialHealTexture, new Rectangle(centerXPaused - 450, 825, 64, 64), Color.White);
        spriteBatch.DrawString(content.ButtonFont, "Primary Weapon",
            new Vector2(centerXPaused - 450, 900), Color.White);

        spriteBatch.Draw(_tutorialDashTexture, new Rectangle(centerXPaused - 350, 825, 64, 64), Color.White);
        spriteBatch.DrawString(content.ButtonFont, "Dash",
            new Vector2(centerXPaused - 350, 900), Color.White);

        string controlsPaused =
            @"WASD  - Move
    M1 - Shoot
    1-5   - Use abilities";

        spriteBatch.DrawString(content.ButtonFont, controlsPaused,
            new Vector2(centerXPaused - 450, 950), Color.White);
    }

    public void DrawStartScreen(SpriteBatch spriteBatch)
    {
        var game = GameManager.Get().Game;
        spriteBatch.Begin();
        var content = ContentsManager.Get();

        spriteBatch.Draw(content.BackgroundTexture,
            new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height),
            Color.White);

        var titleText = "The Cure";
        var titleSize = content.TitleFont.MeasureString(titleText);

        var titlePosition = new Vector2(
            game.GraphicsDevice.Viewport.Width / 2 - titleSize.X / 2,
            game.GraphicsDevice.Viewport.Height / 8f
        );

        spriteBatch.DrawString(content.TitleFont, titleText, titlePosition, Color.Red);

        _startButton.Draw(spriteBatch);
        _settingsButton.Draw(spriteBatch);
        _quitButton.Draw(spriteBatch);

        spriteBatch.End();
    }

    public void DrawTutorial(SpriteBatch spriteBatch)
    {
        var game = GameManager.Get().Game;
        var content = ContentsManager.Get();

        spriteBatch.Begin();

        spriteBatch.Draw(content.BackgroundTexture,
            new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height),
            Color.White);

        spriteBatch.Draw(content.DummyTexture,
            new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height),
            new Color(0, 0, 0, 180));

        int centerX = game.GraphicsDevice.Viewport.Width / 2;

        string title = "THE CURE - TUTORIAL";
        Vector2 titleSize = content.TitleFont.MeasureString(title);
        spriteBatch.DrawString(content.TitleFont, title,
            new Vector2(centerX - titleSize.X / 2, 150),
            Color.White);

        int playerFrameCount = 5;
        int playerFrameWidth = _tutorialPlayerTexture.Width / playerFrameCount;
        int playerFrameHeight = _tutorialPlayerTexture.Height;
        Rectangle playerSourceRect = new Rectangle(0, 0, playerFrameWidth, playerFrameHeight);

        spriteBatch.Draw(_tutorialPlayerTexture,
            new Rectangle(centerX - 290, 360, 100, 100),
            playerSourceRect,
            Color.White);
        spriteBatch.DrawString(content.ButtonFont, "You (Player)",
            new Vector2(centerX - 300, 450), Color.White);

        int zombieFrameCount = 7;
        int zombieFrameWidth = _tutorialZombieTexture.Width / zombieFrameCount;
        int zombieFrameHeight = _tutorialZombieTexture.Height;
        Rectangle zombieSourceRect = new Rectangle(0, 0, zombieFrameWidth, zombieFrameHeight);

        spriteBatch.Draw(_tutorialZombieTexture,
            new Rectangle(centerX - 115, 360, 100, 100),
            zombieSourceRect,
            Color.White);
        spriteBatch.DrawString(content.ButtonFont, "Zombie (Enemy)",
            new Vector2(centerX - 130, 450), Color.White);

        int friendlyFrameCount = 5;
        int friendlyFrameWidth = _tutorialFriendlyTexture.Width / friendlyFrameCount;
        int friendlyFrameHeight = _tutorialFriendlyTexture.Height;
        Rectangle friendlySourceRect = new Rectangle(0, 0, friendlyFrameWidth, friendlyFrameHeight);

        spriteBatch.Draw(_tutorialFriendlyTexture,
            new Rectangle(centerX + 135, 360, 100, 100),
            friendlySourceRect,
            Color.White);
        spriteBatch.DrawString(content.ButtonFont, "Friendly (Ally)",
            new Vector2(centerX + 100, 450), Color.White);

        spriteBatch.DrawString(content.ButtonFont, "Shoot zombies to convert them into friendlies.",
            new Vector2(centerX - 200, 550), Color.White);
        spriteBatch.DrawString(content.ButtonFont, "Try to stay alive and get the highest score possible",
            new Vector2(centerX - 270, 580), Color.White);

        spriteBatch.Draw(_tutorialHealTexture, new Rectangle(centerX - 150, 625, 64, 64), Color.White);
        spriteBatch.DrawString(content.ButtonFont, "Primary Weapon",
            new Vector2(centerX - 158, 700), Color.White);

        spriteBatch.Draw(_tutorialDashTexture, new Rectangle(centerX + 30, 625, 64, 64), Color.White);
        spriteBatch.DrawString(content.ButtonFont, "Dash",
            new Vector2(centerX + 18, 700), Color.White);

        string controls =
            @"WASD  - Move
    M1 - Shoot
    1-5   - Use abilities";

        spriteBatch.DrawString(content.ButtonFont, controls,
            new Vector2(centerX - 120, 775), Color.White);

        string startText = "Press SPACE to start";
        Vector2 startSize = content.ButtonFont.MeasureString(startText);

        spriteBatch.DrawString(content.ButtonFont, startText,
            new Vector2(centerX - startSize.X / 2, 925),
            Color.Yellow);

        spriteBatch.End();
    }

    public void DrawSettings(SpriteBatch spriteBatch)
    {
        var game = GameManager.Get().Game;
        var content = ContentsManager.Get();

        spriteBatch.Begin();

        spriteBatch.Draw(content.BackgroundTexture,
            new Rectangle(0, 0,
                game.GraphicsDevice.Viewport.Width,
                game.GraphicsDevice.Viewport.Height),
            Color.White);

        string title = "Settings";
        Vector2 size = content.TitleFont.MeasureString(title);

        spriteBatch.DrawString(content.TitleFont, title, new Vector2( game.GraphicsDevice.Viewport.Width / 2 - size.X / 2, 150), Color.White);
        spriteBatch.DrawString(content.ButtonFont, "Resolution: ", new Vector2( game.GraphicsDevice.Viewport.Width / 2 - 150, 400), Color.White);
        spriteBatch.DrawString(content.ButtonFont, "Display Mode: ", new Vector2( game.GraphicsDevice.Viewport.Width / 2 - 150, 500), Color.White);

        _settingsBackButton.Draw(spriteBatch);

        _resolutionDropdownButton.Draw(spriteBatch);       
        _displayModeDropdownButton.Draw(spriteBatch);
        
        _applySettingsButton.Draw(spriteBatch);
        
        if (_resolutionDropdownOpen)
            foreach (var button in _resolutionButtons)
                button.Draw(spriteBatch);
        if (_displayModeDropdownOpen)
            foreach (var button in _displayModeButtons)
                button.Draw(spriteBatch);
        spriteBatch.End();
    }

    private void ApplySettings()
    {
        var res = _resolutions[_selectedResolutionIndex];

        ((TheCure)GameManager.Get().Game)
            .ApplyVideoSettings(res.width, res.height, _selectedDisplayMode);

        PositionButtons();
    }

    private void UpdateResolutionDropdownText()
    {
        var res = _resolutions[_selectedResolutionIndex];

        _resolutionDropdownButton.SetText(
            $"{res.width}x{res.height}"
        );
    }

    private void UpdateDisplayModeDropdownText()
    {
        _displayModeDropdownButton.SetText(
            _selectedDisplayMode.ToString()
        );
    }
}