namespace Day6;

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
        var times = input[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Skip(1)
            .Select(int.Parse).ToList();
        var distances = input[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Skip(1)
            .Select(int.Parse).ToList();

        var gameSum = 0;
        for (int race = 0; race < times.Count; race++)
        {
            var wins = 0;
            var maxTime = times[race];
            var targetDistance = distances[race];

            for (int speed = 1; speed < maxTime; speed++)
            {
                var distance = speed * (maxTime - speed);
                if (distance > targetDistance) wins++;
            }

            gameSum = gameSum == 0 ? wins : gameSum * wins;
        }

        Console.WriteLine(gameSum);
    }

    private static void Task2(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);
        var maxTime = long.Parse(string.Join("",
            input[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Skip(1)));
        var targetDistance = long.Parse(string.Join("",
            input[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Skip(1)));

        var wins = 0;

        for (int speed = 1; speed < maxTime; speed++)
        {
            var distance = speed * (maxTime - speed);
            if (distance > targetDistance) wins++;
        }

        Console.WriteLine(wins);
    }
}