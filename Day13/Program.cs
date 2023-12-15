using System.Text;
using Common;

namespace Day13;

internal static class Program
{
    private const string TestInputPath = "TestInput.txt";
    private const string RealInputPath = "RealInput.txt";
    
    static void Main(string[] args)
    {
       // Task1(RealInputPath);
        Task2(RealInputPath);
    }

    private static void Task1(string inputPath)
    {
        var score = 0;
        var input = File.ReadAllLines(inputPath);
        var maps = ParseInput(input);

        int mapCounter = 0;
        foreach (var map in maps)
        {
            mapCounter++;

            if (CheckHorizontal(map, 0, out var scoreIncreaseHorizontal))
            {
                score += scoreIncreaseHorizontal;
                Console.WriteLine($"{mapCounter}: horizontal - {scoreIncreaseHorizontal}");
                continue;
            }

            if (CheckVertical(map, 0, out var scoreIncreaseVertical))
            {
                score += scoreIncreaseVertical;
                Console.WriteLine($"{mapCounter}: vertical - {scoreIncreaseVertical}");
                continue;
            }
        }

        Console.WriteLine(score);
    }

    private static void Task2(string inputPath)
    {
        var score = 0;
        var input = File.ReadAllLines(inputPath);
        var maps = ParseInput(input);

        int mapCounter = 0;
        foreach (var map in maps)
        {
            mapCounter++;

            if (CheckHorizontal(map, 1, out var scoreIncreaseHorizontal))
            {
                score += scoreIncreaseHorizontal;
                Console.WriteLine($"{mapCounter}: horizontal - {scoreIncreaseHorizontal}");
                continue;
            }

            if (CheckVertical(map, 1, out var scoreIncreaseVertical))
            {
                score += scoreIncreaseVertical;
                Console.WriteLine($"{mapCounter}: vertical - {scoreIncreaseVertical}");
                continue;
            }
        }

        Console.WriteLine(score);
    }

    private static bool CheckHorizontal(char[,] map, int requiredDiff, out int scoreIncrease)
    {
        var mapWidth = map.GetLength(0);

        var transitions = new int[] { -1, 2, -3, 4, -5, 6, -7, 8, -9, 10, -11, 12, -13, 14, -15, 16, -17, 18, -19, 20, -21 };
        var counter = 0;
        for (int x = mapWidth / 2; x >= 0 && x < mapWidth - 1; x = x + transitions[counter++])
        {
            bool allColumnsEqual = true;
            var remainingDiff = requiredDiff;

            var offset = 0;
            while (true)
            {
                var leftColumnIndex = x - offset;
                var rightColumnIndex = x + offset + 1;
                
                if (leftColumnIndex < 0) break;
                if (rightColumnIndex >= mapWidth) break;
                
                var leftColumn = GetColumn(map, leftColumnIndex);
                var rightColumn = GetColumn(map, rightColumnIndex);

                var currentDiff = GetDiff(leftColumn, rightColumn);
                if (currentDiff > remainingDiff)
                {
                    allColumnsEqual = false;
                    break;
                }

                remainingDiff -= currentDiff;
                offset++;
            }

            if (allColumnsEqual && remainingDiff == 0)
            {
                scoreIncrease = x + 1;
                return true;
            }
        }
        
        scoreIncrease = 0;
        return false;
    }

    private static bool CheckVertical(char[,] map, int requiredDiff, out int scoreIncrease)
    {
        var mapHeight = map.GetLength(1);

        var transitions = new int[] {-1, 2, -3, 4, -5, 6, -7, 8, -9, 10, -11, 12, -13, 14, -15, 16, -17, 18, -19, 20, -21 };
        var counter = 0;
        for (int y = mapHeight / 2; y >= 0 && y < mapHeight - 1; y = y + transitions[counter++])
        {
            bool allRowsEqual = true;
            var remainingDiff = requiredDiff;
            
            var offset = 0;
            while (true)
            {
                var topRowIndex = y - offset;
                var bottomRowIndex = y + offset + 1;
                
                if (topRowIndex < 0) break;
                if (bottomRowIndex >= mapHeight) break;
                
                var topRow = GetRow(map, topRowIndex);
                var bottomRow = GetRow(map, bottomRowIndex);
                
                var currentDiff = GetDiff(topRow, bottomRow);
                if (currentDiff > remainingDiff)
                {
                    allRowsEqual = false;
                    break;
                }

                remainingDiff -= currentDiff;
                offset++;
            }

            if (allRowsEqual && remainingDiff == 0)
            {
                scoreIncrease = (y + 1) * 100;
                return true;
            }
        }
        
        scoreIncrease = 0;
        return false;
    }

    private static int GetDiff(string s1, string s2)
    {
        var diffCount = 0;
        for (int i = 0; i < s1.Length; i++)
        {
            if (s1[i] != s2[i]) diffCount++;
        }

        return diffCount;
    }

    private static string GetColumn(char[,] map, int x)
    {
        StringBuilder result = new StringBuilder();
        for (int y = 0; y < map.GetLength(1); y++)
        {
            result.Append(map[x, y]);
        }

        return result.ToString();
    }

    private static string GetRow(char[,] map, int y)
    {
        StringBuilder result = new StringBuilder();
        for (int x = 0; x < map.GetLength(0); x++)
        {
            result.Append(map[x, y]);
        }

        return result.ToString();
    }

    private static List<char[,]> ParseInput(string[] input)
    {
        var currentGroup = new List<string>();
        var result = new List<char[,]>();
        foreach (var line in input)
        {
            if (line.Length == 0)
            {
                result.Add(currentGroup.ToArray().BuildCharMap());
                currentGroup = new List<string>();
            }
            else
            {
                currentGroup.Add(line);
            }
        }
        
        result.Add(currentGroup.ToArray().BuildCharMap());

        return result;
    }
}