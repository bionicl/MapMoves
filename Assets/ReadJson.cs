using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public enum ActivityType {
	walking,
	transport,
	cycling,
	train,
	dancing,
	bus,
	tram,
	running,
	car,
	underground,
	airplane
}

public class FullStoryLine {
	public MovesJson[] day;
}
public class MovesJson {
	
	public class SummaryInfo {
		public double distance;
		public double duration;
		public double calories;
		public double steps;
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
			public double distance;
			public float duration;
			public double calories;
			public TrackPointsInfo[] trackPoints;
			public double steps;
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
	public double caloriesIdle;
}

public class ReadJson : MonoBehaviour {
	public static ReadJson instance;

	public RectTransform historySpawn;
	public GameObject activityPrefab;
	[HideInInspector]
	public List<ActivityUI> activitiesList;

	public Dictionary<DateTime, MovesJson> days = new Dictionary<DateTime, MovesJson>();
	public DateTime selectedDay;

	void Awake() {
		instance = this;
	}

	void Start() {
		LoadFiles();
		CheckIfCanDraw();
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			selectedDay = selectedDay.Subtract(new TimeSpan(1, 0, 0, 0));
			CheckIfCanDraw();
		} else if (Input.GetKeyDown(KeyCode.RightArrow)) {
			selectedDay = selectedDay.AddDays(1);
			CheckIfCanDraw();
		}
	}

	// Loading json files
	void LoadFiles() {
		TextAsset jsonData = Resources.Load("storyline") as TextAsset;
		string text = "{ day: " + jsonData.text + "}";
		FullStoryLine m = JsonConvert.DeserializeObject<FullStoryLine>(text);
		Debug.Log("OK2");
		foreach (var item in m.day) {
			Debug.Log("Loaded day " + item.date);
			DateTime timelineDay = ReturnSimpleDate(item.date);
			MovesJson temp = new MovesJson();
			if (!days.TryGetValue(timelineDay, out temp)) {
				days.Add(timelineDay, item);
				selectedDay = timelineDay;
			}
		}
	}

	// Drawing Timeline
	void CheckIfCanDraw() {
		MovesJson timeline = new MovesJson();
		bool loaded = days.TryGetValue(selectedDay, out timeline);
		if (loaded) {
			foreach (Transform child in historySpawn.gameObject.transform) {
				Destroy(child.gameObject);
			}
			activitiesList.Clear();
			DrawTimeline(timeline);
		} else {
			Debug.Log("Can't find day!");
		}
	}
	void DrawTimeline(MovesJson m) {
		foreach (var item in m.segments) {
			if (item.place == null) {
				for (int i = 0; i < item.activities.Length; i++) {
					SpawnActivity(item.activities[i].activity, item.activities[i].distance, item.activities[i].duration, ReturnDateTime(item.activities[i].endTime));
				}
			} else {
				double distance = 0;
				if (item.activities != null)
					foreach (var item2 in item.activities) {
						distance += item2.distance;
					}
				SpawnActivity(null, distance, CalculateTime(item.startTime, item.endTime), ReturnDateTime(item.endTime), item.place.name);
			}
		}
		ValidateIfNoReapeted();
	}
	void SpawnActivity(ActivityType? type, double distance, float time, DateTime endTime, string placeName = null) {
		GameObject activity = Instantiate(activityPrefab, historySpawn.transform.position, historySpawn.transform.rotation);
		RectTransform activityRect = activity.GetComponent<RectTransform>();
		activity.transform.SetParent(historySpawn.transform);
		activityRect.localScale = activityRect.lossyScale;
		activitiesList.Add(activity.GetComponent<ActivityUI>());
		activity.GetComponent<ActivityUI>().Setup(type, distance, time, endTime, placeName);
	}
	void ValidateIfNoReapeted() {
		for (int i = 0; i < activitiesList.Count - 1; i++) {
			if (activitiesList[i].type == activitiesList[i + 1].type) {
				if (activitiesList[i].type != null || (activitiesList[i].type == null && activitiesList[i].placename == activitiesList[i + 1].placename)) {
					activitiesList[i].AddToExisting(activitiesList[i + 1].distance, activitiesList[i + 1].time, activitiesList[i + 1].endTime);
					activitiesList[i + 1].DestroyActivity();
				}
			}
		}
	}

	// Helpers
	DateTime ReturnDateTime(string stringTime) {
		DateTime a = new DateTime(
			Convert.ToInt32(stringTime.Substring(0, 4)),
			Convert.ToInt32(stringTime.Substring(4, 2)),
			Convert.ToInt32(stringTime.Substring(6, 2)),
			Convert.ToInt32(stringTime.Substring(9, 2)),
			Convert.ToInt32(stringTime.Substring(11, 2)),
			Convert.ToInt32(stringTime.Substring(13, 2)));
		return a;
	}
	float CalculateTime(string start, string end) {
		//"endTime": "20180411T083834+0200",
		DateTime a = ReturnDateTime(start);
		DateTime b = ReturnDateTime(end);
		return (float)b.Subtract(a).TotalSeconds;
	}
	DateTime ReturnSimpleDate(string date) {
		return new DateTime(
			Convert.ToInt32(date.Substring(0, 4)),
			Convert.ToInt32(date.Substring(4, 2)),
			Convert.ToInt32(date.Substring(6, 2)));
	}

}
