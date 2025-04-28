using Terminal.Gui;

public static class MainMenu
{
    public static void Init()
    {

        Label Name = new("Character Name:")
        {
            X = Pos.Center() - 20,
            Y = 1,
            Width = 20,
        };
        TextField NameField = new("")
        {
            X = Pos.Center() + 1,
            Y = 1,
            Width = 20
        };
        Button NewGameBtn = new Button("New Game", true)
        {
            X = Pos.Center(),
            Y = 4,
            Width = 14,
            Height = 1
        };
        NewGameBtn.Clicked += () =>
        {
            if (NameField.Text != "")
            {
                Application.Top.RemoveAll();
                StartGame(NameField.Text.ToString());
            }
            else StartGame("Player");
        };

        Button Exit = new("Quit")
        {
            X = Pos.Center(),
            Y = 7,
            Width = 8,
            Height = 1
        };
        Exit.Clicked += () => { Application.RequestStop(Application.Top); };

        Application.Top.Add(Name, NameField, NewGameBtn, Exit);
        Application.Run();
    }

    public static void StartGame(string characterName, string CAIterations = "2")
    {
        int iterations = 2;
        Int32.TryParse(CAIterations, out iterations);
        new Game(characterName);
    }
}
