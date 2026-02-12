public static class AI
{
    static Creature Player = Game.Player;
    public static Random rng = new();
    static Path Pathfinder = new();
    static Dictionary<Creature, AIData> creatures = new();

    // very barebones; should separate it into several functions for every case
    // alternatively, I should look into that weighted action system FEAR has
    public static void ActAll(Map map)
    {
        foreach (KeyValuePair<Creature, AIData> kvp in creatures)
        {
            Creature creature = kvp.Key;
            AIData data = kvp.Value;
            Vector2Int nextPos;
            if (data.aut < 5)
                continue;
            switch (data.state)
            {
                default: // reset to sleep
                    Game.Log(kvp.Key.Name + " has a weird AIState, resetting to Sleep");
                    data.state = AIState.Sleep;
                    goto case AIState.Sleep;
                case AIState.Idle:
                    if (CanSeeEnemies(creature, ref data.enemies))
                    {
                        data.state = AIState.Attack;
                        data.lastEnemyPos = GetClosestEnemy(creature, data.enemies).Pos;
                        goto case AIState.Attack;
                    }
                    else
                    {
                        if (rng.Next(1, 100) > 98)
                        {
                            data.aut -= 35;
                            goto case AIState.Sleep;
                        }

                        while (true)
                        {
                            int x = rng.Next(creature.Pos.X - creature.SightRadius, creature.Pos.X + creature.SightRadius);
                            int y = rng.Next(creature.Pos.Y - creature.SightRadius, creature.Pos.Y + creature.SightRadius);
                            if (x < 1 || x >= map.size.X - 1 || y < 1 || y >= map.size.Y - 1) // OOB check
                                continue;
                            else if (Dungeon.CanSeeTile(creature.Pos, new Vector2Int(x, y)) &&
                                    Pathfinder.Calculate(creature.Pos, new Vector2Int(x, y), Game.CurrentMap.GetObstacles(), ref data.path))
                                break;
                        }
                    }
                    if (data.path.TryDequeue(out nextPos))
                        creature.MoveTo(nextPos);
                    break;
                case AIState.Attack:
                    if (CanSeeEnemies(creature, ref data.enemies))
                    {
                        Creature closestEnemy = GetClosestEnemy(creature, data.enemies);
                        data.lastEnemyPos = closestEnemy.Pos;
                        if (data.path.Last() != data.lastEnemyPos)
                            Pathfinder.Calculate(creature.Pos, data.lastEnemyPos, Game.CurrentMap.GetObstacles(), ref data.path);

                        if (CanReachAttack(creature, closestEnemy.Pos) && data.aut >= creature.AttackSpeed)
                        {
                            closestEnemy.ReceiveDamage(1f);
                            data.aut -= creature.AttackSpeed;
                        }
                        else if (data.path.TryDequeue(out nextPos))
                        {
                            if (data.aut >= creature.MovementSpeed)
                            {
                                creature.MoveTo(nextPos);
                                data.aut -= creature.MovementSpeed;
                            }
                        }
                    }
                    else goto case AIState.Pursuit;
                    break;
                case AIState.Pursuit:
                    if (data.path.TryDequeue(out nextPos))
                        creature.MoveTo(nextPos);
                    if (CanSeeTarget(creature, Player))
                        goto case AIState.Attack;
                    else data.state = AIState.Idle;
                    break;
                case AIState.Sleep:
                    if (!CanSeeEnemies(creature, ref data.enemies) && rng.Next(1, 100) > 60)
                        goto case AIState.Attack;
                    break;

            }
        }
    }
    public static void PassTime(int aut)
    {
        foreach (KeyValuePair<Creature, AIData> kvp in creatures)
        {
            kvp.Value.aut += aut;
        }
    }
    public static bool CanSeeTarget(Creature host, Creature target)
    {
        //check whether the Player is in vision range in the first place
        Vector2Int distance = target.Pos - host.Pos;
        // since our FOV is square shaped (because of equidistant movement), 
        // we don't need to calculate the distance with a square root
        if (Math.Abs(distance.X) > host.SightRadius || Math.Abs(distance.Y) > host.SightRadius)
            return false;

        else return Dungeon.CanSeeTile(host.Pos, target.Pos);
    }

    // needs rework; maybe it's better to go over every creature on the map that isn't of the same faction
    public static bool CanSeeEnemies(Creature creature, ref List<Creature> enemies)
    {
        bool result = false;
        enemies.Clear();
        for (int x = creature.Pos.X - creature.SightRadius; x <= creature.Pos.X + creature.SightRadius; x++)
        {
            for (int y = creature.Pos.Y - creature.SightRadius; y <= creature.Pos.Y + creature.SightRadius; y++)
            {
                if (x < 1 || x > Game.CurrentMap.size.X - 2 || y < 1 || y < Game.CurrentMap.size.Y - 2)
                    continue;
                Vector2Int enemyPos = new(x, y);
                if (Game.CurrentMap.cells[enemyPos].creature is not null)
                {
                    Creature enemy = Game.CurrentMap.cells[enemyPos].creature;
                    if (enemy.Faction != creature.Faction && CanSeeTarget(creature, enemy))
                    {
                        enemies.Add(Game.CurrentMap.cells[new(x, y)].creature);
                        result = true;
                    }
                }
            }
        }
        return result;
    }
    public static Creature GetClosestEnemy(Creature creature, List<Creature> enemies)
    {
        Creature closest = Game.Player;
        Queue<Vector2Int> shortestPath = new();
        Queue<Vector2Int> refPath = new();
        if (enemies.Count == 0)
            return closest;
        foreach (Creature enemy in enemies)
        {
            Pathfinder.Calculate(creature.Pos, enemy.Pos, Game.CurrentMap.GetObstacles(), ref refPath);
            if (refPath.Count < shortestPath.Count)
            {
                closest = enemy;
                shortestPath = new(refPath.ToArray());
            }
        }
        return closest;
    }

    public static bool CanReachAttack(Creature creature, Vector2Int targetPos)
    {
        if (Math.Abs(creature.Pos.X - targetPos.X) <= 1 && Math.Abs(creature.Pos.Y - targetPos.Y) <= 1)
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
    public float aut;
    public Vector2Int lastEnemyPos;
    public Queue<Vector2Int> path;
    public List<Creature> enemies;
    public AIData(AIState state = AIState.Idle, float aut = 0)
    {
        this.state = state;
        this.aut = aut;
        this.path = new();
        this.enemies = new(5);
    }
}

public enum AIState : Byte { Idle, Attack, Pursuit, Sleep }
