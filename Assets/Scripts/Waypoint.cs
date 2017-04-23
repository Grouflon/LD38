using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public GameObject waypointPrefab;
    public GameObject pathSegmentPrefab;

    public int index;

    public delegate void WaypointAction(Waypoint _w);
    public event WaypointAction Moved;

    public void SetPosition(Vector2 _p)
    {
        if (_p == m_position)
            return;

        m_position = _p;
        if (Moved != null) Moved(this);
    }

    public Vector2 GetPosition()
    {
        return m_position;
    }

    public void SetNext(Waypoint _w)
    {
        if (_w == m_next)
            return;

        if (m_next)
        {
            m_next.Moved -= OnWaypointMoved;
        }

        m_next = _w;

        if (ready)
            ComputeNextPath();

        if (m_next)
        {
            m_next.Moved += OnWaypointMoved;
        }
    }

    public void SetPrevious(Waypoint _w)
    {
        if (_w == m_previous)
            return;

        m_previous = _w;
    }

    public Waypoint GetNext()
    {
        return m_next;
    }

    public Waypoint GetPrevious()
    {
        return m_previous;
    }

    public ReadOnlyCollection<Vector2> GetPathToNext()
    {
        return m_nextPath.AsReadOnly();
    }

    public Waypoint()
    {
        m_nextPath = new List<Vector2>();
    }

    void Start ()
	{
        m_lvl = FindObjectOfType<Level>();
        m_text = GetComponentInChildren<TextMesh>();

        m_lvl.pathfindRecomputed += OnPathfindRecomputed;

        ComputeNextPath();
        ready = true;
    }

    void Update ()
	{
        m_text.text = index.ToString();
        transform.position = new Vector3(m_position.x, 0.0f, m_position.y);
	}

    void ComputeNextPath()
    {
        m_nextPath.Clear();

        if (m_next == null)
        {
            m_nextPath.Add(GetPosition());
            return;
        }

        // REMOVE ALREADY TRAVERSED EDGES FROM GRAPH
        {
            List<Waypoint> crossedList = new List<Waypoint>();
            Waypoint w = m_next;
            while (w != null && crossedList.Find(x => (x == w)) == null)
            {
                crossedList.Add(w);
                for (int i = 1; i < w.m_nextPath.Count; ++i)
                {
                    m_lvl.GetPathfindNodeAt(w.m_nextPath[i]).RemoveEdge(m_lvl.GetPathfindNodeAt(w.m_nextPath[i - 1]));
                }
                w = w.m_next;
            }
            w = m_previous;
            while (w != null && crossedList.Find(x => (x == w)) == null)
            {
                crossedList.Add(w);
                for (int i = 1; i < w.m_nextPath.Count; ++i)
                {
                    m_lvl.GetPathfindNodeAt(w.m_nextPath[i]).RemoveEdge(m_lvl.GetPathfindNodeAt(w.m_nextPath[i - 1]));
                }
                w = w.m_previous;
            }
        }

        Pathfinding.Node startNode = m_lvl.GetPathfindNodeAt(GetPosition());
        Pathfinding.Node endNode = m_lvl.GetPathfindNodeAt(m_next.GetPosition());
        List<Pathfinding.Node> path;
        Pathfinding.AStar(startNode, endNode, out path);
        for (int i = 0; i < path.Count - 1; ++i)
        {
            m_nextPath.Add(new Vector2(path[i].position.x, path[i].position.z));

            GameObject pathSegment = Instantiate(pathSegmentPrefab);
            pathSegment.transform.position = path[i].position;
            pathSegment.transform.LookAt(path[i + 1].position);
        }

        m_lvl.ResetPathfindGraph();
    }

    void OnWaypointMoved(Waypoint _w)
    {
        if (_w == m_next && ready)
        {
            ComputeNextPath();
        }
    }

    void OnPathfindRecomputed()
    {
        if (m_next && ready)
        {
            ComputeNextPath();
        }
    }

    Level m_lvl;
    TextMesh m_text;
    Waypoint m_next;
    Waypoint m_previous;
    Vector2 m_position;

    List<Vector2> m_nextPath;
    List<GameObject> m_nextPathView;
    bool ready = false;
}
