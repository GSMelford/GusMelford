namespace GusMelfordBot.Database.Interfaces
{
    using Context;

    public interface IDatabaseManager
    {
        ApplicationContext Context { get; }
    }
}