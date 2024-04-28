using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject LogoScreen;
    [SerializeField] Animator logo;

    [SerializeField] AudioSource MenuTheme;

    public void OnButtonClick()
    {
		SceneManager.LoadScene("MaxScene");
	}

    private void Start()
    {
        LogoScreen.SetActive(true);
        MainMenu.SetActive(false);
    }

    private void Update()
    {
        if( logo.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            LogoScreen.SetActive(false);
            MainMenu.SetActive(true);
        }
    }
}
