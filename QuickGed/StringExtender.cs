namespace QuickGed;

/// <summary>
/// Represents a line from the GEDCOM file parsed into its properties.
/// </summary>
public static class StringExtender
{
    public static bool IsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsSpecified(this string value)
    {
        return !string.IsNullOrEmpty(value);
    }
}