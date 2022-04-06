using System;

namespace GusMelfordBot.Core.Applications;

public static class Helper
{
    public static string GetRandomEmoji()
    {
        return Constants.EmojiList[new Random().Next(0, Constants.EmojiList.Count)];
    }
}