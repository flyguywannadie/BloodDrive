using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RaceProgress : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] CarScript carScript;
    [SerializeField] List<GameObject> racePoints;
    int currentPoint = 0;

    void Start()
    {
        for (int i = 1; i < racePoints.Count; i++) //everything after the first element
        {
            racePoints[i].SetActive(false);
        }
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Point")
        {
            racePoints[currentPoint].SetActive(false);
            currentPoint++;
            if(currentPoint == racePoints.Count)
            {
                currentPoint = 0;
                gameManager.OnLapChange();
            }

			racePoints[currentPoint].SetActive(true);
		}

        if(other.tag == "Death")
        {
            carScript.Die();
        }
	}
}
