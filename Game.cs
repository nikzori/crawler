using Terminal.Gui.App;
using Terminal.Gui.ViewBase;

public class Game
{
    public static Dungeon dungeon = new(1);
    public static Map currentMap { get { return dungeon.GetCurrentFloor(); } }
    public static Player player;
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
        player = new Player(pName, new(xStart, yStart), new('@'));
        dungeon.floors[0].AddCreature(player);
    }
    public static void Update(int aut)
    {
        time += aut;
        foreach (Creature c in currentMap.creatures)
            AI.Act(c, aut);
    }

    public static void ChangeFloor(int floorNumber, Vector2Int pos)
    {
        dungeon.GetCurrentFloor().cells[player.pos].RemoveCreature();
        dungeon.currentFloor = floorNumber;
        player.pos = pos;
        dungeon.GetCurrentFloor().cells[pos].AddCreature(player);
    }
    public static void Descend(Vector2Int pos) { ChangeFloor(dungeon.currentFloor + 1, pos); }
    public static void Ascend(Vector2Int pos) { ChangeFloor(dungeon.currentFloor - 1, pos); }

}

