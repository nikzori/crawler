using Terminal.Gui;

public static class Game
{
    static Label position;

    static MapView mapView;
    static View characterView;
    static Label logView;
    static View inventoryView;

    public static Map map;
    public static GameObject playerGO;
    public static Creature player;
    public static void Init(string pName, int CAIterations)
    {
        int mapSize = 50;
        int xStart = 0;
        int yStart = 0;
        #region Game mechanics
        map = new Map(mapSize, mapSize, CAIterations);
        bool startPosFound = false;
        for (int x = 2; x < mapSize; x++)
        {
            for (int y = 2; y < mapSize; y++)
            {
                if (!map.cells[x, y].IsWall())
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

        map.AddGameObject(playerGO);
        #endregion

        #region UI

        mapView = new()
        {
            X = 0,
            Y = 0,
        };

        characterView = new()
        {
            X = Pos.Right(mapView) + 4,
            Y = 0,
            Width = 20,
            Height = 30
        };

        Label playerName = new(player.name)
        {
            Y = Pos.Top(characterView) + 1,
            Width = Dim.Fill(),
            Height = 1,
            TextAlignment = TextAlignment.Centered
        };

        View statView = new()
        {
            Y = 2,
            Width = 20,
            Height = 5
        };
        Label somRef = new("SOM: " + player.Somatics + " REF: " + player.Reflexes)
        {
            Y = Pos.Top(statView),
            Width = Dim.Fill(1),
            Height = 1
        };
        View cogWil = new("COG: " + player.Cognition + " WIL: " + player.Willpower)
        {
            Y = Pos.Top(statView) + 2,
            Width = Dim.Fill(1),
            Height = 1
        };

        position = new()
        {
            Width = 15,
            Height = 2,
        };

        logView = new()
        {
            X = 1,
            Y = Pos.Bottom(mapView) + 1,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Text = "Game started"
        };

        statView.Add(playerName, position, somRef, cogWil);
        characterView.Add(playerName, statView);
        Application.Top.Add(mapView, characterView, logView);

        UpdatePos();
        #endregion
    }

    public static void OpenInventory()
    {

    }
    public static void OpenMain()
    {
    }
    public static void OpenMenu()
    {
    }

    public static void Log(string txt)
    {
        logView.Text = logView.Text.ToString().Insert(0, txt + "\n");
    }
    public static void UpdatePos()
    {
        position.Text = "X: " + playerGO.pos.x + "\nY: " + playerGO.pos.y;
    }
}

public class MapView : View
{
    public MapView()
    {
        Width = 31;
        Height = 31;
    }

    public override void Redraw(Rect bounds)
    {
        if (Game.map.cells.Length == 0)
        {
            Console.Error.WriteLine("trying to render map, but map is empty!");
            Application.RequestStop();
            return;
        }

        int boundHeight = 31;
        int boundWidth = 31;
        //player's position on the screen
        int pX = boundWidth / 2;
        int pY = boundHeight / 2;

        //upper-left visible map cell coordinates 
        int mX = Game.playerGO.pos.x - pX;
        int mY = Game.playerGO.pos.y - pY;



        for (int tx = 0; tx < boundWidth; tx++)
        {
            for (int ty = 0; ty < boundHeight; ty++)
            {
                Rune c;

                if (mX < Game.map.cells.GetLength(0) && mX > 0 && mY < Game.map.cells.GetLength(1) && mY > 0)
                {
                    c = Game.map.cells[mX, mY].GetRune();
                    if (Game.map.cells[mX, mY].IsWall())
                    {
                        Application.Driver.SetAttribute(Map.WALL_COLOR);
                    }
                    if (!Game.map.cells[mX, mY].IsWall())
                    {
                        Application.Driver.SetAttribute(Map.FLOOR_COLOR);
                    }
                }
                else
                {
                    c = ' ';
                }
                AddRune(tx, boundHeight - ty, c);
                mY++;
            }
            mX++;
            mY = Game.playerGO.pos.y - pY;
        }
    }

    public override bool ProcessHotKey(KeyEvent keyEvent)
    {
        bool keyRegistered = false;
        switch (keyEvent.Key)
        {
            case Key.D1:
                Game.playerGO.Move(-1, -1);
                keyRegistered = true;
                break;
            case Key.D2:
                Game.playerGO.Move(0, -1);
                keyRegistered = true;
                break;
            case Key.D3:
                Game.playerGO.Move(1, -1);
                keyRegistered = true;
                break;
            case Key.D4:
                Game.playerGO.Move(-1, 0);
                keyRegistered = true;
                break;
            case Key.D5:
                //interact mode?
                //just skip for now
                Game.playerGO.Move(0, 0);
                Game.Log("Pressed 5 on numpad");
                keyRegistered = true;
                break;
            case Key.D6:
                Game.playerGO.Move(1, 0);
                keyRegistered = true;
                break;
            case Key.D7:
                Game.playerGO.Move(-1, 1);
                keyRegistered = true;
                break;
            case Key.D8:
                Game.playerGO.Move(0, 1);
                keyRegistered = true;
                break;
            case Key.D9:
                Game.playerGO.Move(1, 1);
                keyRegistered = true;
                break;
            case Key.i:
                //Game.OpenInventory();
                Game.Log("Open inventory.");
                keyRegistered = true;
                break;
            default:
                break;
        }
        if (keyRegistered)
            this.SetNeedsDisplay();
        Game.UpdatePos();
        return keyRegistered;

    }
}

