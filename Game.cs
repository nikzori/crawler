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
        player = new Player(pName, (xStart, yStart), '@');
        player.name = pName;
        dungeon.floors[0].AddCreature(player);

        UI.Init();
    }

    public static void ChangeFloor(int floorNumber, (int x, int y) pos)
    {
        dungeon.GetCurrentFloor().cells[player.pos.x, player.pos.y].RemoveCreature();
        dungeon.currentFloor = floorNumber;
        player.pos = pos;
        dungeon.GetCurrentFloor().cells[pos.x, pos.y].AddCreature(player);
        UI.UpdatePos();
    }
    public static void Descend((int x, int y) pos) { ChangeFloor(dungeon.currentFloor + 1, pos); }
    public static void Ascend((int x, int y) pos) { ChangeFloor(dungeon.currentFloor - 1, pos); }

}

