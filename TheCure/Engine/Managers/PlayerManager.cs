namespace TheCure.Managers;

public class PlayerManager : Manager<PlayerManager>
{
    public Player Player { get; private set; }

    public void Initialize(Player player)
    {
        Player = player;
    }
    
    public void ResetPlayer()
    {
        Player.Reset();
    }
}