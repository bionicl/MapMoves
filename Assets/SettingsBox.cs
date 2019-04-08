using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsBox : MonoBehaviour
{
	public static SettingsBox instance;
	[Header("Units")]
	public Button unitsMetric;
	public Button unitsImperial;
	public Button unitsKg;
	public Button unitsLb;

	[Header("Weight")]
	public InputField weightInputField;
	public Text weightUnit;

	[Header("Resources")]
	public Sprite selectedOption;
	public Sprite unselectedOption;

	[HideInInspector]
	public bool isMetric = true;
	[HideInInspector]
	public bool isKg = true;

	public float weight = 83;

	private void Awake() {
		instance = this;
	}

	public void ChangeUnitDistance(bool isMetric) {
		if (isMetric && !this.isMetric) {
			unitsMetric.image.sprite = selectedOption;
			unitsImperial.image.sprite = unselectedOption;
			this.isMetric = true;
		} else if (!isMetric && this.isMetric) {
			unitsMetric.image.sprite = unselectedOption;
			unitsImperial.image.sprite = selectedOption;
			this.isMetric = false;
		}
	}
	public void ChangeUnitWeight(bool isKg) {
		Debug.Log("勞");
		if (isKg && !this.isKg) {
			unitsKg.image.sprite = selectedOption;
			unitsLb.image.sprite = unselectedOption;
			this.isKg = true;
			weightInputField.text = weight.ToString("F2");
			weightUnit.text = "kg";
		} else if (!isKg && this.isKg) {
			unitsKg.image.sprite = unselectedOption;
			unitsLb.image.sprite = selectedOption;
			this.isKg = false;
			weightInputField.text = (weight * 2.2046226f).ToString("F2");
			weightUnit.text = "lb";
		}
	}

	public void ChangeWeight(string newWeight) {
		float newWeightFloat = System.Convert.ToSingle(newWeight);
		if (isKg)
			weight = newWeightFloat;
		else
			weight = newWeightFloat / 2.2046226f;

	}
}
