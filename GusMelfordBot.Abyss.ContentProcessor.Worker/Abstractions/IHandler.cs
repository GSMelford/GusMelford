using System.Threading.Tasks;

namespace ContentProcessor.Worker.Abstractions;

public interface IHandler<T>
{
    IHandler<T> SetNext(IHandler<T> handler);
    Task<T?> HandleAsync(T processTikTokContent);
}