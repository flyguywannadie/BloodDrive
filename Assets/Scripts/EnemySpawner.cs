using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;
using static UnityEditor.UIElements.ToolbarMenu;

public class EnemySpawner : MonoBehaviour
{
	[SerializeField] SplineContainer[] splines;
    [SerializeField] GameObject[] enemyType;
	[SerializeField] CarScript player;

	[SerializeField] public int spawnMin;
    [SerializeField] public int spawnMax;
	[SerializeField] public float spawnTimeMin = 0.5f;
	[SerializeField] public float spawnTimeMax = 1f;

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
		enemyCount =Random.Range(spawnMin, spawnMax);

		if (enemyCount == 0) return;
		StartCoroutine(spawnCoroutine(0));
    }

	public IEnumerator spawnCoroutine(int currentEnemy)
    {
		//set type of car
		int randomIndex = Random.Range(0, enemyType.Count());
		var enemy = enemyType[randomIndex];

		enemy = Instantiate(enemy);

		var enemyScript = enemy.GetComponent<EnemyAI>();

		//set splines they are assigned to

        randomIndex = Random.Range(0, splines.Count());
		enemyScript.splinetofollow = splines[randomIndex];

		float variance = Random.Range(-10f, 10f);
		enemyScript.speed = (Mathf.Sign(Random.Range(-1f, 1f)) * enemyScript.speed) + variance;
		enemyScript.distance = Random.Range(0, enemyScript.length); //set random distance

		Vector3 position = enemyScript.splinetofollow.EvaluatePosition(enemyScript.tdistance - Mathf.Floor(enemyScript.tdistance));
        enemy.transform.position = position;

		if (currentEnemy != enemyCount)
		{
			yield return new WaitForSeconds(Random.Range(spawnTimeMin, spawnTimeMax));
			StartCoroutine(spawnCoroutine(currentEnemy + 1));
		}
	}
}
