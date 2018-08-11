using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ChartItem : MonoBehaviour {
	public static ChartItem selectedChart;

	int sum;
	int walkingSum;
	int cyclingSum;
	int otherSum;

	int walkingDistance;
	int cyclingDistance;

	public Image background;
	public RectTransform walking;
	public RectTransform cycling;
	public RectTransform other;

	public GameObject walkingIcon;
	public GameObject cyclingIcon;
	public GameObject otherIcon;

	public Text walkingText;
	public Text cyclingText;

	DayClass day;
	MovesJson.SummaryInfo[] summary;

	public void Setup (DayClass day) {
		this.day = day;
		this.summary = day.day.summary;
		AddSums();
		SetHeight();
	}

	void AddSums() {
		foreach (var item in summary) {
			sum += (int)item.calories;
			if (item.activity == ActivityType.walking) {
				walkingSum += (int)item.calories;
				walkingDistance += (int)item.distance;
			} else if (item.activity == ActivityType.cycling) {
				cyclingSum += (int)item.calories;
				cyclingDistance += (int)item.distance;
			} else if (item.activity == ActivityType.running || item.activity == ActivityType.dancing)
				otherSum += (int)item.calories;
		}
	}
	void SetHeight() {
		float heightSum = 0;

		// Walking
		if (walkingSum == 0) {
			walking.gameObject.SetActive(false);
		} else {
			float heightWalking = ChartUI.instance.heightPerCalorie * walkingSum;
			walking.sizeDelta = new Vector2(20, heightWalking);
			heightSum += heightWalking;

			if (heightWalking >= ChartUI.instance.heightToDisplayStats) {
				walkingIcon.SetActive(true);
				walkingText.gameObject.SetActive(true);
				walkingText.text = ConvertToKm(walkingDistance);
			}
		}

		// Cycling
		if (cyclingSum == 0) {
			cycling.gameObject.SetActive(false);
		} else {
			float heightCycling = ChartUI.instance.heightPerCalorie * cyclingSum;
			cycling.sizeDelta = new Vector2(20, heightCycling + heightSum);
			heightSum += heightCycling;

			if (heightCycling >= ChartUI.instance.heightToDisplayStats) {
				cyclingIcon.SetActive(true);
				cyclingText.gameObject.SetActive(true);
				cyclingText.text = ConvertToKm(cyclingDistance);
			}
		}

		// Other
		if (otherSum == 0) {
			other.gameObject.SetActive(false);
		} else {
			float heightOther = ChartUI.instance.heightPerCalorie * otherSum;
			other.sizeDelta = new Vector2(20, heightOther + heightSum);

			if (heightOther >= ChartUI.instance.heightToDisplayStats) {
				otherIcon.SetActive(true);
			}
		}
	}

	public static string ConvertToKm(int meters) {
		float km = (float)meters / 1000;
		return Mathf.Round(km).ToString() + "km";
	}

	public void Select() {
		if (selectedChart != null)
			selectedChart.Deselect();
		background.enabled = true;
		selectedChart = this;
	}

	public void Deselect() {
		background.enabled = false;
	}

	public void JumpToClickedChart() {
		ReadJson.instance.ChangeDay(day.date);
	}
}
