using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Schedule : MonoBehaviour
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

    public GameObject waypointPrefab;
    public GameObject pathSegmentPrefab;

    void Awake()
    {
        firstWaypoint = new Waypoint();
        m_lvl = FindObjectOfType<Level>();
        m_path = new List<Vector2>();
        m_view = new GameObject();
        m_view.name = "ScheduleView";

        m_lvl.pathfindRecomputed += OnPathfindRecomputed;
    }

    public ReadOnlyCollection<Vector2> GetPath()
    {
        return m_path.AsReadOnly();
    }

    public void ComputePath()
    {
        int waypointIndex = 0;
        m_path.Clear();
        List<Waypoint> crossedList = new List<Waypoint>();
        Waypoint currentWaypoint = firstWaypoint;
        m_path.Add(firstWaypoint.position);
        while (currentWaypoint != null && crossedList.Find(x => (x == currentWaypoint)) == null)
        {
            currentWaypoint.pathIndex = m_path.Count - 1;
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

            // CREATE WAYPOINTS
            GameObject waypoint = Instantiate(waypointPrefab);
            waypoint.transform.parent = m_view.transform;
            TextMesh text = waypoint.GetComponentInChildren<TextMesh>();
            text.text = waypointIndex.ToString();
            waypoint.transform.position = new Vector3(currentWaypoint.position.x, 0.0f, currentWaypoint.position.y);
            ++waypointIndex;

            crossedList.Add(currentWaypoint);
            currentWaypoint = currentWaypoint.next;
        }

        // HACKS: too tired to fix the algorithm right now,
        if (currentWaypoint != null)
            m_path.RemoveAt(m_path.Count - 1);

        // CREATE SEGMENTS
        for (int i = 0; i < m_path.Count; ++i)
        {
            if (i == m_path.Count - 1 && currentWaypoint == null)
                break;

            GameObject pathSegment = Instantiate(pathSegmentPrefab);
            pathSegment.transform.parent = m_view.transform;
            pathSegment.transform.position = new Vector3(m_path[i].x, 0.0f, m_path[i].y);
            pathSegment.transform.LookAt(i == m_path.Count - 1 ? new Vector3(currentWaypoint.position.x, 0.0f, currentWaypoint.position.y) : new Vector3(m_path[(i + 1) % m_path.Count].x, 0.0f, m_path[(i + 1) % m_path.Count].y));
        }
        

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
    GameObject m_view;
}
