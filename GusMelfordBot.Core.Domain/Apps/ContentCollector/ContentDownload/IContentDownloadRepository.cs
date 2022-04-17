namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.ContentDownload;

public interface IContentDownloadRepository
{
    Task<DAL.Applications.ContentCollector.Content?> GetContent(Guid contentId);
    Task SetIsNotValid(Guid contentId);
}