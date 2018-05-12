using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaceGroup {
	public Place mapObject;
	public ActivityUI timelineObject;
	public MovesJson.SegmentsInfo.PlaceInfo placeInfo;
	public int icon;
	public int timesVisited;
	public float scaleNormal = 0.1f;
	public DateTime lastVisited;
	public int[] hourSplit = new int[24];

	public PlaceGroup() {
		
	}
	public PlaceGroup (MovesJson.SegmentsInfo.PlaceInfo placeinfo, DateTime timeStart, DateTime timeStop) {
		this.placeInfo = placeinfo;  
		GlobalVariables.inst.SetIcon(placeinfo, (int sprite) => RefreshIcons(sprite));
		CalculateHours(timeStart, timeStop);
	}

	public void AddMapObject(Place mapObject) {
		this.mapObject = mapObject;
		timesVisited = 1;
		RefreshIcons();
	}
	public void AddTimelineObject(ActivityUI timelineObject) {
		this.timelineObject = timelineObject;
		RefreshIcons();
	}

	public void AddHoursSplit(DateTime timeStart, DateTime timeStop) {
		CalculateHours(timeStart, timeStop);
	}
	public void RefreshSize() {
		if (mapObject == null)
			return;
		float scale = 0.1f;
		scale = (float)timesVisited / (float)PlacesRanking.instance.maxValue * 0.24f;
		scale = Mathf.Clamp(scale, 0.1f, 0.24f);
		scaleNormal = scale;
		mapObject.transform.localScale = new Vector3(scale, scale, 1f);
		Vector3 mapObjectPosition = mapObject.transform.position;
		float multiply = scale;
		multiply *= -1;
		mapObjectPosition.z = multiply;
		mapObject.transform.position = mapObjectPosition;
	}
	public void SetZoomSize(float zoom) {
		float zoomMultipler = -1 * (1 - zoom);
		float finalZoom = (1 + zoomMultipler);
		if (finalZoom < 0.4f && zoom <= 2.5f)
			finalZoom = 0.4f;
		else if (zoom >= 2.5f){
			float smallSize = 0.4f - (float)zoomMultipler / 100;
			smallSize *= 10;
			if (finalZoom >= smallSize ) {
				finalZoom = smallSize;
			}
		}
		if (zoom >= 14) {
			finalZoom = 2.5f;
		}
		finalZoom *= scaleNormal;
		mapObject.transform.localScale = new Vector3(finalZoom, finalZoom, 1f);

		if (mapObject != null) {
			mapObject.ChangeIconVisible(zoom);
		}
	}
	public void RefreshIcons(int sprite) {
		icon = sprite;
		if (mapObject != null) {
			mapObject.icon.sprite = FacebookPlaces.instance.iconsImages[icon];
		}
		if (timelineObject != null) {
			timelineObject.placeIcon.sprite = FacebookPlaces.instance.iconsImages[icon];
		}
	}
	void RefreshIcons() {
		if (mapObject != null) {
			mapObject.icon.sprite = FacebookPlaces.instance.iconsImages[icon];
		}
		if (timelineObject != null) {
			timelineObject.placeIcon.sprite = FacebookPlaces.instance.iconsImages[icon];
		}
	}
	void CalculateHours(DateTime timeStart, DateTime timeStop) { // 3:30 - 5:10
		DateTime timeNewDay = new DateTime(2018, 05, 10, 0, 0, 0);
		DateTime timeEndDay = new DateTime(2018, 05, 10, 23, 59, 0);
		if (timeStop.Hour < timeStart.Hour) {
			for (int i = 0; i < 24; i++) {
				if (i >= timeNewDay.Hour && i < (timeStop.Hour + 1))
					hourSplit[i]++;
			}
			for (int i = 0; i < 24; i++) {
				if (i >= timeStart.Hour && i < (timeEndDay.Hour + 1))
					hourSplit[i]++;
			}
		} else {
			for (int i = 0; i < 24; i++) {
				if (i >= timeStart.Hour && i < (timeStop.Hour + 1))
					hourSplit[i]++;
			}
		}
	}

	public void DisplayTimes(CanvasGroup[] hours) {
		float maxHours = hourSplit.Max();
		for (int i = 0; i < 24; i++) {
			float alpha = ((float)hourSplit[i] / maxHours);
			alpha *= 0.95f;
			alpha += 0.05f;
			hours[i].alpha = alpha;
		}
	}

	public Sprite IconSprite {
		get {
			return FacebookPlaces.instance.iconsImages[icon];
		}
	}
}

public class PlacesRanking : MonoBehaviour {
	public static PlacesRanking instance;

	public int theBiggestPlaceVisits = 110;
	[HideInInspector]
	public int maxValue;

	public Dictionary<long, PlaceGroup> places = new Dictionary<long, PlaceGroup>();

	void Awake() {
		instance = this;
	}

	public void AnalyseDay(MovesJson day) {
		foreach (var item in day.segments) {
			if (item.place != null && item.place.name != null) {
				AddOrSetupRanking(item.place, day, item);
			}
		}
	}
	void AddOrSetupRanking(MovesJson.SegmentsInfo.PlaceInfo place, MovesJson day, MovesJson.SegmentsInfo segmentInfo) {
		PlaceGroup placeTarget = new PlaceGroup();
		if (places.TryGetValue(place.id, out placeTarget)) {
			placeTarget.timesVisited += 1;
			placeTarget.AddHoursSplit(ReadJson.ReturnDateTime(segmentInfo.startTime), ReadJson.ReturnDateTime(segmentInfo.endTime));
		} else {
			placeTarget = new PlaceGroup(place, ReadJson.ReturnDateTime(segmentInfo.startTime), ReadJson.ReturnDateTime(segmentInfo.endTime));
			places.Add(place.id, placeTarget);
		}
		placeTarget.lastVisited = ReadJson.ReturnSimpleDate(day.date);

	}

	public PlaceGroup FindPlace(MovesJson.SegmentsInfo.PlaceInfo place, Place mapObject) {
		PlaceGroup thisPlace = new PlaceGroup();
		if (places.TryGetValue(place.id, out thisPlace)) {
			thisPlace.AddMapObject(mapObject);
			return thisPlace;
		} else
			return null;
	}

	public PlaceGroup FindPlace(MovesJson.SegmentsInfo.PlaceInfo place, ActivityUI timelineObject) {
		PlaceGroup thisPlace = new PlaceGroup();
		if (places.TryGetValue(place.id, out thisPlace)) {
			thisPlace.AddTimelineObject(timelineObject);
			return thisPlace;
		} else
			return null;
	}

	public void SortAndDisplay() {
		Dictionary<PlaceGroup, int> ranking = new Dictionary<PlaceGroup, int>();
		foreach (var item in places) {
			ranking.Add(item.Value, item.Value.timesVisited);
		}
		ranking = ranking.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
		maxValue = ranking.First().Value;
		if (maxValue > theBiggestPlaceVisits)
			maxValue = theBiggestPlaceVisits;
		/*foreach (KeyValuePair<PlaceGroup, int> item in ranking) {
			Debug.Log(item.Key.placeInfo.name + "(" + 
			          item.Key.placeInfo.type +") - " + item.Value.ToString());
		}*/
		foreach (var item in places.Values) {
			item.RefreshSize();
		}
		GlobalVariables.inst.MoveCamera(ranking.First().Key.mapObject.transform.position);
	}
	public void ChangePlacesSize(float mapSize) {
		foreach (var item in places) {
			item.Value.SetZoomSize(mapSize);
		}
	}
}
