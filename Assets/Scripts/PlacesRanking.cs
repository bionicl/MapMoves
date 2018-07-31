using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacesRanking : MonoBehaviour {
	public static PlacesRanking instance;

	public PlaceMainCategory[] mainCategories;
	public PlaceCategory[] categories;

	public int theBiggestPlaceVisits = 110;
	[HideInInspector]
	public int maxValue;

	public Dictionary<long, PlaceGroup> places = new Dictionary<long, PlaceGroup>();

	void Awake() {
		instance = this;
		for (int i = 0; i < categories.Length; i++) {
			categories[i].id = i;
		}
		//categories = categories.OrderBy(c => c.category).ToArray();
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

	// Searching
	public List<PlaceGroup> FindStartingWith(string text) {
		List<PlaceGroup> output = new List<PlaceGroup>();
		foreach (var item in places) {
			string placeName = item.Value.placeInfo.name.ToLower();
			if (placeName.StartsWith(text.ToLower()))
				output.Add(item.Value);
		}
		return output;
	}
	public List<PlaceGroup> FindContaining(string text, List<PlaceGroup> startingList) {
		List<PlaceGroup> output = new List<PlaceGroup>();
		foreach (var item in places) {
			string placeName = item.Value.placeInfo.name.ToLower();
			if (placeName.Contains(text.ToLower()) && !startingList.Contains(item.Value))
				output.Add(item.Value);
		}
		return output;
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
