using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class CarSpeed : MonoBehaviour
{
	[SerializeField] TMP_Text speedText;
	[SerializeField] CarScript carScript;

	//Vector3 lastPosition = Vector3.zero;
	//float speed;
	int frames = 0;

	void FixedUpdate()
    {
		frames++;

		if (frames == 3)
		{
			//speed = Vector3.Distance(lastPosition, transform.position) * 100f;
			//lastPosition = transform.position;

			speedText.text = (carScript.GetSpeed() * 37).ToString("0") + " MPH";

			frames = 0;
		}
	}
}
