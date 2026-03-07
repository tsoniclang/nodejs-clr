namespace nodejs;

internal static class StatTime
{
    public static Tsonic.JSRuntime.Date ToJsDate(DateTime value)
    {
        return new Tsonic.JSRuntime.Date(ToUnixMilliseconds(value));
    }

    public static double ToUnixMilliseconds(DateTime value)
    {
        return new DateTimeOffset(value.ToUniversalTime()).ToUnixTimeMilliseconds();
    }
}
