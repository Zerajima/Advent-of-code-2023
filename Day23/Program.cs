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
        SolveMaxPathLength(map);
    }

    private static void Task2(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);
        var map = input.BuildCharMap();
        ReplaceSlopes(map);
        SolveMaxPathLength(map);
    }

    private static void ReplaceSlopes(char[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] != '#') map[x, y] = '.';
            }
        }
    }

    private static void SolveMaxPathLength(char[,] map)
    {
        var startingPoint = LocateStart(map);
        var targetY = map.GetLength(1) - 1;

        var edges = BuildGraph(map, startingPoint);

        var encountered = new Stack<(Point point, int length, HashSet<Point> visited)>();
        encountered.Push((startingPoint, 0, new HashSet<Point>()));

        var maxLength = 0;
        while (encountered.Any())
        {
            var current = encountered.Pop();
            if (current.point.Y == targetY && current.length > maxLength) maxLength = current.length;

            foreach (var neighbor in edges[current.point].ConnectedEdges)
            {
                if (!current.visited.Contains(neighbor.edge))
                {
                    encountered.Push((neighbor.edge, current.length + neighbor.length, [..current.visited, current.point]));
                }
            }
        }

        Console.WriteLine(maxLength);
    }

    private static Dictionary<Point, Edge> BuildGraph(char[,] map, Point startingPoint)
    {
        Dictionary<Point, Edge> edges = new Dictionary<Point, Edge>();
        edges.Add(startingPoint, new Edge(startingPoint));

        var encountered = new Stack<Point>();
        encountered.Push(startingPoint);
        
        var visited = new HashSet<Point> { startingPoint };

        while (encountered.Any())
        {
            var current = encountered.Pop();

            foreach (var neighbor in GetConnectedNodes(map, current))
            {
                edges[current].ConnectedEdges.Add((neighbor.location, neighbor.distance));
                edges.TryAdd(neighbor.location, new Edge(neighbor.location));

                if (neighbor.reversible)
                {
                    edges[neighbor.location].ConnectedEdges.Add((current, neighbor.distance));
                }

                if (visited.Add(neighbor.location))
                {
                    encountered.Push(neighbor.location);
                }
            }
        }

        return edges;
    }

    private static List<(Point location, int distance, bool reversible)> GetConnectedNodes(char[,] map, Point current)
    {
        if (current.Y == map.GetLength(1) - 1) return new List<(Point location, int distance, bool reversible)>();
        
        var neighbors = GetNeighbors(map, current).ToList();

        var result = new List<(Point location, int distance, bool reversible)>();
        foreach (var neighbor in neighbors)
        {
            result.Add(FollowUntilNextNode(map, neighbor, current));
        }

        return result;
    }

    private static (Point location, int distance, bool reversible) FollowUntilNextNode(char[,] map, Point current, Point previous)
    {
        var visited = new HashSet<Point> { current, previous };
        var neighbors = GetNeighbors(map, current).Where(x=>!visited.Contains(x)).ToList();
        var reversible = true;
        
        while (neighbors.Count == 1)
        {
            current = neighbors.First();
            visited.Add(current);
            neighbors = GetNeighbors(map, current).Where(x=>!visited.Contains(x)).ToList();
            if (map[current.X, current.Y] != '.') reversible = false;
        }

        return (current, visited.Count - 1, reversible);
    }

    private static List<Point> GetNeighbors(char[,] map, Point current)
    {
        var width = map.GetLength(0);
        var height = map.GetLength(1);

        char[] allowedLeft = ['.', '<'];
        char[] allowedRight = ['.', '>'];
        char[] allowedUp = ['.', '^'];
        char[] allowedDown = ['.', 'v'];

        var candidates = new List<Point>();
        if (current.X > 0 && allowedLeft.Contains(map[current.X - 1, current.Y]))
            candidates.Add(current with { X = current.X - 1 });
        if (current.X < width - 1 && allowedRight.Contains(map[current.X + 1, current.Y]))
            candidates.Add(current with { X = current.X + 1 });

        if (current.Y > 0 && allowedUp.Contains(map[current.X, current.Y - 1]))
            candidates.Add(current with { Y = current.Y - 1 });
        if (current.Y < height - 1 && allowedDown.Contains(map[current.X, current.Y + 1]))
            candidates.Add(current with { Y = current.Y + 1 });

        return candidates;
    }

    private static Point LocateStart(char[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            if (map[x, 0] == '.') return new Point(x, 0);
        }

        throw new Exception("start not found");
    }
}

public class Edge
{
    public Edge(Point location)
    {
        Location = location;
        ConnectedEdges = new HashSet<(Point edge, int length)>();
    }

    public Point Location { get; set; }
    
    public HashSet<(Point edge, int length)> ConnectedEdges { get; set; }
}