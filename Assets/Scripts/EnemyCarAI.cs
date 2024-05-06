using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCarAI : MonoBehaviour
{
	private enum eState { GROUNDED, TOGROUND, TOINAIR, INAIR }
	[SerializeField] eState state;

	[SerializeField] Vector3 speed; // how fast he car is currently moving
	[SerializeField] float acceleration = 5; // how fast the car accelerates, higher = more
	[SerializeField] float maxSpeed = 25; // maximum speed of car
	[SerializeField] float howIAmTurning = 0; // the angle the car turns at
	[SerializeField] float turnspeed = 5; // how fast the car turns
	[SerializeField] float maxturnAngle = 25; // maximum angle the car turns at
	float originalMaxTurn;
	float originalMaxSpeed;
	[SerializeField] float gravity = -9.81f; // speed of gravity when not on a track
	[SerializeField] float floatingHeight = 0.25f; // how high above the ground the car floats

	[SerializeField] Transform model; // the model of the car

	[SerializeField] LayerMask groundLM;
	[SerializeField] LayerMask wallLM;

	[SerializeField] ParticleSystem driftSparks;

	//[Header("Audio")]
	//[SerializeField] AudioSource vehicleAudio;
	//[SerializeField] AudioSource driftAudio;

	[SerializeField] Vector3 prevpos;

	[SerializeField] GameObject deathPrefab;

	//[SerializeField] GameManager gameManager;

	[Header("AI Stuff")]
	[SerializeField] Transform target;
	[SerializeField] bool speedUp;
	[SerializeField] bool turnLeft;
	[SerializeField] bool turnRight;
	[SerializeField] bool startDrift;
	[SerializeField] bool holdDrift;
	[SerializeField] bool endDrift;

	// Start is called before the first frame update
	void Start()
	{
		originalMaxTurn = maxturnAngle;
		originalMaxSpeed = maxSpeed;
		prevpos = transform.position;
	}

	public void Die()
	{
		Instantiate(deathPrefab, transform.position, transform.rotation);

		// tell gamemanager to gameover
		//gameManager.OnGameOver();

		Destroy(gameObject);
	}

	// Update is called once per frame
	void Update()
	{
		//Debug.Log(state.ToString());

		switch (state)
		{
			case eState.GROUNDED:

				if (Physics.Raycast(transform.position, -transform.up, out RaycastHit ray, floatingHeight * 2, groundLM))
				{
					SnapToGround(ray);
				}
				else
				{
					state = eState.TOINAIR;
					return;
				}

				OnGroundAI();

				MoveCarForward();

				TurnCar();

				if (Physics.Linecast(prevpos, transform.position, out RaycastHit ray3, groundLM))
				{
					//Debug.Log("YOU PASSED THROUGH THE GROUND");
					//Destroy(gameObject);
					SnapToGround(ray3);
				}
				break;
			case eState.TOGROUND:
				//Debug.Log("Float On ground NOW!!!");
				speed = new Vector3(speed.x, 0, speed.z);
				float speed2 = speed.magnitude;
				speed = new Vector3(0, 0, speed2);
				state = eState.GROUNDED;
				if (Input.GetKey(KeyCode.LeftShift))
				{
					driftSparks.Play();
					//driftAudio.Play();
				}
				break;
			case eState.INAIR:
				//Debug.Log("GRAVITY IS A HARNESS");

				//speed.z = Mathf.Lerp(speed.z, 0, 1f * Time.deltaTime);
				//speed.x = Mathf.Lerp(speed.x, 0, 1f * Time.deltaTime);

				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), Time.deltaTime);

				if (Physics.Linecast(prevpos, transform.position + (speed * Time.deltaTime), out RaycastHit ray2, groundLM))
				{
					transform.rotation = Quaternion.LookRotation(transform.forward, ray2.normal);
					SnapToGround(ray2);
					state = eState.TOGROUND;
					return;
				}
				speed.y += gravity * Time.deltaTime;
				transform.position += speed * Time.deltaTime;
				break;
			case eState.TOINAIR:
				//Debug.Log("To In AIR");
				MoveCarForward();
				float speedMagnitude = speed.magnitude;
				speed = transform.position - prevpos;
				speed = speed.normalized * speedMagnitude;
				state = eState.INAIR;
				driftSparks.Stop();
				//driftAudio.Stop();
				break;
		}

		//vehicleAudio.pitch = speed.magnitude / 30;
		CheckSideCollisions();
		prevpos = transform.position;
	}

	private void OnGroundAI()
	{
		Vector3 direction = target.position - transform.position;

		float angle = Vector3.Dot(transform.forward, direction.normalized);
		float sideAngle = Vector3.Dot(transform.right, direction.normalized);

		Debug.Log("Forward Dot Product " + angle);
		Debug.Log("Sideways Dot Product " + sideAngle);

		if (angle > 0)
		{
			speedUp = true;
		}

		if (!holdDrift && (Mathf.Abs(sideAngle) > 0.85f && angle < 0f))
		{
			startDrift = true;
		} else if (holdDrift && (Mathf.Abs(sideAngle) < 0.85f && angle > 0.2f))
		{
			endDrift = true;
		}

		if (sideAngle > 0.05f)
		{
			turnRight = true;
		} 
		if (sideAngle < 0.05f)
		{
			turnLeft = true;
		}
		 if (angle < 0)
		{
			if (sideAngle > 0)
			{
				turnRight = true;
			}
			else 
			{
				turnLeft = true;
			}
		}
	}

	private void LateUpdate()
	{
		if (state == eState.INAIR)
		{
			if (Physics.Linecast(prevpos, transform.position, out RaycastHit ray2, groundLM))
			{
				transform.rotation = Quaternion.LookRotation(transform.forward, ray2.normal);
				SnapToGround(ray2);
				state = eState.TOGROUND;
				return;
			}
		}
		//if (state == eState.GROUNDED)
		//{
		//	if (Physics.Linecast(prevpos, transform.position, out RaycastHit ray2, groundLM))
		//	{
		//		SnapToGround(ray2);
		//		return;
		//	}
		//}
	}

	private void MoveCarForward()
	{
		// Moving forward
		transform.Translate(speed * Time.deltaTime, Space.Self);

		speed.z = Mathf.Lerp(speed.z, 0, ((speed.z / maxSpeed) / 2) * Time.deltaTime);

		if (speedUp)
		{
			if (speed.z < maxSpeed)
			{
				speed.z += acceleration * Time.deltaTime;
			}
			speedUp = false;
		}


	}

	private void CheckSideCollisions()
	{
		if (Physics.Raycast(transform.position - (transform.up * floatingHeight * 0.9f), transform.right, out RaycastHit ray, 0.5f * Time.deltaTime, wallLM))
		{
			howIAmTurning = -5f;
			transform.Rotate(transform.up, howIAmTurning);
			transform.position -= transform.right;
			speed.z *= 0.8f;
		}
		if (Physics.Raycast(transform.position - (transform.up * floatingHeight * 0.9f), -transform.right, out ray, 0.5f * Time.deltaTime, wallLM))
		{
			howIAmTurning = 5f;
			transform.Rotate(transform.up, howIAmTurning);
			transform.position += transform.right;
			speed.z *= 0.8f;
		}
		if (Physics.Linecast(prevpos - (transform.up * floatingHeight * 0.9f), transform.position + (transform.forward * GetSpeed() * Time.deltaTime), out ray, wallLM))
		{
			//Debug.Log("YOU HAVE TO TURN");
			//Destroy(gameObject);
			transform.position -= transform.forward;
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(ray.normal, transform.up), 0.5f);
			speed.z *= 0.5f;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		//Gizmos.DrawLine(transform.position, transform.position + ((transform.right * (howIAmTurning / 90)) + (transform.forward * speed.z)));
		Gizmos.DrawLine(transform.position, target.position);
		//Gizmos.DrawSphere(transform.position, attackRange);
	}

	private void TurnCar()
	{
		// turning left and right
		if (holdDrift && !endDrift)
		{
			maxSpeed *= 1 - (0.75f * Time.deltaTime);
			if (!turnLeft && !turnRight)
			{
				howIAmTurning = Mathf.Lerp(howIAmTurning, 0, (turnspeed * (speed.z / maxSpeed)) * Time.deltaTime);
			}
		}
		else
		{
			howIAmTurning = Mathf.Lerp(howIAmTurning, 0, (turnspeed * (speed.z / maxSpeed)) * Time.deltaTime);
		}

		if (endDrift)
		{
			maxturnAngle = originalMaxTurn;
			maxSpeed = originalMaxSpeed;
			driftSparks.Stop();
			holdDrift = false;
			endDrift = false;
			//driftAudio.Stop();
		}

		transform.Rotate(transform.up, howIAmTurning * Time.deltaTime, Space.World);

		if (startDrift)
		{
			driftSparks.Play();
			//driftAudio.Play();
			maxturnAngle *= 1.75f;
			holdDrift = true;
			startDrift = false;
		}

		if (turnLeft)
		{
			howIAmTurning = Mathf.Lerp(howIAmTurning, -maxturnAngle, turnspeed * Time.deltaTime);
			turnLeft = false;
		}
		if (turnRight)
		{
			howIAmTurning = Mathf.Lerp(howIAmTurning, maxturnAngle, turnspeed * Time.deltaTime);
			turnRight = false;
		}
	}

	private void SnapToGround(RaycastHit hit)
	{
		transform.position = hit.point + (hit.normal * floatingHeight * 0.9f);

		// this works thank you google lol
		//https://discussions.unity.com/t/character-up-vector-to-align-with-normal/119153
		transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
	}

	public float GetSpeed()
	{
		if (state == eState.GROUNDED)
		{
			return speed.z;
		}
		else
		{
			return speed.magnitude;
		}
	}

	public float GetTurn()
	{
		return howIAmTurning;
	}

	public float GetMaxSpeed()
	{
		return maxSpeed;
	}
}