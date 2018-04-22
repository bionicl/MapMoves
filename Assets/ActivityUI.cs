﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ActivityUI : MonoBehaviour {

	public GameObject place;
	public Image move;
	public Text Header;
	public Text Subheader;
	public Color[] activitesColor;
	public Text endTimeText;
	public int[] moveHeights = {30, 40, 60};
	public int[] placeHeights = {60, 70, 80};

	public ActivityType? type;
	public double distance;
	public float time;
	public string placename;
	public DateTime endTime;

	public void Setup(ActivityType? type, double distance, float time, DateTime endTime, string placename = null) {
		this.type = type;
		this.distance = distance;
		this.time = time;
		this.placename = placename;
		this.endTime = endTime;

		TimeSpan t = TimeSpan.FromSeconds(this.time);
		SetSize(t);
		endTimeText.text = string.Format("{0}:{1}", this.endTime.Hour.ToString().PadLeft(2, '0'), this.endTime.Minute.ToString().PadLeft(2, '0'));

		string timeShort = string.Format("{0}min ", t.Minutes);
		if (type == null) {
			place.SetActive(true);
			move.gameObject.SetActive(false);
			Header.text = placename;
			Subheader.text = timeShort;
			if (distance >= 100)
				Subheader.text += distance.ToString() + "m";
		} else {
			place.SetActive(false);
			move.gameObject.SetActive(true);
			move.color = activitesColor[(int)type];
			Header.gameObject.SetActive(false);
			Subheader.text = timeShort + distance.ToString() + "m";
		}
	}

	public void AddToExisting(double distance, float time, DateTime endTime) {
		this.distance += distance;
		this.time += time;
		this.endTime = endTime;

		TimeSpan t = TimeSpan.FromSeconds(this.time);
		SetSize(t);
		endTimeText.text = string.Format("{0}:{1}", this.endTime.Hour.ToString().PadLeft(2, '0'), this.endTime.Minute.ToString().PadLeft(2, '0'));

		string timeShort = string.Format("{0}min ", t.Minutes);
		if (type == null) {
			Subheader.text = timeShort;
			if (distance >= 100)
				Subheader.text += this.distance.ToString() + "m";
		} else {
			Subheader.text = timeShort + this.distance.ToString() + "m";
		}
	}

	void SetSize(TimeSpan t) {
		int[] height = moveHeights;
		if (this.type == null)
			height = placeHeights;
		
		if (t.Minutes < 10)
			GetComponent<RectTransform>().sizeDelta = new Vector2(0, height[0]);
		else if (t.Minutes < 30)
			GetComponent<RectTransform>().sizeDelta = new Vector2(0, height[1]);
		else
			GetComponent<RectTransform>().sizeDelta = new Vector2(0, height[2]);
	}

	public void DestroyActivity() {
		ReadJson.instance.activitiesList.Remove(this);
		Destroy(gameObject);
	}
}