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

    public void Load()
    {
        CreateButtons();
        PositionButtons();
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
        _startButton.SetAction(() => gameManager.SetGameState(GameState.Playing));
        _quitButton.SetAction(gameManager.Game.Exit);
        _continueButton.SetAction(() => gameManager.SetGameState(GameState.Playing));
        _pauseQuitButton.SetAction(GameManager.Get().Game.Exit);
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

        _continueButton.Draw(spriteBatch);
        _pauseQuitButton.Draw(spriteBatch);

        spriteBatch.End();
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
}