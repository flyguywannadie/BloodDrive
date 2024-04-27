using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BloodGiver : MonoBehaviour
{
	[SerializeField] CarScript player;
	[SerializeField] GameObject particleffectprefab;

	private void Start()
	{
		player = FindObjectOfType<CarScript>();
	}

	public void Killed()
	{
		if (particleffectprefab)
		{
			Instantiate(particleffectprefab, transform.position, Quaternion.identity);
		}
		player.AddBlood();
		Destroy(gameObject);
	}
}
