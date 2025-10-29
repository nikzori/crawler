using System.Text;

public static class FOV
{
    // square-shaped FOV using shadowcasting
    public static Rune[,] GetRenderedRunes(Map map, Vector2Int viewportSize, Vector2Int centerPos, int range)
    {
        Rune[,] result = new Rune[viewportSize.X, viewportSize.Y];
        bool[,] checkedCells = new bool[viewportSize.X, viewportSize.Y];

        // define center position for the viewport where the player is
        int hCenter = viewportSize.X / 2;
        int vCenter = viewportSize.Y / 2;

        //set up quadrants (clockwise)
        Quadrant[] quadrants = new Quadrant[4];
        for (int i = 0; i < 4; i++)
            quadrants[i] = new((QuadrantDirection)i, centerPos);


        return result;
    }

    // for NPCs
    public static bool CanSeePosition(Map map, Vector2Int start, Vector2Int target)
    {
        return false;
    }
}

public struct Quadrant
{
    QuadrantDirection direction;
    Vector2Int origin;
    public Quadrant(QuadrantDirection direction, Vector2Int origin)
    {
        this.direction = direction;
        this.origin = origin;
    }

    // convert a row + column position relative to the quadrant into map-relative vector
    public Vector2Int Transform(Vector2Int quadrantPosition)
    {
        Vector2Int result;
        switch (direction)
        {
            case QuadrantDirection.North:
                result = new Vector2Int(origin.X + quadrantPosition.Y, origin.Y - quadrantPosition.X);
                break;
            case QuadrantDirection.East:
                result = new Vector2Int(origin.X + quadrantPosition.Y, origin.Y + quadrantPosition.Y);
                break;
            case QuadrantDirection.South:
                result = new Vector2Int(origin.X + quadrantPosition.Y, origin.Y + quadrantPosition.X);
                break;
            case QuadrantDirection.West:
                result = new Vector2Int(origin.X - quadrantPosition.X, origin.Y + quadrantPosition.Y);
                break;

            default:
                result = new Vector2Int(origin.X + quadrantPosition.X, origin.Y - quadrantPosition.Y);
                break;
        }
        return result;

    }
}
public enum QuadrantDirection { North, East, South, West }
