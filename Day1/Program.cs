namespace Day1;

internal static class Program
{
    private const string TestInputPath = "TestInput.txt";
    private const string RealInputPath = "RealInput.txt";

    private static Dictionary<string, int> spelledDigits = new()
    {
        { "one", 1 },
        { "two", 2 },
        { "three", 3 },
        { "four", 4 },
        { "five", 5 },
        { "six", 6 },
        { "seven", 7 },
        { "eight", 8 },
        { "nine", 9 },
    };
    
    static void Main(string[] args)
    {
        //Task1(RealInputPath);
        Task2(RealInputPath);
    }

    private static void Task1(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);
        
        List<List<char>> digits = new List<List<char>>(); 
        foreach (var line in input)
        {
            digits.Add(new List<char>());
            foreach (var character in line)
            {
                if (Char.IsDigit(character))
                {
                    digits.Last().Add(character);
                }
            }
        }

        var numbers = digits.Select(x =>
        {
            var chars = new[] { x.First(), x.Last() };
            return Convert.ToInt32(new string(chars.ToArray()));
        }).ToList();


        Console.WriteLine(numbers.Sum());
    }
    
    private static void Task2(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);
        
        var digitsPerLine = new List<Dictionary<int, int>>(); 
        
        foreach (var line in input)
        {
            var lineDigits = new Dictionary<int, int>();
            for (var charIndex = 0; charIndex < line.Length; charIndex++)
            {
                var character = line[charIndex];
                if (char.IsDigit(character))
                {
                    lineDigits.Add(charIndex, character - '0');
                }
            }

            foreach (var spelledDigit in spelledDigits)
            {
                var firstOccurenceOfDigit = line.IndexOf(spelledDigit.Key);
                if (firstOccurenceOfDigit >= 0)
                {
                    lineDigits.Add(firstOccurenceOfDigit, spelledDigit.Value);

                    var lastOccurenceOfDigit = line.LastIndexOf(spelledDigit.Key);
                    if (lastOccurenceOfDigit != firstOccurenceOfDigit)
                    {
                        lineDigits.Add(lastOccurenceOfDigit, spelledDigit.Value);
                    }
                }
            }

            digitsPerLine.Add(lineDigits);
        }

        var numbers = digitsPerLine.Select(lineDigits =>
        {
            var sorted = lineDigits.OrderBy(x => x.Key).Select(x => x.Value).ToList();
            var value = 10 * sorted.First() + sorted.Last();
            return value;
        }).ToList();


        Console.WriteLine(numbers.Sum());
    }
}