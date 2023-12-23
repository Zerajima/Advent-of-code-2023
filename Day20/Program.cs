namespace Day20;

using Pulse = (bool Signal, string ReceiverId, string SenderId);

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
            var numberOfPulses = ProcessButtonPress(modules);
            totalNumberOfPulses[true] += numberOfPulses[true];
            totalNumberOfPulses[false] += numberOfPulses[false];
            Console.WriteLine($"Tick {i}");
        }

        Console.WriteLine($"Result: {totalNumberOfPulses[false] * totalNumberOfPulses[true]}");
    }

    private static void Task2(string inputPath)
    {
        var modules = ParseInput(inputPath);
        var relevantModule = (ConjunctionModule)modules["qb"];

        var countersPerModuleInputs = new Dictionary<string, long>();
        
        var counter = 1L;
        while(true)
        {
            var relevantSignals = GetSignalsToModule(modules, relevantModule.Id);
            foreach (var signal in relevantSignals.Where(x=>x.Signal))
            {
                countersPerModuleInputs[signal.SenderId] = counter;
            }

            if (relevantModule.State.All(x => countersPerModuleInputs.ContainsKey(x.Key)))
            {
                break;
            }

            counter++;
        }

        Console.WriteLine($"Result: {countersPerModuleInputs.Values.Aggregate(1L, (acc, val) => acc * val)}");
    }
    
    private static List<Pulse> GetSignalsToModule(Dictionary<string, IModule> modules, string moduleId)
    {
        var queue = new Queue<Pulse>();
        queue.Enqueue((false, "broadcaster", "button"));
        var result = new List<Pulse>();
    
        while (queue.TryDequeue(out var pulse))
        {
            var (signal, receiverId, senderId) = pulse;

            if (receiverId == moduleId)
            {
                result.Add(pulse);
            }
            
            if (modules.TryGetValue(receiverId, out var currentModule))
            {
                foreach (var newPulse in currentModule.ProcessSignal(signal, senderId))
                {
                    queue.Enqueue(newPulse);
                }
            }
        }

        return result;
    }
    
    private static Dictionary<bool, long> ProcessButtonPress(Dictionary<string, IModule> modules)
    {
        var queue = new Queue<Pulse>();
        queue.Enqueue((false, "broadcaster", "button"));
    
        var numberOfPulses = new Dictionary<bool, long>
        {
            { false, 0 },
            { true, 0 }
        };
        while (queue.TryDequeue(out var pulse))
        {
            var (signal, receiverId, senderId) = pulse;
            numberOfPulses[signal] += 1;
    
            if (modules.TryGetValue(receiverId, out var receiverModule))
            {
                foreach (var newPulse in receiverModule.ProcessSignal(signal, senderId))
                {
                    queue.Enqueue(newPulse);
                }
            }
        }
    
        return numberOfPulses;
    }

    private static Dictionary<string, IModule> ParseInput(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);
        var modules = new Dictionary<string, IModule>();
        foreach (var line in input)
        {
            var parts = line.Split("->", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var id = parts[0];
            var outputs = parts[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            IModule module = id[0] switch
            {
                '%' => new FlipFlopModule(id[1..], outputs),
                '&' => new ConjunctionModule(id[1..], outputs),
                _ => new BroadcastModule(id, outputs),
            };
            
            //var module = Module.Parse(line);
            modules.Add(module.Id, module);
        }

        foreach (var module in modules.Values)
        {
            foreach (var outputId in module.Outputs)
            {
                if (modules.TryGetValue(outputId, out var output))
                {
                    if (output is ConjunctionModule conjunctionModule) conjunctionModule.State.Add(module.Id, false);
                }
            }
        }

        return modules;
    }
}

public interface IModule
{
    public string Id { get; set; }
    
    public List<string> Outputs { get; set; }

    public IEnumerable<Pulse> ProcessSignal(bool signal, string senderId);
}

public class BroadcastModule : IModule
{
    public BroadcastModule(string id, List<string> outputs)
    {
        Id = id;
        Outputs = outputs;
    }

    public string Id { get; set; }
    
    public List<string> Outputs { get; set; }

    public IEnumerable<Pulse> ProcessSignal(bool signal, string senderId)
    {
        return Outputs.Select(output => (signal, output, Id));
    }
}

public class FlipFlopModule : IModule
{
    public FlipFlopModule(string id, List<string> outputs)
    {
        Id = id;
        Outputs = outputs;
        State = false;
    }

    public string Id { get; set; }
    
    public List<string> Outputs { get; set; }
    
    public bool State { get; set; }
    
    public IEnumerable<Pulse> ProcessSignal(bool signal, string senderId)
    {
        if (!signal)
        {
            State = !State;
            return Outputs.Select(output => (State, output, Id));
        }

        return [];
    }
}

public class ConjunctionModule : IModule
{
    public ConjunctionModule(string id, List<string> outputs)
    {
        Id = id;
        Outputs = outputs;
        State = new Dictionary<string, bool>();
    }

    public string Id { get; set; }
    
    public List<string> Outputs { get; set; }
    
    public Dictionary<string, bool> State { get; set; }
    
    public IEnumerable<Pulse> ProcessSignal(bool signal, string senderId)
    {
        State[senderId] = signal;
        bool outputSignal = !State.Values.All(x => x);
        return Outputs.Select(output => (outputSignal, output, Id));
    }
}