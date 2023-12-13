using System.Drawing;

namespace Common;

public static class PointExtensions
{
    public static Point MoveHead(this Point position, string direction)
    {
        switch (direction)
        {
            case "R":
                return position with { X = position.X + 1 };
            case "L":
                return position with { X = position.X - 1 };
            case "D":
                return position with { Y = position.Y - 1 };
            case "U":
                return position with { Y = position.Y + 1 };
            default: 
                throw new Exception($"Unknown direction: {direction}");
        }
    }
    
    public static bool IsTouching(this Point position1, Point position2)
    {
        return Math.Abs(position1.X - position2.X) <= 1 && Math.Abs(position1.Y - position2.Y) <= 1;
    }
    
    public static Point MoveToward(this Point tailPosition, Point headPosition)
    {
        var resultX = tailPosition.X;
        var resultY = tailPosition.Y;
        if (tailPosition.X == headPosition.X)
        {
            if (tailPosition.Y > headPosition.Y)
            {
                resultY--;
            }
            else
            {
                resultY++;
            }
        }
        else if (tailPosition.Y == headPosition.Y)
        {
            if (tailPosition.X > headPosition.X)
            {
                resultX--;
            }
            else
            {
                resultX++;
            }
        }
        else
        {
            if (tailPosition.X > headPosition.X)
            {
                resultX--;
            }
            else
            {
                resultX++;
            }
            
            if (tailPosition.Y > headPosition.Y)
            {
                resultY--;
            }
            else
            {
                resultY++;
            }
        }

        return new Point(resultX, resultY);
    }
}