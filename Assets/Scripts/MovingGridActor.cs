using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGridActor : GridActor
{
    public delegate void TickAction();
    public event TickAction OnTick;

    public MovingActorView viewPrefab;
    public Waypoint waypointPrefab;

    protected override void Start ()
	{
        base.Start();

        m_game = FindObjectOfType<Game>();
        m_lvl = FindObjectOfType<Level>();
        m_rootWaypoint = Instantiate(waypointPrefab);
        m_rootWaypoint.SetPosition(GetPosition());
        m_rootWaypoint.index = 0;

        Waypoint w1 = Instantiate(waypointPrefab);
        w1.SetPosition(new Vector2(2, 2));
        m_rootWaypoint.SetNext(w1);
        w1.SetPrevious(m_rootWaypoint);
        w1.index = 1;

        Waypoint w2 = Instantiate(waypointPrefab);
        w2.SetPosition(new Vector2(5, 5));
        w1.SetNext(w2);
        w2.SetPrevious(w1);
        w2.index = 2;
        w2.SetNext(w1);

        Waypoint w3 = Instantiate(waypointPrefab);
        w3.SetPosition(new Vector2(6, 2));
        //w2.SetNext(w3);
        w3.SetPrevious(w2);
        w3.index = 3;

        w3.SetNext(m_rootWaypoint);

        m_currentWaypoint = m_rootWaypoint;

        MovingActorView view = Instantiate(viewPrefab, transform);
        view.target = this;
    }
	
	protected override void Update ()
	{
        base.Update();

        m_timer += Time.deltaTime;
        while(m_timer > m_game.cycleDuration)
        {
            m_timer -= m_game.cycleDuration;
            Tick();
        }
    }

    void Tick()
    {
        if (m_direction == 1 && m_pathIncrement == m_currentWaypoint.GetPathToNext().Count - 1 && m_currentWaypoint.GetNext() == null)
            m_direction = -1;
        else if (m_direction == -1 && m_pathIncrement == 0 && m_currentWaypoint.GetPrevious() == null)
            m_direction = 1;


        do
        {
            if (m_direction == 1)
            {
                ++m_pathIncrement;
                if (m_pathIncrement == m_currentWaypoint.GetPathToNext().Count)
                {
                    if (m_currentWaypoint.GetNext() != null)
                    {
                        m_currentWaypoint = m_currentWaypoint.GetNext();
                        m_pathIncrement = 0;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (m_direction == -1)
            {
                --m_pathIncrement;
                if (m_pathIncrement == -1)
                {
                    if (m_currentWaypoint.GetPrevious() != null)
                    {
                        m_currentWaypoint = m_currentWaypoint.GetPrevious();
                        m_pathIncrement = m_currentWaypoint.GetPathToNext().Count - 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            SetPosition(m_currentWaypoint.GetPathToNext()[m_pathIncrement]);

        } while (false);

        if (OnTick != null) OnTick();
    }

    float m_timer;
    Waypoint m_currentWaypoint;
    int m_pathIncrement;
    int m_direction = 1;
    Waypoint m_rootWaypoint;
    Game m_game;
}
