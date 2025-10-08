// Ripped from here: https://habr.com/ru/articles/513158/
// LINQ makes my brain pulsate weird


public struct Vector2Int : IEquatable<Vector2Int>
{
    public static readonly double Sqr2 = Math.Sqrt(2); // for diagonals

    public int X { get; }
    public int Y { get; }

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Estimated distance without obstacles
    /// </summary>
    public double DistanceEstimate()
    {
        int linearSteps = Math.Abs(Math.Abs(Y) - Math.Abs(X));
        int diagonalSteps = Math.Max(Math.Abs(Y), Math.Abs(X)) - linearSteps;
        return linearSteps + Sqr2 * diagonalSteps;
    }

    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        => new Vector2Int(a.X + b.X, a.Y + b.Y);
    public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        => new Vector2Int(a.X - b.X, a.Y - b.Y);
    public static bool operator ==(Vector2Int a, Vector2Int b)
        => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Vector2Int a, Vector2Int b)
        => !(a == b);

    public bool Equals(Vector2Int other)
        => X == other.X && Y == other.Y;

    public override bool Equals(object obj)
    {
        if (!(obj is Vector2Int))
            return false;

        var other = (Vector2Int)obj;
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
        => HashCode.Combine(X, Y);

    public override string ToString()
        => $"({X}, {Y})";
}

internal class PathNode
{
    public Vector2Int Position { get; }
    public double TraverseDistance { get; }
    public double EstimatedTotalCost { get; }
    public PathNode? Parent { get; } // store parent node to chain nodes together for a path

    public PathNode(Vector2Int Position, double TraverseDistance, double heuristicDistance, PathNode? Parent)
    {
        this.Position = Position;
        this.TraverseDistance = TraverseDistance;
        this.EstimatedTotalCost = TraverseDistance + heuristicDistance;
        this.Parent = Parent;
    }
}

internal static class NodeExtensions
{
    private static readonly (Vector2Int position, double travelCost)[] NeighboursTemplate = {
        (new Vector2Int(1, 0), 1),
        (new Vector2Int(0, 1), 1),
        (new Vector2Int(-1, 0), 1),
        (new Vector2Int(0, -1), 1),
        (new Vector2Int(1, 1), Math.Sqrt(2)),
        (new Vector2Int(1, -1), Math.Sqrt(2)),
        (new Vector2Int(-1, 1), Math.Sqrt(2)),
        (new Vector2Int(-1, -1), Math.Sqrt(2))
    };

    public static IEnumerable<PathNode> GenerateNeighbours(this PathNode parent, Vector2Int target)
    {
        foreach ((Vector2Int position, double travelCost) in NeighboursTemplate)
        {
            Vector2Int nodePosition = position + parent.Position;
            double traverseDistance = parent.TraverseDistance + travelCost;
            double heuristicDistance = (position - target).DistanceEstimate();
            yield return new PathNode(nodePosition, traverseDistance, heuristicDistance, parent);
        }
    }

}

public class Path
{
    private readonly List<PathNode> frontier = new();
    private readonly List<Vector2Int> ignoredPositions = new();

    public IReadOnlyCollection<Vector2Int> Calculate(Vector2Int start, Vector2Int target, IReadOnlyCollection<Vector2Int> obstacles)
    {
        if (!GenerateNodes(start, target, obstacles, out PathNode? node))
            return Array.Empty<Vector2Int>();

        var output = new List<Vector2Int>();

        while (node?.Parent != null)
        {
            output.Add(node.Position);
            node = node.Parent;
        }
        output.Add(start);
        return output.AsReadOnly();
    }

    private bool GenerateNodes(Vector2Int start, Vector2Int target, IEnumerable<Vector2Int> obstacles, out PathNode? node)
    {
        frontier.Clear();
        ignoredPositions.Clear();
        ignoredPositions.AddRange(obstacles);

        // starting point
        frontier.Add(new PathNode(start, 0, 0, null));

        while (frontier.Any())
        {
            // find node with lowest est. cost
            var lowest = frontier.Min(a => a.EstimatedTotalCost);
            PathNode current = frontier.First(a => a.EstimatedTotalCost == lowest);
            ignoredPositions.Add(current.Position);

            // if we've finally reached the target position
            if (current.Position == target)
            {
                node = current;
                return true;
            }

            // haven't reached the target; get neighbours for this node, repeat
            GenerateFrontierNodes(current, target);
        }

        // no nodes left, didn't find the target; target position unreachable
        node = null;
        return false;
    }

    // gets neighbours for a node, but with extra filters
    private void GenerateFrontierNodes(PathNode parent, Vector2Int target)
    {
        foreach (PathNode newNode in parent.GenerateNeighbours(target))
        {
            // check for obstacles and already calculated nodes
            if (ignoredPositions.Contains(newNode.Position))
                continue;

            // go through the frontier list and check if there's a node with one of the positions we iterate through
            var node = frontier.FirstOrDefault(a => a.Position == newNode.Position);
            if (node is null) // this is a new, unique node, add it to the queue
                frontier.Add(newNode);

            // this position is present in the queue, but the path we're taking at the moment is shorter that the one recorded previously
            else if (newNode.TraverseDistance < node.TraverseDistance)
            {
                frontier.Remove(node);
                frontier.Add(newNode);
            }
        }

    }
}
