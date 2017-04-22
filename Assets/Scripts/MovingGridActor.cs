using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGridActor : GridActor
{

    protected override void Start ()
	{
        base.Start();

        m_lvl = FindObjectOfType<Level>();
        m_schedule = new Schedule(m_lvl);
        m_schedule.firstWaypoint.position = GetPosition();

        Schedule.Waypoint w1 = new Schedule.Waypoint(new Vector2(2, 2));
        Schedule.Waypoint w2 = new Schedule.Waypoint(new Vector2(5, 5));
        Schedule.Waypoint w3 = new Schedule.Waypoint(new Vector2(6, 5));

        m_schedule.firstWaypoint.next = w1;
        w1.next = w2;
        w2.next = w3;
        w3.next = m_schedule.firstWaypoint;

        m_schedule.ComputePath();
    }
	
	protected override void Update ()
	{
        base.Update();

        Vector3 offset = new Vector3(0.5f, 0.15f, 0.5f);

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
    }

    Schedule m_schedule;
}
