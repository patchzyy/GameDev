using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TheCure.Managers;

public class ContentsManager : Manager<ContentsManager>
{
    private ContentManager _content;

    public Texture2D BackgroundTexture;
    public Texture2D BackgroundGamePlayTexture;
    public Texture2D BackgroundPauseTexture;
    public Texture2D BackgroundGameOverTexture;
    
    public Texture2D DummyTexture;
    
    public SpriteFont HUDFont;
    public SpriteFont TitleFont;
    public SpriteFont ButtonFont;

    public void Initialize(ContentManager contentManager, Game game)
    {
        _content = contentManager;
        DummyTexture = new(game.GraphicsDevice, 1, 1);
        DummyTexture.SetData(new[] { Color.White });
    }

    public ContentManager GetContent()
    {
        return _content;
    }

    public void Load()
    {
        BackgroundTexture = _content.Load<Texture2D>("ZombieBackground");
        BackgroundGamePlayTexture = _content.Load<Texture2D>("BackGround");
        BackgroundPauseTexture = _content.Load<Texture2D>("BackgroundPause");
        BackgroundGameOverTexture = _content.Load<Texture2D>("GameOverBackground");
        TitleFont = _content.Load<SpriteFont>("TitleFont");
        ButtonFont = _content.Load<SpriteFont>("ButtonFont");
        HUDFont = _content.Load<SpriteFont>("HudFont");
    }
}