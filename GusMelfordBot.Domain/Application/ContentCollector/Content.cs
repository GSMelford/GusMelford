namespace GusMelfordBot.Domain.Application.ContentCollector;

public class Content
{
    public Guid Id { get; }
    public Guid GroupId { get; }
    public List<Guid> UserIds { get; }
    public string Provider { get; }
    public string OriginalLink { get; }
    public MetaContent MetaContent { get; }
    public List<UserComment> UserComments { get; }

    public Content(
        Guid id, 
        Guid groupId, 
        List<Guid> userIds, 
        string provider, 
        string originalLink, 
        MetaContent metaContent, 
        List<UserComment> userComments)
    {
        Id = id;
        GroupId = groupId;
        UserIds = userIds;
        Provider = provider;
        OriginalLink = originalLink;
        MetaContent = metaContent;
        UserComments = userComments;
    }
}