using Terminal.Gui.Views;
using Terminal.Gui.Input;

public class InventoryWindow : Window
{   
    GameWindow gameWindow;
    Player player = Game.player;
    Label test;
    public InventoryWindow(GameWindow GameWindow)
    {
        this.gameWindow = GameWindow;
        test = new() { X = 1, Y = 1, Width = 30, Height = 3, Text = "This is a test entry" };
        this.Add(test);
        SetFocus();
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

