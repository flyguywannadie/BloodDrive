using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] SuperTextMesh gameOver;
    [SerializeField] float timer = 0;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource music;
    
    // Start is called before the first frame update
    void Start()
    {
        gameOver.fade = 0;
        StartCoroutine(Restart());
        audioSource.Play();
        music.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver.fade < 1 && timer <= 0)
        {
            gameOver.fade = Mathf.Lerp(gameOver.fade, 1, Time.deltaTime);

            
            if (gameOver.fade > 0.8f)
            {
                gameOver.fade = 1;
            }
        } else
        {
            timer += Time.deltaTime;

            if (timer > 4)
            {
                gameOver.fade = Mathf.Lerp(gameOver.fade, 0, Time.deltaTime);
            }

            if (gameOver.fade < 0.2f)
            {
                gameOver.fade = 0;
            }
        }
    }

    private void FixedUpdate()
    {
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(8);
        SceneManager.LoadScene("TitleMenu");
        yield return null;
    }

    IEnumerator StartSound()
    {
        yield return new WaitForSeconds(2);
        audioSource.Play();
        yield return null;
    }
}
