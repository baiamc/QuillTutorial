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
                }
            }

            foreach (var node in _nodes.Values)
            {

            }
        }
    }
}
