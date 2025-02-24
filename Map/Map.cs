using System.Security;
using Terminal.Gui;

public class Dungeon
{
    public const char WALL = '#';
    public const char FLOOR = '.';
    public static Terminal.Gui.Attribute FLOOR_COLOR = new(Color.Gray, Color.Black);
    public static Terminal.Gui.Attribute WALL_COLOR = new(Color.Brown, Color.Black);
    public static Terminal.Gui.Attribute REVEALED_COLOR = new(Color.Blue, Color.Black); // For tiles that were seen but not in LOS
    public static Terminal.Gui.Attribute OBSCURED_COLOR = new(Color.DarkGray, Color.Black);

    public List<Map> floors;
    public List<Creature> creatures;
    public int currentFloor = 0;

    public Dungeon(int floorCount)
    {
        Random rng = new();
        floors = new();
        for (int i = 0; i < floorCount; i++)
        {
            
            if (i == floorCount - 1)
            {
                floors.Add(new(0));
            }
            else floors.Add(new Map(10));
        }
        for (int i = 0; i < floors.Count - 1; i++)
        {
            for (int j = 0; j < floors[i].stairs.Count; j++)
            {
                while (true)
                {
                    int x = rng.Next(0, floors[i+1].cells.GetLength(0));
                    int y = rng.Next(0, floors[i+1].cells.GetLength(1));
                    if ( (floors[i+1].cells[x,y].gObjects is null || floors[i+1].cells[x,y].gObjects?.Count == 0) && floors[i+1].cells[x,y].isWalkable)
                    {
                        Stairs s = new Stairs((x, y), StairDirection.Up, floors[i].stairs[j]);
                        floors[i+1].cells[x,y].AddGameObject(s);
                        floors[i].stairs[j].SetLinkedStairs(s);
                        break;
                    }
                }
            }
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
                    y += yi;
                    D += 2 * (dy - dx);
                }
                else D += 2 * dy;
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
                    x += xi;
                    D += 2 * (dx - dy);
                }
                else D += 2 * dx;
            }

            return result;
        }
    }

    /// <summary>
    /// Attempt to draw a Bresenham's Line from start to end. Returns false if any tile in the way is not transparent.
    /// </summary>
    public static bool CanSeeTile((int x, int y) start, (int x, int y) end)
    {
        bool isVisible;

        if (Math.Abs(end.x - start.x) > Math.Abs(end.y - start.y))
        {
            //need this since we're incrementing X in the function
            if (start.x > end.x)
                isVisible = PlotLineLow((end.x, end.y), (start.x, start.y));
            else isVisible = PlotLineLow((start.x, start.y), (end.x, end.y));
        }
        else
        {
            if (start.y > end.y)
                isVisible = PlotLineHigh((end.x, end.y), (start.x, start.y));
            else isVisible = PlotLineHigh((start.x, start.y), (end.x, end.y));
        }
        return isVisible;

        bool PlotLineLow((int x, int y) start, (int x, int y) end)
        {
            bool result = true;

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
                if (!Game.currentMap.cells[x,y].isTransparent && x != end.x && x != start.x)
                    return false;
                if (D > 0)
                {
                    y += yi;
                    D += 2 * (dy - dx);
                }
                else D += 2 * dy;
            }
            return result;
        }


        bool PlotLineHigh((int x, int y) start, (int x, int y) end)
        {
            bool result = true;
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
                if (!Game.currentMap.cells[x,y].isTransparent && y != end.y && y != start.y)
                    return false;
                if (D > 0)
                {
                    x += xi;
                    D += 2 * (dx - dy);
                }
                else D += 2 * dx;
            }

            return result;
        }
    }
}

public class Map
{
    public Cell[,] cells;
    public List<Stairs> stairs;
    public char[,] background; // static chars to draw over unexplored tiles
    public Map(int stairCount, int xLength = 128, int yLength = 128)
    {
        Random rng = new Random();

        cells = MapGen.GenerateCA(xLength, yLength);
        if (stairCount > 0)
        {
            stairs = new(stairCount); 
            for (int i = 0; i < stairCount; i++)
            {
                while (true) // spawn stairs on random empty tiles
                {
                    int x = rng.Next(1, cells.GetLength(0));
                    int y = rng.Next(1, cells.GetLength(1));
                    if (!cells[x,y].IsWall())
                    {
                        stairs.Add(new ((x, y), StairDirection.Down));
                        cells[x,y].AddGameObject(stairs[i]);
                        break;
                    }
                }
            }
        }
        
        background = new char[xLength + 30, yLength + 30]; // 15 extra tiles on each side
        int t;
        for (int x = 0; x < background.GetLength(0); x++)
        {
            for (int y = 0; y < background.GetLength(1); y++)
            {
                background[x,y] = ' ';
                t = rng.Next(0, 161);
                if (t < 5) background[x,y] = '`';
                if (t < 4) background[x,y] = '/';
                if (t < 3) background[x,y] = '*';
                if (t < 2) background[x,y] = '\\';
                if (t < 1) background[x,y] = 'x';
            }
        }
    }
    public void AddGameObject(GameObject gameObject)
    {
        int x = gameObject.pos.x;
        int y = gameObject.pos.y;
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
        if (gObjects == null || gObjects.Count == 0)
            return rune;
        else return gObjects.Last().rune;
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
