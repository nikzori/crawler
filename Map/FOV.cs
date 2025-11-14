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

        // set up quadrants or octants starting from NNW 
        // for each area move from diagonal to cardinal line
        //  


        return result;
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
