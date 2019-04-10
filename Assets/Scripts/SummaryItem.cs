using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummaryItem : MonoBehaviour {

	MovesJson.SummaryInfo summary;
	bool canChangeWeight;

	public Text activityTitle;
	public Image activityImage;
	public Text distance;
	public Text duration;
	public Text calories;
	public Sprite[] icons;

	float distanceValue;
	float caloriesValue;

	public void Setup(MovesJson.SummaryInfo summary, bool canChangeWeight) {
		this.summary = summary;
		this.canChangeWeight = canChangeWeight;
		
		SetupIcon();
		SetupTexts(canChangeWeight);
		SetupColors();
	}

	public void Refresh() {
		SetupTexts(canChangeWeight);
	}

	public static string FirstLetterUpper(string text) {
		string output = text.Substring(0, 1).ToUpper();
		output += text.Substring(1);
		return output;
	}

	void SetupIcon() {
		activityTitle.gameObject.SetActive(true);
		activityTitle.text = FirstLetterUpper(summary.activity.ToString());

		switch (summary.activity) {
			case ActivityType.walking:
				activityImage.sprite = icons[0];
				break;
			case ActivityType.cycling:
				activityImage.sprite = icons[1];
				break;
			case ActivityType.running:
				activityImage.sprite = icons[2];
				break;
			default:
				activityImage.gameObject.SetActive(false);
				activityTitle.gameObject.SetActive(true);
				activityTitle.text = summary.activity.ToString();
				break;
		}
	}
	void SetupTexts(bool canChangeWeight) {
		if (summary.calories > 1) {
			calories.gameObject.SetActive(true);
			double caloriesNumber = summary.calories;
			if (canChangeWeight) {
				caloriesNumber *= SettingsBox.instance.weight;
			}
			calories.text = caloriesNumber.ToString() + "cal";
		}
		if (summary.distance > 1) {
			distance.gameObject.SetActive(true);
			if (SettingsBox.instance.isMetric)
				distance.text = ChartItem.ConvertToKm((int)summary.distance);
			else
				distance.text = ChartItem.ConvertToMiles((int)summary.distance);
		}
		if (summary.duration > 1) {
			duration.gameObject.SetActive(true);
			TimeSpan t = TimeSpan.FromSeconds(summary.duration);
			duration.text = string.Format("{0:D2}:{1:D2}h",
							t.Hours,
							t.Minutes);
		}
	}
	void SetupColors() {
		Color color = ReadJson.instance.activitesColor[(int)summary.activity];
		activityTitle.color = color;
		activityImage.color = color;
		//distance.color = color;
		//duration.color = color;
		//calories.color = color;
	}
}
