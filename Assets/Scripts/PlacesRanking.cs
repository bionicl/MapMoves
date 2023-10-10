using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlacesDataJson;
using Newtonsoft.Json;

public class PlacesRanking : MonoBehaviour {
	public static PlacesRanking instance;

	[HideInInspector] public List<PlaceMainCategory> mainCategories;
	[HideInInspector] public List<PlaceCategory> categories;
	[HideInInspector] public Dictionary<string, PlaceCategory> categoriesDictionary = new Dictionary<string, PlaceCategory>();

	public int theBiggestPlaceVisits = 110;
	[HideInInspector]
	public int maxValue;

	public Dictionary<long, PlaceGroup> places = new Dictionary<long, PlaceGroup>();

	void Awake() {
		instance = this;

		// Load categories
		UnityEngine.Object textFile;
		textFile = Resources.Load("PlacesData");
		TextAsset temp = textFile as TextAsset;
		PlacesDataJsonRoot root = JsonConvert.DeserializeObject<PlacesDataJsonRoot>(temp.text);
		mainCategories = root.placeMainCategories;
		Debug.Log("Loaded " + mainCategories.Count + " main categories");
		categories = root.placeCategories;
		Debug.Log("Loaded " + categories.Count + " categories");
		categories = categories.OrderBy(c => c.category).ToList();
		foreach (var item in categories) {
			categoriesDictionary.Add(item.id, item);
		}

		// Setup categories and icons
		for (int i = 0; i < categories.Count; i++) {
			categories[i].SetupPlaceTypeCategory(mainCategories);
			categories[i].SetupIcons();
		}
	}

	public void AnalyseDay(MovesJson day) {
		foreach (var item in day.segments) {
			if (item.place != null) {
				if (item.place.name == null)
					item.place.name = "???";
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
			// TryRecogniseCategory(place.name, (categoryId) => {
			// 	placeTarget.categoryId = categoryId;
			// });
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

	public void Clear() {
		places.Clear();
	}

	public void SortAndDisplay() {
		Dictionary<PlaceGroup, int> ranking = new Dictionary<PlaceGroup, int>();
		foreach (var item in places) {
			ranking.Add(item.Value, item.Value.timesVisited);
		}
		if (ranking.Count == 0)
			return;
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
		GlobalVariables.instance.MoveCamera(ranking.First().Key.mapObject.transform.position);
	}
	public void ChangePlacesSize(float mapSize) {
		foreach (var item in places) {
			item.Value.SetZoomSize(mapSize);
		}
	}

	// Helpers
	public void TryRecogniseCategory(string placeName, Action<string> onSuccess) {
		string placeNameLower = placeName.ToLower();
		foreach (var item in categories) {
			string outputCategory = item.CheckIfMatchesKeywords(placeNameLower);
			if (outputCategory != null) {
				onSuccess(outputCategory);
				return;
			}
		}
		foreach (var item in categories) {
			string outputCategory = item.CheckIfMatchesChains(placeNameLower);
			if (outputCategory != null) {
				onSuccess(outputCategory);
				return;
			}
		}
	}
}
