using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridActor : MonoBehaviour
{
    public void SetPosition(int _x, int _y)
    {
        SetPosition(new Vector2(_x, _y));
    }

    public void SetPosition(Vector2 _pos)
    {
        if (m_lvl.MoveActor(this, _pos))
            m_position = _pos;
        else
            Debug.LogError("Failed to move actor to pos X:" + _pos.x + " Y:" + _pos.y);
    }

    public Vector2 GetPosition()
    {
        return m_position;
    }

	protected virtual void Awake ()
	{
        m_lvl = FindObjectOfType<Level>();
        m_position = new Vector2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));
	}

    protected virtual void Start()
    {
        /*
        Transform dummy = transform.FindChild("_dummy");
        if (dummy != null)
            Destroy(dummy.gameObject);
        */
    }

    protected virtual void Update ()
	{
        transform.position = new Vector3(m_position.x + 0.5f, 0.0f, m_position.y + 0.5f);
	}

    protected Level m_lvl;
    Vector2 m_position;
}
