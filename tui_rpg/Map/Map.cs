using Terminal.Gui;

public class Map
{
    public const char WALL = '#';
    public const char FLOOR = '.';

    public Cell[,] cells;

    public Map(int width, int height, int caIterations = 2)
    {
        cells = MapGen.GenerateCA(width, height, caIterations);
    }

    public void AddGameObject(GameObject gameObject)
    {
        int x = gameObject.pos.Item1;
        int y = gameObject.pos.Item2;
        cells[x, y].AddGameObject(gameObject);
    }

    // Bresenham's line algorithm, last codeblocks: https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm#All_cases
    // I'm too bad with geometry so far to get how it works. Like, yeah, we're iterating over x or y 
    // because a straight line is ( ax + by + c = 0 ), but this is weird. Math is weird.
    public static (int x, int y)[] GetLine((int x, int y) start, (int x, int y) end)
    {
        List<(int x, int y)> points = new List<(int x, int y)>();

        if (Math.Abs(end.x - start.x) > Math.Abs(end.y - start.y))
        {
            //need this since we're incrementing X in the function
            if (start.x > end.x)
                points = PlotLineLow((end.x, end.y), (start.x, start.y));
            else points = PlotLineLow((start.x, start.y), (end.x, end.y));
        }
        else
        {
            if (start.y > end.y)
                points = PlotLineHigh((end.x, end.y), (start.x, start.y));
            else points = PlotLineHigh((start.x, start.y), (end.x, end.y));
        }
        (int x, int y)[] result = points.ToArray();
        return result;

        List<(int x, int y)> PlotLineLow((int x, int y) start, (int x, int y) end)
        {
            List<(int x, int y)> result = new List<(int x, int y)>();

            int dx = end.x - start.x;
            int dy = end.y - start.y;
            int yi = 1;

            if (dy < 0)
            {
                yi = -1;
                dy = -dy;
            }
            int D = (2 * dy) - dx;
            int y = start.y;
            for (int x = start.x; x <= end.x; x++)
            {
                result.Add((x, y));
                if (D > 0)
                {
                    y = y + yi;
                    D = D + (2 * (dy - dx));
                }
                else D = D + 2 * dy;
            }
            return result;
        }


        List<(int x, int y)> PlotLineHigh((int x, int y) start, (int x, int y) end)
        {
            List<(int x, int y)> result = new List<(int x, int y)>();
            int dx = end.x - start.x;
            int dy = end.y - start.y;
            int xi = 1;
            if (dx < 0)
            {
                xi = -1;
                dx = -dx;
            }

            int D = (2 * dx) - dy;
            int x = start.x;
            for (int y = start.y; y <= end.y; y++)
            {
                result.Add((x, y));
                if (D > 0)
                {
                    x = x + xi;
                    D = D + (2 * (dx - dy));
                }
                else D = D + 2 * dx;
            }

            return result;
        }
    }
}

public struct Cell
{
    public Rune rune;
    public Color color = Color.White;
    public Color bgColor = Color.Black;

    public bool isTransparent = true;
    public bool isWalkable = true;

    public bool isRevealed = false; //for line of sight

    public List<GameObject>? gObjects;
    public Cell()
    {
        rune = new('.');
    }
    public Cell(Rune r, bool isTransparent, bool isWalkable)
    {
        rune = r;
        this.isTransparent = isTransparent;
        this.isWalkable = isWalkable;
    }
    public void AddGameObject(GameObject gObject)
    {
        if (gObjects == null)
            gObjects = new List<GameObject>();
        gObjects.Add(gObject);
    }
    public void RemoveGameObject(GameObject gObject)
    {
        if (gObjects != null)
        {
            if (gObjects.Contains(gObject))
                gObjects.Remove(gObject);
        }
    }

    public Rune GetRune()
    {
        if (gObjects == null)
            return rune;
        else if (gObjects.Count == 0 || gObjects.Last().entity == null)
            return rune;
        else return gObjects.Last().entity.rune;
    }

    //shortcuts for convenience
    public bool IsWalkable()
    {
        if (isWalkable)
            return true;
        else return false;
    }
    public bool IsTransparent()
    {
        if (isTransparent)
            return true;
        else return false;
    }
    public bool isWall()
    {
        return (rune.Value == Map.WALL && !isWalkable) ? true : false;

    }
}
