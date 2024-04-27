using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RaceProgress : MonoBehaviour
{
    [SerializeField] List<GameObject> racePoints;
    [SerializeField] TMP_Text lapText;
    int currentPoint = 0;
    int currentLap = 1;

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
                currentLap++;
                lapText.text = "Lap " + currentLap.ToString();
            }

			racePoints[currentPoint].SetActive(true);
		}
	}
}
