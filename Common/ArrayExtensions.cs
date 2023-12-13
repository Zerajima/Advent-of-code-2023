namespace Common;

public static class ArrayExtensions
{
    public static int[,] BuildIntMap(this string[] input)
    {
        var map = new int[input.First().Length, input.Length];
        for (int x = 0; x < input.First().Length; x++)
        {
            for (int y = 0; y < input.Length; y++)
            {
                var line = input[y];
                var c = line[x];

                map[x, y] = int.Parse(c.ToString());
            }
        }

        return map;
    }
    
    public static char[,] BuildCharMap(this string[] input)
    {
        var map = new char[input.First().Length, input.Length];
        for (int x = 0; x < input.First().Length; x++)
        {
            for (int y = 0; y < input.Length; y++)
            {
                var line = input[y];
                var c = line[x];

                map[x, y] = c;
            }
        }

        return map;
    }
}