using System.Diagnostics;

namespace Day12;

internal static class Program
{
    private const string TestInputPath = "TestInput.txt";
    private const string RealInputPath = "RealInput.txt";
    
    static void Main(string[] args)
    {
        Task1(RealInputPath);
        //Task2(RealInputPath);
    }

    private static void Task1(string inputPath)
    {
        checked
        {
            var score = 0L;
            var input = File.ReadAllLines(inputPath);

            var stopwatch = Stopwatch.StartNew();
            foreach (var line in input)
            {
                var validPermutations = GetNumberOfValidPermutations(line);
                score += validPermutations;
                
                Console.WriteLine($"{line}: {validPermutations}");
            }

            Console.WriteLine(score);
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }
    }

    private static void Task2(string inputPath)
    {
        checked
        {
            var score = 0L;
            var input = File.ReadAllLines(inputPath);

            var stopwatch = Stopwatch.StartNew();
            foreach (var line in input)
            {
                var validPermutations = GetNumberOfValidPermutations(line);
                score += validPermutations;
                
                Console.WriteLine($"{line}: {validPermutations}");
            }

            Console.WriteLine(score);
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }
    }

    private static long GetNumberOfValidPermutations(string line)
    {
        var parts = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var map = parts[0];
        var relevantMapSlices = map.Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var damagedRanges = parts[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

        // Split map into slices, where damaged vents can be located
        // For each slice get all possible permutations:
        //  - it contains no vent: 1 permutation
        //  - it contains 1 vent: permutations == sliceLength
        //  - it contains 2 vents: permutations == ?
        //  - ...
        //  - stop at max possible number of ranges, according to slice length: sliceLength >= sum(maxPermutations) + count(maxPermutations) - 1
        // Each next slice, should get permutations for all solution permutations from previous slices:
        //  - previous slice consumed 0 ranges: 
        //  - previous slice consumed 1 range:
        //  - ... up to max ranges, that could be consumed by previous slices

        foreach (var mapSlice in relevantMapSlices)
        {
            var possibleRanges = GetRangesThatFitInSlice(damagedRanges, mapSlice);

            possibleRanges.Clear();
        }

        return GetSolutionsForMapSlice(map, damagedRanges);
    }

    private static List<int> GetRangesThatFitInSlice(List<int> damagedRanges, string mapSlice)
    {
        var possibleRanges = new List<int>();
        var requiredPadding = 0;
        foreach (var damagedRange in damagedRanges)
        {
            if (damagedRange + requiredPadding <= mapSlice.Length) possibleRanges.Add(damagedRange);
            else break;
            requiredPadding = 1;
        }

        return possibleRanges;
    }

    private static long GetSolutionsForMapSlice(string map, List<int> damagedRanges)
    {
        var solutions = new Stack<Solution>();
        solutions.Push(new Solution()
        {
            Position = 0,
            RamainingRanges = damagedRanges,
            DamagedSpringsKnown = map.Count(x => x == '#')
        });

        var validPermutations = 0L;
        while (solutions.Any())
        {
            var currentSolution = solutions.Pop();

            if (currentSolution.IsComplete)
            {
                validPermutations++;
                continue;
            }

            if (currentSolution.IsInvalid(map)) continue;

            foreach (var possibleSolution in GetPossibleSolutions(currentSolution, map))
            {
                solutions.Push(possibleSolution);
            }
        }

        return validPermutations;
    }

    private static List<Solution> GetPossibleSolutions(Solution currentSolution, string map)
    {
        var possibleSolutions = new List<Solution>();
        var currentRange = currentSolution.RamainingRanges.First();
        var currentSpring = map[currentSolution.Position];

        if (currentSpring == '.')
        {
            possibleSolutions.Add(currentSolution.GetSolutionForNextPosition());
        }
        else
        {
            var requiredSpace = map.Substring(currentSolution.Position, currentRange);
            var springAfterRange = GetSpringAfterRange(map, currentSolution, currentRange);
            if (!requiredSpace.Contains('.') && IsSpringEmptyOrNotDamaged(springAfterRange))
            {
                possibleSolutions.Add(currentSolution.GetSolutionForValidRange(currentRange, requiredSpace));
            }

            if (currentSpring == '?')
            {
                possibleSolutions.Add(currentSolution.GetSolutionForNextPosition());
            }
        }

        return possibleSolutions;
    }

    private static char GetSpringAfterRange(string map, Solution currentSolution, int currentRange)
    {
        return map.Length <= currentSolution.Position + currentRange ? '\0' : map[currentSolution.Position + currentRange];
    }

    private static bool IsSpringEmptyOrNotDamaged(char springAfterRequiredSpace)
    {
        return springAfterRequiredSpace == '\0' || springAfterRequiredSpace == '.' || springAfterRequiredSpace == '?';
    }

    public class Solution
    {
        public int Position { get; set; }
        
        public bool IsComplete => !RamainingRanges.Any() && DamagedSpringsKnown == DamagedSpringsMatched;

        public List<int> RamainingRanges = new List<int>();
        
        public int DamagedSpringsKnown { get; set; }
        
        public int DamagedSpringsMatched { get; set; }
        
        public bool IsInvalid(string map)
        {
            if (!RamainingRanges.Any())
            {
                return true;
            }

            if (map.Length - Position < RamainingRanges.Sum() + RamainingRanges.Count - 1)
            {
                return true;
            }

            return false;
        }
        
        public Solution GetSolutionForNextPosition()
        {
            return new Solution()
            {
                RamainingRanges = RamainingRanges,
                Position = Position + 1,
                DamagedSpringsKnown = DamagedSpringsKnown,
                DamagedSpringsMatched = DamagedSpringsMatched,
            };
        }
        
        public Solution GetSolutionForValidRange(int currentRange, string requiredSpace)
        {
            return new Solution()
            {
                RamainingRanges = RamainingRanges.Skip(1).ToList(),
                Position = Position + currentRange + 1,
                DamagedSpringsKnown = DamagedSpringsKnown,
                DamagedSpringsMatched = DamagedSpringsMatched + requiredSpace.Count(x => x == '#'),
            };
        }
    }
}