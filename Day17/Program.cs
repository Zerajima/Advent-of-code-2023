using System.Drawing;
namespace Day17;

using Common;

internal static class Program
{
    private const string TestInputPath = "TestInput.txt";
    private const string RealInputPath = "RealInput.txt";

    static void Main(string[] args)
    {
        //Task(RealInputPath, 0, 3);
        Task(RealInputPath, 4, 10);
    }

    private static void Task(string inputPath, int minMovesInDirection, int maxMovesInDirection)
    {
        var input = File.ReadAllLines(inputPath);
        var map = input.BuildIntMap();

        var start = new Point(0, 0);
        var finish = new Point(map.GetLength(0) - 1, map.GetLength(1) - 1);

        var bestDistances = new Dictionary<(Point Point, Directions Direction, int MovesInDirection), int>();
        
        var queue = new PriorityQueue<(Point Point, Directions Direction, int MovesInDirection), int>();
        queue.Enqueue((start, Directions.Right, 0), 0);

        while (queue.TryDequeue(out var current, out var currentDistance))
        {
            //Console.WriteLine(queue.Count);

            if (current.Point == finish && current.MovesInDirection >= minMovesInDirection)
            {
                Console.WriteLine(currentDistance);
                return;
            };

            var neighbors = GetNeighbors(current.Point, current.Direction, current.MovesInDirection, minMovesInDirection, maxMovesInDirection).Where(x => CanMoveTo(x.Point, finish));

            foreach (var neighbor in neighbors)
            {
                var distanceToNeighbor = map[neighbor.Point.X, neighbor.Point.Y];
                var totalDistance = currentDistance + distanceToNeighbor;

                var newQueueItem = (neighbor.Point, neighbor.Direction, neighbor.MovesInSameDirection);
                if (!bestDistances.TryGetValue(newQueueItem, out var bestDistance) || totalDistance < bestDistance)
                {
                    bestDistances[newQueueItem] = totalDistance;
                    queue.Enqueue(newQueueItem, totalDistance);
                }
            }
        }
    }

    private static void DrawMap(int[,] map, Dictionary<Point, int> bestDistances)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                var point = new Point(x, y);
                if (bestDistances[point] == int.MaxValue) Console.Write("--- ");
                else Console.Write($"{bestDistances[point]:d3} ");
            }

            Console.WriteLine();
        }
    }

    private static void Task2(string inputPath)
    {
        var score = 0;
        var input = File.ReadAllLines(inputPath);

        List<List<char>> digits = new List<List<char>>();
        foreach (var line in input)
        {

        }

        Console.WriteLine(score);
    }

    private static List<(Point Point, Directions Direction, int MovesInSameDirection)> GetNeighbors(Point currentPoint,
        Directions previousDirection, int movesInPreviousDirection, int minMovesInDirection, int maxMovesInDirection)
    {
        var list = new List<(Point Point, Directions Direction, int MovesInDirection)>(4);

        var canChangeDirection = movesInPreviousDirection >= minMovesInDirection;
        var mustChangeDirection = movesInPreviousDirection >= maxMovesInDirection;

        if (canChangeDirection)
        {
            if (previousDirection != Directions.Left && (!mustChangeDirection || previousDirection != Directions.Right))
                list.Add((new(currentPoint.X + 1, currentPoint.Y), Directions.Right,
                    previousDirection == Directions.Right ? movesInPreviousDirection + 1 : 1));

            if (previousDirection != Directions.Right && (!mustChangeDirection || previousDirection != Directions.Left))
                list.Add((new(currentPoint.X - 1, currentPoint.Y), Directions.Left,
                    previousDirection == Directions.Left ? movesInPreviousDirection + 1 : 1));

            if (previousDirection != Directions.Up && (!mustChangeDirection || previousDirection != Directions.Down))
                list.Add((new(currentPoint.X, currentPoint.Y + 1), Directions.Down,
                    previousDirection == Directions.Down ? movesInPreviousDirection + 1 : 1));

            if (previousDirection != Directions.Down && (!mustChangeDirection || previousDirection != Directions.Up))
                list.Add((new(currentPoint.X, currentPoint.Y - 1), Directions.Up,
                    previousDirection == Directions.Up ? movesInPreviousDirection + 1 : 1));
        }
        else
        {
            if (previousDirection == Directions.Right) list.Add((new (currentPoint.X + 1, currentPoint.Y), Directions.Right, movesInPreviousDirection + 1));
            if (previousDirection == Directions.Left) list.Add((new (currentPoint.X - 1, currentPoint.Y), Directions.Left, movesInPreviousDirection + 1));
            if (previousDirection == Directions.Down) list.Add((new (currentPoint.X, currentPoint.Y + 1), Directions.Down, movesInPreviousDirection + 1));
            if (previousDirection == Directions.Up) list.Add((new (currentPoint.X, currentPoint.Y - 1), Directions.Up, movesInPreviousDirection + 1));
        }

        return list;
    }

    private static bool CanMoveTo(Point point, Point maxPoint)
    {
        return point.X >= 0 && point.X <= maxPoint.X
                            && point.Y >= 0 && point.Y <= maxPoint.Y;
    }

    private static int GetDiagonalPath(int[,] map)
    {
        var heatLoss = -map[0,0];
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                heatLoss += map[x, y];
            }
        }

        return heatLoss;
    }
}

public enum Directions
{
    Undefined,
    Right,
    Left,
    Up,
    Down,
}