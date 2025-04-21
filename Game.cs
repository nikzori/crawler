public class Game
{

    public static Dungeon dungeon = new(10);
    public static Map currentMap { get { return dungeon.GetCurrentFloor(); } }
    public static Player player;
    public Game(string pName)
    {
        int mapSize = dungeon.floors[0].cells.GetLength(0);
        int xStart = 0;
        int yStart = 0;

        bool startPosFound = false;
        for (int x = 2; x < mapSize; x++)
        {
            for (int y = 2; y < mapSize; y++)
            {
                if (!dungeon.floors[0].cells[x, y].IsWall())
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
        player = new Player((xStart, yStart));
        player.name = pName;
        dungeon.floors[0].AddCreature(player);
        Init();
    }
    public static void Init()
    {
        Random rng = new Random();
        foreach (Map map in dungeon.floors)
        {
            int n = rng.Next(10, 30);
            for (int i = 0; i < n; i++)
            {
                while (true)
                {
                    int x = rng.Next(1, map.cells.GetLength(0));
                    int y = rng.Next(1, map.cells.GetLength(1));
                    if (map.cells[x, y].IsWalkable() && map.cells[x, y].creature is null)
                    {
                        Creature goblin = new((x, y));
                        map.cells[x, y].AddCreature(goblin);
                        break;
                    }
                }
            }
        }
        UI.Init();
    }

    public static void ChangeFloor(int floorNumber, (int x, int y) pos)
    {
        dungeon.GetCurrentFloor().cells[player.pos.x, player.pos.y].RemoveCreature(player);
        dungeon.currentFloor = floorNumber;
        player.pos = pos;
        dungeon.GetCurrentFloor().cells[pos.x, pos.y].AddCreature(player);
        UI.UpdatePos();
    }
    public static void Descend((int x, int y) pos) { ChangeFloor(dungeon.currentFloor + 1, pos); }
    public static void Ascend((int x, int y) pos) { ChangeFloor(dungeon.currentFloor - 1, pos); }

}

