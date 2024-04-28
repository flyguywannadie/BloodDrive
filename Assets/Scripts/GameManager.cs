using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int currentLap = 1;
	[SerializeField] SuperTextMesh lapText;
    [SerializeField] EnemySpawner spawner;

	void Start()
    {
        spawner.spawnEnemies();
    }

    void Update()
    {
        
    }

    public void OnLapChange()
    {
        currentLap++;
		lapText.text = "LAP " + currentLap.ToString();
        OnEventStart();
	}

    public void OnEventStart()
    {
        //pick a random event after first lap
        TrafficEvent();
    }

    public void TrafficEvent()
    {
        spawner.spawnMin = 4;
        spawner.spawnMax = 6;
        spawner.spawnTimeMin = 0.1f;
        spawner.spawnTimeMax = 0.5f;

        spawner.spawnEnemies();
    }
}
