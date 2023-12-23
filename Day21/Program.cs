using System.Drawing;
using Common;

namespace Day21;

internal static class Program
{
    private const string TestInputPath = "TestInput.txt";
    private const string RealInputPath = "RealInput.txt";
    
    static void Main(string[] args)
    {
        //Task1(RealInputPath, 64);
        Task2(RealInputPath, 26501365);
    }

    private static void Task1(string inputPath, long maxDistance)
    {
        var input = File.ReadAllLines(inputPath);
        var map = input.BuildCharMap();
        var startingPoint = LocateStart(map);
        
        var result = CountVisitedPoints(map, startingPoint, maxDistance);
        
        Console.WriteLine(result);
    }

    private static void Task2(string inputPath, long maxDistance)
    {
        checked
        {

            var input = File.ReadAllLines(inputPath);
            var map = input.BuildCharMap();
            var startingPoint = LocateStart(map);
            var width = map.GetLength(1);

            var firstDistance = width;
            var secondDistance = width + firstDistance;
            var thirdDistance = width + secondDistance;

            var p0 = CountVisitedPoints(map, startingPoint, firstDistance);
            var p1 = CountVisitedPoints(map, startingPoint, secondDistance);
            var p2 = CountVisitedPoints(map, startingPoint, thirdDistance);

            var c = p0;
            var a = (p2 - 2 * p1 + c) / 2;
            var b = p1 - a - c;

            // quadratic equation: p(n) = an2 + bn + c
            var n = maxDistance / width;
            var visitedForN = a * n * n + b * n + c;

            var nRemainder = maxDistance % width;
            var visitedForRemainder = CountVisitedPoints(map, startingPoint, nRemainder);

            Console.WriteLine($"{firstDistance} - {p0}");
            Console.WriteLine($"{secondDistance} - {p1}");
            Console.WriteLine($"{thirdDistance} - {p2}");
            Console.WriteLine(visitedForN + visitedForRemainder);
        }
    }

    private static long CountVisitedPoints(char[,] map, Point startingPoint, long maxDistance)
    {
        var visited = new HashSet<Point>();
        
        var queue = new PriorityQueue<Point, long>();
        queue.Enqueue(startingPoint, 0);
        visited.Add(startingPoint);
        var visitedPoints = new Dictionary<Point, long>();

        while (queue.TryDequeue(out var currentPoint, out var currentDistance))
        {
            if (currentDistance <= maxDistance)
            {
                visitedPoints.Add(currentPoint, currentDistance);
            }

            if(currentDistance >= maxDistance) continue;
            
            var possibleDestinations = new List<Point>
            {
                new(currentPoint.X + 1, currentPoint.Y),
                new(currentPoint.X - 1, currentPoint.Y),
                new(currentPoint.X, currentPoint.Y + 1),
                new(currentPoint.X, currentPoint.Y - 1),
            }.Where(destination=>
            {
                var normalized = GetNormalizedCoordinates(new Point(destination.X, destination.Y), map.GetLength(0),
                    map.GetLength(1));
                return map[normalized.X, normalized.Y] != '#';
            }).ToList();

            foreach (var possibleDestination in possibleDestinations)
            {
                if(visited.Contains(possibleDestination)) continue;
                
                queue.Enqueue(possibleDestination, currentDistance + 1);
                visited.Add(possibleDestination);
            }
        }

        var expectedRemainder = maxDistance % 2;
        return visitedPoints.Count(x => x.Value % 2 == expectedRemainder);
    }

    private static long GetRemainingVisits(long remainingDistance, List<long> distances)
    {
        var count = 0;
        foreach (var currentDistance in distances)
        {
            if (currentDistance <= remainingDistance)
            {
                count++;
                remainingDistance -= currentDistance;
            }
            else
            {
                break;
            }
        }

        return count;
    }

    private static List<long> GetDistanceDeltas(List<long> distances)
    {
        var distanceDeltas = new List<long>();
        var previous = 0L;
        foreach (var distance in distances)
        {
            var delta = distance - previous;
            distanceDeltas.Add(delta);
            previous = distance;
            //Console.WriteLine(delta);
        }

        return distanceDeltas;
    }

    private static void DrawReachable(char[,] floodedMap, HashSet<Point> points)
    {
        for (int y = 0; y < floodedMap.GetLength(1); y++)
        {
            for (int x = 0; x < floodedMap.GetLength(0); x++)
            {
                if (points.Contains(new Point(x, y))) Console.Write("O");
                else Console.Write(floodedMap[x, y]);
            }

            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine();
    }

    private static Point LocateStart(char[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == 'S') return new Point(x, y);
            }
        }

        throw new Exception();
    }

    private static Point GetNormalizedCoordinates(Point point, int width, int height)
    {
        var actualX = point.X < 0 ? width - (Math.Abs(point.X) % width) : point.X;
        actualX = actualX >= width ? 0 + (actualX % width) : actualX;
        var actualY = point.Y < 0 ? height - (Math.Abs(point.Y) % height) : point.Y;
        actualY = actualY >= height ? 0 + (actualY % height) : actualY;
        return new Point(actualX, actualY);
    }
}