using UnityEngine;
using System.Collections.Generic;
using System;

namespace Pathfinding
{
    public class AStar
    {
        static readonly float sqrt2 = Mathf.Sqrt(2);
        Stack<Tile> _path;

        public AStar(World world, Tile tileStart, Tile tileEnd)
        {
            if (world.TileGraph == null)
            {
                world.TileGraph = new TileGraph(world);
            }
            var nodeMap = world.TileGraph.Nodes;

            if (nodeMap.ContainsKey(tileStart) == false)
            {
                Debug.LogError("AStar: Start tile isn't int he list of tiles");
                return;
            }

            if (nodeMap.ContainsKey(tileEnd) == false)
            {
                Debug.LogError("AStar: End tile isn't int he list of tiles");
                return;
            }

            var start = nodeMap[tileStart];
            var goal = nodeMap[tileEnd];


            var closedSet = new List<Node<Tile>>();
            var openSet = new Priority_Queue.SimplePriorityQueue<Node<Tile>>();
            openSet.Enqueue(start, 0);

            var cameFrom = new Dictionary<Node<Tile>, Node<Tile>>();

            var gScore = new Dictionary<Node<Tile>, float>();
            var fScore = new Dictionary<Node<Tile>, float>();

            foreach (var n in nodeMap.Values)
            {
                gScore[n] = Mathf.Infinity;
                fScore[n] = Mathf.Infinity;
            }

            gScore[start] = 0;
            fScore[start] = HeuristicCostEstimate(start, goal);

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();
                if (current == goal)
                {
                    ReconstructPath(cameFrom, current);
                    return;
                }

                closedSet.Add(current);
                foreach (var edge in current.Edges)
                {
                    var neighbor = edge.Node;
                    if (closedSet.Contains(neighbor))
                    {
                        continue; // Neighbor already checked
                    }

                    float tentativeGScore = gScore[current] + DistBetweenNeighbors(current, neighbor) * neighbor.Data.MovementCost;

                    if (openSet.Contains(neighbor) && tentativeGScore > gScore[neighbor])
                    {
                        continue;
                    }

                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, goal);

                    if (openSet.Contains(neighbor) == false)
                    {
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                    else
                    {
                        openSet.UpdatePriority(neighbor, fScore[neighbor]);
                    }

                }
            }

            // If we reached here, then the goal is unreachable.
        }

        private void ReconstructPath(Dictionary<Node<Tile>, Node<Tile>> cameFrom, Node<Tile> current)
        {
            _path = new Stack<Tile>();
            _path.Push(current.Data);

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                _path.Push(current.Data);
            }
        }

        private float DistBetweenNeighbors(Node<Tile> current, Node<Tile> neighbor)
        {
            if (current.Data.IsNeighbour(neighbor.Data))
            {
                return 1f; // Horizontal/vertical
            }

            return sqrt2; // Diagonal
        }

        float HeuristicCostEstimate(Node<Tile> a, Node<Tile> b)
        {
            return Mathf.Sqrt(Mathf.Pow(a.Data.X - b.Data.X, 2) + Mathf.Pow(a.Data.Y - b.Data.Y, 2));
        }

        public Tile Dequeue()
        {
            if (_path == null || _path.Count == 0)
            {
                return null;
            }
            return _path.Pop();
        }

        public int Length()
        {
            return _path.Count;
        }
    }
}
