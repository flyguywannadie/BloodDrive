using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
	[SerializeField] SplineContainer[] splines;
    [SerializeField] GameObject[] enemyType;
    [SerializeField] Transform[] spawnPositions;
	[SerializeField] CarScript player;

	[SerializeField] int spawnMin;
    [SerializeField] int spawnMax;

    int enemyCount;

	void Start()
    {
        
    }

    void Update()
    {
        
    }
    
    public void spawnEnemies()
    {
		//get number range between min and max for number of enemies spawned
		enemyCount = UnityEngine.Random.Range(spawnMin, spawnMax);

		StartCoroutine(spawnCoroutine(0));
    }

	public IEnumerator spawnCoroutine(int currentEnemy)
    {
		//set type of car
		int randomIndex = UnityEngine.Random.Range(0, enemyType.Count());
		var enemy = enemyType[randomIndex];

		enemy = Instantiate(enemy);

		var enemyScript = enemy.GetComponent<EnemyAI>();

		//set splines they are assigned to
        for (int j = 0; j < splines.Count(); j++)
        {
            randomIndex = UnityEngine.Random.Range(0, 2);
			if (randomIndex == 1)
            {
				enemyScript.splinetofollow.Add(splines[j]);
			}
		}

        if(enemyScript.splinetofollow.Count() == 0)
        {
			enemyScript.splinetofollow.Add(splines[0]);
		}

		enemyScript.distance = UnityEngine.Random.Range(0, enemyScript.length); //set random distance

		Vector3 position = enemyScript.splinetofollow[enemyScript.currentLane].EvaluatePosition(math.frac(enemyScript.tdistance));
        enemy.transform.position = position;

		if (currentEnemy != enemyCount)
        {
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1f));
			StartCoroutine(spawnCoroutine(currentEnemy + 1));
		}
	}
}
