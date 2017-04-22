using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	void Start ()
	{
        m_cam = Camera.main;
        m_gamePlane = new Plane(Vector3.up, Vector3.zero);
    }

    void Update ()
	{
        GridActor a = FindObjectOfType<GridActor>();
        if (Input.GetMouseButtonDown(0))
        {
            a.SetPosition(GetPosUnderMouse());
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
            Debug.LogError("Failed to get pos under mouse");

        return Vector2.zero;
    }

    Camera m_cam;
    Plane m_gamePlane;
}
