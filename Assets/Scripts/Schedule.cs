using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Schedule
{
    public class Waypoint
    {

        public Waypoint()
        {
        }

        public Waypoint(Vector2 _position)
        {
            position = _position;
        }

        public Vector2 position;
        public Waypoint next;
        public Waypoint previous;
        public int pathIndex = -1;
    }

    public Waypoint firstWaypoint;

    public Schedule(Level _level)
    {
        firstWaypoint = new Waypoint();
        m_lvl = _level;
        m_path = new List<Vector2>();

        m_lvl.pathfindRecomputed += OnPathfindRecomputed;
    }

    public ReadOnlyCollection<Vector2> GetPath()
    {
        return m_path.AsReadOnly();
    }

    public void ComputePath()
    {
        m_path.Clear();
        List<Waypoint> crossedList = new List<Waypoint>();
        Waypoint currentWaypoint = firstWaypoint;
        m_path.Add(firstWaypoint.position);
        while (currentWaypoint != null && crossedList.Find(x => (x == currentWaypoint)) == null)
        {
            currentWaypoint.pathIndex = m_path.Count;
            if (currentWaypoint.next != null)
            {
                Pathfinding.Node startNode = m_lvl.GetPathfindNodeAt(currentWaypoint.position);
                Pathfinding.Node endNode = m_lvl.GetPathfindNodeAt(currentWaypoint.next.position);
                List<Pathfinding.Node> path;
                Pathfinding.AStar(startNode, endNode, out path);
                for (int i = 1; i < path.Count; ++i)
                {
                    path[i].RemoveEdge(path[i - 1]);
                    m_path.Add(new Vector2(path[i].position.x, path[i].position.z));
                }
            }

            crossedList.Add(currentWaypoint);
            currentWaypoint = currentWaypoint.next;
        }

        // HACKS: too tired to fix the algorithm right now,
        firstWaypoint.pathIndex = 0;
        if (currentWaypoint != null)
            m_path.RemoveAt(m_path.Count - 1);

        m_doNotRecompute = true;
        m_lvl.ResetPathfindGraph();
        m_doNotRecompute = false;
    }

    void OnPathfindRecomputed()
    {
        if (!m_doNotRecompute)
            ComputePath();
    }

    Level m_lvl;
    List<Vector2> m_path;
    bool m_doNotRecompute = false;
}
