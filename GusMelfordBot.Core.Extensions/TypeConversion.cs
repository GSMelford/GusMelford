using Newtonsoft.Json;

namespace GusMelfordBot.Core.Extensions;

public static class TypeConversion
{
    public static int ToInt(this string? value)
    {
        return int.TryParse(value, out int number) ? number : default;
    }
    
    public static DateTime? ToDateTime(this string? value)
    {
        return DateTime.TryParse(value, out DateTime dateTime) ? dateTime : null;
    }
    
    public static bool ToBool(this string? value)
    {
        return bool.TryParse(value, out bool boolean) ? boolean : default;
    }

    public static Guid ToGuid(this string? value)
    {
        return Guid.TryParse(value, out Guid guid) ? guid : default;
    }

    public static List<string>? ToList(this string? value)
    {
        return string.IsNullOrEmpty(value) ? null : value.Split(",").ToList();
    }

    public static T? ToObject<T>(this string? value) where T : new()
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(value!);
        }
        catch
        {
            return new T();
        }
    }
}