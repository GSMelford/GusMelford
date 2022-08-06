using GusMelfordBot.Domain.Application.ContentCollector;

namespace GusMelfordBot.Api.Services.Applications.ContentCollector;

public class ContentCollectorRoom
{
    public List<string> Users { get; set; } = new ();
    private readonly List<ContentDomain> _contents;
    private int _cursor = -1;
    private bool _isPause;
    private int _route;

    public ContentCollectorRoom(List<ContentDomain> contents)
    {
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
        else if (tempCursor == -1)
        {
            _cursor = _contents.Count - 1;
        }
    }

    public bool ChangePause()
    {
        return _isPause = !_isPause;
    }

    public int ChangeRoute()
    {
        _route += 90;
        if (_route > 360)
        {
            _route = 0;
        }

        return _route;
    }
}