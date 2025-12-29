using Terminal.Gui.Views;
using Terminal.Gui.ViewBase;

public class MainWindow : Window
{
    Game? game;
    public MainWindow()
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
            Text = "New Game",
            ShadowStyle = ShadowStyle.None
        };
        NewGameBtn.Accepting += (s, e) =>
        {
            if (NameField.Text != "")
                StartGame(NameField.Text.ToString());
            else StartGame("Player");
            e.Handled = true;
        };

        Button Exit = new()
        {
            X = Pos.Center(),
            Y = 7,
            Width = 9,
            Height = 3,
            Text = "Quit",
            ShadowStyle = ShadowStyle.None
        };
        Exit.Accepting += (s, e) =>
        {
            this.Dispose();
            e.Handled = true;
        };

        this.Add(Name, NameField, NewGameBtn, Exit);
    }

    public void StartGame(string characterName, string CAIterations = "2")
    {
        int iterations = 2;
        Int32.TryParse(CAIterations, out iterations);

        game = new Game(characterName);
        GameWindow gw = new();
        this.App?.Run(gw);
        gw.Dispose();
    }
}
