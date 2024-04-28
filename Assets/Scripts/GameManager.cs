using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    int currentLap = 1;
	[SerializeField] SuperTextMesh lapText;
    [SerializeField] EnemySpawner spawner;

    [SerializeField] Image gameOverTransition;

    bool gameOver = false;
    float timer = 0;

	void Start()
    {
        spawner.spawnEnemies();
        gameOver = false;
        gameOverTransition.color = new Color(1, 1, 1, 0);
    }

    void Update()
    {
        if (gameOver == true)
        {
            timer += Time.deltaTime;

            if (timer > 2)
            {
                gameOverTransition.color = new Color(1, 1, 1, Mathf.Lerp(gameOverTransition.color.a, 1, 2 * Time.deltaTime));

                if (gameOverTransition.color.a > 0.8f)
                {
                    gameOverTransition.color = new Color(1, 1, 1, 1);
                    SceneManager.LoadScene("GameOverScene");
                }
            }
        }
    }

    public void OnGameOver()
    {
        gameOver = true;
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
