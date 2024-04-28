using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] SplineContainer splinetofollow;
	[Range(-40, 40)] public float speed = 1;
	[Range(0, 1)] public float tdistance = 0;

	public float length { get { return splinetofollow.CalculateLength(); } }
	public float distance { get { return tdistance * length; } set { tdistance = value / length; } }

	// Start is called before the first frame update
	void Start()
    {

    }

	// Update is called once per frame
	void Update()
    {
		distance += speed * Time.deltaTime;
		if (Input.GetKey(KeyCode.Space))
		{
			distance += speed * Time.deltaTime;
		}
		UpdateTransform(math.frac(tdistance));
	}

	void UpdateTransform(float t)
	{
		Vector3 position = splinetofollow.EvaluatePosition(t);
		Vector3 up = splinetofollow.EvaluateUpVector(t);
		Vector3 forward = Vector3.Normalize(splinetofollow.EvaluateTangent(t));
		Vector3 right = Vector3.Cross(up, forward);

		transform.position = position;
		transform.rotation = Quaternion.LookRotation(forward, up);
	}
}
