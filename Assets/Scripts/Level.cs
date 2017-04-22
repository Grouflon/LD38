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
        if (_x >= width || _y >= height)
            return -1;

        return _y * width + _x;
    }

	void Start ()
	{
		
	}
	
	void Update ()
	{
		if (m_groundChanged)
            GenerateGround();
    }

    private void OnValidate()
    {
        m_groundChanged = true;
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
        m_groundChanged = false;
    }

    GameObject[] m_groundTiles;
    bool m_groundChanged = false;
}
