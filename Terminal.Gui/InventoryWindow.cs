using Terminal.Gui.Views;
using Terminal.Gui.Input;

public class InventoryWindow : Window
{
    Creature Player = Game.Player;
    Inventory inventory = Game.Player.Inventory;
    List<Button> itemSlots = new(50);
    Label test;
    public InventoryWindow(GameWindow GameWindow)
    {
        test = new() { X = 1, Y = 1, Width = 30, Height = 3, Text = "This is a test entry" };
        this.Add(test);
        SetFocus();
    }

    public void UpdateInventory()
    {
    }

    protected override bool OnKeyDown(Key key)
    {
        bool processed = false;

        if (key == Key.Esc)
        {
            processed = true;
            App?.RequestStop(this);
        }
        if (key == Key.D5)
        {
            processed = true;
            test.Text = "Another One.";
        }
        if (key == Key.D4)
        {
            processed = true;
            test.Text = "aeiou";
        }
        key.Handled = processed; // unclear if I need to do this, but just in case 
        return processed;
    }
}

