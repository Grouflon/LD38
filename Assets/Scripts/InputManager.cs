using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	void Start ()
	{
        m_lvl = FindObjectOfType<Level>();
        m_cam = Camera.main;
        m_gamePlane = new Plane(Vector3.up, Vector3.zero);
    }

    void Update ()
	{
        Vector2 mousePosition = GetPosUnderMouse();
        GridActor a = FindObjectOfType<GridActor>();

        Pathfinding.Node startNode = m_lvl.GetPathfindNodeAt(a.GetPosition());
        Pathfinding.Node endNode = m_lvl.GetPathfindNodeAt(mousePosition);

        if (startNode != null && endNode != null)
        {
            List<Pathfinding.Node> path;
            if (Pathfinding.AStar(startNode, endNode, out path, Pathfinding.Heuristic.Manhattan))
            {
                Vector3 offset = new Vector3(0.5f, 0.2f, 0.5f);
                for (int i = 0; i < path.Count; ++i)
                {
                    Debug.DrawLine(path[i].position + offset, path[i].position + offset + new Vector3(0.0f, 0.5f, 0.0f), Color.red);
                    if (i > 0)
                        Debug.DrawLine(path[i].position + offset, path[i - 1].position + offset, Color.red);
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            a.SetPosition(mousePosition);
        }
	}

    Vector2 GetPosUnderMouse()
    {
        Ray mouseRay = m_cam.ScreenPointToRay(Input.mousePosition);
        float dist = 0.0f;
        if (m_gamePlane.Raycast(mouseRay, out dist))
        {
            Vector3 point = mouseRay.GetPoint(dist);
            return new Vector2(Mathf.Floor(point.x), Mathf.Floor(point.z));
        }
        else
        {
            //Debug.LogError("Failed to get pos under mouse");
        }

        return Vector2.zero;
    }

    Level m_lvl;
    Camera m_cam;
    Plane m_gamePlane;
}
