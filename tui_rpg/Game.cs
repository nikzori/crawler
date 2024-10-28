using Terminal.Gui;

public static class Game
{
    static MapView mapView = new MapView();
    static FrameView characterView = new FrameView();

    public static Map map;
    public static GameObject playerGO;
    public static Creature player;
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
        player = new Creature(pName, new Rune('@'), 5, 5, 5, 5, 1);
        playerGO = new GameObject((xStart, yStart), player);

        map.AddGameObject(playerGO);
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

        Label playerName = new(player.name)
        {
            Y = Pos.Top(characterView),
            Width = Dim.Fill(),
            Height = 1
        };

        View statView = new()
        {
            Y = 2,
            Width = 20,
            Height = 5
        };
        Label somRef = new("SOM: " + player.Somatics + " REF: " + player.Reflexes)
        {
            Y = Pos.Top(statView),
            Width = Dim.Fill(1),
            Height = 1
        };
        View cogWil = new("COG: " + player.Cognition + "  WIL: " + player.Willpower)
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

        /*
        Application.MainLoop.AddIdle(() =>
        {
            mapView.SetNeedsDisplay();
            position.SetNeedsDisplay();
            return true;
        }
        );
        */
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
        int mX = Game.playerGO.pos.x - pX;
        int mY = Game.playerGO.pos.y - pY;

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
            mY = Game.playerGO.pos.y - pY;
        }
    }

    public override bool ProcessHotKey(KeyEvent keyEvent)
    {
        switch (keyEvent.Key)
        {
            case Key.D1:
                Game.playerGO.Move(-1, -1);
                return true;
            case Key.D2:
                Game.playerGO.Move(0, -1);
                return true;
            case Key.D3:
                Game.playerGO.Move(1, -1);
                return true;
            case Key.D4:
                Game.playerGO.Move(-1, 0);
                return true;
            case Key.D5:
                //interact mode?
                //just skip for now
                Game.playerGO.Move(0, 0);
                return true;
            case Key.D6:
                Game.playerGO.Move(1, 0);
                return true;
            case Key.D7:
                Game.playerGO.Move(-1, 1);
                return true;
            case Key.D8:
                Game.playerGO.Move(0, 1);
                return true;
            case Key.D9:
                Game.playerGO.Move(1, 1);
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
        Text = "X: " + Game.playerGO.pos.x + "; Y: " + Game.playerGO.pos.y;
    }

}
