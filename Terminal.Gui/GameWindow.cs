using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Terminal.Gui.App;
using Terminal.Gui.Input;

public class GameWindow : Window
{
    InventoryWindow inventoryWindow;

    Label position = new();
    Label floorView = new();

    MapView mapView;
    View characterView = new();
    Label logView = new();
    Label timeView;

    static Creature Player = Game.Player;
    public static event EventHandler<Vector2Int> Interact = delegate { };
    public GameWindow()
    {
        inventoryWindow = new(this);
        Player = Game.Player;

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
            Text = Player.Name,
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


        statView.Add(playerName, position, floorView);
        characterView.Add(playerName, statView, timeView);
        this.Add(mapView, characterView, logView);
        Game.LogEvent += (s, e) => this.PrintLog(e);

        UpdatePos();

        IApplication? app = App;
    }

    public void PrintLog(string txt)
    {
        logView.Text = logView.Text.ToString().Insert(0, txt + "\n");
    }
    public void UpdatePos()
    {
        floorView.Text = "Floor: " + Game.dungeon.currentFloor.ToString();
        position.Text = "X: " + Player.Pos.X + "\nY: " + Player.Pos.Y;
        timeView.Text = "Time: " + Game.time;
        mapView.SetNeedsDraw();
    }

    public void OpenInventory()
    {
        App?.Run(inventoryWindow);
    }
    public void OpenMenu()
    {

    }
    public void HideMain()
    {
    }

    public static void InvokeInteract(Vector2Int pos)
    {
        Interact.Invoke(null, pos);
    }
    protected override bool OnKeyDown(Key key)
    {
        bool keyRegistered = false;

        if (key == Key.I)
        {
            keyRegistered = true;
            OpenInventory();
        }
        /* don't even remember what this was
        case (Key)62:
            keyRegistered = true;
            UI.InvokeInteract(Game.player, new(Game.player.pos));
            break;
        */

        if (keyRegistered)
            this.UpdatePos();

        return keyRegistered;

    }

}



