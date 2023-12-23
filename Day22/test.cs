using System.Numerics;
using System.Runtime.CompilerServices;

namespace AdventOfCode2023
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    //[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    class Day22
    {
        class Cube
        {
            public Vector3 OriginalPosition;

            public Vector3 BottomLeft;
            public Vector3 Size;
            public int Index; // we use it for part 2
            public List<Cube> cubesBellow = new();
            public List<Cube> cubesAbove = new();
        }


        private static List<Cube> GetCubes(string input)
        {
            // This code is being shared with Unity. So, we follow
            // same coordinate system. Computationally, it does not 
            // matter for the problem. From the original coordinate 
            // system perspective, we are moving our cubes along y axis
            // (to the left, if looking towards positive x).

            // Bellow is copied from Unity
            // -------------


            // Because unity works with left-handed coordinate system,
            // and we have our data provided with right-handed system
            // we have to switch y and z. this normally is not enough
            // because we need to reverse y after switch, but we don't
            // work with relations here, only world coordinates.
            // We do this only for our cubes to look naturally in 
            // unity camera.

            // this won't affect our computations

            var ret = new List<Cube>();
            var pairs = input.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in pairs)
            {
                var sp1 = pair.Split('~', ',').Select(int.Parse).ToArray();
                var v1 = new Vector3(sp1[0], sp1[2], sp1[1]);
                var v2 = new Vector3(sp1[3], sp1[5], sp1[4]);

                Vector3 bl = new(Math.Min(v1.x, v2.x), Math.Min(v1.y, v2.y), Math.Min(v1.z, v2.z));

                // note, we add 1 to our top right coordinate, because, following instructions:
                //>> A line like 2,2,2~2,2,2 means that both ends of the brick are at the same coordinate -
                //>> in other words, that the brick is a single cube.
                // Also not that we always preserve bottom left position, and expand to the top and right.
                // We do not care about top and right that much, all we care about is to preserve bottom coordinate
                Vector3 tr = new Vector3(1, 1, 1) + new Vector3(Math.Max(v1.x, v2.x), Math.Max(v1.y, v2.y), Math.Max(v1.z, v2.z));

                ret.Add(new Cube { OriginalPosition = v1, BottomLeft = bl, Size = tr - bl, Index = ret.Count });
            }
            return ret;
        }


        private void ApplyGravityAsync(List<Cube> cubes)
        {
            // this method is not the best solution here.
            // what it does is it moves cubes down by n amount
            // this is good for animation (unity)
            // but here, we could just check for down intersections and move to the 
            // first cube as seen from above
            // the following works, but is considerably slower

            // see ApplyGravityInstant()


            // sort cubes by their y coordinate
            cubes.Sort((a, b) => a.BottomLeft.y.CompareTo(b.BottomLeft.y));
            // re-apply indexes after sorting
            for (int i1 = 0; i1 < cubes.Count; i1++)
                cubes[i1].Index = i1;

            // note: position 1 is bottom, so we can't fall from that
            var fallenCubes = new List<Cube>();
            {
                for (int i = 0; i < cubes.Count; i++)
                {
                    Cube cube = cubes[i];

                    while (cube.BottomLeft.y > 1)
                    {
                        var delta = 0.5f; 

                        var ic = GetCubesIntersectingBellow(cube, fallenCubes, out var minDist, useShortRay: false);
                        // simulation problem: we may have bigger delta than our minDist
                        // this means our frame time is large. compensate
                        if (minDist < delta)
                        {
                            cube.BottomLeft.y -= minDist;
                        }
                        else
                        {
                            cube.BottomLeft.y -= delta;  // 1m/s
                        }

                        // check if there are any cubes intersecting with us
                        ic = GetCubesIntersectingBellow(cube, fallenCubes, out _);

                        if (ic.Count > 0 && minDist < 0.03f)
                        {
                            // link cube to cube (cube^2 :P)
                            foreach (var _ic in ic)
                            {
                                if (cube.cubesBellow.Contains(_ic) == false)
                                    cube.cubesBellow.Add(_ic);

                                if (_ic.cubesAbove.Contains(cube) == false)
                                    _ic.cubesAbove.Add(cube);
                            }

                            if (fallenCubes.Contains(cube) == false)
                                fallenCubes.Add(cube);
                            break;
                        }

                        if (cube.BottomLeft.y < 1)
                            cube.BottomLeft.y = 1;
                    }
                    if (fallenCubes.Contains(cube) == false)
                        fallenCubes.Add(cube);
                }


            }
        }

        private static void ApplyGravityInstant(List<Cube> cubes)
        {
            // sort cubes by their y coordinate
            cubes.Sort((a, b) => a.BottomLeft.y.CompareTo(b.BottomLeft.y));
            // re-apply indexes after sorting
            for (int i1 = 0; i1 < cubes.Count; i1++)
                cubes[i1].Index = i1;

            // note: position 1 is bottom, so we can't fall from that
            var fallenCubes = new List<Cube>();
            for (int i = 0; i < cubes.Count; i++)
            {
                Cube cube = cubes[i];
                if (cube.BottomLeft.y == 1)
                {
                    if (fallenCubes.Contains(cube) == false)
                        fallenCubes.Add(cube);
                    continue;
                }

                // check if there are any cubes intersecting with us
                var ic = GetCubesIntersectingBellow(cube, fallenCubes, out var minDist, useShortRay: false);

                if (ic.Count == 0)
                {
                    // we have nothing bellow us, drop to the bottom
                    cube.BottomLeft.y = 1;
                    if (fallenCubes.Contains(cube) == false)
                        fallenCubes.Add(cube);
                    continue;
                }

                // we have something bellow, move to that position
                cube.BottomLeft.y -= minDist;
                if (cube.BottomLeft.y < 1) // in case we moved bellow 1. shouldn't, but just in case
                    cube.BottomLeft.y = 1;


                // we must build our list again, but this time, with short ray, to only detect cubes that are very close to us
                ic = GetCubesIntersectingBellow(cube, fallenCubes, out minDist, useShortRay: true);

                if (ic.Count > 0)
                {
                    foreach (var _ic in ic)
                    {
                        if (cube.cubesBellow.Contains(_ic) == false) cube.cubesBellow.Add(_ic);
                        if (_ic.cubesAbove.Contains(cube) == false) _ic.cubesAbove.Add(cube);
                    }
                    if (fallenCubes.Contains(cube) == false) fallenCubes.Add(cube);
                }
            }
        }

        private static bool DoIntersectInXZPlane(Cube a, Cube b)
        {
            Rect ar = new(a.BottomLeft.x, a.BottomLeft.z, a.Size.x, a.Size.z);
            Rect br = new(b.BottomLeft.x, b.BottomLeft.z, b.Size.x, b.Size.z);

            if (ar == br) return true;
            if (ar.Overlaps(br)) return true;
            if (br.Overlaps(ar)) return true;
            return false;
        }

        private static List<Cube> GetCubesIntersectingBellow(Cube cube, List<Cube> otherCubes, out double minimumFoundDistance, bool useShortRay = true)
        {
            minimumFoundDistance = double.MaxValue;
            var ret = new List<Cube>();
            foreach (var fc in otherCubes)
            {
                // get all cubes that are 
                // a) bellow us
                // b) totally bellow us (together with their size)
                // fallen cubes "should" be bellow us
                // but we need to check for that anyway
                if (fc.BottomLeft.y + fc.Size.y <= cube.BottomLeft.y)
                {
                    // "short" ray check
                    if (useShortRay && cube.BottomLeft.y - (fc.BottomLeft.y + fc.Size.y) > 0.1f)
                        continue;

                    // now, check if our orthogonal (xz) mapping intersects this cube's mapping.
                    // if so, this cube is bellow us and we should consider it
                    if (DoIntersectInXZPlane(cube, fc))
                    {
                        minimumFoundDistance = Math.Min(cube.BottomLeft.y - (fc.BottomLeft.y + fc.Size.y), minimumFoundDistance);
                        ret.Add(fc);
                    }
                }
            }
            return ret;
        }

        private static void ReportPossibleBrickDisintegration(List<Cube> cubes, out int part1, out int part2)
        {
            // technically, we could split this
            // into part 1 and part 2, but let's just keep it same as in unity
            part1 = 0;
            part2 = 0;

            Span<bool> fallenFlags = new(new bool[cubes.Count]);

            for (int i = 0; i < cubes.Count; i++)
            {
                Cube cube = cubes[i];

                // part 1
                if (!cube.cubesAbove.Any(a => a.cubesBellow.Count == 1 && a.cubesBellow[0] == cube))
                {
                    part1++;
                    // because this cube does not cause any other to fall, 
                    // it won't count in part 2. so, we can skip that test.
                    // saves us some time
                    continue;
                }

                // part 2
                part2 += CheckFallenAbove_NR(cube, fallenFlags);
            }
        }

        private static bool CheckIfAllSupportMarkedForFall(Cube cube, Span<bool> fallenFlags)
        {
            foreach (var cb in cube.cubesBellow)
            {
                if (!fallenFlags[cb.Index])
                    return false;
            }
            return true;
        }
        private static int CheckFallenAbove_NR(Cube startCube, Span<bool> fallenFlags)
        {
            var count = 0;

            Queue<Cube> cubesToCheckAndMark = new();
            fallenFlags.Clear();
            fallenFlags[startCube.Index] = true;

            cubesToCheckAndMark.Enqueue(startCube);
            while (cubesToCheckAndMark.TryDequeue(out var cube))
            {
                foreach (var above in cube.cubesAbove)
                {
                    // if checked already, don't queue
                    if (fallenFlags[above.Index])
                        continue;
                    
                    // check if all support cubes of the cube above are marked for fall or deletion
                    if (CheckIfAllSupportMarkedForFall(above, fallenFlags))
                    {
                        // mark this cube for fall
                        fallenFlags[above.Index] = true;
                        
                        // add to our overall counter
                        count++;

                        // also, add it to processing
                        cubesToCheckAndMark.Enqueue(above);
                    }
                }
            }

            return count;
        }


        private static int part2Answer;

        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(string[] input, int lineWidth, int count)
        {
            // because of how unity processed our input,
            // we have to format it in special way
            var cubes = GetCubes(string.Join(" ", input));
            ApplyGravityInstant(cubes);
            ReportPossibleBrickDisintegration(cubes, out var p1, out part2Answer);
            return p1;
        }
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(string input, int lineWidth, int count)
        {
            // we've done everything in part 1...
            return part2Answer;
        }
    }
}

