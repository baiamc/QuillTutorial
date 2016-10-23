using UnityEngine;
using System.Collections;

namespace Pathfinding
{
    public class Node<T>
    {
        public T Data;
        public Edge<T>[] Edges; // Nodes leading out from this node
    }
}
