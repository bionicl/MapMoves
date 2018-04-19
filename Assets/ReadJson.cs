using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

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

	public Text[] selectedDayDateText;
	public Animator animator;
	float startHoldingTime;

	public Dictionary<DateTime, MovesJson> days = new Dictionary<DateTime, MovesJson>();
	public DateTime selectedDay;
	public DateTime? firstDate;
	public DateTime lastDate;

	void Awake() {
		instance = this;
	}
	void Start() {
		LoadFiles();
		CheckIfCanDraw();
	}

	void Update() {
		CheckArrows();
	}

	// KeySwitching
	void CheckArrows() {
		if (Input.GetKey(KeyCode.LeftArrow)) {
			if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				startHoldingTime = Time.timeSinceLevelLoad;
				selectedDay = selectedDay.AddDays(-1);
				CheckIfCanDraw(false);
			}
			if (Time.timeSinceLevelLoad - startHoldingTime >= 4f) {
				selectedDay = selectedDay.AddDays(-2);
				CheckIfCanDraw(false);
			} else if (Time.timeSinceLevelLoad - startHoldingTime >= 0.5f) {
				selectedDay = selectedDay.AddDays(-1);
				CheckIfCanDraw(false);
			}
		} else if (Input.GetKey(KeyCode.RightArrow)) {
			if (Input.GetKeyDown(KeyCode.RightArrow)) {
				startHoldingTime = Time.timeSinceLevelLoad;
				selectedDay = selectedDay.AddDays(1);
				CheckIfCanDraw();
			}
			if (Time.timeSinceLevelLoad - startHoldingTime >= 4f) {
				selectedDay = selectedDay.AddDays(2);
				CheckIfCanDraw(false);
			} else if (Time.timeSinceLevelLoad - startHoldingTime >= 0.5f) {
				selectedDay = selectedDay.AddDays(1);
				CheckIfCanDraw();
			}
		}
	}

	// Loading json files
	void LoadFiles() {
		TextAsset jsonData = Resources.Load("storyline") as TextAsset;
		string text = "{ day: " + jsonData.text + "}";
		FullStoryLine m = JsonConvert.DeserializeObject<FullStoryLine>(text);
		foreach (var item in m.day) {
			DateTime timelineDay = ReturnSimpleDate(item.date);
			MovesJson temp = new MovesJson();
			if (!days.TryGetValue(timelineDay, out temp)) {
				days.Add(timelineDay, item);
				selectedDay = timelineDay;
				if (firstDate == null)
					firstDate = timelineDay;
				lastDate = timelineDay;
			}
			PlacesRanking.instance.AnalyseDay(item);
		}
		PlacesRanking.instance.SortAndDisplay();
	}

	// Drawing Timeline
	void CheckIfCanDraw(bool rightDirection = true) {
		StopAllCoroutines();
		bool loaded = false;
		while (loaded == false) {
			MovesJson timeline = new MovesJson();
			loaded = days.TryGetValue(selectedDay, out timeline);
			if (loaded) {
				foreach (Transform child in historySpawn.gameObject.transform) {
					if (child.gameObject.GetComponent<ActivityUI>() != null)
						Destroy(child.gameObject);
				}
				activitiesList.Clear();
				DrawTimeline(timeline);
			} else {
				if (selectedDay == lastDate.AddDays(1))
					selectedDay = selectedDay.AddDays(-1);
				else if (selectedDay == firstDate.Value.AddDays(-1))
					selectedDay = selectedDay.AddDays(1);
				else {
					if (rightDirection) {
						selectedDay = selectedDay.AddDays(1);
					} else {
						selectedDay = selectedDay.AddDays(-1);
					}
				}
			}
		}
	}
	void DrawTimeline(MovesJson m) {
		animator.SetTrigger("Refresh");
		foreach (var item in selectedDayDateText) {
			item.text = ReturnSimpleDate(m.date).ToString("dd MMMMM yyyy");
		}
		StartCoroutine(RenderAfterTime(m));
	}
	IEnumerator RenderAfterTime(MovesJson m) {
		yield return new WaitForSeconds(0.3f);
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
