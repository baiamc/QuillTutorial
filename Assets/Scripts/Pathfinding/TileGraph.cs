using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding
{
    public class TileGraph
    {
        Dictionary<Tile, Node<Tile>> _nodes;

        public TileGraph(World world)
        {
            _nodes = new Dictionary<Tile, Node<Tile>>();

            foreach (var tile in world.Tiles())
            {
                if (tile.MovementCost > 0)
                {
                    Node<Tile> node = new Node<Tile>();
                    node.Data = tile;
                    _nodes.Add(tile, node);

                    Debug.DrawLine(new Vector3(tile.X - 0.1f, tile.Y - 0.25f, 0), new Vector3(tile.X + 0.1f, tile.Y + 0.25f, 0), Color.red, 999f);
                }
            }

            foreach (var node in _nodes.Values)
            {
                var list = new List<Edge<Tile>>();
                var neighbours = node.Data.GetNeighbors(true);
                for (int i = 0; i < neighbours.Length; i++)
                {
                    if (neighbours[i] == null || neighbours[i].MovementCost == 0)
                    {
                        continue;
                    }

                    var edge = new Edge<Tile>
                    {
                        Cost = neighbours[i].MovementCost,
                        Node = _nodes[neighbours[i]]
                    };
                    list.Add(edge);

                    Debug.DrawLine(new Vector3(node.Data.X, node.Data.Y, 0), new Vector3(neighbours[i].X, neighbours[i].Y, 0), Color.green, 999f);
                }

                node.Edges = list.ToArray();
            }
        }
    }
}
