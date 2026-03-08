using System;

namespace nodejs;

internal static class JsNumeric
{
    public static int ToExactInt32(double value, string paramName)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new ArgumentOutOfRangeException(paramName, "Value must be finite.");
        }

        var truncated = Math.Truncate(value);
        if (truncated != value)
        {
            throw new ArgumentOutOfRangeException(paramName, "Value must be an exact integer.");
        }

        if (truncated < int.MinValue || truncated > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                paramName,
                $"Value must be within Int32 range ({int.MinValue} to {int.MaxValue}).");
        }

        return (int)truncated;
    }

    public static int ToPort(double value, string paramName)
    {
        var port = ToExactInt32(value, paramName);
        if (port < 0 || port > 65535)
        {
            throw new ArgumentOutOfRangeException(paramName, "Port must be between 0 and 65535.");
        }

        return port;
    }

    public static double RequireFiniteNonNegative(double value, string paramName)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, "Value must be finite and non-negative.");
        }

        return value;
    }
}
