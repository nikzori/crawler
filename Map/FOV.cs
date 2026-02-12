// References used: 
// https://www.roguebasin.com/index.php/FOV_using_recursive_shadowcasting - shadowcast concept
// https://fadden.com/tech/ShadowCast.cs.txt - C# code with commentary, very clusterfucky tbh

public static class FOV
{
    public static Transform[] transforms =
    {
        new Transform(1, 0, 0, 1), // NW-NE
        new Transform(0, 1, 1, 0), // NE-SE
        new Transform(-1, 0, 0, -1), // SE-SW
        new Transform(0, -1, -1, 0) // SW-NW
    };
    /// <summary>
    /// Renderer-agnostic shadowcast in a square around target (player) position.
    /// </summary>
    /// <returns>
    /// Two-dimensional boolean array depicting which map cells are visible.
    /// </returns>
    public static bool[,] Shadowcast(Map map, Vector2Int viewerPos, int visionRange)
    {


        bool[,] result = new bool[map.size.X, map.size.Y];

        for (int x = 0; x < map.size.X; x++)
            for (int y = 0; y < map.size.Y; y++)
                result[x, y] = false; //just in case

        result[viewerPos.X, viewerPos.Y] = true; //set player tile as visible

        // This algorythm is a bit too strict, but definitely beats raycasting
        // Slopes are defined with two points as (x1 - x2) / (y1 - y2), where [x1,y1] is a point on the map and [x2,y2] is the viewer(player) pos.
        // Since we're using local coordinates with viewer at [0,0], slopes become (x/y). 
        // Then we add local [x,y] to global coords, multiplying x and y to "rotate" coordinates (see Transform[] transforms above)
        //
        // We use slopes to defy an quadrant, e.g. slopes of -1 and 1 defy quadrant from NW diagonal to NE diagonal
        // for each quadrant, we go row by row for vertical ones and column by column for horizontal ones
        // If the slope of a particualr in the range of quadrant, that point is visible, because
        // if we run into an obstacle, we use it's position to recursively call the lightcast method 
        // using the obstacle's slope as one of the limiting slopes.
        //
        // We continue the initial method call, looking for an open cell. If we find one, we use its pos to assign a new left slope for this call
        // if we end a row on an obstacle, we break the loop.
        // that way, we call separate methods for each gap made by obstacles recursively

        for (int i = 0; i < transforms.Length; i++)
            LumosMaxima(map, viewerPos, 1, visionRange, -1f, 1f, transforms[i], ref result);

        return result;
    }

    private static void LumosMaxima(Map map, Vector2Int viewerPos, int startColumn, int visionRange,
            float leftSlope, float rightSlope, Transform transform, ref bool[,] result)
    {
        try
        {
            bool prevWasBlocked = false;

            for (int yc = startColumn; yc <= visionRange; yc++) // outer loop with y coords
            {
                if (prevWasBlocked)
                    break;

                float slopeBuffer = -1;
                float newRightSlope = rightSlope;
                for (int xc = -visionRange; xc <= visionRange; xc++)
                {
                    int realX = viewerPos.X + xc * transform.xx + yc * transform.xy;
                    int realY = viewerPos.Y + xc * transform.yx + yc * transform.yy;

                    if (realX < 1 || realX >= result.GetLength(0) - 1 || realY < 1 || realY >= result.GetLength(1) - 1) // OOB check
                        continue;

                    float posSlopeLeft = (xc - 0.5f) / (yc + 0.5f);
                    if (posSlopeLeft < leftSlope)
                        continue;
                    float posSlopeRight = (xc + 0.5f) / (yc + 0.5f);
                    if (posSlopeRight > rightSlope)
                        break;

                    Vector2Int currentPos = new(realX, realY);
                    result[realX, realY] = true; // everything between slopes is visible 
                    Cell cell = map.cells[currentPos];
                    cell.SetRevealed(true);

                    if (prevWasBlocked)
                    {
                        if (!cell.IsTransparent)
                            slopeBuffer = posSlopeRight;
                        else
                        {
                            prevWasBlocked = false;
                            leftSlope = slopeBuffer;
                        }
                    }
                    else
                    {
                        if (!cell.IsTransparent)
                        {
                            if (posSlopeLeft >= leftSlope)
                                LumosMaxima(map, viewerPos, yc + 1, visionRange, leftSlope, posSlopeLeft, transform, ref result);

                            prevWasBlocked = true;
                            slopeBuffer = posSlopeRight;
                        }
                    }
                }

            }
        }
        catch (Exception e)
        {
            File.WriteAllText(AppContext.BaseDirectory + "/FOVLog.txt", e.Message);
        }

    }


    // for NPCs
    public static bool CanSeePosition(Map map, Vector2Int start, Vector2Int target)
    {
        return false;
    }
}

public struct Slope
{
    public readonly int X, Y;
    public Slope(int x, int y) { X = x; Y = y; }
}

public class Transform
{
    public int xx { get; private set; }
    public int xy { get; private set; }
    public int yx { get; private set; }
    public int yy { get; private set; }
    public Transform(int xx, int xy, int yx, int yy)
    {
        this.xx = xx;
        this.xy = xy;
        this.yx = yx;
        this.yy = yy;
    }
}
