using System.Diagnostics.CodeAnalysis;

namespace GusMelfordBot.Extensions;

public static class FunctionalMethod
{
    public static T IfNullThrow<T>([NotNull] this T? value, Exception? exception = null)
    {
        if (value is null)
        {
            throw exception ?? new Exception();
        }

        return value;
    }
}