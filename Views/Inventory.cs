using Terminal.Gui.Views;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Input;

public class InventoryView : View
{
    Player player = Game.player;
    public InventoryView()
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

        //here be navigation controls

        return processed;
    }
}

