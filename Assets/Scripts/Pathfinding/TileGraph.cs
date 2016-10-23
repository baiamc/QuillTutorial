using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding
{
    public class TileGraph
    {
        public Dictionary<Tile, Node<Tile>> Nodes;

        public TileGraph(World world)
        {
            Nodes = new Dictionary<Tile, Node<Tile>>();

            foreach (var tile in world.Tiles())
            {

                Node<Tile> node = new Node<Tile>();
                node.Data = tile;
                Nodes.Add(tile, node);

                Debug.DrawLine(new Vector3(tile.X - 0.1f, tile.Y - 0.25f, 0), new Vector3(tile.X + 0.1f, tile.Y + 0.25f, 0), Color.red, 999f);

            }

            foreach (var node in Nodes.Values)
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
                        Node = Nodes[neighbours[i]]
                    };
                    list.Add(edge);

                    Debug.DrawLine(new Vector3(node.Data.X, node.Data.Y, 0), new Vector3(neighbours[i].X, neighbours[i].Y, 0), Color.green, 999f);
                }

                node.Edges = list.ToArray();
            }
        }
    }
}
