//this is where the magic happens

public static class MapGen
{
  // this makes a box with a column in the middle
  public static Cell[,] Generate(int width, int height)
  {
    Cell[,] result = new Cell[width, height];

    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        if (x == 0 || y == 0)
          result[x, y] = new Cell('#', false, false, Dungeon.WALL_COLOR);
        else result[x, y] = new Cell();
      }
    }
    result[width / 2, height / 2].SetToWall();
    return result;
  }

  /// <summary>
  /// Randomly seeds walls around the map and then smooths map out with Cellular Automata rules.
  /// Then places rooms randomly for some variety
  /// </summary>
  /// TODO: move wall seeding into a separate function
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
          result[x, y].SetToWall();
        else result[x, y].SetToFloor();
      }
    }

    // Make random tiles walls
    for (int x = 0; x < result.GetLength(0); x++)
    {
      for (int y = 0; y < result.GetLength(1); y++)
      {
        if (random.Next(1, 100) < 45)
          result[x, y].SetToWall();
      }
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
          result = RunAutomata(result);
        for (int i = 0; i < 3; i++)
          result = RunAutomata(result, true);
        result = RunAutomata(result);

        result = PlaceRooms(result, true, 10, 4, 7); // Sprinkle around rooms connected by corridors

        if (CleanIsolation(result) /* || cntr > 10 */) // Clean up the isolated pockets
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
            if (result[xt, yt].IsWall())
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
              if (result[xt, yt].IsWall())
                distantWallsCounter++;


            }
          }
          if (distantWallsCounter <= 2)
            setToWall = true;
        }

        if (setToWall)
          result[x, y].SetToWall();
        else result[x, y].SetToFloor();
      }
    }
    return result;
  }

  public static Cell[,] PlaceRooms(Cell[,] input, bool connectRooms = true, int roomCount = 5, int minSize = 5, int maxSize = 12)
  {
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
            result[x, y].SetToWall();
          else result[x, y].SetToFloor();
        }
      }

      xc[i] = x1 + ((x2 - x1) / 2);
      yc[i] = y1 + ((y2 - y1) / 2); // since starting point values are always smaller, this should work
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

        // we pick the most left room to be the starting point
        if (xc[i + 1] < xc[i])
        {
          xStart = xc[i + 1];
          xEnd = xc[i];

          // without flipping the Y values as well we won't know which X point to use for the vertical part
          yStart = yc[i + 1];
          yEnd = yc[i];
        }

        // this is kinda ugly. oh well
        for (int x = xStart; x <= xEnd; x++)
          result[x, yStart].SetToFloor();
        if (yStart < yEnd)
          for (int y = yStart; y <= yEnd; y++)
            result[xEnd, y].SetToFloor();
        else for (int y = yStart; y >= yEnd; y--)
            result[xEnd, y].SetToFloor();

      }
    }

    return result;
  }

  /// <summary>
  ///   Takes a Cell array and removes isolated rooms. Returns true if the walkable space is big enough.
  /// </summary>
  public static bool CleanIsolation(Cell[,] input)
  {
    float minFraction = 0.3f;
    // by using flood fill we can check if the interconnected space on the map is big enough
    // and remove any isolated rooms to prevent the player from spawning in them
    int x = 0;
    int y = 0;

    //get a floor tile
    bool check = false;
    for (x = 0; x < input.GetLength(0); x++)
    {
      for (y = 0; y < input.GetLength(1); y++)
      {
        if (input[x, y].IsWalkable())
        {
          check = true;
          break;
        }
      }
      if (check)
        break;
    }
    //get bool array marking all connected floor tiles
    bool[,] selectedCells = FloodSelect(input, (x, y));

    int areaSize = 0;
    for (x = 0; x < selectedCells.GetLength(0); x++)
    {
      for (y = 0; y < selectedCells.GetLength(1); y++)
      {
        if (selectedCells[x, y])
          areaSize++;
      }
    }

    // check walkable map size before clearing up isolated rooms
    //int mapSize = input.GetLength(0) * input.GetLength(1);
    //if ((float)areaSize / (float)mapSize < minFraction)
    //return false;

    for (x = 0; x < input.GetLength(0); x++)
    {
      for (y = 0; y < input.GetLength(1); y++)
      {
        if (!selectedCells[x, y] && input[x, y].isWalkable) // replace isolated floors with walls
          input[x, y].SetToWall();
      }
    }
    return true;
  }


  /// <summary>
  ///   Selects all tiles of the same type interconnected with the `pos` and returns a bool array.
  /// </summary>
  public static bool[,] FloodSelect(Cell[,] input, (int x, int y) pos)
  {
    bool[,] result = new bool[input.GetLength(0), input.GetLength(1)];

    // set all to false just in case
    for (int xt = 0; xt < result.GetLength(0); xt++)
    {
      for (int yt = 0; yt < result.GetLength(1); yt++)
        result[xt, yt] = false;
    }

    result[pos.x, pos.y] = true; // starting point is always true

    // instantiate a queue for the loop
    Queue<(int, int)> q = new();
    q.Enqueue(pos);

    int x, y;
    while (q.Count > 0)
    {
      (x, y) = q.Dequeue(); // pop the first position in the queue

      // go over the adjacent tiles (clockwise)
      // add them in the queue until we run out of possible options
      // if a tile is already marked true in the `result[]`, ignore it
      if (y + 1 < input.GetLength(1) && input[x, y + 1].isWalkable == input[pos.x, pos.y].isWalkable && !result[x, y + 1])
      {
        result[x, y + 1] = true;
        q.Enqueue((x, y + 1));
      }

      if (x + 1 < input.GetLength(0) && y + 1 < input.GetLength(1) && input[x + 1, y + 1].isWalkable == input[pos.x, pos.y].isWalkable && !result[x + 1, y + 1])
      {
        result[x + 1, y + 1] = true;
        q.Enqueue((x + 1, y + 1));
      }

      if (x + 1 < input.GetLength(0) && input[x + 1, y].isWalkable == input[pos.x, pos.y].isWalkable && !result[x + 1, y])
      {
        result[x + 1, y] = true;
        q.Enqueue((x + 1, y));
      }

      if (x + 1 < input.GetLength(0) && y - 1 > 0 && input[x + 1, y - 1].isWalkable == input[pos.x, pos.y].isWalkable && !result[x + 1, y - 1])
      {
        result[x - 1, y - 1] = true;
        q.Enqueue((x - 1, y - 1));
      }

      if (y - 1 > 0 && input[x, y - 1].isWalkable == input[pos.x, pos.y].isWalkable && !result[x, y - 1])
      {
        result[x, y - 1] = true;
        q.Enqueue((x, y - 1));
      }

      if (x - 1 > 0 && y - 1 > 0 && input[x - 1, y - 1].isWalkable == input[pos.x, pos.y].isWalkable && !result[x - 1, y - 1])
      {
        result[x - 1, y - 1] = true;
        q.Enqueue((x - 1, y - 1));
      }

      if (x - 1 > 0 && input[x - 1, y].isWalkable == input[pos.x, pos.y].isWalkable && !result[x - 1, y])
      {
        result[x - 1, y] = true;
        q.Enqueue((x - 1, y));
      }

      if (x - 1 > 0 && y + 1 < input.GetLength(1) && input[x - 1, y + 1].isWalkable == input[pos.x, pos.y].isWalkable && !result[x - 1, y + 1])
      {
        result[x - 1, y + 1] = true;
        q.Enqueue((x - 1, y + 1));
      }
    }

    return result;
  }
}
