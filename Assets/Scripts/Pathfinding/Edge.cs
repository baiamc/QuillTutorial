using UnityEngine;
using System.Collections;

namespace Pathfinding
{
    public class Edge<T>
    {
        public float Cost;
        public Node<T> Node;
    }
}
