using System.Diagnostics.CodeAnalysis;

namespace Day15;

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
        var parts = input.First().Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            var currentValue = HashValue(part);

            Console.WriteLine($"{part} - {currentValue}");
            score += currentValue;
        }

        Console.WriteLine(score);
    }

    private static void Task2(string inputPath)
    {
        checked
        {
            var score = 0;
            var input = File.ReadAllLines(inputPath);
            var instructions = input.First()
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            var boxes = new List<(string Label, int FocalPoint)>[256];
            for (int i = 0; i < 256; i++)
            {
                boxes[i] = new List<(string Label, int FocalPoint)>();
            }

            foreach (var instruction in instructions)
            {
                var parts = instruction.Split(new[] { '=', '-' },
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                var label = parts[0];
                var index = HashValue(label);
                var box = boxes[index];

                var indexOfLens = GetIndexOfLens(box, label);

                if (parts.Length > 1)
                {
                    var lens = int.Parse(parts[1]);
                    if (indexOfLens >= 0) box[indexOfLens] = (label, lens);
                    else box.Add((label, lens));
                }
                else if(indexOfLens >= 0)
                {
                    box.RemoveAt(indexOfLens);
                }
            }

            // The focusing power of a single lens is the result of multiplying together:
            // One plus the box number of the lens in question.
            // The slot number of the lens within the box: 1 for the first lens, 2 for the second lens, and so on.
            // The focal length of the lens.
            
            for (int boxIndex = 0; boxIndex < boxes.Length; boxIndex++)
            {
                var box = boxes[boxIndex];

                for (int lensIndex = 0; lensIndex < box.Count; lensIndex++)
                {
                    var focusingPower = (1 + boxIndex) * (lensIndex + 1) * box[lensIndex].FocalPoint;
                    score += focusingPower;
                }
            }

            Console.WriteLine(score);
        }
    }

    private static int GetIndexOfLens(List<(string Label, int FocalPoint)> box, string label)
    {
        for (int i = 0; i < box.Count; i++)
        {
            if (box[i].Label == label)
            {
                return i;
            }
        }

        return -1;
    }

    private static int HashValue(string part)
    {
        var currentValue = 0;
        foreach (var character in part)
        {
            currentValue = ((currentValue + (int)character) * 17) % 256;
        }

        return currentValue;
    }
}