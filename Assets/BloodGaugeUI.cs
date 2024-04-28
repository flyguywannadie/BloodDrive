using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodGaugeUI : MonoBehaviour
{
	public static BloodGaugeUI Instance { get; private set; }

	[SerializeField] Gradient normalColor;

    [SerializeField] CarScript carScript;
    [SerializeField] Slider bloodSlider;
	[SerializeField] Image sliderfill;

	private void Awake()
	{
		Instance = this;
	}

	private void OnGUI()
	{

		float blood = carScript.GetBloodAmount();
		bloodSlider.value = blood;

		sliderfill.color = normalColor.Evaluate(blood);
	}
}
