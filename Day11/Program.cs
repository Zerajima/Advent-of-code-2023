using System.Drawing;
using Common;

namespace Day11;

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
        var distanceSum = 0;
        var input = File.ReadAllLines(inputPath);
        var map = input.BuildCharMap();

        var galaxies = DetectGalaxies(map);
        galaxies = ExpandMap(map, galaxies, 2);
        var processedGalaxies = new HashSet<(Point, Point)>();


        foreach (var firstGalaxy in galaxies)
        {
            foreach (var secondGalaxy in galaxies)
            {
                if (processedGalaxies.Contains((firstGalaxy, secondGalaxy))
                    || processedGalaxies.Contains((secondGalaxy, firstGalaxy))) continue;

                var distance = Math.Abs(secondGalaxy.X - firstGalaxy.X) + Math.Abs(secondGalaxy.Y - firstGalaxy.Y);
                processedGalaxies.Add((firstGalaxy, secondGalaxy));
                distanceSum += distance;
            }
        }

        Console.WriteLine(distanceSum);
    }

    private static void Task2(string inputPath)
    {
        checked
        {
            var distanceSum = 0L;
            var input = File.ReadAllLines(inputPath);
            var map = input.BuildCharMap();

            var galaxies = DetectGalaxies(map);
            galaxies = ExpandMap(map, galaxies, 1000000);

            var processedGalaxies = new HashSet<(Point, Point)>();

            foreach (var firstGalaxy in galaxies)
            {
                foreach (var secondGalaxy in galaxies)
                {
                    if (processedGalaxies.Contains((firstGalaxy, secondGalaxy))
                        || processedGalaxies.Contains((secondGalaxy, firstGalaxy))) continue;

                    var distance = Math.Abs(secondGalaxy.X - firstGalaxy.X) + Math.Abs(secondGalaxy.Y - firstGalaxy.Y);
                    processedGalaxies.Add((firstGalaxy, secondGalaxy));
                    distanceSum += distance;
                }
            }

            Console.WriteLine(distanceSum);
        }
    }

    private static List<Point> DetectGalaxies(char[,] map)
    {
        var result = new List<Point>();

        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y] == '#') result.Add(new Point(x, y));
            }
        }
        
        return result;
    }

    private static char[,] ExpandMap(char[,] map)
    {
        var emptyRows = new List<long>();
        var emptyColumns = new HashSet<long>();

        for (long y = 0; y < map.GetLength(1); y++)
        {
            var isRowEmpty = true;
            for (long x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y] != '.')
                {
                    isRowEmpty = false;
                    break;
                }
            }       
            if(isRowEmpty) emptyRows.Add(y);
        }
        
        for (long x = 0; x < map.GetLength(0); x++)
        {
            var isColumnEmpty = true;
            for (long y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] != '.')
                {
                    isColumnEmpty = false;
                    break;
                }
            }   
            if(isColumnEmpty) emptyColumns.Add(x);
        }

        var expandedMap = new char[map.GetLength(0) + emptyColumns.Count, map.GetLength(1) + emptyRows.Count];
        var yOffset = 0;
        for (long y = 0; y < map.GetLength(1); y++)
        {
            var xOffset = 0;
            if (emptyRows.Contains(y))
            {
                for (long x = 0; x < map.GetLength(0); x++)
                {
                    expandedMap[x+xOffset, y + yOffset] = '.';
                }

                yOffset++;
            }
            for (long x = 0; x < map.GetLength(0); x++)
            {
                if (emptyColumns.Contains(x))
                {
                    expandedMap[x+xOffset, y + yOffset] = '.';
                    xOffset++;
                }
                
                expandedMap[x+xOffset, y + yOffset] = map[x, y];
            }
        }

        for (long y = 0; y < expandedMap.GetLength(1); y++)
        {
            for (long x = 0; x < expandedMap.GetLength(0); x++)
            {
                if (expandedMap[x, y] == '\0') expandedMap[x, y] = '.';
            }
        }

        DrawMap(expandedMap);
        return expandedMap;
    }

    private static List<Point> ExpandMap(char[,] map, List<Point> galaxies, int expansionFactor)
    {
        var emptyRows = new List<long>();
        var emptyColumns = new List<long>();

        for (long y = 0; y < map.GetLength(1); y++)
        {
            var isRowEmpty = true;
            for (long x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y] != '.')
                {
                    isRowEmpty = false;
                    break;
                }
            }       
            if(isRowEmpty) emptyRows.Add(y);
        }
        
        for (long x = 0; x < map.GetLength(0); x++)
        {
            var isColumnEmpty = true;
            for (long y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] != '.')
                {
                    isColumnEmpty = false;
                    break;
                }
            }   
            if(isColumnEmpty) emptyColumns.Add(x);
        }

        var expandedGalaxies = new List<Point>();
        checked
        {
            foreach (var galaxy in galaxies)
            {
                var xDelta = emptyColumns.Count(x => x < galaxy.X) * (expansionFactor - 1);
                var yDelta = emptyRows.Count(y => y < galaxy.Y) * (expansionFactor - 1);

                expandedGalaxies.Add(new Point(galaxy.X + xDelta, galaxy.Y + yDelta));
            }
        }

        return expandedGalaxies;
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
    
    private static void DrawMap(List<List<char>> floodedMap)
    {
        foreach (var row in floodedMap)
        {
            foreach (var character in row)
            {
                Console.Write(character);
            }
            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine();
    }
}