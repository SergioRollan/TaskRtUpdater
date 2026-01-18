using TaskRtUpdater.src.Domain.Exceptions;

namespace TaskRtUpdater.src.Domain
{
    public class DirectedAcyclicGraph
    {
        private readonly Dictionary<int, List<int>> _adjacencyList = new();
        private readonly HashSet<(int, int)> _edges = new();

        public void AddNode(int nodeId)
        {
            if (!_adjacencyList.ContainsKey(nodeId))
            {
                _adjacencyList[nodeId] = new List<int>();
            }
        }

        public void AddEdge(int from, int to)
        {
            AddNode(from);
            AddNode(to);

            if (_edges.Contains((from, to)))
            {
                return;
            }

            if (HasCycle(from, to))
            {
                throw new CircularDependencyException(from, to);
            }

            _adjacencyList[from].Add(to);
            _edges.Add((from, to));
        }

        public bool HasEdge(int from, int to)
        {
            return _edges.Contains((from, to));
        }

        public IEnumerable<int> GetDependencies(int nodeId)
        {
            return _adjacencyList.ContainsKey(nodeId) 
                ? _adjacencyList[nodeId].ToList() 
                : Enumerable.Empty<int>();
        }

        public void RemoveEdge(int from, int to)
        {
            if (_adjacencyList.ContainsKey(from))
            {
                _adjacencyList[from].Remove(to);
            }
            _edges.Remove((from, to));
        }

        public void RemoveNode(int nodeId)
        {
            _adjacencyList.Remove(nodeId);
            _edges.RemoveWhere(e => e.Item1 == nodeId || e.Item2 == nodeId);
            
            foreach (var adjList in _adjacencyList.Values)
            {
                adjList.Remove(nodeId);
            }
        }

        private bool HasCycle(int from, int to)
        {
            var visited = new HashSet<int>();
            return DFS(to, from, visited);
        }

        private bool DFS(int current, int target, HashSet<int> visited)
        {
            if (current == target)
            {
                return true;
            }

            if (visited.Contains(current))
            {
                return false;
            }

            visited.Add(current);

            if (_adjacencyList.ContainsKey(current))
            {
                foreach (var neighbor in _adjacencyList[current])
                {
                    if (DFS(neighbor, target, visited))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public List<int> TopologicalSort()
        {
            var inDegree = new Dictionary<int, int>();
            var result = new List<int>();
            var queue = new Queue<int>();

            foreach (var node in _adjacencyList.Keys)
            {
                inDegree[node] = 0;
            }

            foreach (var (from, toList) in _adjacencyList)
            {
                foreach (var to in toList)
                {
                    inDegree[to] = inDegree.GetValueOrDefault(to, 0) + 1;
                }
            }

            foreach (var (node, degree) in inDegree)
            {
                if (degree == 0)
                {
                    queue.Enqueue(node);
                }
            }

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                result.Add(node);

                if (_adjacencyList.ContainsKey(node))
                {
                    foreach (var neighbor in _adjacencyList[node])
                    {
                        inDegree[neighbor]--;
                        if (inDegree[neighbor] == 0)
                        {
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            return result;
        }
    }
}
