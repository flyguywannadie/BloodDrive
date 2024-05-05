using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] public SplineContainer splinetofollow;
	[SerializeField] public int currentLane = 0;
	public float speed = 1;
	[Range(0, 1)] public float tdistance = 0;

	public float length { get { return splinetofollow.CalculateLength(); } }
	public float distance { get { return tdistance * length; } set { tdistance = value / length; } }

	// Start is called before the first frame update
	void Start()
    {
		//float variance = UnityEngine.Random.Range(-10f, 10f);
		//speed = (Mathf.Sign(UnityEngine.Random.Range(-1f, 1f)) * speed) + variance;
		//StartCoroutine(SwapLanesCoroutine());
	}

	// Update is called once per frame
	void Update()
    {
		distance += speed * Time.deltaTime;
		UpdateTransform(tdistance - Mathf.Floor(tdistance));

		if (tdistance >= 1)
		{
			tdistance = 0;
		}
		
	}

	void UpdateTransform(float t)
	{
		Vector3 position = splinetofollow.EvaluatePosition(t);
		Vector3 up = splinetofollow[currentLane].EvaluateUpVector(t);
		Vector3 forward = Vector3.Normalize(splinetofollow.EvaluateTangent(t));

		if (speed < 0)
		{
			forward *= -1;
		}

		transform.position = position;
		transform.rotation = Quaternion.LookRotation(forward, up);
	}

}
