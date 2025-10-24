using System.Text;
using Terminal.Gui.Drawing;
using Attribute = Terminal.Gui.Drawing.Attribute;

public class Dungeon
{
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
                if (!Game.currentMap.cells[dPos].isTransparent && x != end.X && x != start.X)
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
                if (!Game.currentMap.cells[dPos].isTransparent && y != end.Y && y != start.Y)
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

        for (int i = 0; i < 10; i++)
        {
            int x = rng.Next(0, xLength);
            int y = rng.Next(0, yLength);
            Vector2Int pos = new(x, y);
            if (cells[pos].isWalkable)
            {
                Creature goblin = new Creature("Goblin", pos, new('g'));
                AddCreature(goblin);
                creatures.Add(goblin);
            }
        }

        foreach (KeyValuePair<Vector2Int, Cell> kvp in cells)
        {
            if (!cells[kvp.Key].isWalkable)
                walls.Add(kvp.Key);
        }

    }
    public HashSet<Vector2Int> GetObstacles()
    {
        HashSet<Vector2Int> obstacles = new(walls);
        foreach (Creature crtr in creatures)
            obstacles.Add(crtr.pos);

        return obstacles;
    }

    public void AddCreature(Creature creature)
    {
        cells[creature.pos].AddCreature(creature);
    }
}


public class Cell
{
    public Rune Rune
    {
        get
        {
            if (creature != null)
                return creature.Rune;
            if (items != null && items.Count > 0)
                return items[0].Rune;
            return field;
        }
        set;
    }
    public Attribute Color
    {
        get
        {
            if (creature != null)
                return creature.color;
            if (items != null && items.Count > 0)
                return items[0].Color;
            else return field;
        }
        set;
    }
    public bool isTransparent = true;
    public bool isWalkable = true;

    public bool isRevealed = false;

    public Creature? creature;
    public List<Item>? items;
    public Cell(Rune rune, bool isTransparent, bool isWalkable, Attribute colors)
    {
        this.Rune = rune;
        this.isTransparent = isTransparent;
        this.isWalkable = isWalkable;
        this.Color = colors;
    }
    public Cell(char c, bool isTransparent, bool isWalkable, Attribute colors)
    {
        Rune r = new(c);
        this.isTransparent = isTransparent;
        this.isWalkable = isWalkable;
        this.Color = colors;
    }
    public void AddCreature(Creature creature)
    {
        this.creature = creature;
    }
    public void RemoveCreature()
    {
        creature = null;
    }

    //shortcuts for convenience
    public bool IsTransparent()
    {
        return isTransparent;
    }
    public bool HasCreature()
    {
        return !(creature is null);
    }
    public void SetRune(Rune rune)
    {
        this.Rune = rune;
    }
    public void SetRune(char c)
    {
        this.Rune = new Rune(c);
    }
    public void Set(Rune rune, bool isWalkable, bool isTransparent, Terminal.Gui.Drawing.Attribute colors)
    {
        this.Rune = rune;
        this.isWalkable = isWalkable;
        this.isTransparent = isTransparent;
        this.Color = colors;
    }
    public void SetToWall()
    {
        this.Set(Dungeon.WALL, false, false, Dungeon.WALL_COLOR);
    }
    public void SetToFloor()
    {
        this.Set(Dungeon.FLOOR, true, true, Dungeon.FLOOR_COLOR);
    }
    public void SetRevealed(bool value)
    {
        this.isRevealed = value;
    }
}
