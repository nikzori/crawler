//this is where the magic happens

public static class MapGen
{
    // this makes a box with a column in the middle
    public static Dictionary<Vector2Int, Cell> GenerateBox(int width, int height)
    {
        Dictionary<Vector2Int, Cell> result = new(width * height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || y == 0)
                    result.Add(new(x, y), new(Dungeon.WALL, false, false, Dungeon.WALL_COLOR));
                else result.Add(new(x, y), new(Dungeon.FLOOR, true, true, Dungeon.FLOOR_COLOR));
            }
        }
        result[new(width / 2, height / 2)].SetToWall();
        return result;
    }

    /// <summary>
    /// Randomly seeds walls around the map and then smooths map out with Cellular Automata rules.
    /// Then places rooms randomly for some variety
    /// </summary>
    /// TODO: move wall seeding into a separate function
    public static Dictionary<Vector2Int, Cell> GenerateCA(int width, int height, int iterations = 2)
    {
        Random random = new Random();
        // I'll do this step by step 'cause I'm stupid
        // https://roguebasin.com/index.php/Cellular_Automata_Method_for_Generating_Random_Cave-Like_Levels

        // Make a boxed in room

        Vector2Int pos;
        Cell cell;
        Dictionary<Vector2Int, Cell> result = new Dictionary<Vector2Int, Cell>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = new Cell(Dungeon.FLOOR, true, true, Dungeon.FLOOR_COLOR);
                pos = new(x, y);
                result.Add(pos, cell);
            }
        }

        foreach (KeyValuePair<Vector2Int, Cell> kvp in result)
        {

            // wall border
            if (kvp.Key.X == 0 || kvp.Key.X == width - 1 || kvp.Key.Y == 0 || kvp.Key.Y == height - 1)
                result[kvp.Key].SetToWall();

            // throw random walls on the map for automata
            if (random.Next(1, 100) < 45)
                result[kvp.Key].SetToWall();
        }



        bool isMapReady = false;
        try
        {
            //generate new maps until one of them has fully ineterconnected rooms/caverns taking up enough space
            int cntr = 0; // just in case
            while (!isMapReady)
            {
                // iterate automata

                for (int i = 0; i < iterations; i++)
                    result = RunAutomata(result, width, height);
                for (int i = 0; i < 3; i++)
                    result = RunAutomata(result, width, height, true);
                result = RunAutomata(result, width, height);

                result = PlaceRooms(result, width, height, true, 10, 4, 7); // Sprinkle around rooms connected by corridors

                if (CleanIsolation(result, width, height) || cntr > 10) // Clean up the isolated pockets
                    isMapReady = true;
                cntr++;
            }
        }
        catch (Exception err)
        {
            Console.WriteLine("Error: " + err);
        }

        return result;
    }

    /// <summary>
    /// Cellular Automata rules.
    /// </summary>
    public static Dictionary<Vector2Int, Cell> RunAutomata(Dictionary<Vector2Int, Cell> input, int width, int height, bool placeColumns = false)
    {
        Dictionary<Vector2Int, Cell> result = input;

        // a tile becomes a wall tile if it's surrounded by at least 5 walls 
        // a tile will stay a wall tile if it's surrounded by at least 4 walls;
        // a tile will become a wall tile if there are zero walls in 2 tile radius around it
        int neighbourWallsCounter;
        int distantWallsCounter;
        bool setToWall;
        for (int x = 2; x < width - 1; x++) // smaller range of coordinated to ignore border walls
        {
            for (int y = 2; y < height - 1; y++)
            {
                // variable reset
                neighbourWallsCounter = 0;
                distantWallsCounter = 0;
                setToWall = false;

                for (int xt = x - 1; xt < x + 2; xt++)
                {
                    for (int yt = y - 1; yt < y + 2; yt++)
                    {
                        if (result[new(xt, yt)].IsWall())
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
                            if (xt <= 0 || yt <= 0 || xt > width - 1 || yt > height - 1)
                                continue;
                            // if tile is close on both axises, it's adjacent => skip
                            if (Math.Abs(yt - y) < 2 && Math.Abs(xt - x) < 2)
                                continue;
                            if (result[new(xt, yt)].IsWall())
                                distantWallsCounter++;


                        }
                    }
                    if (distantWallsCounter <= 2)
                        setToWall = true;
                }

                if (setToWall)
                    result[new(x, y)] = new(Dungeon.WALL, false, false, Dungeon.WALL_COLOR);
                else result[new(x, y)] = new(Dungeon.FLOOR, true, true, Dungeon.FLOOR_COLOR);
            }
        }
        return result;
    }

    public static Dictionary<Vector2Int, Cell> PlaceRooms(Dictionary<Vector2Int, Cell> input, int width, int height, bool connectRooms = true, int roomCount = 5, int minSize = 5, int maxSize = 12)
    {
        Random random = new Random();
        Dictionary<Vector2Int, Cell> result = input;

        // center point coordinates
        Vector2Int[] roomCenters = new Vector2Int[roomCount];

        for (int i = 0; i < roomCount; i++)
        {
            //pick a random point on the map
            int x1 = random.Next(2, width - 1);
            int y1 = random.Next(2, height - 1);
            int x2;
            int y2;

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
                        result[new(x, y)].SetToWall();
                    else result[new(x, y)].SetToFloor();
                }
            }

            roomCenters[i] = new(x1 + ((x2 - x1) / 2), y1 + ((y2 - y1) / 2)); // since starting point values are always smaller, this should work
        }

        if (connectRooms) // draw a simple corridor with a single 90deg turn if it is required
        {
            for (int i = 0; i < roomCount - 1; i++) // don't need to do anything with the last room
            {
                Vector2Int start = roomCenters[i];
                Vector2Int end = roomCenters[i + 1];

                if (start.X < end.X)
                {
                    for (int x = start.X; x <= end.X; x++)
                        result[new(x, start.Y)].SetToFloor();
                }
                else
                {
                    for (int x = end.X; x <= start.X; x++)
                        result[new(x, end.Y)].SetToFloor();
                }

                if (start.Y < end.Y)
                {
                    for (int y = start.Y; y <= end.Y; y++)
                        result[new(start.X, y)].SetToFloor();
                }
                else
                {
                    for (int y = end.Y; y <= start.Y; y++)
                        result[new(end.X, y)].SetToFloor();
                }

            }
        }

        return result;
    }

    /// <summary>
    ///   Takes a Cell array and removes isolated rooms. Returns true if the walkable space is big enough.
    /// </summary>
    public static bool CleanIsolation(Dictionary<Vector2Int, Cell> input, int width, int height) // should account for stairs later
    {
        float minFraction = 0.3f;
        // by using flood fill we can check if the interconnected space on the map is big enough
        // and remove any isolated rooms to prevent the player from spawning in them
        int x = 0;
        int y = 0;

        //get a floor tile
        bool check = false;
        for (x = 0; x < width; x++)
        {
            for (y = 0; y < height; y++)
            {
                if (input[new(x, y)].IsWalkable())
                {
                    check = true;
                    break;
                }
            }
            if (check)
                break;
        }
        //get bool array marking all connected floor tiles
        Dictionary<Vector2Int, bool> selectedCells = FloodSelect(input, width, height, new(x, y));

        int areaSize = 0;
        for (x = 0; x < width; x++)
        {
            for (y = 0; y < height; y++)
            {
                if (selectedCells[new(x, y)])
                    areaSize++;
            }
        }

        // check walkable map size before clearing up isolated rooms
        int mapSize = width * height;
        if ((float)areaSize / (float)mapSize < minFraction)
            return false;

        for (x = 0; x < width; x++)
        {
            for (y = 0; y < height; y++)
            {
                if (!selectedCells[new(x, y)] && input[new(x, y)].isWalkable) // replace isolated floors with walls
                    input[new(x, y)].SetToWall();
            }
        }
        return true;
    }


    /// <summary>
    ///   Selects all tiles of the same type interconnected with the `pos` and returns a bool array.
    /// </summary>
    public static Dictionary<Vector2Int, bool> FloodSelect(Dictionary<Vector2Int, Cell> input, int width, int height, Vector2Int pos)
    {
        Dictionary<Vector2Int, bool> result = new Dictionary<Vector2Int, bool>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                result.Add(new Vector2Int(x, y), false);
            }
        }

        result[pos] = true; // starting point is always true

        // instantiate a queue for the loop
        Queue<Vector2Int> q = new();
        q.Enqueue(pos);

        Vector2Int currentPos;
        while (q.Count > 0)
        {
            currentPos = q.Dequeue(); // pop the first position in the queue

            // go over the adjacent tiles 
            // add them in the queue until we run out of possible options
            // if a tile is already marked true in the `result`, ignore it
            Vector2Int dPos;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    dPos = currentPos + new Vector2Int(x, y);

                    if (dPos == currentPos)
                        continue;
                    if (dPos.X > -1 && dPos.X < width && dPos.Y > -1 && dPos.Y < height)
                    {
                        if (input[dPos].isWalkable == input[pos].isWalkable)
                        {
                            if (!result[dPos])
                            {
                                result[dPos] = true;
                                q.Enqueue(dPos);
                            }
                        }
                    }
                }
            }
        }

        return result;
    }
}
