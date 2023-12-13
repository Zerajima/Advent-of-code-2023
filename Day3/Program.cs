using System.Drawing;
using Common;

namespace Day3;

internal static class Program
{
    private const string TestInputPath = "TestInput.txt";
    private const string RealInputPath = "RealInput.txt";

    static void Main(string[] args)
    {
        Task1(RealInputPath);
        Task2(RealInputPath);
    }

    private static void Task1(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);

        var map = input.BuildCharMap();
        var (numberRanges, symbols) = ScanMap(map);

        var result = 0;
        foreach (var range in numberRanges)
        {
            foreach (var symbol in symbols)
            {
                if (range.Any(point => point.IsTouching(symbol)))
                {
                    result += ParseRange(range, map);
                }
            }
        }

        Console.WriteLine(result);
    }

    private static void Task2(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);

        var map = input.BuildCharMap();
        var (numberRanges, symbols) = ScanMap(map);

        var result = 0;
        foreach (var symbol in symbols)
        {
            var validRanges = numberRanges.Where(range => range.Any(point => point.IsTouching(symbol))).ToList();
            if (validRanges.Count == 2)
            {
                result += ParseRange(validRanges[0], map) * ParseRange(validRanges[1], map);
            }
        }

        Console.WriteLine(result);
    }

    private static int ParseRange(List<Point> range, char[,] map)
    {
        var numString = "";
        foreach (var validPoint in range)
        {
            numString += map[validPoint.X, validPoint.Y];
        }

        return int.Parse(numString);
    }

    private static (List<List<Point>> numberRanges, List<Point> symbols) ScanMap(char[,] map)
    {
        var numberRanges = new List<List<Point>>();
        var symbols = new List<Point>();

        for (int y = 0; y < map.GetLength(1); y++)
        {
            var numberPoints = new List<Point>();
            for (int x = 0; x < map.GetLength(0); x++)
            {
                var character = map[x, y];
                if (char.IsDigit(character))
                {
                    numberPoints.Add(new Point(x, y));
                }
                else
                {
                    if (character != '.')
                    {
                        symbols.Add(new Point(x, y));
                    }

                    if (numberPoints.Any())
                    {
                        numberRanges.Add(numberPoints);
                        numberPoints = new List<Point>();
                    }
                }
            }

            if (numberPoints.Any())
            {
                numberRanges.Add(numberPoints);
            }
        }

        return (numberRanges, symbols);
    }
}