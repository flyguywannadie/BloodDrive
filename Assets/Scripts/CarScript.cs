using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    [SerializeField] float speed = 5; // how fast he car is currently moving
    [SerializeField] float acceleration = 5; // how fast the car accelerates, higher = more
    [SerializeField] float maxSpeed = 25; // maximum speed of car
    [SerializeField] float friction = 4; // higher the number the quicker it slows down
    [SerializeField] float howIAmTurning = 0; // the angle the car turns at
    [SerializeField] float turnspeed = 5; // how fast the car turns
    [SerializeField] float maxturnAngle = 25; // maximum angle the car turns at
    float originalMaxTurn;
    float originalMaxSpeed;
    [SerializeField] float gravity = -9.81f; // speed of gravity when not on a track
    [SerializeField] float floatingHeight = 0.25f; // how high above the ground the car floats

    [SerializeField] Transform model; // the model of the car

    [SerializeField] LayerMask groundLM;

    // Start is called before the first frame update
    void Start()
    {
        originalMaxTurn = maxturnAngle;   
        originalMaxSpeed = maxSpeed;   
    }

    // Update is called once per frame
    void Update()
    {
        if (!Physics.Raycast(transform.position, -transform.up, out RaycastHit ray, floatingHeight, groundLM))
        {
			Debug.Log("GRAVITY IS A HARNESS");
            transform.position += transform.up * gravity * Time.deltaTime;
		} 
        else
        {
            Debug.Log("Float On ground NOW!!!");
            transform.position = ray.point + (ray.normal * floatingHeight * 0.9f);

			//https://discussions.unity.com/t/character-up-vector-to-align-with-normal/119153
			transform.rotation = Quaternion.FromToRotation(transform.up, ray.normal) * transform.rotation;
		}

		// Moving forward and slowing down
		transform.Translate(transform.forward * Time.deltaTime * speed, Space.World);

        speed = Mathf.Lerp(speed, 0, ((speed/maxSpeed)/2) * Time.deltaTime);

		if (Input.GetKey(KeyCode.W))
        {
            if (speed < maxSpeed)
            {
                speed += acceleration * Time.deltaTime;
            }
        }


		// turning left and right
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (speed > maxSpeed * 0.75f)
            {
				speed = maxSpeed * 0.75f;
			}
		}


        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
			maxturnAngle = originalMaxTurn;
            maxSpeed = originalMaxSpeed;
		}

        howIAmTurning = Mathf.Lerp(howIAmTurning, 0, (turnspeed * (speed / maxSpeed)) * Time.deltaTime);

		transform.Rotate(transform.up, howIAmTurning * Time.deltaTime, Space.World);

		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			maxturnAngle *= 1.75f;

		}
		if (Input.GetKey(KeyCode.A))
		{
			howIAmTurning = Mathf.Lerp(howIAmTurning, -maxturnAngle, turnspeed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.D))
		{
			howIAmTurning = Mathf.Lerp(howIAmTurning, maxturnAngle, turnspeed * Time.deltaTime);
		}

        model.localRotation = Quaternion.Euler(0,0,-howIAmTurning);
	}
}
