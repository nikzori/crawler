using Terminal.Gui.Views;
using Terminal.Gui.Input;

public class InventoryWindow : Window
{
    Player player = Game.player;
    public InventoryWindow()
    {
        Label test = new() { X = 1, Y = 1, Width = 30, Height = 3, Text = "This is a test entry" };
        this.Add(test);

    }

    // re-checks player's item list to render it correctly
    public void Update()
    {

    }

    protected override bool OnKeyDown(Key key)
    {
        bool processed = false;

        if (key == Key.Esc)
        {
            // Close/Hide this View and show GameUI
        }

        return processed;
    }
}

