using System.Drawing;

namespace Day18;

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
        var score = 0;
        var input = File.ReadAllLines(inputPath);
        List<(Directions Direction, long Length)> moves = input.Select(x =>
        {
            var parts = x.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var direction = parts[0] switch
            {
                "D" => Directions.Down,
                "R" => Directions.Right,
                "L" => Directions.Left,
                "U" => Directions.Up,
            };
            return (direction, long.Parse(parts[1]));
        }).ToList();

        Solve(moves);
    }

    private static void Task2(string inputPath)
    {
        var score = 0;
        var input = File.ReadAllLines(inputPath);
        List<(Directions Direction, long Length)> moves = input.Select(x =>
        {
            var encoded = x.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Last().Trim('(', ')');
            var size = encoded.Substring(1, 5);
            var directionCode = encoded.Last();
            var direction = directionCode switch
            {
                '0' => Directions.Right,
                '1' => Directions.Down,
                '2' => Directions.Left,
                '3' => Directions.Up,
            };
            return (direction, long.Parse(size, System.Globalization.NumberStyles.HexNumber));
        }).ToList();

        Solve(moves);   
    }
    
    private static void Solve(List<(Directions Direction, long Length)> moves)
    {
        checked
        {
            var points = new List<(long X, long Y)>();
            (long X, long Y) initialPoint = (0, 0);
            var lines = new List<((long X, long Y) Start, (long X, long Y) End)>();
            points.Add(initialPoint);
            foreach (var move in moves)
            {
                var currentPoint = points.Last();
                var nextPoint = GetNextPoint(currentPoint, move);
                points.Add(nextPoint);
                lines.Add((currentPoint, nextPoint));
            }
            points.RemoveAt(points.Count - 1);

            var sum1 = 0L;
            for (int i = 0; i < points.Count; i++)
            {
                long x1 = points[i].X;
                long y2 = points[i == points.Count - 1 ? 0 : i + 1].Y;

                sum1 += x1 * y2;
            }

            var sum2 = 0L;
            for (int i = 0; i < points.Count; i++)
            {
                long y1 = points[i].Y;
                long x2 = points[i == points.Count - 1 ? 0 : i + 1].X;

                sum2 += y1 * x2;
            }
            
            var borderSurface = lines.Sum(line=>Math.Abs(line.Start.X - line.End.X) + Math.Abs(line.Start.Y - line.End.Y));
            Console.WriteLine($"Border: {borderSurface}");

            var shoelaceSurface = Math.Abs(sum1 - sum2) / 2;
            Console.WriteLine($"Shoelace surface: {shoelaceSurface}");
            
            var actualSurface = shoelaceSurface + borderSurface / 2 + 1;
            
            Console.WriteLine($"Surface: {actualSurface}");
        }
    }

    private static (long X, long Y) GetNextPoint((long X, long Y) currentPoint, (Directions Direction, long Length) move)
    {
        if (move.Direction == Directions.Right) return (currentPoint.X + move.Length, currentPoint.Y);
        else if (move.Direction == Directions.Left) return (currentPoint.X - move.Length, currentPoint.Y);
        else if (move.Direction == Directions.Down) return (currentPoint.X, currentPoint.Y + move.Length);
        else return (currentPoint.X, currentPoint.Y - move.Length); // Directions.Up
    }
}

public enum Directions
{
    Right,
    Left,
    Up,
    Down,
}