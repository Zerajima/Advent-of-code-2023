using System.Drawing;

namespace Common;

public struct Line(Point start, Point end)
{
    public Point Start = start;
    public Point End = end;

    public static bool operator ==(Line l1, Line l2)
    {
        return l1.Start == l2.Start && l1.End == l2.End;
    }

    public static bool operator !=(Line l1, Line l2)
    {
        return !(l1 == l2);
    }

    public bool Intersects(Line anotherLine)
    {
        if (this == anotherLine) return true;
        
        //four direction for two lines and points of other line
        int dir1 = GetDirection(this.Start, this.End, anotherLine.Start);
        int dir2 = GetDirection(this.Start, this.End, anotherLine.End);
        int dir3 = GetDirection(anotherLine.Start, anotherLine.End, this.Start);
        int dir4 = GetDirection(anotherLine.Start, anotherLine.End, this.End);

        if (dir1 != dir2 && dir3 != dir4)
            return true; //they are intersecting

        if (dir1 == 0 && IsPointOnLine(this, anotherLine.Start)) //when p2 of line2 are on the line1
            return true;

        if (dir2 == 0 && IsPointOnLine(this, anotherLine.End)) //when p1 of line2 are on the line1
            return true;

        if (dir3 == 0 && IsPointOnLine(anotherLine, this.Start)) //when p2 of line1 are on the line2
            return true;

        if (dir4 == 0 && IsPointOnLine(anotherLine, this.End)) //when p1 of line1 are on the line2
            return true;

        return false;
    }

    private static int GetDirection(Point a, Point b, Point c) {
        int val = (b.Y-a.Y)*(c.X-b.X)-(b.X-a.X)*(c.Y-b.Y);
        if (val == 0)
            return 0;     //colinear
        else if(val < 0)
            return 2;    //anti-clockwise direction
        return 1;    //clockwise direction
    }

    private static bool IsPointOnLine(Line line, Point point)
    {
        //check whether p is on the line or not
        return point.X <= Math.Max(line.Start.X, line.End.X)
               && point.X >= Math.Min(line.Start.X, line.End.X)
               && point.Y <= Math.Max(line.Start.Y, line.End.Y) &&
               point.Y >= Math.Min(line.Start.Y, line.End.Y);
    }
}