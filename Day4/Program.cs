namespace Day4;

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

        var gameSum = 0;
        foreach (var line in input)
        {
            var lineParts = line.Split(new[] { ':', '|' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var winningNumbers = lineParts[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToHashSet();
            var myNumbers = lineParts[2].Split(' ', StringSplitOptions.TrimEntries);

            var lineResult = 0;
            foreach (var myNumber in myNumbers)
            {
                if (winningNumbers.Contains(myNumber))
                {
                    lineResult = lineResult == 0 ? 1 : lineResult * 2;
                }
            }

            gameSum += lineResult;
        }

        Console.WriteLine(gameSum);
    }
    
    private static void Task2(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);

        var gameSum = 0;
        var repetitionsOfLine = new Dictionary<int, int>();
        foreach (var line in input)
        {
            var lineParts = line.Split(new[] { ':', '|' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var currentLine = int.Parse(lineParts[0]
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1]);
            var winningNumbers = lineParts[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToHashSet();
            var myNumbers = lineParts[2].Split(' ', StringSplitOptions.TrimEntries);

            var numberOfCards = repetitionsOfLine.ContainsKey(currentLine) ? repetitionsOfLine[currentLine] + 1 : 1;
            
            var matchingNumbers = 0;
            foreach (var myNumber in myNumbers)
            {
                if (winningNumbers.Contains(myNumber))
                {
                    matchingNumbers += 1;
                }
            }

            for (int i = currentLine + 1; i < currentLine + matchingNumbers + 1; i++)
            {
                if (repetitionsOfLine.ContainsKey(i)) repetitionsOfLine[i] += numberOfCards;
                else repetitionsOfLine[i] = numberOfCards;
            }

            gameSum += numberOfCards;
        }

        Console.WriteLine(gameSum);
    }
}