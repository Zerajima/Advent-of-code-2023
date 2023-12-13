namespace Day5;

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

        var seeds = input[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Skip(1)
            .Select(x => new Seed()
            {
                Id = long.Parse(x)
            }).ToList();

        for (long i = 2; i < input.Length; i++)
        {
            var line = input[i];
            if (line == "seed-to-soil map:")
            {
                i = ConvertValues(seeds, i + 1, input, x => x.Id, (seed, value) => seed.Soil = value);
            }
            
            if (line == "soil-to-fertilizer map:")
            {
                i = ConvertValues(seeds, i + 1, input, x => x.Soil, (seed, value) => seed.Fertilizer = value);
            }
            
            if (line == "fertilizer-to-water map:")
            {
                i = ConvertValues(seeds, i + 1, input, x => x.Fertilizer, (seed, value) => seed.Water = value);
            }
            
            if (line == "water-to-light map:")
            {
                i = ConvertValues(seeds, i + 1, input, x => x.Water, (seed, value) => seed.Light = value);
            }
            
            if (line == "light-to-temperature map:")
            {
                i = ConvertValues(seeds, i + 1, input, x => x.Light, (seed, value) => seed.Temperature = value);
            }
            
            if (line == "temperature-to-humidity map:")
            {
                i = ConvertValues(seeds, i + 1, input, x => x.Temperature, (seed, value) => seed.Humidity = value);
            }
            
            if (line == "humidity-to-location map:")
            {
                i = ConvertValues(seeds, i + 1, input, x => x.Humidity, (seed, value) => seed.Location = value);
            }
        }

        Console.WriteLine(seeds.Min(x=>x.Location));
    }

    private static long ConvertValues(List<Seed> seeds, long inputIndex, string[] input, Func<Seed, long> sourceGetter, Action<Seed, long> destinationSetter)
    {
        var line = input[inputIndex];
        var map = new List<(long source, long destination, long length)>();
        while (!string.IsNullOrEmpty(line))
        {
            var mapParts = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse).ToList();
            map.Add((mapParts[1], mapParts[0], mapParts[2]));

            inputIndex++;
            if(inputIndex == input.Length) break;
            line = input[inputIndex];
        }

        foreach (var seed in seeds)
        {
            var source = sourceGetter(seed);
            var relevantMapping = map.FirstOrDefault(x => source >= x.source && source < x.source + x.length);
            var offset = source - relevantMapping.source;
            
            if(relevantMapping != default)
            {
                destinationSetter(seed, relevantMapping.destination + offset);
            }
            else
            {
                destinationSetter(seed, source);
            }
        }

        return inputIndex;
    }

    private static void Task2(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);

        var sourceRanges = input[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Skip(1)
            .Select(long.Parse)
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / 2)
            .Select(x => x.Select(v => v.Value).ToList())
            .Select(x=>(x.First(), x.Last()))
            .Cast<(long start, long length)>()
            .ToList();

        PrintRanges("seeds:", sourceRanges);
        
        for (long i = 2; i < input.Length; i++)
        {
            var line = input[i];
            if (line == "seed-to-soil map:")
            {
                (i, sourceRanges) = ConvertValues(sourceRanges, i + 1, input);
                PrintRanges("seed-to-soil map:", sourceRanges);
            }
            
            if (line == "soil-to-fertilizer map:")
            {
                (i, sourceRanges) = ConvertValues(sourceRanges, i + 1, input);
                PrintRanges("soil-to-fertilizer map:", sourceRanges);
            }
            
            if (line == "fertilizer-to-water map:")
            {
                (i, sourceRanges) = ConvertValues(sourceRanges, i + 1, input);
                PrintRanges("fertilizer-to-water map:", sourceRanges);
            }
            
            if (line == "water-to-light map:")
            {
                (i, sourceRanges) = ConvertValues(sourceRanges, i + 1, input);
                PrintRanges("water-to-light map:", sourceRanges);
            }
            
            if (line == "light-to-temperature map:")
            {
                (i, sourceRanges) = ConvertValues(sourceRanges, i + 1, input);
                PrintRanges("light-to-temperature map:", sourceRanges);
            }
            
            if (line == "temperature-to-humidity map:")
            {
                (i, sourceRanges) = ConvertValues(sourceRanges, i + 1, input);
                PrintRanges("temperature-to-humidity map:", sourceRanges);
            }
            
            if (line == "humidity-to-location map:")
            {
                (i, sourceRanges) = ConvertValues(sourceRanges, i + 1, input);
                PrintRanges("humidity-to-location map:", sourceRanges);
            }
        }

        Console.WriteLine(sourceRanges.Min(x=>x.start));
    }

    private static void PrintRanges(string header, List<(long start, long length)> sourceRanges)
    {
        Console.WriteLine(header);
        foreach (var sourceRange in sourceRanges)
        {
            Console.WriteLine($"{sourceRange.start}, {sourceRange.length}");
        }
        Console.WriteLine();
    }

    private static (long, List<(long start, long length)>) ConvertValues(List<(long start, long length)> sourceRanges, long inputIndex, string[] input)
    {
        var line = input[inputIndex];
        var map = new List<(long source, long destination, long length)>();
        while (!string.IsNullOrEmpty(line))
        {
            var mapParts = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse).ToList();
            map.Add((mapParts[1], mapParts[0], mapParts[2]));

            inputIndex++;
            if(inputIndex == input.Length) break;
            line = input[inputIndex];
        }

        var result = new List<(long ThreadStart, long length)>();
        foreach (var sourceRange in sourceRanges)
        {
            result.AddRange(ConvertRange(map, sourceRange));
        }

        return (inputIndex, result);
    }

    private static List<(long ThreadStart, long length)> ConvertRange(List<(long source, long destination, long length)> map,
        (long start, long length) sourceRange)
    {
        var result = new HashSet<(long ThreadStart, long length)>();

        foreach (var mapping in map.Where(mapping=>(sourceRange.start >= mapping.source && sourceRange.start < mapping.source + mapping.length)
                 || (mapping.source >= sourceRange.start && mapping.source < sourceRange.start + sourceRange.length)))
        {
            var matchingStart = Math.Max(mapping.source, sourceRange.start);
            var matchingEnd = Math.Min(sourceRange.start + sourceRange.length, mapping.source + mapping.length);
            var matchingLength = matchingEnd - matchingStart;
            
            var delta = mapping.source - mapping.destination;

            (long start, long length) destinationRange = (matchingStart - delta, matchingLength);

            result.Add(destinationRange);
            
            if (destinationRange.length != sourceRange.length)
            {
                if (sourceRange.start < mapping.source)
                {
                    var additionalRanges = ConvertRange(map, (sourceRange.start, mapping.source - sourceRange.start));
                    additionalRanges.ForEach(x=>result.Add(x));
                }

                if (sourceRange.start + sourceRange.length > mapping.source + mapping.length)
                {
                    var additionalRanges = ConvertRange(map,
                        (matchingStart + destinationRange.length,
                            sourceRange.start + sourceRange.length - (matchingStart + destinationRange.length)));
                    additionalRanges.ForEach(x=>result.Add(x));
                }
            }
        }

        if (!result.Any())
        {
            result.Add(sourceRange);
        }
        
        return result.ToList();
    }

    public class Seed
    {
        public long Id { get; set; }
        
        public long Soil { get; set; }
        
        public long Fertilizer { get; set; }
        
        public long Water { get; set; }
        
        public long Light { get; set; }
        
        public long Temperature { get; set; }
        
        public long Humidity { get; set; }
        
        public long Location { get; set; }
    }
}