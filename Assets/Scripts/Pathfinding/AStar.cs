using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding
{
    public class AStar
    {
        Queue<Tile> _path;

        public AStar(World world, Tile start, Tile end)
        {

        }

        public Tile GetNextTile()
        {
            return _path.Dequeue();
        }
    }
}
