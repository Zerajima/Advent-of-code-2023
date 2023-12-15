using System.Drawing;
using Common;

namespace Day10;

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
        var startingPoint = LocateStart(map);

        var previousTile = startingPoint;
        var currentTile = LocateConnectedTiles(map, startingPoint).First();
        var length = 1;

        while (map[currentTile.X, currentTile.Y] != 'S')
        {
            var temp = currentTile;
            currentTile = MoveToConnectedTile(map, currentTile, previousTile);
            previousTile = temp;

            length++;
        }

        Console.WriteLine(length / 2);
    }

    private static void Task2(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);
        var map = input.BuildCharMap();
        var blowoutMap = new char[map.GetLength(0) * 3, map.GetLength(1) * 3];

        var startingPoint = LocateStart(map);
        var previousTile = startingPoint;
        var startingPointConnectors = LocateConnectedTiles(map, startingPoint);

        map[startingPoint.X, startingPoint.Y] = DetectStartingPointType(startingPoint, startingPointConnectors);

        var currentTile = startingPointConnectors.First();
        DrawCurrentTile(map, blowoutMap, currentTile);

        while (currentTile != startingPoint)
        {
            var temp = currentTile;
            currentTile = MoveToConnectedTile(map, currentTile, previousTile);
            previousTile = temp;
            DrawCurrentTile(map, blowoutMap, currentTile);
        }

        DrawMap(blowoutMap);

        foreach (var borderTile in GetEmptyStartingPoints(blowoutMap))
        {
            FloodEmptyTiles(blowoutMap, borderTile);
        }

        Console.WriteLine();
        DrawMap(blowoutMap);

        var collapsedMap = CollapseMap(blowoutMap);
        DrawMap(collapsedMap);

        var enclosedCount = 0;
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (collapsedMap[x, y] == 'X')
                {
                    enclosedCount++;
                }
            }
        }

        Console.WriteLine(enclosedCount);
    }

    private static char[,] CollapseMap(char[,] blowoutMap)
    {
        var result = new char[blowoutMap.GetLength(0) / 3, blowoutMap.GetLength(1) / 3];

        for (int x = 0; x < result.GetLength(0); x++)
        {
            for (int y = 0; y < result.GetLength(1); y++)
            {
                var blowoutX = x * 3;
                var blowoutY = y * 3;
                var chars = new char[9]
                {
                    blowoutMap[blowoutX, blowoutY], blowoutMap[blowoutX + 1, blowoutY], blowoutMap[blowoutX + 2, blowoutY],
                    blowoutMap[blowoutX, blowoutY + 1], blowoutMap[blowoutX + 1, blowoutY + 1], blowoutMap[blowoutX + 2, blowoutY + 1],
                    blowoutMap[blowoutX, blowoutY + 2], blowoutMap[blowoutX + 1, blowoutY + 2], blowoutMap[blowoutX + 2, blowoutY + 2]
                };

                if (chars.Contains('-')) result[x, y] = '-';
                else if (chars.Contains('|')) result[x, y] = '|';
                else if (chars.Contains('L')) result[x, y] = 'L';
                else if (chars.Contains('F')) result[x, y] = 'F';
                else if (chars.Contains('7')) result[x, y] = '7';
                else if (chars.Contains('J')) result[x, y] = 'J';
                else if (chars.Contains('O')) result[x, y] = 'O';
                else if (chars.Contains('.')) result[x, y] = '.';
                else result[x, y] = 'X';
            }
        }

        return result;
    }

    // private static void Task2(string inputPath)
    // {
    //     var input = File.ReadAllLines(inputPath);
    //     var map = input.BuildCharMap();
    //     var floodedMap = new char[map.GetLength(0), map.GetLength(1)];// (char[,])map.Clone();
    //     
    //     var startingPoint = LocateStart(map);
    //     var previousTile = startingPoint;
    //     var startingPointConnectors = LocateConnectedTiles(map, startingPoint);
    //     var currentTile = startingPointConnectors.First();
    //     DrawCurrentTile(map, floodedMap, currentTile);
    //     pipeTiles.Add(currentTile);
    //     
    //     while (map[currentTile.X, currentTile.Y] != 'S')
    //     {
    //         var temp = currentTile;
    //         currentTile = MoveToConnectedTile(map, currentTile, previousTile);
    //         previousTile = temp;
    //         DrawCurrentTile(map, floodedMap, currentTile);
    //         pipeTiles.Add(currentTile);
    //     }
    //
    //     map[startingPoint.X, startingPoint.Y] = DetectStartingPointType(startingPoint, startingPointConnectors); 
    //     DrawCurrentTile(map, floodedMap, startingPoint);
    //
    //     var directions = GetDirections(map, startingPoint);
    //     
    //     DrawMap(floodedMap, directions);
    //     
    //     foreach (var borderTile in GetEmptyStartingPoints(floodedMap))
    //     {
    //         FloodEmptyTiles(floodedMap, borderTile);
    //     }
    //     
    //     Console.WriteLine();
    //     DrawMap(floodedMap, directions);
    //
    //     for (int y = 0; y < floodedMap.GetLength(1); y++)
    //     {
    //         for (int x = 0; x < floodedMap.GetLength(0); x++)
    //         {
    //             if(floodedMap[x,y] != '\0') continue;
    //             
    //             var pipesEncountered = 0;
    //
    //             for (int lookahead = x-1; lookahead >= 0; lookahead--)
    //             {
    //                 var currentPoint = new Point(lookahead, y);
    //                 var type = floodedMap[currentPoint.X, currentPoint.Y];
    //                 if (type != '\0' && type != 'O' && type != 'X') 
    //                 {
    //                     pipesEncountered++;
    //                 }
    //             }
    //             
    //
    //             if (pipesEncountered % 2 != 0)
    //             {
    //                 floodedMap[x, y] = 'O';
    //             }
    //             else
    //             {
    //                 floodedMap[x, y] = 'X';
    //             }
    //         }
    //     }
    //     
    //     Console.WriteLine();
    //     DrawMap(floodedMap, directions);
    //     
    //     foreach (var borderTile in GetOpenPoints(floodedMap))
    //     {
    //         FloodClosedTiles(floodedMap, borderTile);
    //     }
    //     
    //     Console.WriteLine();
    //     DrawMap(floodedMap, directions);
    //     
    //     var enclosedCount = 0;
    //     for (int y = 0; y < map.GetLength(1); y++)
    //     {
    //         for (int x = 0; x < map.GetLength(0); x++)
    //         {
    //             if (floodedMap[x, y] == 'X')
    //             {
    //                 enclosedCount++;
    //             }
    //         }
    //     }
    //     
    //     Console.WriteLine(enclosedCount);
    // }

    private static void DrawMap(char[,] floodedMap)
    {
        for (int y = 0; y < floodedMap.GetLength(1); y++)
        {
            for (int x = 0; x < floodedMap.GetLength(0); x++)
            {
                Console.Write(floodedMap[x, y]);
            }

            Console.WriteLine();
        }

        // Console.WriteLine();
        // for (int y = 0; y < directions.GetLength(1); y++)
        // {
        //     for (int x = 0; x < directions.GetLength(0); x++)
        //     {
        //         char c = directions[x, y] switch
        //         {
        //             Directions.Up => 'u',
        //             Directions.Down => 'd',
        //             Directions.Left => 'l',
        //             Directions.Right => 'r',
        //             _ => '.',
        //         };
        //         Console.Write(c);
        //     }
        //
        //     Console.WriteLine();
        // }

        Console.WriteLine();
        Console.WriteLine();
    }

    private static char DetectStartingPointType(Point startingPoint, List<Point> startingPointConnectors)
    {
        foreach (var possibleType in new char[] { '-', '|', 'F', 'L', '7', 'J' })
        {
            if (ConnectsToType(startingPoint, startingPointConnectors.First(), possibleType) &&
                ConnectsToType(startingPoint, startingPointConnectors.Last(), possibleType))
                return possibleType;
        }

        throw new Exception();
    }

    private static Dictionary<char, char[,]> Tiles = new Dictionary<char, char[,]>
    {
        { '.', new char[3, 3] { { '.', '.', '.' }, { '.', '.', '.' }, { '.', '.', '.' } } },
        { 'S', new char[3, 3] { { 'S', 'S', 'S' }, { 'S', 'S', 'S' }, { 'S', 'S', 'S' } } },
        { '|', new char[3, 3] { { '.', '|', '.' }, { '.', '|', '.' }, { '.', '|', '.' } } },
        { '-', new char[3, 3] { { '.', '.', '.' }, { '-', '-', '-' }, { '.', '.', '.' } } },
        { 'L', new char[3, 3] { { '.', 'L', '.' }, { '.', 'L', 'L' }, { '.', '.', '.' } } },
        { '7', new char[3, 3] { { '.', '.', '.' }, { '7', '7', '.' }, { '.', '7', '.' } } },
        { 'F', new char[3, 3] { { '.', '.', '.' }, { '.', 'F', 'F' }, { '.', 'F', '.' } } },
        { 'J', new char[3, 3] { { '.', 'J', '.' }, { 'J', 'J', '.' }, { '.', '.', '.' } } },
    };

    private static void DrawCurrentTile(char[,] originalMap, char[,] blowoutMap, Point currentTile)
    {
        var originalValue = originalMap[currentTile.X, currentTile.Y];
        var tileMap = Tiles[originalValue];
        var blowoutPosition = new Point(currentTile.X * 3, currentTile.Y * 3);

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                blowoutMap[blowoutPosition.X + x, blowoutPosition.Y + y] = tileMap[y, x];
            }
        }

    }

    private static char[] emptyPoints = { '\0', '.' };

    private static HashSet<Point> visited = new HashSet<Point>();
    private static void FloodEmptyTiles(char[,] map, Point startingPoint)
    {
        if (!emptyPoints.Contains(map[startingPoint.X, startingPoint.Y])) return;
        
        var encountered = new Queue<Point>();
        encountered.Enqueue(startingPoint);

        while (encountered.Count > 0)
        {
            var current = encountered.Dequeue();
            map[current.X, current.Y] = 'O';
            visited.Add(current);

            var possibleDestinations = new List<Point>
            {
                new(current.X + 1, current.Y),
                new(current.X - 1, current.Y),
                new(current.X, current.Y + 1),
                new(current.X, current.Y - 1),
            }.Where(x => CanMoveTo(x, map)).ToList();

            foreach (var possibleDestination in possibleDestinations)
            {
                if(visited.Contains(possibleDestination)) continue;

                var destinationType = map[possibleDestination.X, possibleDestination.Y];
                if (emptyPoints.Contains(destinationType))
                {
                    visited.Add(possibleDestination);
                    encountered.Enqueue(possibleDestination);
                }
            }
        }
    }
    

    private static bool CanMoveTo(Point point, char[,] map)
    {
        return point.X >= 0 && point.X < map.GetLength(0)
                            && point.Y >= 0 && point.Y < map.GetLength(1);
    }

    private static List<Point> GetEmptyStartingPoints(char[,] modifiedMap)
    {
        var result = new List<Point>();
        var maxX = modifiedMap.GetLength(0) - 1;
        var maxY = modifiedMap.GetLength(1) - 1;

        // for (int x = 0; x < modifiedMap.GetLength(0); x++)
        // {
        //     for (int y = 0; y < modifiedMap.GetLength(1); y++)
        //     {
        //         if(modifiedMap[x, y] == '\0') result.Add(new Point(x, y));
        //     }
        // }
        
        for (int x = 0; x < modifiedMap.GetLength(0); x++)
        {
            if (modifiedMap[x, 0] == '\0') result.Add(new Point(x, 0));
            if (modifiedMap[x, maxY] == '\0') result.Add(new Point(x, maxY));
        }
        
        for (int y = 0; y < modifiedMap.GetLength(1); y++)
        {
            if (modifiedMap[0, y] == '\0') result.Add(new Point(0, y));
            if (modifiedMap[maxX, y] == '\0') result.Add(new Point(maxX, y));
        }
        
        return result;
    }
    
    private static List<Point> GetOpenPoints(char[,] modifiedMap)
    {
        var result = new List<Point>();

        for (int x = 0; x < modifiedMap.GetLength(0); x++)
        {
            for (int y = 0; y < modifiedMap.GetLength(1); y++)
            {
                if(modifiedMap[x, y] == 'O') result.Add(new Point(x, y));
            }
        }

        return result;
    }

    private static Point MoveToConnectedTile(char[,] map, Point currentPoint, Point previousPoint)
    {
        for (int y = currentPoint.Y - 1; y <= currentPoint.Y + 1; y++)
        {
            for (int x = currentPoint.X - 1; x <= currentPoint.X + 1; x++)
            {
                if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1) 
                    || (x == previousPoint.X && y == previousPoint.Y)
                    || Math.Abs(x - currentPoint.X) + Math.Abs(y - currentPoint.Y) != 1)
                {
                    continue;
                }

                var testPoint = new Point(x, y);
                if (ConnectsTo(map, currentPoint, testPoint))
                {
                    return testPoint;
                }
            }   
        }

        throw new Exception("MoveToConnectedTile failed");
    }

    private static List<Point> LocateConnectedTiles(char[,] map, Point startingPoint)
    {
        var result = new List<Point>();
        for (int y = startingPoint.Y - 1; y <= startingPoint.Y + 1; y++)
        {
            for (int x = startingPoint.X - 1; x <= startingPoint.X + 1; x++)
            {
                if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1) || (x == startingPoint.X && y == startingPoint.Y))
                {
                    continue;
                }

                var currentPoint = new Point(x, y);
                if (ConnectsTo(map, currentPoint, startingPoint))
                {
                    result.Add(currentPoint);
                }
            }   
        }

        return result;
    }

    private static bool ConnectsTo(char[,] map, Point currentPoint, Point anotherPoint)
    {
        var type = map[currentPoint.X, currentPoint.Y];
        return ConnectsToType(currentPoint, anotherPoint, type);
    }

    private static bool ConnectsToType(Point currentPoint, Point anotherPoint, char type)
    {
        switch (type)
        {
            case '|':
                return currentPoint.X == anotherPoint.X && Math.Abs(currentPoint.Y - anotherPoint.Y) == 1;
            case '-':
                return currentPoint.Y == anotherPoint.Y && Math.Abs(currentPoint.X - anotherPoint.X) == 1;
            case 'L':
                return currentPoint.Y == anotherPoint.Y && currentPoint.X == anotherPoint.X - 1
                       || currentPoint.Y == anotherPoint.Y + 1 && currentPoint.X == anotherPoint.X;
            case 'J':
                return currentPoint.Y == anotherPoint.Y && currentPoint.X == anotherPoint.X + 1
                       || currentPoint.Y == anotherPoint.Y + 1 && currentPoint.X == anotherPoint.X;
            case '7':
                return currentPoint.Y == anotherPoint.Y && currentPoint.X == anotherPoint.X + 1
                       || currentPoint.Y == anotherPoint.Y - 1 && currentPoint.X == anotherPoint.X;
            case 'F':
                return currentPoint.Y == anotherPoint.Y && currentPoint.X == anotherPoint.X - 1
                       || currentPoint.Y == anotherPoint.Y - 1 && currentPoint.X == anotherPoint.X;
        }

        return false;
    }

    private static Point LocateStart(char[,] map)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y] == 'S') return new Point(x, y);
            }
        }

        throw new Exception("Start not found");
    }
}