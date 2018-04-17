using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public enum ActivityType {
	walking,
	transport,
	cycling
}

public class MovesJson {
	
	public class SummaryInfo {
		public int distance;
		public int duration;
		public int calories;
		public int steps;
		public string group;
		public ActivityType activity;
	}
	public class SegmentsInfo {
		public class ActivitiesInfo {
			public class TrackPointsInfo {
				public string time;
				public float lon;
				public float lat;
			}

			public string startTime;
			public bool manual;
			public int distance;
			public int duration;
			public int calories;
			public TrackPointsInfo[] trackPoints;
			public int steps;
			public string endTime;
			public string group;
			public ActivityType activity;
		}
		public class PlaceInfo {
			public class LocationInfo {
				public float lon;
				public float lat;
			}
			public long id;
			public LocationInfo location;
			public string name;
			public string type;
			public string facebookPlaceId;
		}

		public string startTime;
		public string lastUpdate;
		public ActivitiesInfo[] activities;
		public PlaceInfo place;
		public string endTime;
		public string type;
	}

	public SummaryInfo[] summary;
	public SegmentsInfo[] segments;
	public string lastUpdate;
	public string date;
	public int caloriesIdle;
}

public class ReadJson : MonoBehaviour {

	void Start() {
		TextAsset jsonData = Resources.Load("jsonstoryline2") as TextAsset;
		MovesJson m = JsonConvert.DeserializeObject<MovesJson>(jsonData.text);
		foreach (var item in m.segments) {
			string output = item.type + "(";
			int distance = 0;
			foreach (var item2 in item.activities) {
				distance += item2.distance;
			}
			if (item.type == "move") {
				output += distance.ToString() + ")";
			} else {
				output += item.place.name + ", " + distance.ToString() + ")";
			}
			Debug.Log(output);
		}
	}
}
