namespace GusMelfordBot.Domain.Application;

public interface IFeatureRepository
{
    Task<Feature> GetFeatureAsync(long chatId);
}