public struct Vector3
{
    public static readonly Vector3 zero = new Vector3(0d, 0d, 0d);

    public double x;
    public double y;
    public double z;

    public Vector3(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public Vector3(double[] numbers, int offset)
    {
        x = numbers[0 + offset];
        y = numbers[1 + offset];
        z = numbers[2 + offset];
    }
    public Vector3(long[] numbers, int offset)
    {
        x = numbers[0 + offset];
        y = numbers[1 + offset];
        z = numbers[2 + offset];
    }
    public Vector3(int[] numbers, int offset)
    {
        x = numbers[0 + offset];
        y = numbers[1 + offset];
        z = numbers[2 + offset];
    }
    public Vector3(float[] numbers, int offset)
    {
        x = numbers[0 + offset];
        y = numbers[1 + offset];
        z = numbers[2 + offset];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator -(Vector3 a) => new Vector3(0d - a.x, 0d - a.y, 0d - a.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator /(Vector3 a, double d) => new Vector3(a.x / d, a.y / d, a.z / d);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Vector3(Vector2 v2) => new(v2.x, v2.y, 0);


    public override readonly string ToString() => $"{x}, {y}, {z}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Cross(Vector3 lhs, Vector3 rhs) => new(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Dot(Vector3 lhs, Vector3 rhs) => lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
    public double sqrMagnitude
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => x * x + y * y + z * z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator *(Vector3 a, double d) => new Vector3(a.x * d, a.y * d, a.z * d);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator *(double d, Vector3 a) => new Vector3(a.x * d, a.y * d, a.z * d);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Magnitude(Vector3 vector) => Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Normalize()
    {
        double num = Magnitude(this);
        if (num > 1E-05d)
        {
            this /= num;
        }
        else
        {
            this = zero;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Normalize(Vector3 value)
    {
        double num = Magnitude(value);
        if (num > 1E-05d)
        {
            return value / num;
        }

        return zero;
    }
}

public struct Vector2
{
    public static readonly Vector2 zero = new Vector2(0d, 0d);

    public double x;
    public double y;

    public Vector2(double x, double y)
    {
        this.x = x;
        this.y = y;
    }
    public Vector2(Vector3 v3)
    {
        this.x = v3.x;
        this.y = v3.y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 operator *(Vector2 a, double b) => new(a.x * b, a.y * b);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.x + b.x, a.y + b.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.x - b.x, a.y - b.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 operator /(Vector2 a, double d) => new Vector2(a.x / d, a.y / d);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Vector2(Vector3 v3) => new(v3.x, v3.y);


    public override readonly string ToString() => $"{x}, {y}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Cross(Vector2 v1, Vector2 v2) => v1.x * v1.y - v1.y * v2.x;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Dot(Vector2 lhs, Vector2 rhs) => lhs.x * rhs.x + lhs.y * rhs.y;
    public double sqrMagnitude
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => x * x + y * y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Angle(Vector2 from, Vector2 to)
    {
        double num = (double)Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
        if (num < 1E-15d)
        {
            return 0d;
        }

        double num2 = Math.Clamp(Dot(from, to) / num, -1d, 1d);
        return (double)Math.Acos(num2) * 57.29578; // 57.295... == 1.000 rad (radian)
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsParallel(Vector2 v1, Vector2 v2)
    {
        //2 vectors are parallel if the angle between the vectors are 0 or 180 degrees
        var a = Angle(v1, v2);
        return a == 0d || a == 180;
    }
    //Are 2 vectors orthogonal?
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOrthogonal(Vector2 v1, Vector2 v2)
    {
        //2 vectors are orthogonal is the dot product is 0
        //We have to check if close to 0 because of floating numbers
        return Math.Abs(Dot(v1, v2)) < 0.000001;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsParallel(Vector2 v2)
    {
        //2 vectors are parallel id the angle between the vectors are 0 or 180 degrees
        var a = Angle(this, v2);
        return a == 0d || a == 180;
    }
    //Are 2 vectors orthogonal?
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOrthogonal(Vector2 v2)
    {
        //2 vectors are orthogonal is the dot product is 0
        //We have to check if close to 0 because of floating numbers
        return Math.Abs(Dot(this, v2)) < 0.000001;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Magnitude(Vector2 vector) => Math.Sqrt(vector.x * vector.x + vector.y * vector.y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Normalize()
    {
        double num = Magnitude(this);
        if (num > 1E-05d)
        {
            this /= num;
        }
        else
        {
            this = zero;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Normalize(Vector2 value)
    {
        double num = Magnitude(value);
        if (num > 1E-05d)
        {
            return value / num;
        }

        return zero;
    }


}

public struct Rect
{
    public double x;
    public double y;
    public double width;
    public double height;

    public Rect(double x, double y, double width, double height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
    public bool Contains(Vector2 vector2) => vector2.x >= x && vector2.y >= y && vector2.x <= x + width && vector2.y <= y + height;
    public bool Overlaps(Rect other) => (other.width + other.x) > x && other.x < (width + x) && (other.height + other.y) > y && other.y < (height + y);
    public static bool operator !=(Rect lhs, Rect rhs) => !(lhs == rhs);
    public static bool operator ==(Rect lhs, Rect rhs) => lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;

    public override readonly bool Equals(object? obj) => obj != null && obj is Rect rect && this == rect;
    public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode() ^ width.GetHashCode() ^ height.GetHashCode();
}

