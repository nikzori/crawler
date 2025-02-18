using System.Security;
using Terminal.Gui;

public class Dungeon
{
    public const char WALL = '#';
    public const char FLOOR = '.';
    public static Terminal.Gui.Attribute FLOOR_COLOR = new(Color.Green, Color.Black);
    public static Terminal.Gui.Attribute WALL_COLOR = new(Color.DarkGray, Color.Black);
    public static Terminal.Gui.Attribute REVEALED_COLOR = new(Color.Blue, Color.Black); // For tiles that were seen but not in LOS

    public List<Map> floors;
    public int currentFloor = 0;

    public Dungeon(int floorCount)
    {
        floors = new();
        for (int i = 0; i < floorCount; i++)
        {
            floors.Add(new Map(5));
        }
    }

    public Map GetCurrentFloor()    
    {
        return floors[currentFloor];
    }


    // Bresenham's line algorithm, last codeblocks: https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
    // I'm too bad with geometry so far to get how it works. Like, yeah, we're iterating over x or y 
    // because a straight line is ( ax + by + c = 0 ), but this is weird. Math is weird.
    public static (int x, int y)[] GetLine((int x, int y) start, (int x, int y) end)
    {
        List<(int x, int y)> points;

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

public class Map
{
    public Cell[,] cells;
    public List<Stair> stairs;
    public Map(int stairCount)
    {
        cells = MapGen.GenerateCA(256, 256);
        if (stairCount > 0)
        {
            stairs = new(stairCount); 
            Random rng = new Random();
            for (int i = 0; i < stairs.Count; i++)
            {
                while (true) // spawn stairs on random empty tiles
                {
                    int x = rng.Next(1, cells.GetLength(0));
                    int y = rng.Next(1, cells.GetLength(1));
                    if (!cells[x,y].IsWall())
                    {
                        stairs[i] = new ((x, y), StairDirection.Down);
                        cells[x,y].AddGameObject(stairs[i]);
                        break;
                    }
                }
            }
        }
    }
    public void AddGameObject(GameObject gameObject)
    {
        int x = gameObject.pos.Item1;
        int y = gameObject.pos.Item2;
        cells[x, y].AddGameObject(gameObject);
    }
}

public struct Cell
{
    public Rune rune;
    public Terminal.Gui.Attribute colors;
    public bool isTransparent = true;
    public bool isWalkable = true;

    public bool isRevealed = false; //for line of sight

    public List<GameObject>? gObjects;
    public Cell()
    {
        rune = new('.');
    }
    public Cell(Rune rune, bool isTransparent, bool isWalkable, Terminal.Gui.Attribute colors)
    {
        this.rune = rune;
        this.isTransparent = isTransparent;
        this.isWalkable = isWalkable;
        this.colors = colors;
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
        if (IsWall())
            return rune;
        if (gObjects == null || gObjects.Count == 0 || gObjects.Last().entity == null)
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
    public bool IsWall()
    {
        return (rune.Value == Dungeon.WALL && !isWalkable) ? true : false;

    }
    public void Set(Rune rune, bool isWalkable, bool isTransparent, Terminal.Gui.Attribute colors)
    {
        this.rune = rune;
        this.isWalkable = isWalkable;
        this.isTransparent = isTransparent;
        this.colors = colors;
    }
    public void SetToWall()
    {
        this.Set(Dungeon.WALL, false, false, Dungeon.WALL_COLOR);
    }
    public void SetToFloor()
    {
        this.Set(Dungeon.FLOOR, true, true, Dungeon.FLOOR_COLOR);
    }
}
