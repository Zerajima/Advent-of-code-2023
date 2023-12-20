using System.Collections;

namespace Day20;

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
        var modules = ParseInput(inputPath);

        var totalNumberOfPulses = new Dictionary<bool, long>
        {
            { false, 0 },
            { true, 0 }
        };
        for (int i = 1; i <= 1000; i++)
        {
            var (numberOfPulses, numberOfPulsesToRx) = ProcessButtonPress(modules);
            totalNumberOfPulses[true] += numberOfPulses[true];
            totalNumberOfPulses[false] += numberOfPulses[false];
            Console.WriteLine($"Tick {i}");
        }

        Console.WriteLine($"Result: {totalNumberOfPulses[false] * totalNumberOfPulses[true]}");
    }

    private static void Task2(string inputPath)
    {
        var modules = ParseInput(inputPath);

        var counter = 1;
        while(true)
        {
            var (numberOfPulses, numberOfPulsesToRx) = ProcessButtonPress(modules);
            
            if(numberOfPulsesToRx == 1) break;
            Console.WriteLine($"Tick {counter}: {numberOfPulsesToRx}");

            counter++;
        }

        Console.WriteLine($"Result: {counter}");
    }

    private static (Dictionary<bool, long> numberOfpulses, long numberOfPulsesToRx) ProcessButtonPress(Dictionary<string, Module> modules)
    {
        var queue = new Queue<(bool Signal, string ReceiverId, string SenderId)>();
        queue.Enqueue((false, "broadcaster", "button"));

        var numberOfPulses = new Dictionary<bool, long>
        {
            { false, 0 },
            { true, 0 }
        };
        var numberOfPulsesToRx = 0;

        while (queue.Any())
        {
            var (signal, receiverId, senderId) = queue.Dequeue();
            numberOfPulses[signal] += 1;

            if (receiverId == "rx") numberOfPulsesToRx++;
            
            if (modules.TryGetValue(receiverId, out var currentModule))
            {
                var recipients = currentModule.ProcessSignal(signal, senderId, modules);
                foreach (var recipient in recipients)
                {
                    queue.Enqueue((currentModule.State, recipient, currentModule.Id));
                }
            }
        }

        return (numberOfPulses, numberOfPulsesToRx);
    }

    private static Dictionary<string, Module> ParseInput(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);
        var modules = new Dictionary<string, Module>();
        foreach (var line in input)
        {
            var module = Module.Parse(line);
            modules.Add(module.Id, module);
        }

        foreach (var module in modules.Values)
        {
            foreach (var outputId in module.Outputs)
            {
                if (modules.TryGetValue(outputId, out var output))
                {
                    if (output.Type == '&') output.Memory.Add(module.Id, false);
                }
            }
        }

        return modules;
    }
}

public class Module
{
    public string Id { get; set; }
    
    public char Type { get; set; }
    
    public List<string> Outputs { get; set; }
    
    public bool State { get; set; }

    public Dictionary<string, bool> Memory { get; set; } = new Dictionary<string, bool>();

    public static Module Parse(string line)
    {
        var parts = line.Split("->", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var type = parts[0].First();
        var id = type == 'b' ? parts[0] : parts[0].Substring(1);
        var outputs = parts[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        return new Module()
        {
            Id = id,
            Type = type,
            Outputs = outputs.ToList(),
            State = false,
        };
    }

    public List<string> ProcessSignal(bool signal, string senderId, Dictionary<string, Module> modules)
    {
        if (Type == '%' && signal == false)
        {
            State = !State;
            return Outputs;
        }
        else if (Type == '&')
        {
            Memory[senderId] = signal;
            State = Memory.Values.Any(x => x != true);
            return Outputs;
        }
        else if (Type == 'b')
        {
            State = signal;
            return Outputs;
        }

        return [];
    }
}