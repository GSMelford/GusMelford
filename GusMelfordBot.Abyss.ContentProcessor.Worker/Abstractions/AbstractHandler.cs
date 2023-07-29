using System.Threading.Tasks;

namespace ContentProcessor.Worker.Abstractions;

public abstract class AbstractHandler<T> : IHandler<T>
{
    private IHandler<T>? _nextHandler;

    public IHandler<T> SetNext(IHandler<T> handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public virtual async Task<T?> HandleAsync(T processTikTokContent)
    {
        if (_nextHandler is not null)
        {
            return await _nextHandler.HandleAsync(processTikTokContent);
        }
        
        return processTikTokContent;
    }
}