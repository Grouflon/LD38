using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Level : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject groundTilePrefab;
    public delegate void LevelAction();
    public event LevelAction pathfindRecomputed;

    [Header("Debug")]
    public bool displayPathfindGraph = false;

    public int GetIndex(int _x, int _y)
    {
        if (_x < 0 || _y < 0 || _x >= width || _y >= height)
            return -1;

        return _y * width + _x;
    }

    public int GetIndex(Vector2 _position)
    {
        return GetIndex(Mathf.FloorToInt(_position.x), Mathf.FloorToInt(_position.y));
    }

    public Vector2 GetPosition(int _index)
    {
        return new Vector2(_index % width, _index / width);
    }

    public Pathfinding.Node GetPathfindNodeAt(Vector2 _position)
    {
        int i = GetIndex(_position);

        if (i < 0)
            return null;

        return m_pathfindGraph[i];
    }

    public bool MoveActor(GridActor _actor, Vector2 _destination)
    {
        if (_destination == _actor.GetPosition())
            return true;

        int destIndex = GetIndex(_destination);
        if (destIndex < 0 || m_occupationMap[destIndex] != null)
            return false;

        int actorIndex = GetIndex(_actor.GetPosition());
        if (actorIndex != -1)
            m_occupationMap[actorIndex] = null;

        m_occupationMap[destIndex] = _actor;
        return true;
    }

    public void PushPathfindGraph()
    {

    }

    public void PopPathfindGraph()
    {

    }

	void Awake ()
	{
        ResetOccupationMap();
        GenerateGround();
        ResetPathfindGraph();
	}
	
	void Update ()
	{
		if (m_sizeChanged)
        {
            GenerateGround();
            ResetOccupationMap();
            ResetPathfindGraph();
        }
    }

    private void OnDrawGizmos()
    {
        if (displayPathfindGraph)
        {
            Vector3 offset = new Vector3(0.0f, 0.1f, 0.0f);
            foreach (Pathfinding.Node node in m_pathfindGraph)
            {
                Gizmos.DrawWireSphere(node.position + offset, 0.25f);
                foreach (KeyValuePair<Pathfinding.Node, float> pair in node.neighbours)
                {
                    Gizmos.DrawLine(node.position + offset, pair.Key.position + offset);
                }
            }
        }
    }

    private void OnValidate()
    {
        m_sizeChanged = true;
    }

    void GenerateGround()
    {
        if (m_groundTiles != null)
        {
            foreach(GameObject g in m_groundTiles)
            {
                DestroyImmediate(g);
            }
            m_groundTiles = null;
        }

        m_groundTiles = new GameObject[width * height];
        for (int i = 0; i < width * height; ++i)
        {
            Vector2 p = GetPosition(i);
            m_groundTiles[i] = Instantiate(groundTilePrefab, new Vector3(p.x, 0.0f, p.y), Quaternion.identity, transform);
            m_groundTiles[i].hideFlags = HideFlags.DontSaveInEditor;
        }
        m_sizeChanged = false;
    }

    void ResetOccupationMap()
    {
        if (m_occupationMap != null)
            m_occupationMap = null;

        m_occupationMap = new GridActor[width * height];
        GridActor[] actors = FindObjectsOfType<GridActor>();
        foreach(GridActor actor in actors)
        {
            int index = GetIndex(actor.GetPosition());
            if (m_occupationMap[index] != null)
            {
                Debug.LogError("two actors on the same tile");
                continue;
            }
            m_occupationMap[index] = actor;
        }
    }

    public void ResetPathfindGraph(bool emitEvent = false)
    {
        if (m_pathfindGraph != null)
        {
            m_pathfindGraph = null;
        }

        m_pathfindGraph = new Pathfinding.Node[width * height];
        for (int i = 0; i < width * height; ++i)
        {
            Vector2 p = GetPosition(i);
            m_pathfindGraph[i] = new Pathfinding.Node();
            m_pathfindGraph[i].position = new Vector3(p.x, 0.0f, p.y);
        }

        for (int i = 0; i < width * height; ++i)
        {
            Pathfinding.Node node = m_pathfindGraph[i];
            Vector2 p = GetPosition(i);
            int[] neighbours = new int[4];
            neighbours[0] = GetIndex((int)p.x + 1, (int)p.y);
            neighbours[1] = GetIndex((int)p.x, (int)p.y + 1);
            neighbours[2] = GetIndex((int)p.x - 1, (int)p.y);
            neighbours[3] = GetIndex((int)p.x, (int)p.y - 1);

            for (int j = 0; j < 4; ++j)
            {
                if (neighbours[j] < 0)
                    continue;

                node.AddEdge(m_pathfindGraph[neighbours[j]], 1.0f, false);
            }
        }

        if (emitEvent && pathfindRecomputed != null) pathfindRecomputed();
    }

    GridActor[] m_occupationMap;
    GameObject[] m_groundTiles;
    Pathfinding.Node[] m_pathfindGraph;
    bool m_sizeChanged = false;
}
