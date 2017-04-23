using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Pathfinding
{
    public enum Heuristic
    {
        Manhattan
    }

    struct OpenNode
    {
        public Node node;
        public float priority;
    }
    static public bool AStar(Node _start, Node _dest, out List<Node> _result, Heuristic _heuristic = Pathfinding.Heuristic.Manhattan)
    {
        _result = new List<Node>();
        List<OpenNode> openList = new List<OpenNode>();
        Dictionary<Node, float> costList = new Dictionary<Node, float>();
        Dictionary<Node, Node> originList = new Dictionary<Node, Node>();

        originList[_start] = null;
        OpenNode o = new OpenNode();
        o.node = _start;
        o.priority = 0;
        openList.Add(o);
        costList[_start] = 0.0f;

        while (openList.Count > 0)
        {
            Node current = openList[0].node;
            openList.RemoveAt(0);

            if (current == _dest)
                break;

            foreach (KeyValuePair<Node, float> pair in current.neighbours)
            {
                Node next = pair.Key;
                float nextCost = pair.Value;
                float cost = costList[current] + nextCost;
                if (!costList.ContainsKey(next) || cost < costList[next])
                {
                    costList[next] = cost;
                    float priority = cost;
                    switch (_heuristic)
                    {
                        case Heuristic.Manhattan:
                            {
                                priority += Mathf.Abs(_dest.position.x - next.position.x);
                                priority += Mathf.Abs(_dest.position.y - next.position.y);
                                priority += Mathf.Abs(_dest.position.z - next.position.z);
                            }
                            break;
                    }

                    
                    int i = 0;
                    for (i = 0; i < openList.Count; ++i)
                    {
                        if (priority < openList[i].priority)
                            break;
                    }
                    o.node = next;
                    o.priority = priority;
                    openList.Insert(i, o);
                    originList[next] = current;
                }
            }
        }

        if (!originList.ContainsKey(_dest))
            return false;

        Node currentNode = _dest;
        do
        {
            _result.Insert(0, currentNode);
            currentNode = originList[currentNode];
        }
        while (currentNode != null);
        return true;
    }

    public class Node
    {
        public Node()
        {
            neighbours = new Dictionary<Node, float>();
        }

        public void AddEdge(Node _target, float _cost, bool _reciprocal = true)
        {
            if (_target == this)
            {
                Debug.LogError("Node Can't link with itself");
                return;
            }

            if (neighbours.ContainsKey(_target))
            {
                Debug.LogError("Node Can't link twice with same node");
                return;
            }

            if (_reciprocal && _target.neighbours.ContainsKey(this))
            {
                Debug.LogError("Node Can't link with same node twice");
                return;
            }

            neighbours.Add(_target, _cost);
            if (_reciprocal) _target.neighbours.Add(this, _cost);
        }

        public void RemoveEdge(Node _target, bool _reciprocal = true)
        {
            neighbours.Remove(_target);
            if (_reciprocal) _target.neighbours.Remove(this);
        }

        public Vector3 position;
        public Dictionary<Node, float> neighbours; 
    }
}
