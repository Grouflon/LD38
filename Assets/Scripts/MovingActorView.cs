using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingActorView : MonoBehaviour
{
    public MovingGridActor target;

	void Start ()
	{
        m_game = FindObjectOfType<Game>();
        m_input = FindObjectOfType<InputManager>();
        target.OnTick += OnTick;
	}
	
	void Update ()
	{
        m_timer += Time.deltaTime;
		while (m_timer > m_game.cycleDuration)
        {
            m_timer -= m_game.cycleDuration;
        }

        transform.position = Vector3.Lerp(new Vector3(m_previousPosition.x, 0.0f, m_previousPosition.y), new Vector3(m_currentPosition.x, 0.0f, m_currentPosition.y), m_timer / m_game.cycleDuration);
        transform.position = transform.position;
	}

    void OnTick()
    {
        m_previousPosition = m_currentPosition;
        m_currentPosition = target.GetPosition();
    }

    private void OnMouseUpAsButton()
    {
        m_input.OnActorPressed(target);
    }

    float m_timer;
    Game m_game;
    InputManager m_input;
    Vector2 m_previousPosition;
    Vector2 m_currentPosition;
}
