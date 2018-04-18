using System;
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

	public ActivityType? type;
	public int distance;
	public float time;
	public string placename;

	public void Setup(ActivityType? type, int distance, float time, string placename = null) {
		this.type = type;
		this.distance = distance;
		this.time = time;
		this.placename = placename;

		TimeSpan t = TimeSpan.FromSeconds(this.time);
		SetSize(t);
			
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

	public void AddToExisting(int distance, float time) {
		Debug.Log("Distance before: " + this.distance);
		this.distance += distance;
		Debug.Log("Distance after: " + this.distance);
		this.time += time;

		TimeSpan t = TimeSpan.FromSeconds(this.time);
		SetSize(t);

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
		if (this.type == null) {
			if (t.Minutes < 10)
				GetComponent<RectTransform>().sizeDelta = new Vector2(0, 60);
			else if (t.Minutes < 30)
				GetComponent<RectTransform>().sizeDelta = new Vector2(0, 70);
			else
				GetComponent<RectTransform>().sizeDelta = new Vector2(0, 80);
		} else {
			if (t.Minutes < 10)
				GetComponent<RectTransform>().sizeDelta = new Vector2(0, 30);
			else if (t.Minutes < 30)
				GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40);
			else
				GetComponent<RectTransform>().sizeDelta = new Vector2(0, 60);
		}
	}

	public void DestroyActivity() {
		ReadJson.instance.activitiesList.Remove(this);
		Destroy(gameObject);
	}
}
