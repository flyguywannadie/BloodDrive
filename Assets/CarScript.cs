using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float maxSpeed;
    [SerializeField] float friction;
    [SerializeField] float turnspeed;
    [SerializeField] float gravity;
    [SerializeField] float floatingHeight = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Physics.Raycast(transform.position, -transform.up, out RaycastHit ray))
        {
			Debug.Log("GRAVITY IS A HARNESS");
		} 
        else
        {
            Debug.Log("Float On ground NOW!!!");
        }

        if (Input.GetKey(KeyCode.W))
        {

        }
		if (Input.GetKey(KeyCode.W))
		{

		}
		if (Input.GetKey(KeyCode.W))
		{

		}
		if (Input.GetKey(KeyCode.W))
		{

		}
	}
}
