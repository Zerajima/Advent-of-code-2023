namespace Day2;

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

        var maxRed = 12;
        var maxGreen = 13;
        var maxBlue = 14;

        var gameSum = 0;
        foreach (var line in input)
        {
            var game = Game.Parse(line);
            if (game.Samples.All(x => x.Red <= maxRed && x.Blue <= maxBlue && x.Green <= maxGreen))
            {
                gameSum += game.Id;
            }
        }

        Console.WriteLine(gameSum);
    }
    
    private static void Task2(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);

        var gameSum = 0;
        foreach (var line in input)
        {
            var game = Game.Parse(line);
            var minRed = game.Samples.Max(x => x.Red);
            var minBlue = game.Samples.Max(x => x.Blue);
            var minGreen = game.Samples.Max(x => x.Green);

            var power = minRed * minBlue * minGreen;
            gameSum += power;
        }

        Console.WriteLine(gameSum);
    }

    private class Game
    {
        public int Id { get; set; }
        
        public List<Sample> Samples { get; set; }

        public static Game Parse(string line)
        {
            var parts = line.Split(new[] { ':', ';' });
            var id = int.Parse(parts[0].Split(' ')[1]);

            var game = new Game()
            {
                Id = id,
                Samples = new List<Sample>()
            };

            foreach (var samplesString in parts.Skip(1))
            {
                game.Samples.Add(Sample.Parse(samplesString));
            }

            return game;
        }
    }
}

internal class Sample
{
    public int Red { get; set; }
    
    public int Blue { get; set; }
    
    public int Green { get; set; }

    public static Sample Parse(string samplesString)
    {
        var result = new Sample();
        var parts = samplesString.Trim().Split(", ");
        foreach (var part in parts)
        {
            var subparts = part.Split(' ');
            switch (subparts[1])
            {
                case "red":
                    result.Red = int.Parse(subparts[0]);
                    break;
                case "blue":
                    result.Blue = int.Parse(subparts[0]);
                    break;
                case "green":
                    result.Green = int.Parse(subparts[0]);
                    break;
            }
        }

        return result;
    }
}