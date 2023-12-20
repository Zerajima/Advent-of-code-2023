namespace Day19;

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
        var (workflows, parts) = ParseInput(input);

        foreach (var part in parts)
        {
            var currentWorkflow = workflows["in"];

            var destination = "";
            while (true)
            {
                destination = currentWorkflow.Evaluate(part);

                if (destination == "A" || destination == "R")
                {
                    break;
                }
                
                currentWorkflow = workflows[destination];
            }
            
            if (destination == "A") score += part.Sum(x => x.Value);
        }
        
        Console.WriteLine(score);
    }

    private static void Task2(string inputPath)
    {
        var score = 0L;
        var input = File.ReadAllLines(inputPath);
        var (workflows, _) = ParseInput(input);

        var evaluationQueue = new Queue<(Workflow Workflow, Dictionary<char, (long min, long max)> possibleRanges)>();
        evaluationQueue.Enqueue((workflows["in"], new Dictionary<char, (long min, long max)>
        {
            {'x', (1, 4000)},
            {'m', (1, 4000)},
            {'a', (1, 4000)},
            {'s', (1, 4000)},
        }));

        while (evaluationQueue.Any())
        {
            var current = evaluationQueue.Dequeue();

            var workingRanges = current.possibleRanges; 
            foreach (var condition in current.Workflow.Conditions)
            {
                if(condition.Category == char.MinValue)
                {
                    if (condition.Destination == "A") score += CalculateCombinations(workingRanges);
                    else if (condition.Destination != "R") evaluationQueue.Enqueue((workflows[condition.Destination], workingRanges));
                }
                else
                {
                    var nextRanges = ReduceRangesWithCondition(workingRanges, condition);
                    if (condition.Destination == "A") score += CalculateCombinations(nextRanges);
                    else if (condition.Destination != "R")
                    {
                        evaluationQueue.Enqueue((workflows[condition.Destination], nextRanges));
                    }
                    
                    workingRanges = ReduceRangesWithOppositeCondition(workingRanges, condition);
                }
            }
        }

        Console.WriteLine(score);
    }

    private static Dictionary<char, (long min, long max)> ReduceRangesWithOppositeCondition(
        Dictionary<char, (long min, long max)> workingRanges, Condition condition)
    {
        var newRanges = new Dictionary<char, (long min, long max)>();

        foreach (var range in workingRanges)
        {
            if (condition.Category == range.Key)
            {
                newRanges.Add(range.Key,
                    condition.Comparer == '>'
                        ? (range.Value.min, condition.Value)
                        : (condition.Value, range.Value.max));
            }
            else
            {
                newRanges.Add(range.Key, range.Value);
            }
        }

        return newRanges;
    }

    private static Dictionary<char, (long min, long max)> ReduceRangesWithCondition(
        Dictionary<char, (long min, long max)> workingRanges, Condition condition)
    {
        var newRanges = new Dictionary<char, (long min, long max)>();

        foreach (var range in workingRanges)
        {
            if (condition.Category == range.Key)
            {
                newRanges.Add(range.Key,
                    condition.Comparer == '>'
                        ? (condition.Value + 1, range.Value.max)
                        : (range.Value.min, condition.Value - 1));
            }
            else
            {
                newRanges.Add(range.Key, range.Value);
            }
        }

        return newRanges;
    }

    private static long CalculateCombinations(Dictionary<char,(long min, long max)> currentPossibleRanges)
    {
        var result = 1L;
        foreach (var range in currentPossibleRanges.Values)
        {
            result *= range.max - range.min + 1;
        }
        return result;
    }

    private static (Dictionary<string, Workflow> workflows, List<Dictionary<char, int>> parts) ParseInput(string[] input)
    {
        var workflows = new Dictionary<string, Workflow>();
        var parts = new List<Dictionary<char, int>>();

        var parsingWorkflows = true;
        foreach (var line in input)
        {
            if (string.IsNullOrEmpty(line))
            {
                parsingWorkflows = false;
                continue;
            }

            if (parsingWorkflows)
            {
                var workflow = Workflow.Parse(line);
                workflows.Add(workflow.Name, workflow);
            }
            else
            {
                var lineParts = line.Trim(new[] { '{', '}' }).Split(',',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                var part = new Dictionary<char, int>();
                foreach (var linePart in lineParts)
                {
                    var values = linePart.Split('=');
                    part.Add(values[0].First(), int.Parse(values[1]));
                }

                parts.Add(part);
            }
        }

        return (workflows, parts);
    }
}

public class Workflow
{
    public string Name { get; set; }
    
    public List<Condition> Conditions { get; set; }

    public static Workflow Parse(string line)
    {
        var conditionIndex = line.IndexOf('{');
        var name = line.Substring(0, conditionIndex);
        var conditionsString = line.Substring(conditionIndex + 1, line.Length - conditionIndex - 2);
        var conditionsParts =
            conditionsString.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return new Workflow()
        {
            Name = name,
            Conditions = conditionsParts.Select(Condition.Parse).ToList(),
        };
    }

    public string Evaluate(Dictionary<char, int> part)
    {
        return Conditions.First(x => x.Matches(part)).Destination;
    }
}

public class Condition
{
    public char Category { get; set; }
    
    public char Comparer { get; set; }
    
    public long Value { get; set; }
    
    public string Destination { get; set; }

    public static Condition Parse(string input)
    {
        if (input.Contains(':'))
        {
            var category = input[0];
            var comparer = input[1];
            var indexOfDestinationDelimiter = input.IndexOf(':');
            var value = long.Parse(input.Substring(2, indexOfDestinationDelimiter - 2));
            var destination = input.Substring(indexOfDestinationDelimiter + 1);

            return new Condition()
            {
                Category = category,
                Comparer = comparer,
                Value = value,
                Destination = destination,
            };
        }
        else
        {
            return new Condition()
            {
                Destination = input,
            };
        }
    }

    public bool Matches(Dictionary<char, int> part)
    {
        if (Category == char.MinValue) return true;

        if (Comparer == '>')
        {
            return part[Category] > Value;
        }
        else
        {
            return part[Category] < Value;
        }
    }
}