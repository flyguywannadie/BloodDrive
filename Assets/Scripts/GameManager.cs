using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int currentLap = 1;
	[SerializeField] TMP_Text lapText;
    //[SerializeField] 

	void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnLapChange()
    {
        currentLap++;
		lapText.text = "LAP " + currentLap.ToString();
	}

    public void OnEventStart()
    {

    }

    public void TrafficEvent()
    {

    }
}
