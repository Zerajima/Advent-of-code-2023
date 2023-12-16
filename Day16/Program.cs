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
        var score = 0;
        var input = File.ReadAllLines(inputPath);
        var map = input.BuildCharMap();
        var beams = new Queue<(Point Position, Directions Direction)>();
        beams.Enqueue((new Point(0,0), Directions.Right));
        var allBeams = new HashSet<(Point Position, Directions Direction)>();
        var allBeamPositions = new HashSet<Point>();

        while (beams.Any())
        {
            var currentBeam = beams.Dequeue();

            while (currentBeam.Position.X >= 0 && currentBeam.Position.X < map.GetLength(0) && currentBeam.Position.Y >= 0 &&
                   currentBeam.Position.Y < map.GetLength(1)
                   && !allBeams.Contains(currentBeam))
            {
                if (!allBeamPositions.Contains(currentBeam.Position)) score++;
                allBeams.Add(currentBeam);
                allBeamPositions.Add(currentBeam.Position);

                var newDirection = GetNewDirection(map, currentBeam);
                if (ShouldCreateNewBeam(map, currentBeam))
                {
                    beams.Enqueue((currentBeam.Position, GetOppositeDirection(newDirection)));
                }

                var nextPosition = GetNextPosition(currentBeam.Position, newDirection);

                currentBeam.Position = nextPosition;
                currentBeam.Direction = newDirection;
            }
        }
        
        Console.WriteLine(score);
    }

    private static void Task2(string inputPath)
    {
        var maxScore = 0;
        var input = File.ReadAllLines(inputPath);
        var map = input.BuildCharMap();

        var startingBeams = GetStartingBeams(map);

        foreach (var startingBeam in startingBeams)
        {
            var score = 0;
            var beams = new Queue<(Point Position, Directions Direction)>();
            beams.Enqueue(startingBeam);
            var allBeams = new HashSet<(Point Position, Directions Direction)>();
            var allBeamPositions = new HashSet<Point>();

            while (beams.Any())
            {
                var currentBeam = beams.Dequeue();

                while (currentBeam.Position.X >= 0 && currentBeam.Position.X < map.GetLength(0) && currentBeam.Position.Y >= 0 &&
                       currentBeam.Position.Y < map.GetLength(1)
                       && !allBeams.Contains(currentBeam))
                {
                    if (!allBeamPositions.Contains(currentBeam.Position)) score++;
                    allBeams.Add(currentBeam);
                    allBeamPositions.Add(currentBeam.Position);

                    var newDirection = GetNewDirection(map, currentBeam);
                    if (ShouldCreateNewBeam(map, currentBeam))
                    {
                        beams.Enqueue((currentBeam.Position, GetOppositeDirection(newDirection)));
                    }

                    var nextPosition = GetNextPosition(currentBeam.Position, newDirection);

                    currentBeam.Position = nextPosition;
                    currentBeam.Direction = newDirection;
                }
            }

            if (score > maxScore) maxScore = score;
        }

        Console.WriteLine(maxScore);
    }

    private static List<(Point Position, Directions Direction)> GetStartingBeams(char[,] map)
    {
        var width = map.GetLength(0);
        var heigth = map.GetLength(1);
        var result = new List<(Point Position, Directions Direction)>();
        for (int x = 0; x < width; x++)
        {
            result.Add((new Point(x, 0), Directions.Down));
            result.Add((new Point(x, heigth - 1), Directions.Up));
        }

        for (int y = 0; y < heigth; y++)
        {
            result.Add((new Point(0, y), Directions.Right));
            result.Add((new Point(width - 1, y), Directions.Left));
        }

        return result;
    }

    private static bool ShouldCreateNewBeam(char[,] map, (Point Position, Directions Direction) currentBeam)
    {
        var tile = map[currentBeam.Position.X, currentBeam.Position.Y];
        
        if(tile == '|' && (currentBeam.Direction == Directions.Right || currentBeam.Direction == Directions.Left)) return true;
        if (tile == '-' && (currentBeam.Direction == Directions.Up || currentBeam.Direction == Directions.Down)) return true;
        return false;
    }

    private static Directions GetNewDirection(char[,] map, (Point Position, Directions Direction) currentBeam)
    {
        var tile = map[currentBeam.Position.X, currentBeam.Position.Y];
        switch (tile)
        {
            case '.':
                return currentBeam.Direction;
            case '/':
            case '\\':
                return ChangeDirection(currentBeam.Direction, tile);
            case '|':
                if (currentBeam.Direction == Directions.Right || currentBeam.Direction == Directions.Left)
                {
                    return Directions.Up;
                }
                else
                {
                    return currentBeam.Direction;
                }
            case '-':
                if (currentBeam.Direction == Directions.Up || currentBeam.Direction == Directions.Down)
                {
                    return Directions.Right;
                }
                else
                {
                    return currentBeam.Direction;
                }
            default:
                throw new Exception();
        }
    }

    private static Directions ChangeDirection(Directions currentBeamDirection, char tile)
    {
        switch (currentBeamDirection)
        {
            case Directions.Right:
                if (tile == '/') return Directions.Up;
                else return Directions.Down;
            case Directions.Left:
                if (tile == '/') return Directions.Down;
                else return Directions.Up;
            case Directions.Up:
                if (tile == '/') return Directions.Right;
                else return Directions.Left;
            case Directions.Down:
                if (tile == '/') return Directions.Left;
                else return Directions.Right;
            default:
                throw new ArgumentOutOfRangeException(nameof(currentBeamDirection), currentBeamDirection, null);
        }
    }

    private static Directions GetOppositeDirection(Directions currentBeamDirection)
    {
        switch (currentBeamDirection)
        {
            case Directions.Right:
                return Directions.Left;
            case Directions.Left:
                return Directions.Right;
            case Directions.Up:
                return Directions.Down;
            case Directions.Down:
                return Directions.Up;
            default:
                throw new ArgumentOutOfRangeException(nameof(currentBeamDirection), currentBeamDirection, null);
        }
    }

    private static Point GetNextPosition(Point currentBeamPosition, Directions currentBeamDirection)
    {
        switch (currentBeamDirection)
        {
            case Directions.Right:
                return currentBeamPosition with { X = currentBeamPosition.X + 1 };
            case Directions.Left:
                return currentBeamPosition with { X = currentBeamPosition.X - 1 };
            case Directions.Up:
                return currentBeamPosition with { Y = currentBeamPosition.Y - 1 };
            case Directions.Down:
                return currentBeamPosition with { Y = currentBeamPosition.Y + 1 };
            default:
                throw new ArgumentOutOfRangeException(nameof(currentBeamDirection), currentBeamDirection, null);
        }
    }


    public enum Directions
    {
        Right,
        Left,
        Up,
        Down
    }
}