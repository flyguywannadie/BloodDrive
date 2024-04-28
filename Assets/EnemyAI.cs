using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] SplineContainer[] splinetofollow;
	[SerializeField] int currentLane = 0;
	public float speed = 1;
	[Range(0, 1)] public float tdistance = 0;

	public float length { get { return splinetofollow[currentLane].CalculateLength(); } }
	public float distance { get { return tdistance * length; } set { tdistance = value / length; } }

	// Start is called before the first frame update
	void Start()
    {
		StartCoroutine(SwapLanesCoroutine());
	}

	// Update is called once per frame
	void Update()
    {
		distance += speed * Time.deltaTime;
		UpdateTransform(math.frac(tdistance));
		if (tdistance >= 1)
		{
			tdistance = 0;
		}
		
	}

	void UpdateTransform(float t)
	{
		Vector3 position = splinetofollow[currentLane].EvaluatePosition(t);
		Vector3 up = splinetofollow[currentLane].EvaluateUpVector(t);
		Vector3 forward = Vector3.Normalize(splinetofollow[currentLane].EvaluateTangent(t));
		Vector3 right = Vector3.Cross(up, forward);

		transform.position = Vector3.Lerp(transform.position, position, 3 * Time.deltaTime);
		transform.rotation = Quaternion.LookRotation(forward, up);
	}

	private IEnumerator SwapLanesCoroutine()
	{
		currentLane = UnityEngine.Random.Range(0, splinetofollow.Length);
		yield return new WaitForSeconds(UnityEngine.Random.Range(3, 10));
		StartCoroutine(SwapLanesCoroutine());
	}
}
