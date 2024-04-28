using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    private enum eState { GROUNDED, TOGROUND, TOINAIR, INAIR}
    [SerializeField] eState state;

    [SerializeField] Vector3 speed; // how fast he car is currently moving
    [SerializeField] float acceleration = 5; // how fast the car accelerates, higher = more
    [SerializeField] float maxSpeedBlood = 25; // maximum speed of car
    float maxSpeed = 0; // maximum speed of car
    //[SerializeField] float friction = 4; // higher the number the quicker it slows down
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

	[SerializeField] Animator carAnimator;
	[SerializeField] SpriteRenderer carSprite;
	[SerializeField] Sprite[] turningSprites;

	[SerializeField] CinemachineVirtualCamera virtualCamera;
	float shakeTimer = 0;

	[SerializeField] ParticleSystem driftSparks;

	[SerializeField, Range(0f, 1f)] float BloodAmount = 1f;

	[SerializeField] float howLongToRunOutOfBlood = 20;
	[SerializeField] float attackRange = 10f;

	[Header("Audio")]
	[SerializeField] AudioSource vehicleAudio;
	[SerializeField] AudioSource driftAudio;

	[SerializeField] Vector3 prevpos;

	CinemachineBasicMultiChannelPerlin noise;

	// Start is called before the first frame update
	void Start()
    {
        originalMaxTurn = maxturnAngle;   
        originalMaxSpeed = maxSpeedBlood;
		prevpos = transform.position;
		PlayIntro();

		noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		noise.m_AmplitudeGain = 0f;

	}

    // Update is called once per frame
    void Update()
    {
		//Debug.Log(state.ToString());
		maxSpeed = maxSpeedBlood * BloodAmount;

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

				MoveCarForward();

				TurnCar();

				if (Physics.Linecast(prevpos, transform.position, out RaycastHit ray3, groundLM))
				{
					Debug.Log("YOU PASSED THROUGH THE GROUND");
					//Destroy(gameObject);
					SnapToGround(ray3);
				}

				if (!carAnimator.isActiveAndEnabled && Input.GetKeyDown(KeyCode.Space))
				{
					SpintAttack();
				}
				break;
            case eState.TOGROUND:
				Debug.Log("Float On ground NOW!!!");
				speed = new Vector3(speed.x, 0, speed.z);
				float speed2 = speed.magnitude;
				speed = new Vector3(0,0,speed2);
				state = eState.GROUNDED;
				if (Input.GetKey(KeyCode.LeftShift))
				{
					driftSparks.Play();
					driftAudio.Play();
				}
				break;
            case eState.INAIR:
				Debug.Log("GRAVITY IS A HARNESS");

				//speed.z = Mathf.Lerp(speed.z, 0, 1f * Time.deltaTime);
				//speed.x = Mathf.Lerp(speed.x, 0, 1f * Time.deltaTime);

				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), Time.deltaTime);

				if (Physics.Linecast(prevpos, transform.position + speed.normalized, out RaycastHit ray2, groundLM))
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
				Debug.Log("To In AIR");
				MoveCarForward();
				float speedMagnitude = speed.magnitude;
                speed = transform.position - prevpos;
                speed = speed.normalized * speedMagnitude;
                state = eState.INAIR;
				driftSparks.Stop();
				driftAudio.Stop();
				break;
        }

		if (shakeTimer > 0)
		{
			shakeTimer -= Time.deltaTime;
			if (shakeTimer <= 0f)
			{
				noise.m_AmplitudeGain = 0f;
			}
		}

		if (!carAnimator.isActiveAndEnabled)
		{
			SelectCarSprite();
		}

		if (BloodAmount > 0.15f)
		{
			BloodAmount -= (1f/howLongToRunOutOfBlood) * Time.deltaTime;
		}

		vehicleAudio.pitch = speed.magnitude / 30;
		CheckSideCollisions();
		prevpos = transform.position;
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

		if (Input.GetKey(KeyCode.W))
		{
			if (speed.z < maxSpeed)
			{
				speed.z += acceleration * Time.deltaTime;
			}
		}
	}

	public IEnumerator SpinAttackKill()
	{
		for (int i = 0; i < 10; i++)
		{
			var hits = Physics.OverlapSphere(transform.position, attackRange);

			foreach (var hit in hits)
			{
				if (hit.TryGetComponent<BloodGiver>(out BloodGiver bg))
				{
					bg.Killed();
					noise.m_AmplitudeGain = 10f;
					shakeTimer = 0.25f;
				}
			}
			yield return new WaitForFixedUpdate();
		}
	}

	public void AddBlood(float amount)
	{
		BloodAmount += amount;
	}

	private void CheckSideCollisions()
	{
		if (Physics.Raycast(transform.position, transform.right, out RaycastHit ray, 0.5f, wallLM))
		{
			howIAmTurning = -15f;
			transform.Rotate(transform.up, howIAmTurning, Space.World);
			transform.position += ray.normal;
			speed.z *= 0.8f;
			noise.m_AmplitudeGain = 10f;
			shakeTimer = 0.25f;
			if (ray.transform.TryGetComponent<BloodGiver>(out BloodGiver bg))
			{
				BloodAmount -= bg.bloodDamage;
			}
		} else
		if (Physics.Raycast(transform.position, -transform.right, out ray, 0.5f, wallLM))
		{
			howIAmTurning = 15f;
			transform.Rotate(transform.up, howIAmTurning, Space.World);
			transform.position += ray.normal;
			speed.z *= 0.8f;
			noise.m_AmplitudeGain = 10f;
			shakeTimer = 0.25f;
			if (ray.transform.TryGetComponent<BloodGiver>(out BloodGiver bg))
			{
				BloodAmount -= bg.bloodDamage;
			}
		} else
		if (Physics.Linecast(prevpos, transform.position + transform.forward, out ray, wallLM))
		{
			Debug.Log("YOU HAVE TO TURN");
			//Destroy(gameObject);
			if (ray.transform.TryGetComponent<BloodGiver>(out BloodGiver bg))
			{
				BloodAmount -= bg.bloodDamage;
				howIAmTurning = howIAmTurning + UnityEngine.Random.Range(-15f, 15f);
				transform.Rotate(transform.up, howIAmTurning, Space.World);
			}
			else
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(ray.normal, transform.up), 0.5f);
			}
			speed.z *= 0.5f;
			noise.m_AmplitudeGain = 10f;
			shakeTimer = 0.25f;

		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		//Gizmos.DrawLine(transform.position, transform.position + ((transform.right * (howIAmTurning / 90)) + (transform.forward * speed.z)));
		Gizmos.DrawLine(prevpos,transform.position + transform.forward);
		//Gizmos.DrawSphere(transform.position, attackRange);
	}

	private void TurnCar()
	{
		// turning left and right
		if (Input.GetKey(KeyCode.LeftShift))
		{
			maxSpeedBlood *= 1 - (0.75f * Time.deltaTime);
			if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
			{
				howIAmTurning = Mathf.Lerp(howIAmTurning, 0, (turnspeed * (speed.z / maxSpeed)) * Time.deltaTime);
			}
		}
		else
		{
			howIAmTurning = Mathf.Lerp(howIAmTurning, 0, (turnspeed * (speed.z / maxSpeed)) * Time.deltaTime);
		}

		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			maxturnAngle = originalMaxTurn;
			maxSpeedBlood = originalMaxSpeed;
			driftSparks.Stop();
			driftAudio.Stop();
		}

		transform.Rotate(transform.up, howIAmTurning * Time.deltaTime, Space.World);

		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			driftSparks.Play();
			driftAudio.Play();
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
	}

	private void SelectCarSprite()
	{
		int spriteIndex = (int)(Mathf.Abs(howIAmTurning)/20);
		if (spriteIndex > turningSprites.Length - 1)
		{
			spriteIndex = turningSprites.Length - 1;
		}
		//Debug.Log(spriteIndex);
		if (spriteIndex != 0)
		{
			if (howIAmTurning < 0)
			{
				carSprite.flipX = true;
			}
			else
			{
				carSprite.flipX = false;
			}
		}

		driftSparks.transform.localRotation = Quaternion.Euler(0, (spriteIndex * (-15 * Mathf.Sign(howIAmTurning))), 0);

		carSprite.sprite = turningSprites[spriteIndex];
		//Debug.Log(turningSprites[spriteIndex].name);
	}

	private void SnapToGround(RaycastHit hit)
	{
		transform.position = hit.point + (hit.normal * floatingHeight * 0.9f);

		// this works thank you google lol
		//https://discussions.unity.com/t/character-up-vector-to-align-with-normal/119153
		transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
	}

	private void SpintAttack()
	{
		carAnimator.enabled = true;
		carAnimator.SetTrigger("Spin");
		StartCoroutine(SpinAttackKill());
	}

	private void PlayIntro()
	{
		carAnimator.enabled = true;
		carAnimator.SetTrigger("DoIntro");
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
		return maxSpeedBlood;
	}

	public float GetMaxSpeedWithBlood()
	{
		return maxSpeedBlood * BloodAmount;
	}

	public float GetBloodAmount()
	{
		return BloodAmount;
	}
}
