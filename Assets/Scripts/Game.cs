using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public float cycleDuration = 1.0f;

	void Start ()
	{
	}
	
	void Update ()
	{
		
	}

    void OnValidate()
    {
        if (cycleDuration <= 0.0f)
        {
            cycleDuration = 0.1f;
        }
    }
}
