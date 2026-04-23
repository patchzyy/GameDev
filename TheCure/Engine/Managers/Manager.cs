namespace TheCure.Managers;

public abstract class Manager<TManager>
    where TManager : Manager<TManager>, new()
{
    private static TManager? _instance;

    public static TManager Get()
    {
        return _instance ??= new TManager();
    }
}