// References used: 
// https://www.roguebasin.com/index.php/FOV_using_recursive_shadowcasting - shadowcast concept
// https://fadden.com/tech/ShadowCast.cs.txt - C# code with commentary, very clusterfucky tbh

public static class FOV
{
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

        // slopes are defined with two points as (x1 - x2) / (y1 - y2), where [x1,y1] is a point on the map and [x2,y2] is the player pos
        // we use slopes to defy an quadrand (?), e.g. slopes of -1 and 1 defy area from NW diagonal to NE diagonal
        // for each quadrand, we go row by row for vertical ones and column by column for horizontal ones
        // if the slope between a point on the map and playerPos is in the range of quadrand, that point is visible
        // if we run into an obstacle, we use it's position to call the same method using the obstacle's slope as one of the limiting slopes
        // we continue the initial method call, looking for an open cell. If we find one, we use its pos to assign a new left slope for this call
        // if we end the row on an obstacle, we break the loop.
        // that way, we call separate methods for each gap made by obstacles recursively

        LumosMaxima(map, viewerPos, viewerPos.Y + 1, visionRange, -1f, 1f, ref result);

        return result;
    }

    private static void LumosMaxima(Map map, Vector2Int viewerPos, int startColumn, int visionRange,
            float leftSlope, float rightSlope, ref bool[,] result)
    {
        try
        {
            bool prevWasBlocked = false;
            bool newLeftSlopeNeeded = false;
            for (int yc = startColumn; yc <= viewerPos.Y + visionRange; yc++) // outer loop with y coords
            {
                if (prevWasBlocked)
                    break;

                float newLeftSlope = leftSlope;
                float newRightSlope = rightSlope; //just in case
                for (int xc = viewerPos.X - visionRange; xc <= viewerPos.X + visionRange; xc++)
                {
                    if (xc < 1 || xc >= result.GetLength(0) - 1 || yc < 1 || yc >= result.GetLength(1) - 1) // OOB check
                        continue;

                    float posSlopeLeft = (xc - viewerPos.X - 0.5f) / (yc - viewerPos.Y + 0.5f);
                    float posSlopeRight = (xc - viewerPos.X + 0.5f) / (yc - viewerPos.Y + 0.5f);

                    if (posSlopeLeft >= leftSlope && posSlopeRight <= rightSlope)
                    {
                        Vector2Int currentPos = new(xc, yc);
                        result[xc, yc] = true; // because we're limited by the slopes, every cell between them will be visible
                        Cell cell = map.cells[currentPos];
                        cell.SetRevealed(true);
                        if (!cell.IsTransparent)
                        {
                            prevWasBlocked = true;
                            newRightSlope = posSlopeLeft;
                            if (!newLeftSlopeNeeded)
                            {
                                newLeftSlopeNeeded = true;
                                LumosMaxima(map, viewerPos, yc + 1, visionRange, leftSlope, newRightSlope, ref result);
                            }
                        }
                        else
                        {
                            prevWasBlocked = false;
                            if (newLeftSlopeNeeded)
                            {
                                leftSlope = posSlopeLeft;
                                newLeftSlopeNeeded = false;
                            }
                        }
                    }
                }

            }
        }
        catch (Exception e)
        {
            Game.Log(e.Message);
            // string LogFilePath = AppContext.BaseDirectory + "/ShadowcastErrorLog.txt";
            // File.AppendAllText(LogFilePath, e.Message);
        }

    }


    // for NPCs
    public static bool CanSeePosition(Map map, Vector2Int start, Vector2Int target)
    {
        return false;
    }

}

public enum QuadrantDirection { North, East, South, West }

public struct Slope
{
    public readonly int X, Y;
    public Slope(int x, int y) { X = x; Y = y; }
}
