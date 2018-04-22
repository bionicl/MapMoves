using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacesRanking : MonoBehaviour {
	public static PlacesRanking instance;

	public Dictionary<long, MovesJson.SegmentsInfo.PlaceInfo> places = new Dictionary<long, MovesJson.SegmentsInfo.PlaceInfo>();
	public Dictionary<long, int> placesRanking = new Dictionary<long, int>();

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
		bool found = placesRanking.ContainsKey(place.id);
		if (found) {
			placesRanking[place.id] += 1;
		} else {
			places.Add(place.id, place);
			placesRanking.Add(place.id, 1);
		}
	}

	public void SortAndDisplay() {
		placesRanking = placesRanking.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
		foreach (KeyValuePair<long, int> item in placesRanking) {
			//Debug.Log(places[item.Key].name + "(" + places[item.Key].type +") - " + placesRanking[item.Key]);
		}
	}
}
