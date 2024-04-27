using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    private enum eState { GROUNDED, TOGROUND, TOINAIR, INAIR}
    [SerializeField] eState state;

    [SerializeField] Vector3 speed; // how fast he car is currently moving
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

    Vector3 prevpos;

    // Start is called before the first frame update
    void Start()
    {
        originalMaxTurn = maxturnAngle;   
        originalMaxSpeed = maxSpeed;   
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case eState.GROUNDED:
				if (Physics.Raycast(transform.position, -transform.up, out RaycastHit ray, floatingHeight, groundLM))
				{
					transform.position = ray.point + (ray.normal * floatingHeight * 0.9f);
					transform.rotation = Quaternion.FromToRotation(transform.up, ray.normal) * transform.rotation;
				} 
				else
				{
					state = eState.TOINAIR;
					return;
				}

				// Moving forward
				transform.Translate(speed * Time.deltaTime, Space.Self);

				speed.z = Mathf.Lerp(speed.z, 0, ((speed.z / maxSpeed) / 2) * Time.deltaTime);

				if (Input.GetKey(KeyCode.W))
				{
					if (speed.z < maxSpeed)
					{
						speed.z += acceleration * Time.deltaTime;
					}
				}

				// turning left and right
				if (Input.GetKey(KeyCode.LeftShift))
				{
					maxSpeed *= 1 - (0.75f * Time.deltaTime);
				}
				else
				{
					howIAmTurning = Mathf.Lerp(howIAmTurning, 0, (turnspeed * (speed.z / maxSpeed)) * Time.deltaTime);
				}
				if (Input.GetKeyUp(KeyCode.LeftShift))
				{
					maxturnAngle = originalMaxTurn;
					maxSpeed = originalMaxSpeed;
				}
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
				model.localRotation = Quaternion.Euler(0, 0, -howIAmTurning);
				break;
            case eState.TOGROUND:
				Debug.Log("Float On ground NOW!!!");
				speed = Vector3.zero;
				state = eState.GROUNDED;
				break;
            case eState.INAIR:
				Debug.Log("GRAVITY IS A HARNESS");
				if (Physics.Raycast(transform.position, speed.normalized, out RaycastHit ray2, floatingHeight, groundLM))
				{
					transform.position = ray2.point + (ray2.normal * floatingHeight * 0.9f);

					// this works thank you google lol
					//https://discussions.unity.com/t/character-up-vector-to-align-with-normal/119153
					transform.rotation = Quaternion.FromToRotation(transform.up, ray2.normal) * transform.rotation;
					state = eState.TOGROUND;
					return;
				}
				speed.y += gravity * Time.deltaTime;
				transform.position += speed * Time.deltaTime;
				break;
            case eState.TOINAIR:
                float speedMagnitude = speed.magnitude;
                speed = prevpos - transform.position;
                speed = speed.normalized * speedMagnitude;
                state = eState.INAIR;
				break;
        }

		
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(transform.position, transform.position + speed.normalized * 5f);
	}
}
