using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlacesDataJson;
using UnityEngine;

public class PlaceGroup {
	public Place mapObject;
	public ActivityUI timelineObject;
	public MovesJson.SegmentsInfo.PlaceInfo placeInfo;
	public string categoryId;
	public int timesVisited;
	public float scaleNormal = 0.1f;
	public DateTime lastVisited;
	public int[] hourSplit = new int[24];
	public int[] weekDaysSplit = new int[7];

	public PlaceCategory Category {
		get {
			return PlacesRanking.instance.categoriesDictionary[categoryId];
		}
	}

	public PlaceGroup() {

	}
	public PlaceGroup(MovesJson.SegmentsInfo.PlaceInfo placeinfo, DateTime timeStart, DateTime timeStop) {
		this.placeInfo = placeinfo;
		GlobalVariables.instance.SetIcon(placeinfo, (sprite) => RefreshIcons(sprite));
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
		if (zoom <= 0.0626f)
			finalZoom = 0.18f;
		else if (zoom <= 0.126f)
			finalZoom = 0.3f;
		else if (finalZoom < 0.4f && zoom <= 2.5f)
			finalZoom = 0.4f;
		else if (zoom >= 2.5f) {
			float smallSize = 0.4f - (float)zoomMultipler / 100;
			smallSize *= 10;
			if (finalZoom >= smallSize) {
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
	public void RefreshIcons(string sprite = null) {
		if (!string.IsNullOrWhiteSpace(sprite))
			categoryId = sprite;
		if (mapObject != null) {
			mapObject.icon.sprite = PlacesRanking.instance.categoriesDictionary[categoryId].smallIcon;
		}
		if (timelineObject != null) {
			timelineObject.placeIcon.sprite = PlacesRanking.instance.categoriesDictionary[categoryId].smallIcon;
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

		int weekDay = CheckIfMondayFirst((int)timeStart.DayOfWeek);
		weekDaysSplit[weekDay]++;
		int weekDay2 = CheckIfMondayFirst((int)timeStop.DayOfWeek);
		if (weekDay != weekDay2) {
			weekDaysSplit[weekDay2]++;
		}
	}

	int CheckIfMondayFirst(int dayOfWeek) {
		if (!GlobalVariables.instance.firstWeekMonday)
			return dayOfWeek;
		int output = dayOfWeek;
		output--;
		if (output == -1)
			output = 6;
		return output;
	}

	public void DisplayTimes(RectTransform[] hours, int maxHeight) {
		float maxHours = hourSplit.Max();
		for (int i = 0; i < 24; i++) {
			float height = ((float)hourSplit[i] / maxHours);
			height *= (float)maxHeight;
			hours[i].sizeDelta = new Vector2(hours[0].sizeDelta.x, height);
		}
	}

	public void DisplayWeekDays(RectTransform[] weekDays, int maxHeight) {
		float maxWeekDay = weekDaysSplit.Max();
		for (int i = 0; i < 7; i++) {
			float height = ((float)weekDaysSplit[i] / maxWeekDay);
			height *= (float)maxHeight;
			height *= 0.95f;
			height += 0.05f * maxHeight;
			weekDays[i].sizeDelta = new Vector2(weekDays[0].sizeDelta.x, height);
		}
	}

	public Sprite IconSprite {
		get {
			return PlacesRanking.instance.categoriesDictionary[categoryId].smallIcon;
		}
	}
}
