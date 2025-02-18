public static class Game
{

    public static Dungeon dungeon;
    public static Map currentMap { get { return dungeon.GetCurrentFloor(); } }
    public static Creature player = new("Player", new Rune('@'));
    public static GameObject playerGO = new(player);
    public static void Init(string pName)
    {
        int mapSize = 50;
        int xStart = 0;
        int yStart = 0;

        dungeon = new Dungeon(10);

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
        player = new Creature(pName, new Rune('@'), 5, 5, 5, 5, 1);
        playerGO = new GameObject((xStart, yStart), player);

        dungeon.floors[0].AddGameObject(playerGO);

        UI.Init();
    }

    public static void ChangeFloor(int floorNumber) 
    {

    }
    public static void Descend() { ChangeFloor(dungeon.currentFloor + 1); }
    public static void Ascend() { ChangeFloor(dungeon.currentFloor - 1); }

}

