using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Terminal.Gui.App;
using Terminal.Gui.Input;

public class UI : Window
{
    Label position = new();
    Label floorView = new();

    MapView mapView;
    View characterView = new();
    Label logView = new();
    Label timeView;
    Inventory inventoryView = new();

    static Player player = Game.player;
    public static event EventHandler<Vector2Int> Interact = delegate { };
    public static event EventHandler<string> LogEvent = delegate { };
    public UI()
    {
        player = Game.player;
        mapView = new() { X = 0, Y = 0 };
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

        View statView = new()
        {
            Y = 2,
            Width = 20,
            Height = 10
        };
        timeView = new Label()
        {
            Y = 14,
            Width = 20,
            Height = 3
        };

        position = new()
        {
            Y = Pos.Top(statView) + 2,
            Width = 15,
            Height = 3
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
        characterView.Add(playerName, statView, timeView);
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
        timeView.Text = "Time: " + Game.time;
        mapView.SetNeedsDraw();
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



