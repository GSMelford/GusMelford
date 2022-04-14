namespace GusMelfordBot.Core;

public static class TypeConversion
{
    public static int ToInt(this string value)
    {
        return int.TryParse(value, out int parseValue) ? parseValue : default;
    }
}