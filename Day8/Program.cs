namespace Day2023_8;

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
        var instructions = input.First();
        var currentInstructionIndex = 0;

        var map = input.Skip(2).Select(x =>
        {
            var parts = x.Split(" = ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var current = parts[0];
            var parts2 = parts[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var left = parts2[0].Trim('(');
            var right = parts2[1].Trim(')');
            return new
            {
                Current = current,
                Left = left,
                Right = right
            };
        }).ToDictionary(x => x.Current);

        var currentPosition = map["AAA"];
        var steps = 0;

        while (true)
        {
            steps++;
            var currentInstruction = instructions[currentInstructionIndex];

            if (currentInstruction == 'L')
            {
                currentPosition = map[currentPosition.Left];
            }
            else
            {
                currentPosition = map[currentPosition.Right];
            }

            if (currentPosition.Current == "ZZZ")
            {
                break;
            } 

            currentInstructionIndex++;
            if (currentInstructionIndex >= instructions.Length) currentInstructionIndex = 0;
        }

        Console.WriteLine(steps);
    }

    private static void Task2(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);
        var instructions = input.First();
        var currentInstructionIndex = 0;

        var map = input.Skip(2).Select(x =>
        {
            var parts = x.Split(" = ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var current = parts[0];
            var parts2 = parts[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var left = parts2[0].Trim('(');
            var right = parts2[1].Trim(')');
            return new
            {
                Current = current,
                Left = left,
                Right = right
            };
        }).ToDictionary(x => x.Current);

        var currentPositions = map.Where(x=>x.Key.EndsWith('A')).Select(x=>x.Value).ToList();
        var steps = 0L;

        while (true)
        {
            checked
            {
                steps++;   
            }
            var currentInstruction = instructions[currentInstructionIndex];

            currentPositions = currentPositions.Select(currentPosition =>
            {
                if (currentInstruction == 'L')
                {
                    return map[currentPosition.Left];
                }
                else
                {
                    return map[currentPosition.Right];
                }
            }).ToList();

            if (currentPositions.Any(x => x.Current.EndsWith('Z')))
            {
                Console.WriteLine(
                    $"{steps}: {string.Join(", ", currentPositions.Select((position, index) => new { position, index }).Where(x => x.position.Current.EndsWith('Z')).Select(x => x.index))}");
            }
            
            if (currentPositions.All(x=>x.Current.EndsWith('Z')))
            {
                break;
            }

            // if (steps > 10000000)
            // {
            //     throw new Exception("fasdgasd");
            // }

            currentInstructionIndex++;
            if (currentInstructionIndex >= instructions.Length) currentInstructionIndex = 0;
        }

        Console.WriteLine(steps);
    }
}