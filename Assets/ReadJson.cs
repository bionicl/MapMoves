using System;
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
	public static ReadJson instance;

	public RectTransform historySpawn;
	public GameObject activityPrefab;
	public List<ActivityUI> activitiesList;

	void Awake() {
		instance = this;
	}

	void Start() {
		TextAsset jsonData = Resources.Load("jsonstoryline2") as TextAsset;
		MovesJson m = JsonConvert.DeserializeObject<MovesJson>(jsonData.text);
		foreach (var item in m.segments) {
			if (item.place == null) {
				for (int i = 0; i < item.activities.Length; i++) {
					SpawnActivity(item.activities[i].activity, item.activities[i].distance, item.activities[i].duration);
				}
			} else {
				int distance = 0;
				foreach (var item2 in item.activities) {
					distance += item2.distance;
				}
				SpawnActivity(null, distance, CalculateTime(item.startTime, item.endTime), item.place.name);
			}
		}
		ValidateIfNoReapeted();
	}

	float CalculateTime(string start, string end) {
		//"endTime": "20180411T083834+0200",
		DateTime a = new DateTime(
			Convert.ToInt32(start.Substring(0,4)),
			Convert.ToInt32(start.Substring(4, 2)),
			Convert.ToInt32(start.Substring(6, 2)),
			Convert.ToInt32(start.Substring(9, 2)),
			Convert.ToInt32(start.Substring(11, 2)),
			Convert.ToInt32(start.Substring(13, 2)));
		DateTime b = new DateTime(
			Convert.ToInt32(end.Substring(0, 4)),
			Convert.ToInt32(end.Substring(4, 2)),
			Convert.ToInt32(end.Substring(6, 2)),
			Convert.ToInt32(end.Substring(9, 2)),
			Convert.ToInt32(end.Substring(11, 2)),
			Convert.ToInt32(end.Substring(13, 2)));
		return (float)b.Subtract(a).TotalSeconds;
	}
	void ValidateIfNoReapeted() {
		for (int i = 0; i < activitiesList.Count - 1; i++) {
			if (activitiesList[i].type == activitiesList[i+1].type) {
				if (activitiesList[i].type != null || (activitiesList[i].type == null && activitiesList[i].placename == activitiesList[i + 1].placename)) {
					activitiesList[i].AddToExisting(activitiesList[i + 1].distance, activitiesList[i + 1].time);
					activitiesList[i + 1].DestroyActivity();
				}
			}
		}
	}

	void SpawnActivity(ActivityType? type, int distance, float time, string placeName = null) {
		GameObject activity = Instantiate(activityPrefab, historySpawn.transform.position, historySpawn.transform.rotation);
		RectTransform activityRect = activity.GetComponent<RectTransform>();
		activity.transform.SetParent(historySpawn.transform);
		activityRect.localScale = activityRect.lossyScale;
		activitiesList.Add(activity.GetComponent<ActivityUI>());
		activity.GetComponent<ActivityUI>().Setup(type, distance, time, placeName);
	}
}
