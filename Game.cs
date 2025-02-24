public static class Game
{

    public static Dungeon dungeon = new(10);
    public static Map currentMap { get { return dungeon.GetCurrentFloor(); } }
    public static Creature? player;
    public static GameObject? playerGO;
    public static void Init(string pName)
    {
        int mapSize = 50;
        int xStart = 0;
        int yStart = 0;

        //dungeon = new Dungeon(10);

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
        playerGO = new GameObject((xStart, yStart));
        player = new Creature(playerGO, pName, new Rune('@'), 5, 5, 5, 5, 1, false);
        

        dungeon.floors[0].AddGameObject(playerGO);

        UI.Init();
    }

    public static void ChangeFloor(int floorNumber, (int x, int y) pos) 
    {
        dungeon.GetCurrentFloor().cells[playerGO.pos.x, playerGO.pos.y].RemoveGameObject(playerGO);
        dungeon.currentFloor = floorNumber;
        playerGO.pos = pos;
        dungeon.GetCurrentFloor().cells[pos.x, pos.y].AddGameObject(playerGO);
        UI.UpdatePos();
    }
    public static void Descend((int x, int y) pos) { ChangeFloor(dungeon.currentFloor + 1, pos); }
    public static void Ascend((int x, int y) pos) { ChangeFloor(dungeon.currentFloor - 1, pos); }

}

