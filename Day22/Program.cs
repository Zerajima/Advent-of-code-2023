using System.Drawing;
using Common;

namespace Day22;

using Point3D = (int X, int Y, int Z);

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
        
        var bricks = ParseInput(input);
        bricks = MoveBricksToBottom(bricks);
        var result = CountDestructibleBricks(bricks);
        
        Console.WriteLine(result);
    }

    private static void Task2(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);
        
        var bricks = ParseInput(input);
        bricks = MoveBricksToBottom(bricks);
        var result = 0;
        foreach (var brick in bricks)
        {
            result += CountDisplacedBricks(brick, bricks);
        }
        
        Console.WriteLine(result);
    }

    private static int CountDisplacedBricks(Brick brick, List<Brick> bricks)
    {
        var displaced = new HashSet<Brick>();
        var encountered = new Stack<Brick>();
        encountered.Push(brick);
        displaced.Add(brick);
        var count = 0;

        while (encountered.Any())
        {
            var current = encountered.Pop();

            var bricksAbove = current.AboveBricks.Where(above=>above.BelowBricks.All(below=>displaced.Contains(below)));
            foreach (var displacedBrick in bricksAbove)
            {
                encountered.Push(displacedBrick);
                if(displaced.Add(displacedBrick)) count++;
            }
        }

        return count;
    }

    private static int CountDestructibleBricks(List<Brick> bricks)
    {
        var count = 0;
        foreach (var brick in bricks)
        {
            if (brick.AboveBricks.All(x => x.BelowBricks.Count > 1)) count++;
        }

        return count;
    }

    private static List<Brick> MoveBricksToBottom(List<Brick> bricks)
    {
        var movedBricks = new List<Brick>();

        foreach (var brick in bricks.OrderBy(x => x.LowestPoint))
        {
            movedBricks.Add(MoveBrickToBottom(brick, movedBricks));
        }

        return movedBricks;
    }

    private static Brick MoveBrickToBottom(Brick brick, List<Brick> movedBricks)
    {
        if (brick.LowestPoint == 1) return brick;

        var potentialBlockingBricks = movedBricks.Where(x => x.VerticalProjection.Intersects(brick.VerticalProjection)).ToList();
        if (!potentialBlockingBricks.Any())
        {
            return MoveBrickToHeight(brick, 1);
        }

        var actualBlockingBricks =
            potentialBlockingBricks.GroupBy(x => x.HighestPoint).OrderByDescending(x => x.Key).First();
        brick.BelowBricks.AddRange(actualBlockingBricks);
        foreach (var blockingBrick in actualBlockingBricks)
        {
            blockingBrick.AboveBricks.Add(brick);
        }
        return MoveBrickToHeight(brick, actualBlockingBricks.First().HighestPoint + 1);
    }

    private static Brick MoveBrickToHeight(Brick brick, int height)
    {
        var diff = brick.Start.Z - height;
        brick.Start = (brick.Start.X, brick.Start.Y, height);
        brick.End = (brick.End.X, brick.End.Y, brick.End.Z - diff);
        return brick;
    }

    private static List<Brick> ParseInput(string[] input)
    {
        var result = new List<Brick>();
        foreach (var line in input)
        {
            var coordinates = line.Split(new[] { ',', '~' },
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToList();
            
            result.Add(new Brick(start: (coordinates[0], coordinates[1], coordinates[2]),
                end: (coordinates[3], coordinates[4], coordinates[5])));
        }

        return result;
    }
}

public class Brick
{
    public Brick(Point3D start, Point3D end)
    {
        Start = start;
        End = end;
        VerticalProjection = new Line(new Point(Start.X, Start.Y), new Point(End.X, End.Y));
    }

    public Point3D Start { get; set; }

    public Point3D End { get; set; }

    public int LowestPoint => Math.Min(Start.Z, End.Z);
    
    public int HighestPoint => Math.Max(Start.Z, End.Z);

    public Line VerticalProjection { get; }

    public readonly List<Brick> AboveBricks = new List<Brick>();
    
    public readonly List<Brick> BelowBricks = new List<Brick>();
}