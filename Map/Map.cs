using System.Text;
using Terminal.Gui.Drawing;
using Attribute = Terminal.Gui.Drawing.Attribute;

public class Dungeon
{
    // should move these to a separate container for ncurses
    public static Rune WALL = new Rune('#');
    public static Rune FLOOR = new Rune('.');
    public static Attribute FLOOR_COLOR = new(Color.Green, Color.Black);
    public static Attribute WALL_COLOR = new(Color.Yellow, Color.Black);
    public static Attribute REVEALED_COLOR = new(Color.Gray, Color.Black); // For tiles that were seen but not in LOS
    public static Attribute OBSCURED_COLOR = new(Color.DarkGray, Color.Black);

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

        //TODO: new stairs generation
    }

    public Map GetCurrentFloor()
    {
        return floors[currentFloor];
    }


    // Bresenham's line algorithm, last codeblocks: https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
    // I'm too bad with geometry so far to get how it works. Like, yeah, we're iterating over x or y 
    // because a straight line is ( ax + by + c = 0 ), but this is weird. Math is weird.
    public static Vector2Int[] GetLine(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> points;

        if (Math.Abs(end.X - start.X) > Math.Abs(end.Y - start.Y))
        {
            //need this since we're incrementing X in the function
            if (start.X > end.X)
                points = PlotLineLow(end, start);
            else points = PlotLineLow(start, end);
        }
        else
        {
            if (start.Y > end.Y)
                points = PlotLineHigh(end, start);
            else points = PlotLineHigh(start, end);
        }
        Vector2Int[] result = points.ToArray();
        return result;

        List<Vector2Int> PlotLineLow(Vector2Int start, Vector2Int end)
        {
            List<Vector2Int> result = new();

            int dx = end.X - start.X;
            int dy = end.Y - start.Y;
            int yi = 1;

            if (dy < 0)
            {
                yi = -1;
                dy = -dy;
            }
            int D = (2 * dy) - dx;
            int y = start.Y;

            for (int x = start.X; x <= end.X; x++)
            {
                result.Add(new(x, y));
                if (D > 0)
                {
                    y += yi;
                    D += 2 * (dy - dx);
                }
                else D += 2 * dy;
            }
            return result;
        }


        List<Vector2Int> PlotLineHigh(Vector2Int start, Vector2Int end)
        {
            List<Vector2Int> result = new();
            int dx = end.X - start.X;
            int dy = end.Y - start.Y;
            int xi = 1;
            if (dx < 0)
            {
                xi = -1;
                dx = -dx;
            }

            int D = (2 * dx) - dy;
            int x = start.X;
            for (int y = start.Y; y <= end.Y; y++)
            {
                result.Add(new(x, y));
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
    public static bool CanSeeTile(Vector2Int start, Vector2Int end)
    {
        bool isVisible;
        if (start == end)
            return true;
        if (Math.Abs(end.X - start.X) > Math.Abs(end.Y - start.Y))
        {
            //need this since we're incrementing X in the function
            if (start.X > end.X)
                isVisible = PlotLineLow(end, start);
            else isVisible = PlotLineLow(start, end);
        }
        else
        {
            if (start.Y > end.Y)
                isVisible = PlotLineHigh(end, start);
            else isVisible = PlotLineHigh(start, end);
        }
        return isVisible;

        bool PlotLineLow(Vector2Int start, Vector2Int end)
        {
            Vector2Int dPos;
            bool result = true;

            int dx = end.X - start.X;
            int dy = end.Y - start.Y;
            int yi = 1;

            if (dy < 0)
            {
                yi = -1;
                dy = -dy;
            }
            int D = (2 * dy) - dx;
            int y = start.Y;
            for (int x = start.X; x <= end.X; x++)
            {
                dPos = new(x, y);
                if (!Game.currentMap.cells[dPos].IsTransparent && x != end.X && x != start.X)
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


        bool PlotLineHigh(Vector2Int start, Vector2Int end)
        {
            Vector2Int dPos;
            bool result = true;
            int dx = end.X - start.X;
            int dy = end.Y - start.Y;
            int xi = 1;
            if (dx < 0)
            {
                xi = -1;
                dx = -dx;
            }

            int D = (2 * dx) - dy;
            int x = start.X;
            for (int y = start.Y; y <= end.Y; y++)
            {
                dPos = new(x, y);
                if (!Game.currentMap.cells[dPos].IsTransparent && y != end.Y && y != start.Y)
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
    public Dictionary<Vector2Int, Cell> cells;
    public Dictionary<Vector2Int, char> background; // static chars to draw over unexplored tiles
    public List<Creature> creatures = new();
    public Vector2Int size;
    public HashSet<Vector2Int> walls;
    public Map(int stairCount, int creatureLevel = 1, int xLength = 128, int yLength = 128)
    {
        size = new(xLength, yLength);
        walls = new HashSet<Vector2Int>();
        Random rng = new Random();
        cells = MapGen.GenerateCA(xLength, yLength);
        background = new Dictionary<Vector2Int, char>();
        int t;
        for (int x = 0; x < size.X + 500; x++) // 15 extra tiles on each side in case camera sees them
        {
            for (int y = 0; y < size.Y + 500; y++)
            {
                Vector2Int pos = new(x, y);
                background.Add(pos, ' ');
                t = rng.Next(0, 300);
                if (t < 5) background[pos] = '`';
                if (t < 4) background[pos] = '/';
                if (t < 3) background[pos] = '*';
                if (t < 2) background[pos] = '\\';
                if (t < 1) background[pos] = 'x';
            }
        }

        while (true)
        {
            int x = rng.Next(0, xLength);
            int y = rng.Next(0, yLength);
            Vector2Int pos = new(x, y);
            if (cells[pos].IsWalkable)
            {
                Creature goblin = new Creature("Goblin", pos);
                AddCreature(goblin);
                creatures.Add(goblin);
                break;
            }
        }

        foreach (KeyValuePair<Vector2Int, Cell> kvp in cells)
        {
            if (!cells[kvp.Key].IsWalkable)
                walls.Add(kvp.Key);
        }

    }
    public HashSet<Vector2Int> GetObstacles()
    {
        HashSet<Vector2Int> obstacles = new(walls);
        foreach (Creature crtr in creatures)
            obstacles.Add(crtr.Pos);

        return obstacles;
    }

    public void AddCreature(Creature creature)
    {
        cells[creature.Pos].AddCreature(creature);
    }
}


public class Cell
{
    public CellType Type { get; set; }
    public bool IsTransparent = true;
    public bool IsWalkable = true;

    public bool isRevealed = false;

    public Creature? creature;
    public List<Item>? items;
    public Cell(CellType type, bool isTransparent, bool isWalkable)
    {
        this.Type = type;
        this.IsTransparent = isTransparent;
        this.IsWalkable = isWalkable;
    }
    public void AddCreature(Creature creature)
    {
        this.creature = creature;
    }
    public void RemoveCreature()
    {
        creature = null;
    }

    public bool HasCreature() { return !(creature is null); }

    public void Set(CellType type, bool isWalkable, bool isTransparent)
    {
        this.Type = type;
        this.IsWalkable = isWalkable;
        this.IsTransparent = isTransparent;
    }
    public void SetToWall() { this.Set(CellType.Wall, false, false); }
    public void SetToFloor() { this.Set(CellType.Floor, true, true); }
    public void SetRevealed(bool value) { this.isRevealed = value; }
}

public enum CellType { Floor, Wall }
