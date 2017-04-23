using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGridActor : GridActor
{
    public delegate void TickAction();
    public event TickAction OnTick;

    public MovingActorView viewPrefab;

    protected override void Start ()
	{
        base.Start();

        m_game = FindObjectOfType<Game>();
        m_lvl = FindObjectOfType<Level>();
        m_schedule = GetComponent<Schedule>();
        m_schedule.firstWaypoint.position = GetPosition();

        Schedule.Waypoint w1 = new Schedule.Waypoint(new Vector2(2, 2));
        Schedule.Waypoint w2 = new Schedule.Waypoint(new Vector2(5, 5));
        Schedule.Waypoint w3 = new Schedule.Waypoint(new Vector2(6, 5));

        m_schedule.firstWaypoint.next = w1; w1.previous = m_schedule.firstWaypoint;
        w1.next = w2; w2.previous = w1;
        w2.next = w3; w3.previous = w2;
        w3.next = w1;

        m_schedule.ComputePath();

        m_currentWaypoint = m_schedule.firstWaypoint;

        MovingActorView view = Instantiate(viewPrefab, transform);
        view.target = this;
    }
	
	protected override void Update ()
	{
        base.Update();

        Vector3 offset = new Vector3(0.0f, 0.15f, 0.0f);

        List < Schedule.Waypoint> crossedList = new List<Schedule.Waypoint>();
        Schedule.Waypoint currentWaypoint = m_schedule.firstWaypoint;
        while (currentWaypoint!= null && crossedList.Find(x => (x == currentWaypoint)) == null)
        {
            Debug.DrawLine(new Vector3(currentWaypoint.position.x, 0.0f, currentWaypoint.position.y) + offset, new Vector3(currentWaypoint.position.x, 0.0f, currentWaypoint.position.y) + offset + new Vector3(0.0f, 0.5f, 0.0f), Color.yellow);

            crossedList.Add(currentWaypoint);
            currentWaypoint = currentWaypoint.next;
        }

        for (int i = 1; i < m_schedule.GetPath().Count; ++i)
        {
            Vector2 pos0 = m_schedule.GetPath()[i - 1];
            Vector2 pos1 = m_schedule.GetPath()[i];
            Debug.DrawLine(new Vector3(pos0.x, 0.0f, pos0.y) + offset, new Vector3(pos1.x, 0.0f, pos1.y) + offset, Color.yellow);
        }

        m_timer += Time.deltaTime;
        while(m_timer > m_game.cycleDuration)
        {
            m_timer -= m_game.cycleDuration;
            Tick();
        }
    }

    void Tick()
    {
        int currentPathIndex = 0;
        if (m_direction == 1)
        {
            ++m_pathIncrement;
            currentPathIndex = (m_currentWaypoint.pathIndex + m_pathIncrement);
            if (currentPathIndex == m_currentWaypoint.next.pathIndex || currentPathIndex == m_schedule.GetPath().Count)
            {
                m_currentWaypoint = m_currentWaypoint.next;
                currentPathIndex = m_currentWaypoint.pathIndex;
                m_pathIncrement = 0;

                if (m_currentWaypoint.next == null)
                    m_direction = -1;
            }
        }
        else if (m_direction == -1)
        {
            --m_pathIncrement;
            currentPathIndex = m_currentWaypoint.pathIndex + m_pathIncrement;
            if (currentPathIndex == m_currentWaypoint.previous.pathIndex || currentPathIndex == -1)
            {
                m_currentWaypoint = m_currentWaypoint.previous;
                currentPathIndex = m_currentWaypoint.pathIndex;
                m_pathIncrement = 0;

                if (m_currentWaypoint.previous == null)
                    m_direction = 1;
            }
        }
        

        SetPosition(m_schedule.GetPath()[currentPathIndex]);

        if (OnTick != null) OnTick();
    }

    float m_timer;
    Schedule.Waypoint m_currentWaypoint;
    int m_pathIncrement;
    int m_direction = 1;
    Schedule m_schedule;
    Game m_game;
}
