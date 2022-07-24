namespace ContentCollector.MircoService.Domain.ContentProviders;

public interface IContentProviderService
{
    ContentProvider Define(string message);
}