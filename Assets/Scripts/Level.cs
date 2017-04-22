using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Level : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject groundTilePrefab;

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

	void Start ()
	{
        ResetOccupationMap();
        GenerateGround();
	}
	
	void Update ()
	{
		if (m_sizeChanged)
        {
            GenerateGround();
            ResetOccupationMap();
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
            m_groundTiles[i] = Instantiate(groundTilePrefab, new Vector3(i % width, 0.0f, i / width), Quaternion.identity, transform);
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

    GridActor[] m_occupationMap;
    GameObject[] m_groundTiles;
    bool m_sizeChanged = false;
}
