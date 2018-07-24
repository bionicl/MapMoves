using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public enum ActivityType {
	walking,	//00D55A	0
	transport,	//FBE7B1	1
	cycling,	//00CDEC	2
	train,		//FF802D	3
	dancing,	//F32DFF	4
	bus,		//F43838	5
	tram,		//FF802D	6
	running,	//F32DFF	7
	car,		//FFC52D	8
	underground,//FF802D	9
	airplane	//6033EE	10
}

public enum PlaceType {
	school,
	facebook,
	home,
	work,
	user,
	unknown
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
			public PlaceType type;
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

public class DayClass {
	public DateTime date;
	public MovesJson day;
	public ChartItem chart;
	public int dayNumber;

	public DayClass(DateTime date, MovesJson day, int number) {
		this.date = date;
		this.day = day;
		dayNumber = number;
	}

	public DayClass() {
		
	}
}

public class ReadJson : MonoBehaviour {
	public static ReadJson instance;
	public static Color[] colors;
	public static Color PlaceColor;

	public RectTransform historySpawn;
	public GameObject activityPrefab;
	[HideInInspector]
	public List<ActivityUI> activitiesList;
	public Color[] activitesColor;
	public Color placeColor;

	public Text[] selectedDayDateText;
	public Animator animator;
	float startHoldingTime;

	public Dictionary<DateTime, DayClass> days = new Dictionary<DateTime, DayClass>();
	public DateTime selectedDay;
	public DateTime? firstDate;
	public DateTime lastDate;

	[Header("Summary")]
	public GameObject summaryPrefab;

	void Awake() {
		instance = this;
		colors = activitesColor;
		PlaceColor = placeColor;
	}
	void Start() {
		PlacesSave.Load();
		LoadFiles();
		CheckIfCanDraw();
	}

	void Update() {
		CheckArrows();
	}

	// SwitchingDays
	void CheckArrows() {
		if (Input.GetKey(KeyCode.LeftArrow)) {
			ChangeLeft();
		} else if (Input.GetKey(KeyCode.RightArrow)) {
			ChangeRight();
		}
	}
	public void ChangeDay(DateTime day) {
		selectedDay = day;
		CheckIfCanDraw();
	}
	public void ChangeRight() {
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
	public void ChangeLeft() {
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
	}

	// Loading json files
	void LoadFiles() {
		TextAsset jsonData = Resources.Load("storyline") as TextAsset;
		string text = "{ day: " + jsonData.text + "}";
		FullStoryLine m = JsonConvert.DeserializeObject<FullStoryLine>(text);
		int dayNumber = 0;
		foreach (var item in m.day) {
			DateTime timelineDay = ReturnSimpleDate(item.date);
			if (!days.ContainsKey(timelineDay) && item.summary != null) {
				days.Add(timelineDay, new DayClass(timelineDay, item, dayNumber++));
				selectedDay = timelineDay;
				if (firstDate == null)
					firstDate = timelineDay;
				lastDate = timelineDay;
				PlacesRanking.instance.AnalyseDay(item);
				ChartUI.instance.CheckMaxCalories(item);
				RenderMap.instance.RenderDay(item);
			}
		}
		//RenderMap.instance.ChangeDaysRangeFilter(new DateTime(2018, 03, 01), new DateTime(2018, 03, 10));
		PlacesRanking.instance.SortAndDisplay();
		ChartUI.instance.SetupCharts();
	}

	// Drawing Timeline
	void CheckIfCanDraw(bool rightDirection = true) {
		StopAllCoroutines();
		bool loaded = false;
		while (loaded == false) {
			DayClass timeline = new DayClass();
			loaded = days.TryGetValue(selectedDay, out timeline);
			if (loaded) {
				foreach (Transform child in historySpawn.gameObject.transform) {
					if (child.gameObject.GetComponent<ActivityUI>() != null || child.gameObject.GetComponent<SummaryItem>() != null)
						Destroy(child.gameObject);
				}
				activitiesList.Clear();
				DrawTimeline(timeline.day);
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
		ChartUI.instance.CheckChartSelected();
		StartCoroutine(RenderAfterTime(m));
	}
	IEnumerator RenderAfterTime(MovesJson m) {
		yield return new WaitForSeconds(0.3f);

		// Summary
		foreach (var item in m.summary) {
			SpawnSummary(item);
		}

		// ActivityUI
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
				SpawnActivity(null, distance, CalculateTime(item.startTime, item.endTime), ReturnDateTime(item.endTime), item.place);
			}
		}
		ValidateIfNoReapeted();
	}
	void SpawnSummary(MovesJson.SummaryInfo summary) {
		if (summary.group == "transport")
			return;
		GameObject summaryObject = Instantiate(summaryPrefab, historySpawn.transform.position, historySpawn.transform.rotation);
		RectTransform summaryObjectRect = summaryObject.GetComponent<RectTransform>();
		summaryObject.transform.SetParent(historySpawn.transform);
		summaryObjectRect.localScale = summaryObjectRect.lossyScale;
		summaryObject.GetComponent<SummaryItem>().Setup(summary);
	}
	void SpawnActivity(ActivityType? type, double distance, float time, DateTime endTime, MovesJson.SegmentsInfo.PlaceInfo placeInfo = null) {
		GameObject activity = Instantiate(activityPrefab, historySpawn.transform.position, historySpawn.transform.rotation);
		RectTransform activityRect = activity.GetComponent<RectTransform>();
		activity.transform.SetParent(historySpawn.transform);
		activityRect.localScale = activityRect.lossyScale;
		activitiesList.Add(activity.GetComponent<ActivityUI>());
		activity.GetComponent<ActivityUI>().Setup(type, distance, time, endTime, placeInfo);
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
	public static DateTime ReturnDateTime(string stringTime) {
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
	public static DateTime ReturnSimpleDate(string date) {
		return new DateTime(
			Convert.ToInt32(date.Substring(0, 4)),
			Convert.ToInt32(date.Substring(4, 2)),
			Convert.ToInt32(date.Substring(6, 2)));
	}
}
