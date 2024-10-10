using Terminal.Gui;

public static class Game
{
    static MapView mapView = new MapView();
    static FrameView characterView = new FrameView();

    public static Map map;
    public static GameObject player = new GameObject();
    public static void Init(string pName, int CAIterations)
    {
        int mapSize = 50;
        int xStart = 0;
        int yStart = 0;
        #region Game mechanics
        map = new Map(mapSize, mapSize, CAIterations);
        bool startPosFound = false;
        for (int x = 2; x < mapSize; x++)
        {
            for (int y = 2; y < mapSize; y++)
            {
                if (!map.cells[x, y].isWall())
                {
                    xStart = x;
                    yStart = y;
                    startPosFound = true;
                    break;
                }
            }
            if (startPosFound)
                break;
        }

        player = new GameObject(new Character(), xStart, yStart);
        player.entity.Init('@', pName, 100f);
        (player.entity as Character).faction = Faction.Player;

        map.AddGameObject(player);
        #endregion

        #region UI
        mapView = new()
        {
            X = 0,
            Y = 0,
            Width = Dim.Percent(60),
            Height = Dim.Fill()
        };

        characterView = new()
        {
            X = Pos.Right(mapView),
            Y = 0,
            Width = Dim.Percent(39),
            Height = Dim.Fill()
        };

        Label playerName = new(player.entity.name)
        {
            Y = Pos.Top(characterView),
            Width = Dim.Fill(),
            Height = 1,
            TextAlignment = TextAlignment.Centered
        };

        View statView = new()
        {
            Y = 2,
            Width = 20,
            Height = 5
        };
        Label somRef = new("SOM: " + (player.entity as Character).Somatics + "  REF: " + (player.entity as Character).Reflexes)
        {
            Y = Pos.Top(statView),
            Width = Dim.Fill(1),
            Height = 1
        };
        View cogWil = new("COG: " + (player.entity as Character).Cognition + "  WIL: " + (player.entity as Character).Willpower)
        {
            Y = Pos.Top(statView) + 2,
            Width = Dim.Fill(1),
            Height = 1
        };

        PositionTracker position = new()
        {
            Width = 15,
            Height = 1,
        };

        Application.MainLoop.AddIdle(() =>
        {
            mapView.SetNeedsDisplay();
            position.SetNeedsDisplay();
            return true;
        }
        );
        statView.Add(somRef, cogWil);
        characterView.Add(playerName, statView, position);
        Application.Top.Add(mapView, characterView, position);
        #endregion
    }
}

public class MapView : View
{
    public MapView()
    {
        Width = 35;
        Height = 35;
    }

    public override void Redraw(Rect bounds)
    {
        // I guess this is where I'll have to check for visibility?

        if (Game.map.cells.Length == 0)
        {
            Console.Error.WriteLine("trying to render map, but map is empty!");
            Application.RequestStop();
            return;
        }

        int boundHeight = 33;
        int boundWidth = 33;
        //player's position on the screen
        int pX = boundWidth / 2;
        int pY = boundHeight / 2;

        //upper-left visible map cell coordinates 
        int mX = Game.player.x - pX;
        int mY = Game.player.y - pY;

        for (int tx = 0; tx < boundWidth; tx++)
        {
            for (int ty = 0; ty < boundHeight; ty++)
            {
                Rune c;

                if (mX < Game.map.cells.GetLength(0) && mX > 0 && mY < Game.map.cells.GetLength(1) && mY > 0)
                {
                    //toLOS[tx, ty] = Game.map.cells[mX, mY];
                    c = Game.map.cells[mX, mY].GetRune();
                }
                else
                {
                    c = ' ';
                }
                AddRune(tx, boundHeight - ty, c);
                mY++;
            }
            mX++;
            mY = Game.player.y - pY;
        }
    }

    public override bool ProcessHotKey(KeyEvent keyEvent)
    {
        switch (keyEvent.Key)
        {
            case Key.D1:
                Game.player.Move(-1, -1);
                return true;
            case Key.D2:
                Game.player.Move(0, -1);
                return true;
            case Key.D3:
                Game.player.Move(1, -1);
                return true;
            case Key.D4:
                Game.player.Move(-1, 0);
                return true;
            case Key.D5:
                //interact mode?
                //just skip for now
                Game.player.Move(0, 0);
                return true;
            case Key.D6:
                Game.player.Move(1, 0);
                return true;
            case Key.D7:
                Game.player.Move(-1, 1);
                return true;
            case Key.D8:
                Game.player.Move(0, 1);
                return true;
            case Key.D9:
                Game.player.Move(1, 1);
                return true;

            default:
                return false;
        }
    }

}

public class PositionTracker : Label
{
    public override void Redraw(Rect bounds)
    {
        base.Redraw(bounds);
        Text = "X: " + Game.player.pos.Item1 + "; Y: " + Game.player.pos.Item2;
    }

}
