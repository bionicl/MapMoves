using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaceGroup {
	public Place mapObject;
	public ActivityUI timelineObject;
	public MovesJson.SegmentsInfo.PlaceInfo placeInfo;
	public Sprite icon;
	public int timesVisited;

	public PlaceGroup() {
		
	}
	public PlaceGroup (MovesJson.SegmentsInfo.PlaceInfo placeinfo) {
		this.placeInfo = placeinfo;  
		GlobalVariables.inst.SetIcon(placeinfo, (Sprite sprite) => RefreshIcons(sprite));
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

	public void RefreshSize() {
		if (mapObject == null)
			return;
		float scale = 0.1f;
		scale = (float)timesVisited / (float)PlacesRanking.instance.maxValue * 0.3f;
		scale = Mathf.Clamp(scale, 0.1f, 0.3f);
		mapObject.transform.localScale = new Vector3(scale, scale, 1f);
		Vector3 mapObjectPosition = mapObject.transform.position;
		float multiply = scale;
		multiply *= -1;
		mapObjectPosition.z = multiply;
		mapObject.transform.position = mapObjectPosition;
	}
	void RefreshIcons(Sprite sprite) {
		icon = sprite;
		if (mapObject != null) {
			mapObject.icon.sprite = icon;
		}
		if (timelineObject != null) {
			timelineObject.placeIcon.sprite = icon;
		}
	}
	void RefreshIcons() {
		if (mapObject != null) {
			mapObject.icon.sprite = icon;
		}
		if (timelineObject != null) {
			timelineObject.placeIcon.sprite = icon;
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
			if (item.place != null) {
				AddOrSetupRanking(item.place);
			}
		}
	}
	void AddOrSetupRanking(MovesJson.SegmentsInfo.PlaceInfo place) {
		PlaceGroup placeTarget = new PlaceGroup();
		if (places.TryGetValue(place.id, out placeTarget)) {
			placeTarget.timesVisited += 1;
		} else {
			places.Add(place.id, new PlaceGroup(place));
		}
	}

	public PlaceGroup FindPlace(MovesJson.SegmentsInfo.PlaceInfo place, Place mapObject) {
		PlaceGroup thisPlace = new PlaceGroup();
		if (places.TryGetValue(place.id, out thisPlace)) {
			thisPlace.AddMapObject(mapObject);
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
		foreach (KeyValuePair<PlaceGroup, int> item in ranking) {
			Debug.Log(item.Key.placeInfo.name + "(" + 
			          item.Key.placeInfo.type +") - " + item.Value.ToString());
		}
		foreach (var item in places.Values) {
			item.RefreshSize();
		}
	}
}
