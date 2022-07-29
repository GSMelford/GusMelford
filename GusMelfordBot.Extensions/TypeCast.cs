namespace GusMelfordBot.Extensions;

public static class TypeCast
{
    public static int ToInt(this string? value)
    {
        if (string.IsNullOrEmpty(value)) {
            return default;
        }
        
        return int.TryParse(value, out int result) ? result : default;
    }
    
    public static DateTime ToDateTime(this string? value)
    {
        if (string.IsNullOrEmpty(value)) {
            return default;
        }
        
        return DateTime.TryParse(value, out DateTime result) ? result : default;
    }
}