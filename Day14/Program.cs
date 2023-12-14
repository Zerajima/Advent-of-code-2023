using System.Diagnostics;
using System.Drawing;
using Common;

namespace Day1;

internal static class Program
{
    private const string TestInputPath = "TestInput.txt";
    private const string RealInputPath = "RealInput.txt";

    static void Main(string[] args)
    {
        //Task1(RealInputPath);
        Task2(RealInputPath);
    }

    private static void Task1(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);
        var map = input.BuildCharMap();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            MoveRocks(map, Enumerable.Range(0, map.GetLength(1)).Select(y => new Point(x, y)).ToList());
        }

        DrawMap(map);
        
        var score = GetTotalWeight(map);

        Console.WriteLine(score);
    }

    private static void Task2(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);
        var map = input.BuildCharMap();

        var results = new List<long>();
        for (int loopCounter = 0; loopCounter < 10000; loopCounter++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                MoveRocks(map, Enumerable.Range(0, map.GetLength(1)).Select(y => new Point(x, y)).ToList());
            }

            for (int y = 0; y < map.GetLength(1); y++)
            {
                MoveRocks(map, Enumerable.Range(0, map.GetLength(0)).Select(x => new Point(x, y)).ToList());
            }

            for (int x = 0; x < map.GetLength(0); x++)
            {
                MoveRocks(map,
                    Enumerable.Range(0, map.GetLength(1)).Select(y => new Point(x, map.GetLength(1) - 1 - y))
                        .ToList());
            }

            for (int y = 0; y < map.GetLength(1); y++)
            {
                MoveRocks(map,
                    Enumerable.Range(0, map.GetLength(0)).Select(x => new Point(map.GetLength(1) - 1 - x, y))
                        .ToList());
            }

            results.Add(GetTotalWeight(map));
        }
        
        var score = GetFinalResult(results, 1000000000L);

        Console.WriteLine(score);
    }

    private static long GetFinalResult(List<long> results, long numberOfRepetitions)
    {
        var windowSize = 100;
        
        var slow = results.Take(windowSize);
        var fast = results.Skip(windowSize).Take(windowSize);
        var slowIndex = 0;
        var fastIndex = 0;
        while (!slow.SequenceEqual(fast))
        {
            slowIndex++;
            fastIndex = slowIndex * 2;
            slow = results.Skip(slowIndex).Take(windowSize);
            fast = results.Skip(fastIndex).Take(windowSize);
        }

        var firstRepetition = 0;
        slow = results.Take(windowSize);
        slowIndex = 0;
        while (!slow.SequenceEqual(fast))
        {
            slowIndex++;
            fastIndex++;
            slow = results.Skip(slowIndex).Take(windowSize);
            fast = results.Skip(fastIndex).Take(windowSize);
            firstRepetition++;
        }

        var cycleLength = 1;
        fastIndex = slowIndex + 1;
        fast = results.Skip(fastIndex).Take(windowSize);
        while (!slow.SequenceEqual(fast))
        {
            fastIndex++;
            fast = results.Skip(fastIndex).Take(windowSize);
            cycleLength++;
        }
        
        Console.WriteLine($"First cycle repetition: {firstRepetition} - {results[firstRepetition]}");
        Console.WriteLine($"Cycle length: {cycleLength}");

        var numberOfCycles = (numberOfRepetitions - firstRepetition) / cycleLength;
        Console.WriteLine($"Number of cycles = {numberOfCycles}");
        
        var remainderAfterLastCycle = numberOfRepetitions - firstRepetition - numberOfCycles * cycleLength;
        Console.WriteLine($"Remainder after last cycle = {remainderAfterLastCycle}");
        
        var calculatedResult = results[firstRepetition + (int)remainderAfterLastCycle - 1];
        return calculatedResult;
    }

    private static void MoveRocks(char[,] map, IList<Point> points)
    {
        var emptyPositions = new Queue<Point>();
        foreach (var point in points)
        {
            var currentValue = map[point.X, point.Y];
            if (currentValue == '.') emptyPositions.Enqueue(point);
            else if (currentValue == '#') emptyPositions.Clear();
            else if (currentValue == 'O' && emptyPositions.Any())
            {
                var bestEmptyPosition = emptyPositions.Dequeue();
                map[point.X, point.Y] = '.';
                map[bestEmptyPosition.X, bestEmptyPosition.Y] = 'O';
                emptyPositions.Enqueue(point);
            }
        }
    }

    private static long GetTotalWeight(char[,] map)
    {
        var score = 0L;
        var mapHeight = map.GetLength(1);
        for (int y = 0; y < mapHeight; y++)
        {
            var weight = mapHeight - y;
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y] == 'O') score += weight;
            }
        }

        return score;
    }

    private static Point MoveRockNorth(char[,] map, Point roundedRock)
    {
        var bestLocation = roundedRock;
        for (int y = roundedRock.Y - 1; y >= 0; y--)
        {
            if (map[roundedRock.X, y] == '.')
            {
                bestLocation = roundedRock with { Y = y };
            }
            else
            {
                return bestLocation;
            }
        }

        return bestLocation;
    }

    private static Point MoveRockSouth(char[,] map, Point roundedRock)
    {
        var bestLocation = roundedRock;
        for (int y = roundedRock.Y + 1; y < map.GetLength(1); y++)
        {
            if (map[roundedRock.X, y] == '.')
            {
                bestLocation = roundedRock with { Y = y };
            }
            else
            {
                return bestLocation;
            }
        }

        return bestLocation;
    }
    
    private static Point MoveRockWest(char[,] map, Point roundedRock)
    {
        var bestLocation = roundedRock;
        for (int x = roundedRock.X - 1; x >= 0; x--)
        {
            if (map[x, roundedRock.Y] == '.')
            {
                bestLocation = roundedRock with { X = x };
            }
            else
            {
                return bestLocation;
            }
        }

        return bestLocation;
    }
    
    private static Point MoveRockEast(char[,] map, Point roundedRock)
    {
        var bestLocation = roundedRock;
        for (int x = roundedRock.X + 1; x < map.GetLength(0); x++)
        {
            if (map[x, roundedRock.Y] == '.')
            {
                bestLocation = roundedRock with { X = x };
            }
            else
            {
                return bestLocation;
            }
        }

        return bestLocation;
    }

    private static List<Point> GetRoundedRocks(char[,] map)
    {
        var result = new List<Point>();
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y] == 'O')
                {
                    result.Add(new Point(x, y));
                }
            }
        }

        return result;
    }

    private static void DrawMap(char[,] floodedMap)
    {
        for (long y = 0; y < floodedMap.GetLength(1); y++)
        {
            for (long x = 0; x < floodedMap.GetLength(0); x++)
            {
                Console.Write(floodedMap[x, y]);
            }

            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine();
    }
}