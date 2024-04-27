using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodGaugeUI : MonoBehaviour
{
	public static BloodGaugeUI Instance { get; private set; }

    [SerializeField] CarScript carScript;
    [SerializeField] Slider bloodSlider;

	private void Awake()
	{
		Instance = this;
	}

	private void OnGUI()
	{
		bloodSlider.value = carScript.GetBloodAmount();
	}
}
