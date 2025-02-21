using Terminal.Gui;

public static class UI 
{
    static Label position = new();

    static MapView mapView = new(31);
    static View characterView = new();
    static Label logView = new();
    static Inventory inventoryView = new();

    static Creature player;
    static GameObject playerGO;
    public static event EventHandler<InteractEventArgs> Interact = delegate { };
    public static void Init()
    {
        player = Game.player;
        playerGO = Game.playerGO;
        mapView = new(31) { X = 0, Y = 0, };

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
        inventoryView = new() 
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Visible = false
        };

        statView.Add(playerName, position, somRef, cogWil);
        characterView.Add(playerName, statView);
        Application.Top.Add(mapView, characterView, logView, inventoryView);

        UpdatePos();
        Task.Delay(1000).ContinueWith(_ =>
        {
            List<Stairs> currentStairs = Game.dungeon.GetCurrentFloor().stairs;
            if (currentStairs.Count == 0)
                Log("no stairs, wtf");
            foreach (Stairs stair in currentStairs)
                Log("Added stairs at x: " + stair.pos.x + "; y: " + stair.pos.y);
        });
        
    }

    public static void Log(string txt)
    {
        logView.Text = logView.Text.ToString().Insert(0, txt + "\n");
    }
    public static void UpdatePos()
    {
        position.Text = "X: " + playerGO.pos.x + "\nY: " + playerGO.pos.y;
        mapView.Redraw(mapView.Bounds);
    }

    public static void ShowInventory()
    {
        HideMain();
        inventoryView.Init();
        inventoryView.Visible = true;
    }
    public static void OpenMenu()
    {
    }
    public static void ShowMain() 
    {
        mapView.Visible = true;
        characterView.Visible = true;
        logView.Visible = true;
    }
    public static void HideMain() 
    {
        mapView.Visible = false;
        characterView.Visible = false;
        logView.Visible = false;
    }

    // this feels wrong. Oh well.
    public static void InvokeInteract(object sender, InteractEventArgs e)
    {
        Interact.Invoke(sender, e);
    }
}

public class MapView : View
{
    int boundWidth, boundHeight; // int values for viewport size
    int pX, pY; // player position on the screen
    int mX, mY;
    public MapView(int size)
    {
        // viewport size needs to be an odd number to put player in the center
        if (size % 2 != 1)
            size--;

        Width = size;
        Height = size;
        boundWidth = boundHeight = size;
        pX = pY = size / 2; // center the player on the screen
    }

    #region Line of Sight
    public override void Redraw(Rect bounds) 
    {
        Random rng = new();
        //upper-left visible map cell coordinates 
        mX = Game.playerGO.pos.x - pX;
        mY = Game.playerGO.pos.y - pY;


        for (int tx = 0; tx < boundWidth; tx++)
        {
            for (int ty = 0; ty < boundHeight; ty++)
            {
                Rune c = ' ';

                if (mX < Game.currentMap.cells.GetLength(0) && mX > 0 && mY < Game.currentMap.cells.GetLength(1) && mY > 0)
                {
                    if (Math.Abs(tx - pX) < Game.player.sightRadius && Math.Abs(ty - pY) < Game.player.sightRadius)
                    {
                        // very unnatural (and probably very inefficient) LOS made with Bresenham's algorythm, 
                        // but hey, it works
                        if (Dungeon.CanSeeTile(Game.playerGO.pos, (mX, mY)))
                        {
                            c = Game.currentMap.cells[mX, mY].GetRune();
                            Game.currentMap.cells[mX,mY].isRevealed = true;

                            if (Game.currentMap.cells[mX, mY].IsWall())
                                Application.Driver.SetAttribute(Dungeon.WALL_COLOR);
                            else Application.Driver.SetAttribute(Dungeon.FLOOR_COLOR);
                        }
                        else if (Game.currentMap.cells[mX,mY].isRevealed)
                        {
                            c = Game.currentMap.cells[mX, mY].GetRune();
                            Application.Driver.SetAttribute(Dungeon.REVEALED_COLOR);
                        }
                        else 
                        {
                            c = Game.currentMap.background[mX + 15, mY + 15];
                            Application.Driver.SetAttribute(Dungeon.OBSCURED_COLOR);
                        }
                    }
                    else if (Game.currentMap.cells[mX,mY].isRevealed)
                    {
                        c = Game.currentMap.cells[mX, mY].GetRune();
                        Application.Driver.SetAttribute(Dungeon.REVEALED_COLOR);
                    }
                    else 
                    {
                        c = Game.currentMap.background[mX + 15, mY + 15];
                        Application.Driver.SetAttribute(Dungeon.OBSCURED_COLOR);
                    }
                }
                else
                {
                    c = Game.currentMap.background[mX + 15, mY + 15];
                    Application.Driver.SetAttribute(Dungeon.OBSCURED_COLOR);
                }
                AddRune(tx, boundHeight - ty, c);
                mY++;
            }
            mX++;
            mY = Game.playerGO.pos.y - pY;
        }
        
    }
    #endregion
    public override bool ProcessHotKey(KeyEvent keyEvent)
    {
        if (!Visible)
            return false;
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
                UI.Log("Pressed 5 on numpad");
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
                UI.ShowInventory();
                UI.Log("Open inventory.");
                keyRegistered = true;
                break;
            case (Key)62:
                keyRegistered = true;
                UI.Log("hit closing angle bracket");
                UI.InvokeInteract(Game.playerGO, new(Game.playerGO.pos));
                break;
            case Key.Esc:
                keyRegistered = true;
                Application.RequestStop(Application.Top);
                break;
            default:
                break;
        }
        if (keyRegistered)
            this.SetNeedsDisplay();
        UI.UpdatePos();
        return keyRegistered;

    }
    
}

public class InteractEventArgs : EventArgs
{
    public (int x, int y) pos;
    public InteractEventArgs (int x, int y)
    {
        pos.x = x;
        pos.y = y;
    }
    public InteractEventArgs((int x, int y) pos)
    {
        this.pos = pos;
    }
}

