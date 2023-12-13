namespace Common;

public static class StringExtensions
{
    public static (int Min, int Max) ParseRange(this string input, char separator = '-')
    {
        var parts = input.Split(separator, StringSplitOptions.TrimEntries);
        return (int.Parse(parts[0]), int.Parse(parts[1]));
    }
}