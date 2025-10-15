using Terminal.Gui.Views;
using Terminal.Gui.ViewBase;
using Terminal.Gui.App;

public class MainMenu : Window
{
    public MainMenu()
    {

        Label Name = new()
        {
            X = Pos.Center() - 20,
            Y = 1,
            Width = 20,
            Text = "Character Name:"
        };
        TextField NameField = new()
        {
            X = Pos.Center() + 1,
            Y = 1,
            Width = 20,
            Text = ""
        };
        Button NewGameBtn = new Button()
        {
            X = Pos.Center(),
            Y = 4,
            Width = 14,
            Height = 3,
            Text = "New Game"
        };
        NewGameBtn.Accepting += (s, e) =>
        {
            if (NameField.Text != "")
            {
                Application.Top.RemoveAll();
                StartGame(NameField.Text.ToString());
            }
            else StartGame("Player");
        };

        Button Exit = new()
        {
            X = Pos.Center(),
            Y = 7,
            Width = 9,
            Height = 3,
            Text = "Quit"
        };
        Exit.Accepting += (s, e) => { Application.RequestStop(Application.Top); };

        this.Add(Name, NameField, NewGameBtn, Exit);
    }

    public static void StartGame(string characterName, string CAIterations = "2")
    {
        int iterations = 2;
        Int32.TryParse(CAIterations, out iterations);
        new Game(characterName);
    }
}
