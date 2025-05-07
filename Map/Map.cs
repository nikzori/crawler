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

        //TODO: new stairs generation
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
                if (!Game.currentMap.cells[x, y].isTransparent && x != end.x && x != start.x)
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
                if (!Game.currentMap.cells[x, y].isTransparent && y != end.y && y != start.y)
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
    public char[,] background; // static chars to draw over unexplored tiles

    public Node[,] navNodes;
    public Node? startNode;
    public Node? endNode;

    public Map(int stairCount, int creatureLevel = 1, int xLength = 128, int yLength = 128)
    {
        Random rng = new Random();
        cells = MapGen.GenerateCA(xLength, yLength);
        background = new char[xLength + 30, yLength + 30]; // 15 extra tiles on each side
        int t;
        for (int x = 0; x < background.GetLength(0); x++)
        {
            for (int y = 0; y < background.GetLength(1); y++)
            {
                background[x, y] = ' ';
                t = rng.Next(0, 300);
                if (t < 5) background[x, y] = '`';
                if (t < 4) background[x, y] = '/';
                if (t < 3) background[x, y] = '*';
                if (t < 2) background[x, y] = '\\';
                if (t < 1) background[x, y] = 'x';
            }
        }

        for (int i = 0; i < 10; i++)
        {
            int x = rng.Next(0, xLength);
            int y = rng.Next(0, yLength);
            if (cells[x, y].IsWalkable())
                AddCreature(new Creature("Goblin", (x, y), 'g'));
        }

        // pathfinding
        navNodes = new Node[cells.GetLength(0), cells.GetLength(1)];
        for (int x = 0; x < navNodes.GetLength(0); x++)
        {
            for (int y = 0; y < navNodes.GetLength(1); y++)
            {
                navNodes[x, y].position = (x, y);
                navNodes[x, y].isWalkable = cells[x, y].IsWalkable();
                navNodes[x, y].state = NodeState.Open;
            }
        }
    }

    public void AddCreature(Creature creature)
    {
        int x = creature.pos.x;
        int y = creature.pos.y;
        cells[x, y].AddCreature(creature);
    }
}

public static class Pathfinding
{
    public static Node? startNode;
    public static Node? endNode;
    public static Node[,] nodeMap = Game.currentMap.navNodes;
    public static List<Node> GetPath((int x, int y) start, (int x, int y) finish)
    {
        // using this for reference: https://web.archive.org/web/20170505034417/http://blog.two-cats.com/2014/06/a-star-example/ 

        // we need navnodes that know their position and reset to Open state on each path request
        // set start and finish nodes, autocalculate distance to them in every other node

        // beginning with the start node, get all surrounding Open (walkable) nodes
        // sort by the sum of distance to start node and distance to the finish node
        // select an node with the smallest value
        // set its state to Closed and its parent node to the previous node in the loop, repeat
        // if we can't find a next node with lesser or equal sum, return to the parent node, select the next node in the sorting order
        // if all of the nodes around the current node are Closed, we can't reach the target

        // reset all nodes
        for (int i = 0; i < nodeMap.GetLength(0); i++)
        {
            for (int j = 0; j < nodeMap.GetLength(1); j++)
            {
                nodeMap[i, j].parent = null;
                nodeMap[i, j].state = NodeState.Open;
            }
        }
        List<Node> path = new();
        startNode = nodeMap[start.x, start.y];
        endNode = nodeMap[finish.x, finish.y];
        bool success = Search(startNode);
        if (success)
        {
            Node node = endNode;
            while (node.parent != null)
            {
                path.Add(node);
                node = node.parent;
            }
            path.Reverse();
        }
        return path;
    }
    public static bool Search(Node currentNode)
    {
        currentNode.state = NodeState.Closed;
        List<Node> adjacentNodes = GetAdjacentNodes(currentNode);
        adjacentNodes.Sort((node1, node2) => node1.F.CompareTo(node2.F));
        foreach (Node nextNode in adjacentNodes)
        {
            if (nextNode == endNode)
                return true;
            else if (Search(nextNode)) // recurse into itself until we get finish or run out of adjacent nodes
                return true;
        }
        return false;
    }
    public static List<Node> GetAdjacentNodes(Node currentNode)
    {
        List<Node> result = new();

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                int dx = currentNode.position.x + x;
                int dy = currentNode.position.x + y;
                if (dx < 0 || dx >= nodeMap.GetLength(0) || dy < 0 || dy >= nodeMap.GetLength(1))
                    continue;

                Node node = nodeMap[dx, dy];

                if (!node.isWalkable)
                    continue;

                if (node.state == NodeState.Closed)
                    continue;

                if (node.state == NodeState.Open)
                    result.Add(node);
            }
        }
        return result;
    }


}

public class Node
{
    public (int x, int y) position;
    public bool isWalkable = false;
    public float G
    {
        get { return Pathfinding.startNode is null ? 0 : (float)Math.Sqrt((position.x - Pathfinding.startNode.position.x) ^ 2 + (position.y - Pathfinding.startNode.position.y) ^ 2); }
    }
    public float H
    {
        get { return Pathfinding.endNode is null ? 0 : (float)Math.Sqrt((position.x - Pathfinding.endNode.position.x) ^ 2 + (position.y - Pathfinding.endNode.position.y) ^ 2); }
    }
    public float F { get { return this.G + this.H; } }
    public NodeState state { get; set; }
    public Node? parent { get; set; }
}
public enum NodeState { Open, Closed }
public struct Cell
{
    public Rune rune;
    public Terminal.Gui.Attribute colors;
    public bool isTransparent = true;
    public bool isWalkable = true;

    public bool isRevealed = false; //for line of sight

    public bool isStairsDown = false;
    public bool isStairsUp = false;

    public Creature? creature;
    public List<Item>? items;
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
    public void AddCreature(Creature creature)
    {
        this.creature = creature;
    }
    public void RemoveCreature()
    {
        creature = null;
    }

    public Rune GetRune()
    {
        if (IsWall())
            return rune;
        if (creature != null)
            return creature.rune;
        else if (items != null && items.Count > 0)
            return items[0].rune;
        else return rune;
    }

    //shortcuts for convenience
    public bool IsWalkable()
    {
        return isWalkable;
    }
    public bool IsTransparent()
    {
        return isTransparent;
    }
    public bool HasCreature()
    {
        return !(creature is null);
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
