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

public static class MapGen
{
    public static Cell[,] Generate(int width, int height)
    {
        Cell[,] result = new Cell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || y == 0)
                    result[x, y] = new Cell('O', false, false);
                else result[x, y] = new Cell();
            }
        }
        result[width / 2, height / 2].isWalkable = false;
        result[width / 2, height / 2].isTransparent = false;
        result[width / 2, height / 2].rune = new(Map.WALL);
        return result;


    }
    //TODO: cellurar automata generation

    public static Cell[,] GenerateCA(int width, int height, int iterations = 2)
    {
        Random random = new Random();
        // I'll do this step by step 'cause I'm stupid
        // https://roguebasin.com/index.php/Cellular_Automata_Method_for_Generating_Random_Cave-Like_Levels

        // Make a boxed in room
        Cell[,] result = new Cell[width, height];
        for (int x = 1; x < result.GetLength(0); x++)
        {
            for (int y = 1; y < result.GetLength(1); y++)
            {
                if (x == 1 || x == result.GetLength(0) - 1 || y == 1 || y == result.GetLength(1) - 1)
                    result[x, y] = new Cell(new Rune(Map.WALL), false, false);
                else result[x, y] = new Cell(new Rune(Map.FLOOR), false, false);
            }
        }

        // Make random tiles walls
        for (int x = 0; x < result.GetLength(0); x++)
        {
            for (int y = 0; y < result.GetLength(1); y++)
            {
                if (random.Next(1, 100) < 45)
                    result[x, y] = new Cell(new Rune(Map.WALL), false, false);
            }
        }

        bool isMapReady = false;

        //generate new maps until one of them has fully ineterconnected rooms/caverns taking up enough space
        while (!isMapReady)
        {
            // iterate automata

            for (int i = 0; i < iterations; i++)
                result = RunAutomata(result);
            for (int i = 0; i < 3; i++)
                result = RunAutomata(result, true);
            result = RunAutomata(result);

            result = PlaceRooms(result, true, 10, 4, 7);

            //if (CleanIsolation(result))
            isMapReady = true;
        }
        return result;
    }

    public static Cell[,] RunAutomata(Cell[,] input, bool placeColumns = false)
    {
        Cell[,] result = input;

        // a tile becomes a wall tile if it's surrounded by at least 5 walls 
        // a tile will stay a wall tile if it's surrounded by at least 4 walls;
        // a tile will become a wall tile if there are zero walls in 2 tile radius around it
        int neighbourWallsCounter = 0;
        int distantWallsCounter = 0;
        bool setToWall = false;
        for (int x = 2; x < result.GetLength(0) - 1; x++) // smaller range of coordinated to ignore border walls
        {
            for (int y = 2; y < result.GetLength(1) - 1; y++)
            {
                // variable reset
                neighbourWallsCounter = 0;
                distantWallsCounter = 0;
                setToWall = false;

                for (int xt = x - 1; xt < x + 2; xt++)
                {
                    for (int yt = y - 1; yt < y + 2; yt++)
                    {
                        if (result[xt, yt].isWall())
                            neighbourWallsCounter++;
                        if (neighbourWallsCounter > 4)
                        {
                            setToWall = true;
                            break;
                        }
                    }

                    if (setToWall)
                        break;
                }
                if (!setToWall && placeColumns)
                {
                    for (int xt = x - 2; xt <= x + 2; xt++)
                    {
                        for (int yt = y - 2; yt <= y + 2; yt++)
                        {
                            if (xt <= 0 || yt <= 0 || xt > result.GetLength(0) - 1 || yt > result.GetLength(1) - 1)
                                continue;
                            // if tile is close on both axises, it's adjacent => skip
                            if (Math.Abs(yt - y) < 2 && Math.Abs(xt - x) < 2)
                                continue;
                            if (result[xt, yt].isWall())
                                distantWallsCounter++;


                        }
                    }
                    if (distantWallsCounter <= 2)
                        setToWall = true;
                }

                if (setToWall)
                    result[x, y] = new Cell(new Rune(Map.WALL), false, false);
                else result[x, y] = new Cell(new Rune(Map.FLOOR), true, true);
            }
        }
        return result;
    }

    public static bool CleanIsolation(Cell[,] input)
    {
        Random random = new Random();
        Cell[,] result = input;
        int width = result.GetLength(0);
        int height = result.GetLength(1);

        //pick a random Floor tile on the map
        Cell origin;
        while (true)
        {
            int xr = random.Next(2, width - 2);
            int yr = random.Next(2, height - 2);
            if (input[xr, yr].isWall())
            {
                origin = input[xr, yr];
                break;
            }
        }

        // iterate over adjacent tiles, keep track of the last floor


        return true;
    }

    public static Cell[,] PlaceRooms(Cell[,] input, bool connectRooms = true, int roomCount = 5, int minSize = 5, int maxSize = 12)
    {
        //TODO: connect rooms with corridors; 

        Random random = new Random();
        Cell[,] result = input;
        int width = result.GetLength(0);
        int height = result.GetLength(1);

        // center point coordinates
        // too lazy to use a Vector or something
        int[] xc = new int[roomCount];
        int[] yc = new int[roomCount];

        for (int i = 0; i < roomCount; i++)
        {
            //pick a random point on the map
            int x1 = random.Next(2, width - 1);
            int y1 = random.Next(2, height - 1);
            int x2 = 0;
            int y2 = 0;

            // pick the end point of rectangle
            if (x1 < width / 2)
                x2 = random.Next(x1 + minSize, x1 + maxSize);
            else
            {
                x2 = x1;
                x1 = random.Next(x1 - maxSize, x1 - minSize);
            }

            if (y1 < height / 2)
                y2 = random.Next(y1 + minSize, y1 + maxSize);
            else
            {
                y2 = y1;
                y1 = random.Next(y1 - maxSize, y1 - minSize);
            }

            // paste floors
            for (int x = x1; x <= x2; x++)
            {

                for (int y = y1; y <= y2; y++)
                {
                    if (x == x1 || x == x2 || y == y1 || y == y2)
                        result[x, y] = new Cell(new Rune(Map.WALL), false, false);
                    else result[x, y] = new Cell(new Rune(Map.FLOOR), true, true);
                }
            }

            xc[i] = x1 + ((x2 - x1) / 2);
            yc[i] = y1 + ((y2 - y1) / 2); // since starting point values are always smaller, this should work

            char c = i.ToString()[0];
            result[x1, y1] = new Cell(new Rune(c), true, true);
        }

        if (connectRooms)
        {
            // for now, let's use simplest lines to connect rooms so as to not bother beautifying the jagged diagonal lines

            for (int i = 0; i < roomCount - 1; i++) // don't need to do anything with the last room
            {
                int xStart = xc[i];
                int xEnd = xc[i + 1];

                int yStart = yc[i];
                int yEnd = yc[i + 1];
                bool xFlipped = false;

                //we pick the most left room to be the starting point
                if (xc[i + 1] < xc[i])
                {
                    xStart = xc[i + 1];
                    xEnd = xc[i];

                    yStart = yc[i + 1];
                    yEnd = yc[i];
                }

                // this is so ugly. oh well
                for (int x = xStart; x <= xEnd; x++)
                    result[x, yStart] = new Cell(new Rune(Map.FLOOR), true, true);
                if (yStart < yEnd)
                    for (int y = yStart; y <= yEnd; y++)
                        result[xEnd, y] = new Cell(new Rune(Map.FLOOR), true, true);
                else for (int y = yStart; y >= yEnd; y--)
                        result[xEnd, y] = new Cell(new Rune(Map.FLOOR), true, true);

            }
        }

        return result;
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
        else if (gObjects.Count == 0 || !gObjects.Last().rune.HasValue)
            return rune;
        else return gObjects.Last().rune.Value;
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
