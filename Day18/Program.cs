namespace Day18;

internal static class Program
{
    private const string TestInputPath = "TestInput.txt";
    private const string RealInputPath = "RealInput.txt";
   
    static void Main(string[] args)
    {
        Task1(TestInputPath);
        Task2(TestInputPath);
    }

    private static void Task1(string inputPath)
    {
        var score = 0;
        var input = File.ReadAllLines(inputPath);
        
        foreach (var line in input)
        {
        }


        Console.WriteLine(score);
    }
    
    private static void Task2(string inputPath)
    {
        var score = 0;
        var input = File.ReadAllLines(inputPath);
        
        foreach (var line in input)
        {
        }


        Console.WriteLine(score);   
    }
}