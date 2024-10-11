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
