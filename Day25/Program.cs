namespace Day25;

internal static class Program
{
    private const string TestInputPath = "TestInput.txt";
    private const string RealInputPath = "RealInput.txt";
    
    static void Main(string[] args)
    {
        Task1(RealInputPath);
        Task2(TestInputPath);
    }

    private static void Task1(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);
        var edges = ParseInput(input);

        var cuts = FindCuts(edges);
        while(cuts.Count == 0)
        {
            cuts = FindCuts(edges);
        }

        var edgesWithoutCuts =
            edges.Where(edge => !cuts.Any(cut =>
                edge.OriginalVertex1 == cut.OriginalVertex1 && edge.OriginalVertex2 == cut.OriginalVertex2)).ToList();

        var graph = BuildGraph(edgesWithoutCuts);
        
        var count1 = GetCountOfConnected(cuts.First().Vertex1, graph);
        var count2 = GetCountOfConnected(cuts.First().Vertex2, graph);
        
        Console.WriteLine(count1 * count2);
    }

    private static Dictionary<string, List<string>> BuildGraph(List<Edge> edges)
    {
        var result = new Dictionary<string, List<string>>();
        foreach (var edge in edges)
        {
            if (!result.TryAdd(edge.Vertex1, [edge.Vertex2]))
            {
                result[edge.Vertex1].Add(edge.Vertex2);
            }
            
            if (!result.TryAdd(edge.Vertex2, [edge.Vertex1]))
            {
                result[edge.Vertex2].Add(edge.Vertex1);
            }
        }

        return result;
    }

    private static int GetCountOfConnected(string node, Dictionary<string, List<string>> graph)
    {
        var visited = new HashSet<string>();
        var encountered = new Queue<string>();
        encountered.Enqueue(node);

        var count = 0;
        while (encountered.Any())
        {
            var current = encountered.Dequeue();
            visited.Add(current);
            count++;

            foreach (var connection in graph[current])
            {
                if (visited.Add(connection))
                {
                    encountered.Enqueue(connection);
                }
            }
        }

        return count;
    }

    private static List<Edge> FindCuts(List<Edge> originalGraph)
    {
        var workingGraph = CopyGraph(originalGraph);

        while (workingGraph.Count > 3)
        {
            ContractEdge(workingGraph);
        }

        return workingGraph.Count == 3 ? workingGraph : new List<Edge>();
    }

    private static readonly Random Rng = new Random();
    private static void ContractEdge(List<Edge> graph)
    {
        var randomEdgeIndex = Rng.Next(0, graph.Count);
        var randomEdge = graph[randomEdgeIndex];
        var vertexToRemove = randomEdge.Vertex1;

        var edgesToJoin = graph.Where(x => randomEdge.Vertex1 == x.Vertex1 || randomEdge.Vertex1 == x.Vertex2).ToList();
        foreach (var edge in edgesToJoin)
        {
            if (edge.Vertex1 == vertexToRemove) edge.Vertex1 = randomEdge.Vertex2;
            else edge.Vertex2 = randomEdge.Vertex2;
        }

        graph.RemoveAll(x=>x.Vertex1 == x.Vertex2);
    }

    private static List<Edge> CopyGraph(List<Edge> originalGraph)
    {
        var result = new List<Edge>();

        foreach (var edge in originalGraph)
        {
            result.Add(new Edge(edge.Vertex1, edge.Vertex2));
        }
        
        return result;
    }

    private static List<Edge> ParseInput(string[] input)
    {
        var graph = new List<Edge>();
        foreach (var line in input)
        {
            var parts = line.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var vertex1 = parts[0];
            var connectedVertices =
                parts[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            foreach (var vertex2 in connectedVertices)
            {
                graph.Add(new Edge(vertex1, vertex2));
            }
        }

        return graph;
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

public class Edge
{
    public Edge(string vertex1, string vertex2)
    {
        Vertex1 = vertex1;
        Vertex2 = vertex2;
        OriginalVertex1 = vertex1;
        OriginalVertex2 = vertex2;
    }

    public string Vertex1 { get; set; }
    
    public string Vertex2 { get; set; }
    
    public string OriginalVertex1 { get; set; }
    
    public string OriginalVertex2 { get; set; }

    public override string ToString()
    {
        return $"{Vertex1} - {Vertex2}";
    }
}