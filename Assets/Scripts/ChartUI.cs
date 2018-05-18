using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartUI : MonoBehaviour {
	public static ChartUI instance;

	public int barsHeight;

	[HideInInspector]
	public int maxCalories;
	[HideInInspector]
	public float heightPerCalorie;
	public GameObject spawnGameObject;


	public GameObject chartItemPrefab;

	void Awake() {
		instance = this;
	}

	public void CheckMaxCalories(MovesJson day) {
		double caloriesCount = 0;
		if (day.summary != null) {
			foreach (var item in day.summary) {
				caloriesCount += item.calories;
			}
			if (maxCalories < caloriesCount)
				maxCalories = (int)caloriesCount;
		}
	}

	public void CheckChartSelected() {
		DayClass selectedDay = new DayClass();
		ReadJson.instance.days.TryGetValue(ReadJson.instance.selectedDay, out selectedDay);
		selectedDay.chart.Select();
	}
	public void SetupCharts() {
		heightPerCalorie = (float)barsHeight / (float)maxCalories;
		foreach (var item in ReadJson.instance.days) {
			if (item.Value.day.summary != null) {
				GameObject chart = Instantiate(chartItemPrefab, spawnGameObject.transform.position, spawnGameObject.transform.rotation);
				RectTransform rectT = chart.GetComponent<RectTransform>();
				chart.transform.SetParent(spawnGameObject.transform);
				rectT.localScale = rectT.lossyScale;
				ChartItem chartItem = chart.GetComponent<ChartItem>();
				chartItem.Setup(item.Value);
				item.Value.chart = chartItem;
			}
		}
	}
}
