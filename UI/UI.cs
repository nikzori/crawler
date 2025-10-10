using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Terminal.Gui.App;
using Terminal.Gui.Input;


public static class UI
{
    static Label position = new();
    static Label floorView = new();

    static MapView mapView = new(31);
    static View characterView = new();
    static Label logView = new();
    static Inventory inventoryView = new();

    static Player player = Game.player;
    public static event EventHandler<InteractEventArgs> Interact = delegate { };
    public static void Init()
    {
        player = Game.player;
        mapView = new(33) { X = 0, Y = 0, };

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
            Height = 1,
            Text = player.name,
            TextAlignment = Alignment.Center
        };

        View statView = new()
        {
            Y = 2,
            Width = 20,
            Height = 5
        };

        position = new()
        {
            Y = Pos.Top(statView) + 2,
            Width = 15,
            Height = 2,
        };
        floorView = new()
        {
            Width = 15,
            Height = 2,
        };

        logView = new()
        {
            X = 1,
            Y = Pos.Right(characterView) + 1,
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
        Application.Top.Add(mapView, characterView, logView, inventoryView);

        UpdatePos();
    }

    public static void Log(string txt)
    {
        logView.Text = logView.Text.ToString().Insert(0, txt + "\n");
    }
    public static void UpdatePos()
    {
        floorView.Text = "Floor: " + Game.dungeon.currentFloor.ToString();
        position.Text = "X: " + player.pos.x + "\nY: " + player.pos.y;
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
        //upper-left visible map cell coordinates 
        mX = Game.player.pos.X - pX;
        mY = Game.player.pos.Y - pY;


        for (int tx = 0; tx < boundWidth; tx++)
        {
            for (int ty = 0; ty < boundHeight; ty++)
            {
                System.Text.Rune c = new(' ');

                if (mX < Game.currentMap.size.X && mX > 0 && mY < Game.currentMap.size.Y && mY > 0)
                {
                    if (Math.Abs(tx - pX) < Game.player.sightRadius && Math.Abs(ty - pY) < Game.player.sightRadius)
                    {
                        // very unnatural (and probably very inefficient) LOS made with Bresenham's algorythm, 
                        // but hey, it works
                        if (Dungeon.CanSeeTile(Game.player.pos, new(mX, mY)))
                        {
                            c = Game.currentMap.cells[new(mX, mY)].GetRune();
                            Game.currentMap.cells[new(mX, mY)].SetRevealed(true);

                            if (Game.currentMap.cells[new(mX, mY)].IsWall())
                                Application.Driver.SetAttribute(Dungeon.WALL_COLOR);
                            else Application.Driver.SetAttribute(Dungeon.FLOOR_COLOR);
                        }
                        else if (Game.currentMap.cells[new(mX, mY)].isRevealed)
                        {
                            c = Game.currentMap.cells[new(mX, mY)].GetRune();
                            Application.Driver.SetAttribute(Dungeon.REVEALED_COLOR);
                        }
                        else
                        {
                            c = new System.Text.Rune(Game.currentMap.background[new(mX + 15, mY + 15)]);
                            Application.Driver.SetAttribute(Dungeon.OBSCURED_COLOR);
                        }
                    }
                    else if (Game.currentMap.cells[new(mX, mY)].isRevealed)
                    {
                        c = Game.currentMap.cells[new(mX, mY)].GetRune();
                        Application.Driver.SetAttribute(Dungeon.REVEALED_COLOR);
                    }
                    else
                    {
                        c = Game.currentMap.background[new(mX + 15, mY + 15)];
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
            mY = Game.player.pos.Y - pY;
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
                Game.player.TileInteract(-1, -1);
                keyRegistered = true;
                break;
            case Key.D2:
                Game.player.TileInteract(0, -1);
                keyRegistered = true;
                break;
            case Key.D3:
                Game.player.TileInteract(1, -1);
                keyRegistered = true;
                break;
            case Key.D4:
                Game.player.TileInteract(-1, 0);
                keyRegistered = true;
                break;
            case Key.D5:
                //interact mode?
                Game.player.Move(0, 0);
                Game.Update(10);
                UI.Log("Key Registered");
                keyRegistered = true;
                break;
            case Key.D6:
                Game.player.TileInteract(1, 0);
                keyRegistered = true;
                break;
            case Key.D7:
                Game.player.TileInteract(-1, 1);
                keyRegistered = true;
                break;
            case Key.D8:
                Game.player.TileInteract(0, 1);
                keyRegistered = true;
                break;
            case Key.D9:
                Game.player.TileInteract(1, 1);
                keyRegistered = true;
                break;
            case Key.i:
                UI.ShowInventory();
                UI.Log("Open inventory.");
                keyRegistered = true;
                break;
            case (Key)62:
                keyRegistered = true;
                UI.InvokeInteract(Game.player, new(Game.player.pos));
                break;
            case Key.Esc:
                keyRegistered = true;
                Application.RequestStop(Application.Top);
                break;
            default:
                break;
        }
        if (keyRegistered)
        {
            this.SetNeedsDraw();
            UI.UpdatePos();
        }
        return keyRegistered;

    }
}

public class InteractEventArgs : EventArgs
{
    public (int x, int y) pos;
    public InteractEventArgs(int x, int y)
    {
        pos.x = x;
        pos.y = y;
    }
    public InteractEventArgs((int x, int y) pos)
    {
        this.pos = pos;
    }
}

