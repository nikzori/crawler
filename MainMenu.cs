using Terminal.Gui;

public static class MainMenu
{
    public static void Init()
    {
        Button NewGameBtn = new Button("New Game", true)
        {
            X = Pos.Center(),
            Y = 3,
            Width = 14,
            Height = 1
        };
        NewGameBtn.Clicked += () =>
        {
            Application.Top.RemoveAll();
            NewGame();
        };

        Button Exit = new("Quit")
        {
            X = Pos.Center(),
            Y = 7,
            Width = 8,
            Height = 1
        };
        Exit.Clicked += () => { Application.RequestStop(Application.Top); };

        Application.Top.Add(NewGameBtn, Exit);
        Application.Run();
    }

    public static void NewGame()
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

        Label CALabel = new("Iterations:")
        {
            X = Pos.Center() + 1,
            Y = 3,
            Width = 20,
        };
        TextField CAIterations = new("")
        {
            X = Pos.Center() + 1,
            Y = 3,
            Width = 20
        };

        Button StartBtn = new("_Start ", false)
        {
            X = Pos.Center(),
            Y = 5
        };
        StartBtn.Clicked += () =>
        {
            if (NameField.Text != "")
            {
                Application.Top.RemoveAll();
                StartGame(NameField.Text.ToString(), CAIterations.Text.ToString());
            }
            else Application.Top.Add(new Label("Enter Character Name"));
        };

        Button BackBtn = new("_Back")
        {
            X = Pos.Center(),
            Y = 7
        };
        BackBtn.Clicked += () =>
        {
            Application.Top.RemoveAll();
            Init();
        };

        Application.Top.Add(Name, NameField, /* CALabel, CAIterations, */ StartBtn, BackBtn);
    }

    public static void StartGame(string characterName, string CAIterations)
    {
        int iterations = 2;
        Int32.TryParse(CAIterations, out iterations);
        Game.Init(characterName, iterations);

    }
}
