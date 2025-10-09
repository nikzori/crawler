// Ripped from here: https://habr.com/ru/articles/513158/
// Unironically go run it through google translate and do it step-by-step if you're a surface-level dumbass like I am. 

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

internal readonly struct PathNode
{
    public Vector2Int Position { get; }
    public double TraverseDistance { get; }
    public double EstimatedTotalCost { get; }

    public PathNode(Vector2Int Position, Vector2Int target, double TraverseDistance)
    {
        this.Position = Position;
        this.TraverseDistance = TraverseDistance;
        double heuristicDistance = (Position - target).DistanceEstimate();
        this.EstimatedTotalCost = TraverseDistance + heuristicDistance;
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
    public static void Fill(this PathNode[] buffer, PathNode parent, Vector2Int target)
    {
        int i = 0;
        foreach ((Vector2Int position, double cost) in NeighboursTemplate)
        {
            Vector2Int nodePosition = position + parent.Position;
            double traverseDistance = parent.TraverseDistance + cost;
            buffer[i++] = new PathNode(nodePosition, target, traverseDistance);
        }
    }
}

public class Path
{
    private const int MaxNeighbours = 8;
    private readonly PathNode[] neighbours = new PathNode[MaxNeighbours];

    private readonly int maxSteps;
    private readonly IBinaryHeap<Vector2Int, PathNode> frontier;
    private readonly HashSet<Vector2Int> ignoredPositions;
    private readonly IList<Vector2Int> output;
    private readonly Dictionary<Vector2Int, Vector2Int> links;

    public Path(int maxSteps = int.MaxValue, int initialCapacity = 0)
    {
        this.maxSteps = maxSteps;

        var comparer = Comparer<PathNode>.Create((a, b) => b.EstimatedTotalCost.CompareTo(a.EstimatedTotalCost));
        frontier = new BinaryHeap(comparer);
        ignoredPositions = new HashSet<Vector2Int>(initialCapacity);
        output = new List<Vector2Int>(initialCapacity);
        links = new Dictionary<Vector2Int, Vector2Int>(initialCapacity);
    }

    public bool Calculate(Vector2Int start, Vector2Int target, IReadOnlyCollection<Vector2Int> obstacles,
            out IReadOnlyCollection<Vector2Int> path)
    {
        if (!GenerateNodes(start, target, obstacles))
        {
            path = Array.Empty<Vector2Int>();
            return false;
        }

        output.Clear();
        output.Add(target);

        while (links.TryGetValue(target, out target))
            output.Add(target);
        path = (IReadOnlyCollection<Vector2Int>)output;
        return true;
    }

    public IReadOnlyCollection<Vector2Int> Calculate(Vector2Int start, Vector2Int target, IReadOnlyCollection<Vector2Int> obstacles)
    {
        if (!GenerateNodes(start, target, obstacles))
            return Array.Empty<Vector2Int>();

        output.Clear();
        output.Add(target);

        while (links.TryGetValue(target, out target))
            output.Add(target);

        return output.AsReadOnly();
    }

    private bool GenerateNodes(Vector2Int start, Vector2Int target, IReadOnlyCollection<Vector2Int> obstacles)
    {
        frontier.Clear();
        ignoredPositions.Clear();
        links.Clear();

        // starting point
        frontier.Enqueue(new PathNode(start, target, 0));
        ignoredPositions.UnionWith(obstacles);
        var step = 0;

        while (frontier.Count > 0 && step++ <= maxSteps)
        {
            // get last node in binary heap
            PathNode current = frontier.Dequeue();
            ignoredPositions.Add(current.Position);

            if (current.Position.Equals(target)) return true;

            // haven't reached the target; get neighbours for this node, repeat
            GenerateFrontierNodes(current, target);
        }

        // no nodes left, didn't find the target; target position unreachable
        return false;
    }

    // gets neighbours for a node, but with extra filters
    private void GenerateFrontierNodes(PathNode parent, Vector2Int target)
    {
        neighbours.Fill(parent, target);
        foreach (var neighbour in neighbours)
        {
            // check for obstacles and already calculated nodes
            if (ignoredPositions.Contains(neighbour.Position))
                continue;

            // Node is not present in the queue
            if (!frontier.TryGet(neighbour.Position, out PathNode existingNode))
            {
                frontier.Enqueue(neighbour);
                links[neighbour.Position] = parent.Position;
            }

            // Node is present in queue and new optimal path is detected
            else if (neighbour.TraverseDistance < existingNode.TraverseDistance)
            {
                frontier.Modify(neighbour);
                links[neighbour.Position] = parent.Position;
            }
        }

    }

    // using binary heap instead of a List to speed up the node sorting (log(n) speed, baby)

    internal class BinaryHeap : IBinaryHeap<Vector2Int, PathNode>
    {
        private readonly IDictionary<Vector2Int, int> map;
        private readonly IList<PathNode> collection;
        private readonly IComparer<PathNode> comparer;

        public BinaryHeap(IComparer<PathNode> comparer)
        {
            this.comparer = comparer;
            collection = new List<PathNode>();
            map = new Dictionary<Vector2Int, int>();
        }

        public int Count => collection.Count;

        public void Clear()
        {
            collection.Clear();
            map.Clear();
        }

        public void Enqueue(PathNode item)
        {
            collection.Add(item);
            int i = collection.Count - 1;
            map[item.Position] = i;

            // Sort nodes from bottom to top
            while (i > 0)
            {
                int j = (i - 1) / 2;
                if (comparer.Compare(collection[i], collection[j]) <= 0)
                    break;

                Swap(i, j);
                i = j;
            }
        }
        private void Swap(int i, int j)
        {
            PathNode temp = collection[i];
            collection[i] = collection[j];
            collection[j] = temp;
            map[collection[i].Position] = i;
            map[collection[j].Position] = j;
        }

        public PathNode Dequeue()
        {
            if (collection.Count == 0) return default;

            PathNode result = collection.First();
            RemoveRoot();
            map.Remove(result.Position);
            return result;
        }

        private void RemoveRoot()
        {
            collection[0] = collection.Last();
            map[collection[0].Position] = 0;
            collection.RemoveAt(collection.Count - 1);

            // sort nodes
            var i = 0;
            while (true)
            {
                int largest = LargestIndex(i);
                if (largest == i)
                    return;

                Swap(i, largest);
                i = largest;
            }
        }
        private int LargestIndex(int i)
        {
            int leftIndex = 2 * i + 1;
            int rightIndex = 2 * i + 2;
            int largest = i;

            if (leftIndex < collection.Count && comparer.Compare(collection[leftIndex], collection[largest]) > 0)
                largest = leftIndex;
            if (rightIndex < collection.Count && comparer.Compare(collection[rightIndex], collection[largest]) > 0)
                largest = rightIndex;

            return largest;

        }

        public bool TryGet(Vector2Int key, out PathNode value)
        {
            if (!map.TryGetValue(key, out int index))
            {
                value = default;
                return false;
            }

            value = collection[index];
            return true;
        }

        public void Modify(PathNode value)
        {
            if (!map.TryGetValue(value.Position, out int index))
                throw new KeyNotFoundException(nameof(value));

            collection[index] = value;

        }
    }


    internal interface IBinaryHeap<in TKey, T>
    {
        void Enqueue(T item);
        T Dequeue();
        void Clear();
        bool TryGet(TKey key, out T value);
        void Modify(T value);
        int Count { get; }

    }
}



