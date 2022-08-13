using GusMelfordBot.Domain.Application.ContentCollector;

namespace GusMelfordBot.Api.Services.Applications.ContentCollector;

public class ContentCollectorRoom
{
    public string RoomCode { get; }
    public List<string> Users { get; set; } = new ();
    private readonly List<ContentDomain> _contents;
    private int _cursor;
    private bool _isPause;
    private int _rotate;

    public ContentCollectorRoom(string roomCode, List<ContentDomain> contents)
    {
        RoomCode = roomCode;
        _contents = contents;
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