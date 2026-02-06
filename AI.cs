public static class AI
{
    static Creature Player = Game.Player;
    public static Random rng = new();
    static Path Pathfinder = new();


    public static bool CanSeePlayer(Creature creature)
    {
        //check whether the Player is in vision range in the first place
        Vector2Int distance = Player.Pos - creature.Pos;
        // since our FOV is square shaped (because of equidistant movement), 
        // we don't need to calculate the distance with a square root
        if (Math.Abs(distance.X) > creature.SightRadius || Math.Abs(distance.Y) > creature.SightRadius)
            return false;

        else return Dungeon.CanSeeTile(creature.Pos, Player.Pos);
    }

    public static bool CanReachAttack(Creature host, Creature target)
    {
        if (Math.Abs(host.Pos.X - target.Pos.X) <= 1 && Math.Abs(host.Pos.Y - target.Pos.Y) <= 1)
            return true;
        else return false;
    }
}

/// <summary>
/// Container for AI-related values.
/// </summary>
public class AIData
{
    public AIState state;
    public int aut;
    public Vector2Int lastEnemyPos;

    public AIData(AIState state = AIState.Idle, int aut = 0)
    {
        this.state = state;
        this.aut = aut;
    }
}

public enum AIState { Idle, Attack, Pursuit, Sleep }
