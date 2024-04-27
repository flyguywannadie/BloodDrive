using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodGaugeUI : MonoBehaviour
{
    [SerializeField] CarScript carScript;
    [SerializeField] Slider bloodSlider;

	private void OnGUI()
	{
		bloodSlider.value = carScript.GetBloodAmount();
	}
}
