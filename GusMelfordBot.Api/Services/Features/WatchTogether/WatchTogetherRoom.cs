using GusMelfordBot.Domain.Application.ContentCollector;

namespace GusMelfordBot.Api.Services.Features.WatchTogether;

public class WatchTogetherRoom
{
    public string RoomCode { get; }
    private List<ContentCollectorUser> Users { get; set; } = new ();
    private readonly List<ContentDomain> _contents;
    private int _cursor;
    private bool _isPause;
    private int _rotate;

    public WatchTogetherRoom(string roomCode, List<ContentDomain> contents)
    {
        RoomCode = roomCode;
        _contents = contents;
    }

    public IEnumerable<ContentCollectorUser> GetUsers()
    {
        return Users;
    }

    public void AddUser(ContentCollectorUser newContentCollectorUser)
    {
        ContentCollectorUser? contentCollectorUser = Users.FirstOrDefault(x => x.Id == newContentCollectorUser.Id);
        if (contentCollectorUser is null)
        {
            Users.Add(newContentCollectorUser);
        }
    }
    
    public void RemoveUser(Guid userId)
    {
        ContentCollectorUser? contentCollectorUser = Users.FirstOrDefault(x => x.Id == userId);
        
        if (contentCollectorUser is not null) {
            Users.Remove(contentCollectorUser);
        }
    }

    public bool IsRoomEmpty()
    {
        return !Users.Any();
    }
    
    public bool IsUserExist(Guid userId)
    {
        return Users.FirstOrDefault(x => x.Id == userId) != null;
    }
    
    public void SetReady(Guid userId)
    {
        ContentCollectorUser? contentCollectorUser = Users.FirstOrDefault(x => x.Id == userId);
        if (contentCollectorUser is null) {
            return;
        }

        contentCollectorUser.IsReady = true;
    }
    
    public ContentDomain GetContentInfo()
    {
        return _contents[_cursor];
    }

    public void Next()
    {
        int tempCursor = _cursor + 1;
        if (tempCursor < _contents.Count)
        {
            _cursor++;
            _rotate = 0;
        }
        else if (tempCursor == _contents.Count)
        {
            _cursor = 0;
        }
    }
    
    public void Prev()
    {
        int tempCursor = _cursor - 1;
        if (tempCursor > 0)
        {
            _cursor--;
            _rotate = 0;
        }
        else
        {
            _cursor = _contents.Count - 1;
        }
    }

    public bool ChangePause()
    {
        return _isPause = !_isPause;
    }

    public int ChangeRotate()
    {
        _rotate += 90;
        if (_rotate > 360)
        {
            _rotate = 0;
        }

        return _rotate;
    }
}