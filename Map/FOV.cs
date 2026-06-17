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

        for (int i = 0; i < transforms.Length; i++)
        //for (int i = 0; i < 1; i++)
            LumosMaxima(map, viewerPos, 1, visionRange, -1f, 1f, transforms[i], ref result);

        return result;
    }

    /**
     * This algorithm is a bit too strict, but it sure beats raycasting. 
     * You can probably check for different points in a tile to make it more permissive, idk.
     *
     * Slopes between two points can be defined as (x1 - x2) / (y1 - y2). We're using local coordinates for the slope, 
     * with player position always at [0, 0], thus a slope between any random point [x1, y1] and player will always be (x1 / y1).
     * 
     * This function goes row by row, left to right through a quadrant defined as space between slopes (x1 / y1) = -1 and (x1 / y1) = 1, 
     * e.g. the area between NW diagonal and NE one. If a tile in a row is between the slopes, it's visible. The trick is that,
     * every time we find a wall, we set our left slope to the wall's position. Thus, everything behind the wall is now outside the slope.
     * However, this would also exclude open tiles that appeared before the wall; to counter this, we track whether the previous tile in the loop
     * was empty, and if the current tile is a wall, we recursively call this same method using yCurrent+1, current left slope, and the wall's position
     * for the right slope.
     * 
     * Since we're using local coords, we need to convert them to global to get correct tiles from the map object. 
     * This is done via transforms: see Transform[] above and realX with realY below).
     *
     * To get the entire area around the Player, go through all 4 quadrants with appropriate transforms. 
     */
    private static void LumosMaxima(Map map, Vector2Int viewerPos, int startColumn, int visionRange,
            float leftSlope, float rightSlope, Transform transform, ref bool[,] result)
    {
        try
        {
            bool prevWasBlocked = false;
            float slopeBuffer = -1;

            for (int yc = startColumn; yc <= visionRange; yc++)
            {
                for (int xc = -visionRange; xc <= visionRange; xc++)
                {
                    int realX = viewerPos.X + xc * transform.xx + yc * transform.xy;
                    int realY = viewerPos.Y + xc * transform.yx + yc * transform.yy;

                    if (realX < 1 || realX >= result.GetLength(0) - 1 ||
                        realY < 1 || realY >= result.GetLength(1) - 1) // OOB check
                        continue;

                    float posSlopeLeft = (xc - 0.5f) / (yc + 0.5f);
                    float posSlopeRight = (xc + 0.5f) / (yc + 0.5f);

                    // quadrant voodoo to pick correct tile corners when going past xc=0
                    float tileLeftSlope = Math.Min(posSlopeLeft, posSlopeRight);
                    float tileRightSlope = Math.Max(posSlopeLeft, posSlopeRight);
                    
                    if (tileLeftSlope >= rightSlope)  // has to be '>=' or the +x side of the quadrant will have an extra visible tile
                        break;
                    if (tileRightSlope <= leftSlope)  // has to be '<=' or the tiles on slope (-1/1) 
                        continue;                     // will be visible even when they are obscured by a wall

                    Vector2Int currentPos = new(realX, realY);
                    result[realX, realY] = true; // everything between slopes is visible
                    Cell cell = map.cells[currentPos];
                    cell.SetRevealed(true);

                    if (!cell.IsTransparent)
                    {
                        if (!prevWasBlocked)
                        {
                            // found an obstacle after open tiles - start a cycle for it;
                            // this check prevents the first tile in every row being always visible
                            if (tileLeftSlope > leftSlope)
                                LumosMaxima(map, viewerPos, yc + 1, visionRange, leftSlope, tileLeftSlope, transform, ref result);

                            prevWasBlocked = true;
                            slopeBuffer = tileRightSlope;
                        }
                        leftSlope = Math.Max(leftSlope, tileRightSlope);
                    }
                    else
                    {
                        prevWasBlocked = false;
                    }
                }
                // last tile in a row was a wall, we've already started cycles for gaps -- terminate this cycle
                if (prevWasBlocked)
                    break;
            }
        }
        catch (Exception e)
        {
            File.WriteAllText(AppContext.BaseDirectory + "/FOVLog.txt", e.Message);
        }

    }


    // for NPCs
    // better switch this to DCSS style creature vision
    public static bool CanSeePosition(Map map, Vector2Int start, Vector2Int target)
    {
        return Dungeon.CanSeeTile(map, start, target);
    }
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
