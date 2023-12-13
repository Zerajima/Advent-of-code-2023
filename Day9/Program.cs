namespace Day2023_9;

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
        var sum = 0L;
        var input = File.ReadAllLines(inputPath);
        foreach (var line in input)
        {
            var sequences = new List<List<long>>();

            var currentSequence = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
            sequences.Add(currentSequence);

            while (currentSequence.Any(x => x != 0))
            {
                var nextSequence = new List<long>(currentSequence.Count - 1);

                for (int i = 0; i < currentSequence.Count - 1; i++)
                {
                    nextSequence.Add(currentSequence[i + 1] - currentSequence[i]);
                }
                
                sequences.Add(nextSequence);
                currentSequence = nextSequence;
            }

            var currentDiff = 0L;
            for (int i = sequences.Count - 2; i >= 0; i--)
            {
                currentDiff = sequences[i].Last() + currentDiff;
            }

            sum += currentDiff;
        }

        Console.WriteLine(sum);
    }

    private static void Task2(string inputPath)
    {
        var sum = 0L;
        var input = File.ReadAllLines(inputPath);
        foreach (var line in input)
        {
            var sequences = new List<List<long>>();

            var currentSequence = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
            sequences.Add(currentSequence);

            while (currentSequence.Any(x => x != 0))
            {
                var nextSequence = new List<long>(currentSequence.Count - 1);

                for (int i = 0; i < currentSequence.Count - 1; i++)
                {
                    nextSequence.Add(currentSequence[i + 1] - currentSequence[i]);
                }
                
                sequences.Add(nextSequence);
                currentSequence = nextSequence;
            }

            var currentDiff = 0L;
            for (int i = sequences.Count - 2; i >= 0; i--)
            {
                currentDiff = sequences[i].First() - currentDiff;
            }

            sum += currentDiff;
        }

        Console.WriteLine(sum);
    }
}