using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChartItem : MonoBehaviour {
	public static ChartItem selectedChart;

	int sum;
	int walkingSum;
	int cyclingSum;
	int otherSum;

	public Image background;
	public RectTransform walking;
	public RectTransform cycling;
	public RectTransform other;

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
			if (item.activity == ActivityType.walking)
				walkingSum += (int)item.calories;
			else if (item.activity == ActivityType.cycling)
				cyclingSum += (int)item.calories;
			else if (item.activity == ActivityType.running || item.activity == ActivityType.dancing)
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
		}

		// Cycling
		if (cyclingSum == 0) {
			cycling.gameObject.SetActive(false);
		} else {
			float heightCycling = ChartUI.instance.heightPerCalorie * cyclingSum;
			cycling.sizeDelta = new Vector2(20, heightCycling + heightSum);
			heightSum += heightCycling;
		}

		// Other
		if (otherSum == 0) {
			other.gameObject.SetActive(false);
		} else {
			float heightOther = ChartUI.instance.heightPerCalorie * otherSum;
			other.sizeDelta = new Vector2(20, heightOther + heightSum);
		}
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
