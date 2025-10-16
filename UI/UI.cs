using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Terminal.Gui.App;
using Terminal.Gui.Input;

public class UI : Window
{
    Label position = new();
    Label floorView = new();

    MapView mapView = new(31);
    View characterView = new();
    Label logView = new();
    Inventory inventoryView = new();

    static Player player = Game.player;
    public static event EventHandler<Vector2Int> Interact = delegate { };
    public static event EventHandler<string> LogEvent = delegate { };
    public UI()
    {
        player = Game.player;
        mapView = new(33) { X = 0, Y = 0 };

        characterView = new()
        {
            X = Pos.Right(mapView) + 4,
            Y = 0,
            Width = 20,
            Height = 30
        };

        Label playerName = new Label()
        {
            Y = Pos.Top(characterView) + 1,
            Width = Dim.Fill(),
            Height = 3,
            Text = player.name,
            TextAlignment = Alignment.Center
        };

        Window statView = new()
        {
            Y = 2,
            Width = 20,
            Height = 10
        };

        position = new()
        {
            Y = Pos.Top(statView) + 2,
            Width = 15,
            Height = 3,
        };
        floorView = new()
        {
            Width = 15,
            Height = 3,
        };

        logView = new()
        {
            X = 1,
            Y = Pos.Bottom(characterView) + 1,
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

        statView.Add(playerName, position, floorView);
        characterView.Add(playerName, statView);
        this.Add(mapView, characterView, logView, inventoryView);
        UI.LogEvent += (s, e) => this.PrintLog(e);

        UpdatePos();
    }

    public static void Log(string text)
    {
        LogEvent.Invoke(null, text);
    }

    public void PrintLog(string txt)
    {
        logView.Text = logView.Text.ToString().Insert(0, txt + "\n");
    }
    public void UpdatePos()
    {
        floorView.Text = "Floor: " + Game.dungeon.currentFloor.ToString();
        position.Text = "X: " + player.pos.X + "\nY: " + player.pos.Y;
        mapView.UpdateMap();
    }

    public void ShowInventory()
    {
        HideMain();
        inventoryView.Init();
        inventoryView.Visible = true;
    }
    public void OpenMenu()
    {

    }
    public void ShowMain()
    {
        mapView.Visible = true;
        characterView.Visible = true;
        logView.Visible = true;
    }
    public void HideMain()
    {
        mapView.Visible = false;
        characterView.Visible = false;
        logView.Visible = false;
    }

    public static void InvokeInteract(Vector2Int pos)
    {
        Interact.Invoke(null, pos);
    }
    protected override bool OnKeyDown(Key key)
    {
        if (!Visible)
            return false;
        bool keyRegistered = false;

        // switch-case doesn't like Key.Parameters for some reason
        // so this will be ugly for now
        if (key == Key.D1)
        {
            Game.player.TileInteract(new(-1, -1));
            keyRegistered = true;
        }
        if (key == Key.D2)
        {
            Game.player.TileInteract(new(0, -1));
            keyRegistered = true;
        }
        if (key == Key.D3)
        {
            Game.player.TileInteract(new(1, -1));
            keyRegistered = true;
        }
        if (key == Key.D4)
        {
            Game.player.TileInteract(new(-1, 0));
            keyRegistered = true;
        }
        if (key == Key.D5)
        {
            //interact mode?
            Game.Update(10);
            UI.Log("Key Registered");
            keyRegistered = true;
        }
        if (key == Key.D6)
        {
            Game.player.TileInteract(new(1, 0));
            keyRegistered = true;
        }
        if (key == Key.D7)
        {
            Game.player.TileInteract(new(-1, 1));
            keyRegistered = true;
        }
        if (key == Key.D8)
        {
            Game.player.TileInteract(new(0, 1));
            keyRegistered = true;
        }
        if (key == Key.D9)
        {
            Game.player.TileInteract(new(1, 1));
            keyRegistered = true;
        }
        if (key == Key.I)
        {
            this.ShowInventory();
            UI.Log("Open inventory.");
            keyRegistered = true;
        }
        /* don't even remember what this was
        case (Key)62:
            keyRegistered = true;
            UI.InvokeInteract(Game.player, new(Game.player.pos));
            break;
        */
        if (key == Key.Esc)
        {
            keyRegistered = true;
            Application.RequestStop(Application.Top);
        }

        if (keyRegistered)
            this.UpdatePos();

        return keyRegistered;

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
        UpdateMap();
    }

    #region Line of Sight
    public void UpdateMap()
    {
        //upper-left visible map cell coordinates 
        mX = Game.player.pos.X - pX;
        mY = Game.player.pos.Y - pY;

        Vector2Int currentPos = new(mX, mY);
        System.Text.Rune c = new('.');
        for (int tx = 0; tx < boundWidth; tx++)
        {
            for (int ty = 0; ty < boundHeight; ty++)
            {
                if (mX < Game.currentMap.size.X && mX > 0 && mY < Game.currentMap.size.Y && mY > 0)
                {
                    c = Game.currentMap.cells[currentPos].GetRune();
                    Game.currentMap.cells[currentPos].SetRevealed(true);

                    if (Game.currentMap.cells[currentPos].IsWall())
                        SetAttribute(Dungeon.WALL_COLOR);
                    else SetAttribute(Dungeon.FLOOR_COLOR);
                    /*
                    if (Math.Abs(tx - pX) < Game.player.sightRadius && Math.Abs(ty - pY) < Game.player.sightRadius)
                    {
                                
                        // very unnatural (and probably very inefficient) LOS made with Bresenham's algorythm, 
                        // but hey, it works
                        if (Dungeon.CanSeeTile(Game.player.pos, currentPos))
                        {
                            c = Game.currentMap.cells[currentPos].GetRune();
                            Game.currentMap.cells[currentPos].SetRevealed(true);

                            if (Game.currentMap.cells[currentPos].IsWall())
                                Application.Driver.SetAttribute(Dungeon.WALL_COLOR);
                            else Application.Driver.SetAttribute(Dungeon.FLOOR_COLOR);
                        }
                        else if (Game.currentMap.cells[currentPos].isRevealed)
                        {
                            c = Game.currentMap.cells[currentPos].GetRune();
                            Application.Driver.SetAttribute(Dungeon.REVEALED_COLOR);
                        }
                        else
                        {
                            c = new System.Text.Rune(Game.currentMap.background[new(mX + 15, mY + 15)]);
                            Application.Driver.SetAttribute(Dungeon.OBSCURED_COLOR);
                        }
                        
                    }
                */
                    /*
                    else if (Game.currentMap.cells[currentPos].isRevealed)
                    {
                        c = Game.currentMap.cells[currentPos].GetRune();
                        Application.Driver.SetAttribute(Dungeon.REVEALED_COLOR);
                    }
                    else
                    {
                        c = new(Game.currentMap.background[new(mX + 15, mY + 15)]);
                        Application.Driver.SetAttribute(Dungeon.OBSCURED_COLOR);
                    }
                    */
                }
                else
                {
                    c = new(Game.currentMap.background[new(mX + pX, mY + pY)]);
                    Application.Driver.SetAttribute(Dungeon.OBSCURED_COLOR);
                }
                Application.Driver.Move(tx, boundHeight - ty);
                Application.Driver.AddRune(/*tx, boundHeight - ty,*/ c);
                //else UI.Log("Couldn't add rune");
                mY++;
                currentPos = new(mX, mY);
            }
            mX++;
            mY = Game.player.pos.Y - pY;
            currentPos = new(mX, mY);
        }
        SetNeedsDraw();
    }
    #endregion

}

