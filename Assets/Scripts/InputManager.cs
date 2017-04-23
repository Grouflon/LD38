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
                Vector3 offset = new Vector3(0.0f, 0.2f, 0.0f);
                for (int i = 0; i < path.Count; ++i)
                {
                    Debug.DrawLine(path[i].position + offset, path[i].position + offset + new Vector3(0.0f, 0.5f, 0.0f), Color.red);
                    if (i > 0)
                        Debug.DrawLine(path[i].position + offset, path[i - 1].position + offset, Color.red);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = m_cam.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0.0f));
            RaycastHit hit;
            bool deselect = true;
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                MovingActorView view = hit.collider.GetComponent<MovingActorView>();
                if (view != null && view.target == m_selectedActor)
                {
                    deselect = false;
                }
            }

            if (deselect)
            {
                m_selectedActor = null;
            }
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

    public void OnActorPressed(MovingGridActor _actor)
    {
        m_selectedActor = _actor;
    }

    MovingGridActor m_selectedActor;

    Level m_lvl;
    Camera m_cam;
    Plane m_gamePlane;
}
