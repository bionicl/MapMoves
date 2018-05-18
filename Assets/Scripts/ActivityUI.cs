using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ActivityUI : MonoBehaviour {

	public GameObject place;
	public Image placeIcon;
	public Image move;
	public Text Header;
	public Text Subheader;
	public Text endTimeText;
	public int[] moveHeights = {30, 40, 60};
	public int[] placeHeights = {60, 70, 80};

	public Text MoveType;
	public Text MoveTime;
	public Text MoveMinText;

	public ActivityType? type;
	public double distance;
	public float time;
	public string placename;
	public DateTime endTime;
	public PlaceType? placeType;
	public string placeFbId;
	public PlaceGroup placeGroup;

	string[] activityTypeText = {
		"Walk",
		"Transport",
		"Cycling",
		"Train",
		"Dancing",
		"Bus",
		"Tram",
		"Running",
		"Car",
		"Underground",
		"Airplane"
	};

	public void Setup(ActivityType? type, double distance, float time, DateTime endTime, MovesJson.SegmentsInfo.PlaceInfo placeInfo) {
		this.type = type;
		this.distance = distance;
		this.time = time;
		if (placeInfo != null) {
			placeGroup = PlacesRanking.instance.FindPlace(placeInfo, this);
			this.placename = placeInfo.name;
			this.endTime = endTime;
			this.placeType = placeInfo.type;
			if (placeType == PlaceType.facebook)
				placeFbId = placeInfo.facebookPlaceId;
		}

		TimeSpan t = TimeSpan.FromSeconds(this.time);
		SetSize(t);
		endTimeText.text = string.Format("{0}:{1}", this.endTime.Hour.ToString().PadLeft(2, '0'), this.endTime.Minute.ToString().PadLeft(2, '0'));

		string timeShort = string.Format("{0}", t.Minutes);
		if (type == null) {
			place.SetActive(true);
			move.gameObject.SetActive(false);
			Header.text = placename;
			Subheader.gameObject.SetActive(false);
			//Subheader.text = timeShort;
			MoveType.gameObject.SetActive(false);
			MoveTime.gameObject.SetActive(false);
			MoveMinText.gameObject.SetActive(false);
			//if (distance >= 100)
			//	Subheader.text += distance.ToString() + "m";
			if (placeGroup.placeInfo != null)
				placeIcon.sprite = placeGroup.IconSprite;
		} else {
			place.SetActive(false);
			move.gameObject.SetActive(true);
			move.color = ReadJson.colors[(int)type];
			Subheader.text = timeShort + distance.ToString() + "m";
			Header.gameObject.SetActive(false);
			Subheader.gameObject.SetActive(false);
			MoveType.text = activityTypeText[(int)type.Value];
			MoveType.color = ReadJson.colors[(int)type];
			MoveTime.text = timeShort;
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

	public void ClickOnPlace() {
		RightListUI.instance.NewPlace(placeGroup);
	}

	public void DestroyActivity() {
		ReadJson.instance.activitiesList.Remove(this);
		Destroy(gameObject);
	}
}
