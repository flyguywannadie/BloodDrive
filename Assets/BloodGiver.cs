using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BloodGiver : MonoBehaviour
{
	[SerializeField] CarScript player;
	[SerializeField] GameObject[] particleffectprefab;

	[SerializeField] float amountToGive = 0.15f;
	public float bloodDamage = 0.07f;

	private void Start()
	{
		player = FindObjectOfType<CarScript>();
		Random.Range(0, 10);
	}

	public void Killed()
	{
		if (particleffectprefab.Length > 0)
		{
			foreach (var prefab in particleffectprefab)
			{
				Instantiate(prefab, transform.position, Quaternion.identity);
			}
		}
		player.AddBlood(amountToGive);
		Destroy(gameObject);
	}
}
