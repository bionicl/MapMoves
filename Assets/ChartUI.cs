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

	public void SetupCharts() {
		heightPerCalorie = (float)barsHeight / (float)maxCalories;
		foreach (var item in ReadJson.instance.days) {
			if (item.Value.summary != null) {
				GameObject chart = Instantiate(chartItemPrefab, transform.position, transform.rotation);
				RectTransform rectT = chart.GetComponent<RectTransform>();
				rectT.localScale = rectT.lossyScale;
				chart.transform.SetParent(transform);
				chart.GetComponent<ChartItem>().Setup(item.Value.summary);
			}
		}
	}
}
