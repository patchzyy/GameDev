using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheCure.Managers;

public class ScreenManager : Manager<ScreenManager>
{
    private Button _startButton;
    private Button _quitButton;
    private Button _continueButton;
    private Button _pauseQuitButton;
    private Button _restartButton;

    private Texture2D _tutorialPlayerTexture;
    private Texture2D _tutorialZombieTexture;
    private Texture2D _tutorialFriendlyTexture;
    private Texture2D _tutorialThrowTexture;
    private Texture2D _tutorialDashTexture;

    public void Load()
    {
        LoadTutorialContent();
        CreateButtons();
        PositionButtons();
    }

    private void LoadTutorialContent()
    {
        var content = ContentsManager.Get().GetContent();

        _tutorialPlayerTexture = content.Load<Texture2D>("Character-Joe-Idle");
        _tutorialZombieTexture = content.Load<Texture2D>("Zombie-Atk");
        _tutorialFriendlyTexture = content.Load<Texture2D>("Character-Unknown-Idle");
        _tutorialThrowTexture = content.Load<Texture2D>("Throw");
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
            _quitButton.Update(mouseState);

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
            {
                gameManager.SetGameState(GameState.Playing);
            }

            _continueButton.Update(mouseState);
            _pauseQuitButton.Update(mouseState);

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

        _startButton = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Start",
            ContentsManager.Get().ButtonFont);
        _quitButton = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Quit",
            ContentsManager.Get().ButtonFont);
        _continueButton = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Continue",
            ContentsManager.Get().ButtonFont);
        _pauseQuitButton = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Quit",
            ContentsManager.Get().ButtonFont);
        _restartButton = new Button(new Rectangle(0, 0, buttonWidth, buttonHeight), "Opnieuw spelen",
            ContentsManager.Get().ButtonFont);

        var gameManager = GameManager.Get();

        _startButton.SetAction(() => gameManager.SetGameState(GameState.Tutorial));
        _quitButton.SetAction(gameManager.Game.Exit);
        _continueButton.SetAction(() => gameManager.SetGameState(GameState.Playing));
        _pauseQuitButton.SetAction(gameManager.Game.Exit);
        _restartButton.SetAction(RestartButtonAction);
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

        string scoreText = $"Eindscore: {ScoreManager.Get().GetScore()}";
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

        string pauseText = "Game gepauzeerd";
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
        _pauseQuitButton.Draw(spriteBatch);

        spriteBatch.End();
    }

    private void DrawPauseControls(SpriteBatch spriteBatch)
    {
        var game = GameManager.Get().Game;
        var content = ContentsManager.Get();

        int centerXPaused = game.GraphicsDevice.Viewport.Width / 4;

        spriteBatch.Draw(_tutorialThrowTexture, new Rectangle(centerXPaused - 450, 825, 64, 64), Color.White);
        spriteBatch.DrawString(content.ButtonFont, "Wapen 1",
            new Vector2(centerXPaused - 450, 900), Color.White);

        spriteBatch.Draw(_tutorialDashTexture, new Rectangle(centerXPaused - 350, 825, 64, 64), Color.White);
        spriteBatch.DrawString(content.ButtonFont, "Wapen 2",
            new Vector2(centerXPaused - 350, 900), Color.White);

        string controlsPaused =
@"WASD  - Bewegen
MUIS  - Schieten
1-2   - Wissel wapens";

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
        spriteBatch.DrawString(content.ButtonFont, "Jij (Player)",
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
        spriteBatch.DrawString(content.ButtonFont, "Friendly (Helper)",
            new Vector2(centerX + 100, 450), Color.White);

        spriteBatch.DrawString(content.ButtonFont, "Schiet zombies -> maak friendlies",
            new Vector2(centerX - 200, 550), Color.White);

        spriteBatch.Draw(_tutorialThrowTexture, new Rectangle(centerX - 150, 625, 64, 64), Color.White);
        spriteBatch.DrawString(content.ButtonFont, "Wapen 1",
            new Vector2(centerX - 158, 700), Color.White);

        spriteBatch.Draw(_tutorialDashTexture, new Rectangle(centerX + 30, 625, 64, 64), Color.White);
        spriteBatch.DrawString(content.ButtonFont, "Wapen 2",
            new Vector2(centerX + 18, 700), Color.White);

        string controls =
@"WASD  - Bewegen
MUIS  - Schieten
1-2   - Wissel wapens";

        spriteBatch.DrawString(content.ButtonFont, controls,
            new Vector2(centerX - 120, 775), Color.White);

        string startText = "Druk op SPATIE om te starten";
        Vector2 startSize = content.ButtonFont.MeasureString(startText);

        spriteBatch.DrawString(content.ButtonFont, startText,
            new Vector2(centerX - startSize.X / 2, 925),
            Color.Yellow);

        spriteBatch.End();
    }
}