using System.Text;
using Terminal.Gui.Drawing;
using Attribute = Terminal.Gui.Drawing.Attribute;

public class Dungeon
{
    // should move these to a separate container for ncurses
    public static Rune WALL = new('#');
    public static Rune FLOOR = new('.');
    public static Rune STAIR_DOWN = new('>');
    public static Rune STAIR_UP = new('<');
    public static Attribute FLOOR_COLOR = new(Color.BrightGreen, Color.Black);
    public static Attribute WALL_COLOR = new(Color.Yellow, Color.Black);
    public static Attribute STAIR_COLOR = new(Color.Green, Color.Black);
    public static Attribute REVEALED_COLOR = new(Color.Gray, Color.Black); // For tiles that were seen but not in LOS
    public static Attribute OBSCURED_COLOR = new(Color.DarkGray, Color.Black);

    public List<Map> floors;
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
    public static bool CanSeeTile(Map map, Vector2Int start, Vector2Int end)
    { 
        bool isVisible;
        if (start == end)
            return true;
        if (Math.Abs(end.X - start.X) > Math.Abs(end.Y - start.Y))
        {
            //need this since we're incrementing X in the function
            if (start.X > end.X)
                isVisible = PlotLineLow(map, end, start);
            else isVisible = PlotLineLow(map, start, end);
        }
        else
        {
            if (start.Y > end.Y)
                isVisible = PlotLineHigh(map, end, start);
            else isVisible = PlotLineHigh(map, start, end);
        }
        return isVisible;

        bool PlotLineLow(Map map, Vector2Int start, Vector2Int end)
        {
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
                if (!map.cells[x, y].IsTransparent && x != end.X && x != start.X)
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


        bool PlotLineHigh(Map map, Vector2Int start, Vector2Int end)
        {
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
                if (!map.cells[x, y].IsTransparent && y != end.Y && y != start.Y)
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
    public Creature?[,] creatureGrid;
    public Dictionary<Vector2Int, char> background; // static chars to draw over unexplored tiles
    const int BACKGROUND_EXTRA_SIZE = 100;
    public Vector2Int size;
    public HashSet<Vector2Int> walls;
    public Dictionary<Creature, AIData> creatures = new();
    public Map(int stairCount, int creatureLevel = 1, int xLength = 128, int yLength = 128)
    {
        size = new(xLength, yLength);
        walls = new HashSet<Vector2Int>();
        Random rng = new Random();
        cells = MapGen.GenerateCA(xLength, yLength);
        creatureGrid = new Creature?[xLength, yLength];
        background = new Dictionary<Vector2Int, char>();
        int t;
        for (int x = 0; x < size.X + BACKGROUND_EXTRA_SIZE; x++) // extra tiles for camera to see past map borders
        {
            for (int y = 0; y < size.Y + BACKGROUND_EXTRA_SIZE; y++)
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
            if (CanOccupy(pos))
            {
                Creature goblin = new Creature("Goblin", pos);
                AddCreature(goblin);
                creatures.Add(goblin, new());
                break;
            }
        }

        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                if (!cells[x, y].IsWalkable)
                    walls.Add(new(x, y));
            }
        }

    }
    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < size.X && y >= 0 && y < size.Y;
    }

    public bool IsInBounds(Vector2Int pos)
    {
        return IsInBounds(pos.X, pos.Y);
    }

    public bool CanOccupy(int x, int y)
    {
        return IsInBounds(x, y) && cells[x, y].IsWalkable && creatureGrid[x, y] == null;
    }

    public bool CanOccupy(Vector2Int pos) => CanOccupy(pos.X, pos.Y);

    public Creature? GetCreature(int x, int y)
    {
        return IsInBounds(x, y) ? creatureGrid[x, y] : null;
    }

    public Creature? GetCreature(Vector2Int pos) => GetCreature(pos.X, pos.Y);

    public bool HasCreature(int x, int y)
    {
        return IsInBounds(x, y) && creatureGrid[x, y] != null;
    }

    public bool HasCreature(Vector2Int pos) => HasCreature(pos.X, pos.Y);

    public bool AddCreature(Creature creature)
    {
        if (!IsInBounds(creature.Pos)) return false;
        creatureGrid[creature.Pos.X, creature.Pos.Y] = creature;
        return true;
    }

    public bool RemoveCreature(Creature creature)
    {
        if (IsInBounds(creature.Pos) && creatureGrid[creature.Pos.X, creature.Pos.Y] == creature)
        {
            creatureGrid[creature.Pos.X, creature.Pos.Y] = null;
            return true;
        }
        return false;
    }

    public bool RemoveCreatureAt(Vector2Int pos)
    {
        if (IsInBounds(pos))
        {
            creatureGrid[pos.X, pos.Y] = null;
            return true;
        }
        return false;
    }

    public bool TryMoveCreature(Creature creature, Vector2Int targetPos)
    {
        if (!CanOccupy(targetPos))
            return false;

        if (IsInBounds(creature.Pos) && creatureGrid[creature.Pos.X, creature.Pos.Y] == creature)
        {
            creatureGrid[creature.Pos.X, creature.Pos.Y] = null;
        }
        creature.Pos = targetPos;
        creatureGrid[targetPos.X, targetPos.Y] = creature;
        return true;
    }

    public bool TryBump(Creature actor, Vector2Int dir)
    {
        Vector2Int targetPos = actor.Pos + dir;
        if (!IsInBounds(targetPos))
            return false;

        Creature? targetCreature = creatureGrid[targetPos.X, targetPos.Y];
        if (targetCreature != null)
        {
            if (targetCreature.Faction != actor.Faction)
            {
                actor.Attack(targetCreature);
                return true;
            }
            return false;
        }

        if (!cells[targetPos.X, targetPos.Y].IsWalkable)
            return false;

        return TryMoveCreature(actor, targetPos);
    }

    public HashSet<Vector2Int> GetObstacles()
    {
        HashSet<Vector2Int> obstacles = new(walls);
        foreach (KeyValuePair<Creature, AIData> kvp in creatures)
            obstacles.Add(kvp.Key.Pos);
        if (Game.Player != null && IsInBounds(Game.Player.Pos))
            obstacles.Add(Game.Player.Pos);

        return obstacles;
    }

    public void ResetRevealedTiles() 
    {
        for (int x = 0; x < size.X; x++)
        {
            for (int y = 0; y < size.Y; y++)
            {
                cells[x, y].SetRevealed(false);
            }
        }
    }
}
