public class Game
{
    public static Dungeon dungeon = new(1);
    public static Map CurrentMap { get { return dungeon.GetCurrentFloor(); } }
    public static Creature Player = new("Player", new(1, 1));
    public static int time = 0;
    public Game(string pName)
    {
        dungeon = new(1);
        Vector2Int mapSize = dungeon.floors[0].size;
        int xStart = 0;
        int yStart = 0;

        bool startPosFound = false;
        for (int x = 2; x < mapSize.X; x++)
        {
            for (int y = 2; y < mapSize.Y; y++)
            {
                if (dungeon.floors[0].cells[new(x, y)].IsWalkable)
                {
                    xStart = x;
                    yStart = y;
                    startPosFound = true;
                    break;
                }
            }
            if (startPosFound)
                break;
        }
        Player.Name = pName;
        Player.Pos = new(xStart, yStart);
        dungeon.floors[0].AddCreature(Player);
    }
    public static void Update(int aut)
    {
        time += aut;
        // poke AI class to go through NPCs
    }

    public static void ChangeFloor(int floorNumber, Vector2Int pos)
    {
        dungeon.GetCurrentFloor().cells[Player.Pos].RemoveCreature();
        dungeon.currentFloor = floorNumber;
        Player.Pos = pos;
        dungeon.GetCurrentFloor().cells[pos].AddCreature(Player);
    }
    public static void Descend(Vector2Int pos) { ChangeFloor(dungeon.currentFloor + 1, pos); }
    public static void Ascend(Vector2Int pos) { ChangeFloor(dungeon.currentFloor - 1, pos); }

    public static event EventHandler<string> LogEvent = delegate { };
    public static void Log(string text) { LogEvent.Invoke(null, text); }

}

