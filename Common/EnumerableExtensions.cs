namespace Common;

public static class EnumerableExtensions
{
    public static IEnumerable<T[]> GroupByIndex<T>(this IEnumerable<T> input, int groupSize)
    {
        var groups = input
            .Select((value, index) => new { PairNum = index / groupSize, value })
            .GroupBy(pair => pair.PairNum)
            .Select(grp => grp.Select(g => g.value).ToArray());
        return groups;
    }
}