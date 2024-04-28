using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconShake : MonoBehaviour
{
    [SerializeField] Transform whoIShakeTo;
    Vector3 prevpos;
	CarScript car;

    Vector3 originalPos;
	float originalScale;

	private void Start()
	{
		car = FindObjectOfType<CarScript>();
		originalPos = transform.position;
		originalScale = transform.localScale.x;
	}

	void Update()
    {

        Vector3 difference = whoIShakeTo.position - prevpos;
		prevpos = whoIShakeTo.position;
		transform.position = originalPos;
		transform.localScale = new Vector3(originalScale,originalScale,originalScale);

		transform.Translate(new Vector3((0.2f * (car.GetTurn()/90)),-difference.y,0) * 100);
		transform.localScale *= (1 - ((car.GetSpeed()/100) * 0.2f));
		//Debug.Log(difference);
    }

	private void LateUpdate()
	{

	}
}
