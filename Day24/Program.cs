using System.Collections;

namespace DafPoint2sPoint2y;

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
        (long minX, long minY, long maxX, long maxY) boundary = (200000000000000, 200000000000000, 400000000000000,
            400000000000000);
        var input = File.ReadAllLines(inputPath);
        var hailstones = ParseInput(input);

        var intersections = 0;
        for (int firstHailstoneIndex = 0; firstHailstoneIndex < hailstones.Count; firstHailstoneIndex++)
        {
            for (int secondHailstoneIndex = firstHailstoneIndex + 1; secondHailstoneIndex < hailstones.Count; secondHailstoneIndex++)
            {
                var firstHailstone = hailstones[firstHailstoneIndex];
                var secondHailstone = hailstones[secondHailstoneIndex];

                var intersection = GetIntersection(firstHailstone.GetXYProjection(), secondHailstone.GetXYProjection());
                if (intersection.exists && PointInBoundary(intersection.x, intersection.y, boundary)) intersections++;
            }
        }

        Console.WriteLine(intersections);
    }

    private static void Task2(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);
        var hailstones = ParseInput(input).Skip(18).Take(4);
        
        foreach (var velocityX in EnumerateVelocities())
        {
            foreach (var velocityY in EnumerateVelocities())
            {
                var intersectionXY =
                    TryGetCommonIntersection(hailstones.Select(x =>
                        x.GetXYProjection().AdjustFrameTo(velocityX, velocityY)).ToList());

                if (intersectionXY.exists)
                {
                    foreach (var velocityZ in EnumerateVelocities())
                    {
                        var intersectionXZ = TryGetCommonIntersection(hailstones.Select(x =>
                            x.GetXZProjection().AdjustFrameTo(velocityX, velocityZ)).ToList());

                        if (intersectionXZ.exists)
                        {
                            Console.WriteLine(intersectionXY.x + intersectionXY.y + intersectionXZ.y);
                            return;
                        }
                    }
                }
            }
        }
        
        Console.WriteLine("nothing");
    }

    private static IEnumerable<int> EnumerateVelocities()
    {
        return Enumerable.Range(-500, 1000);
    }

    private static (bool exists, decimal x, decimal y) TryGetCommonIntersection(List<HailstoneProjection> hailstones)
    {
        var commonX = 0m;
        var commonY = 0m;
        var currentIndex = 0;
        while (currentIndex < hailstones.Count - 1)
        {
            var intersection = GetIntersection(hailstones[currentIndex], hailstones[currentIndex + 1]);
            if (!intersection.exists) return intersection;

            if (currentIndex == 0)
            {
                commonX = intersection.x;
                commonY = intersection.y;
            }
            else if (intersection.x != commonX || intersection.y != commonY)
            {
                return (false, 0, 0);
            }

            currentIndex++;
        }

        return (true, commonX, commonY);
    }

    private static (bool exists, decimal x, decimal y) GetIntersection(HailstoneProjection firstHailstone, HailstoneProjection secondHailstone)
    {
        if (firstHailstone.VelocityX == 0 || secondHailstone.VelocityX == 0) return (false, 0, 0);
        
        if (firstHailstone.Slope == secondHailstone.Slope)
        {
            return (false, 0, 0);
        };

        var ix = (secondHailstone.YIntercept - firstHailstone.YIntercept) / (firstHailstone.Slope - secondHailstone.Slope);
        var iy = firstHailstone.Slope * ix + firstHailstone.YIntercept;
        
        var t1 = (ix - firstHailstone.InitialX) / firstHailstone.VelocityX;
        var t2 = (ix - secondHailstone.InitialX) / secondHailstone.VelocityX;

        if (t1 < 0 || t2 < 0) return (false, 0, 0);
        return (true, Math.Round(ix, 3), Math.Round(iy, 3));
    }

    private static bool PointInBoundary(decimal x, decimal y, (long minX, long minY, long maxX, long maxY) boundary)
    {
        return x >= boundary.minX && x <= boundary.maxX && y >= boundary.minY && y <= boundary.maxY;
    }

    private static List<Hailstone> ParseInput(string[] input)
    {
        var hailstones = new List<Hailstone>();
        foreach (var line in input)
        {
            var parts = line.Split(new[] { ' ', '@', ',' },
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
            
            hailstones.Add(new Hailstone()
            {
                InitialX = parts[0],
                InitialY = parts[1],
                InitialZ = parts[2],
                VelocityX = parts[3],
                VelocityY = parts[4],
                VelocityZ = parts[5],
            });
        }

        return hailstones;
    }
}

internal class Hailstone
{
    public long InitialX { get; set; }
    
    public long InitialY { get; set; }
    
    public long InitialZ { get; set; }
    
    public long VelocityX { get; set; }
    
    public long VelocityY { get; set; }
    
    public long VelocityZ { get; set; }

    public HailstoneProjection GetXYProjection()
    {
        return new HailstoneProjection()
        {
            InitialX = InitialX,
            InitialY = InitialY,
            VelocityX = VelocityX,
            VelocityY = VelocityY,
        };
    }
    
    public HailstoneProjection GetXZProjection()
    {
        return new HailstoneProjection()
        {
            InitialX = InitialX,
            InitialY = InitialZ,
            VelocityX = VelocityX,
            VelocityY = VelocityZ,
        };
    }
}

internal class HailstoneProjection
{
    public long InitialX { get; set; }
    
    public long InitialY { get; set; }
    
    public long VelocityX { get; set; }
    
    public long VelocityY { get; set; }

    public decimal Slope => VelocityY / (decimal)VelocityX;
    
    public decimal YIntercept => InitialY - (Slope * InitialX);

    public HailstoneProjection AdjustFrameTo(long frameVelocityX, long frameVelocityY)
    {
        return new HailstoneProjection()
        {
            InitialX = InitialX,
            InitialY = InitialY,
            VelocityX = VelocityX - frameVelocityX,
            VelocityY = VelocityY - frameVelocityY,
        };
    }
}