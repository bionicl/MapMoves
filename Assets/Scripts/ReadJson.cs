using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SFB;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

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
	airplane,	//6033EE	10
	boat,
	escalator,
	ferry,
	funicular,
	motorcycle,
	sailing,
	scooter,
	cross_country_skiing,
	downhill_skiing,
	golfing,
	kayaking,
	paddling,
	paintball,
	riding,
	roller_skiing,
	rollerblading,
	rollerskating,
	rowing,
	skateboarding,
	skating,
	snowboarding,
	snowshoeing,
	wheel_chair,
	indoor_cycling,
	running_on_treadmill,
	walking_on_treadmill,
	aerobics,
	american_football,
	badminton,
	ballet,
	bandy,
	baseball,
	basketball,
	beach_volleyball,
	bodypump,
	bowling,
	boxing,
	circuit_training,
	cleaning,
	climbing,
	cricket,
	curling,
	disc_ultimate,
	elliptical_training,
	fencing,
	floorball,
	gym_training,
	gymnastics,
	handball,
	hockey,
	kettlebell,
	kite_surfing,
	lacrosse,
	martial_arts,
	parkour,
	petanque,
	pilates,
	polo,
	racquetball,
	rugby,
	scuba_diving,
	soccer,
	spinning,
	squash,
	stair_climbing,
	stretching,
	surfing,
	swimming,
	table_tennis,
	tennis,
	volleyball,
	water_polo,
	weight_training,
	windsurfing,
	wrestling,
	yoga,
	zumba,
}

public enum PlaceType {
	school,
	facebook,
	foursquare,
	home,
	work,
	user,
	unknown
}

public class FullStoryLine {
	public MovesJson[] day;
}
[Serializable]
public class MovesJson {
	[Serializable]
	public class SummaryInfo {
		public double distance;
		public double duration;
		public double calories;
		public double steps;
		public string group;
		public ActivityType activity;
	}
	[Serializable]
	public class SegmentsInfo {
		[Serializable]
		public class ActivitiesInfo {
			[Serializable]
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
		[Serializable]
		public class PlaceInfo {
			[Serializable]
			public class LocationInfo {
				public float lon;
				public float lat;
			}
			public long id;
			public LocationInfo location;
			public string name;
			public PlaceType type;
			public string facebookPlaceId;
			public string foursquareId;
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

[Serializable]
public class DayClass {
	public DateTime date;
	public MovesJson day;
	//public ChartItem chart;
	public int dayNumber;
	public bool canChangeWeight;


	public DayClass(DateTime date, MovesJson day, int number, bool canChangeWeight) {
		this.date = date;
		this.day = day;
		dayNumber = number;
		this.canChangeWeight = canChangeWeight;
	}

	public DayClass() {
		
	}
}

public class ReadJson : MonoBehaviour {
	public static ReadJson instance;
	public static Color[] colors;
	public static Color PlaceColor;

	[Header("Day timeline")]
	public RectTransform historySpawn;
	public GameObject activityPrefab;
	[HideInInspector]
	public List<ActivityUI> activitiesList;
	public Color[] activitesColor;
	public Color placeColor;
	public GameObject blankPlaceholder;
	public GameObject dateTimeArrows;

	public Text[] selectedDayDateText;
	public Animator animator;
	float startHoldingTime;

	public Dictionary<DateTime, DayClass> days = new Dictionary<DateTime, DayClass>();
	public DateTime selectedDay;
	public DateTime? firstDate;
	public DateTime lastDate;

	[Header("Summary")]
	public GameObject summariesGO;
	public GameObject summaryPrefab;

	[Header("Uploaded files")]
	public List<string> uploadedFiles;
	public FilesBox filesBox;
	List<DayClass> daysToDraw = new List<DayClass>();
	public GameObject loadingDialogGo;
	public Text loadingDialogText;
	List<SummaryItem> summaries = new List<SummaryItem>();

	void Awake() {
		instance = this;
		colors = activitesColor;
		PlaceColor = placeColor;
	}
	void Start() {
		SaveSystem.Load();
		//OpenFileDialog();
		blankPlaceholder.SetActive(true);
		dateTimeArrows.SetActive(false);
		if (uploadedFiles.Count > 0) {
			CalculationAfterLoadedFiles(false);
			filesBox.SetupTexts(uploadedFiles);
			CheckIfCanDraw();
		}

	}

	void Update() {
		CheckArrows();
	}

	// SwitchingDays
	void CheckArrows() {
		if (days.Count <= 3)
			return;
		if (Input.GetKey(KeyCode.LeftArrow)) {
			ChangeLeft();
			Calendar.instance.SetDay(selectedDay);
		} else if (Input.GetKey(KeyCode.RightArrow)) {
			ChangeRight();
			Calendar.instance.SetDay(selectedDay);
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

	// Files UI
	public void OpenMoreFilesButton() {
		OpenFileDialog();
	}
	public void RestartFiles() {
		days.Clear();
		RenderMap.instance.Clear();
		PlacesRanking.instance.Clear();
		dayNumber = 0;
		uploadedFiles.Clear();
		FilesBox.instance.SetupTexts(uploadedFiles);
		SaveSystem.Save();
		PlacesSave.Clear();
		blankPlaceholder.SetActive(true);
		dateTimeArrows.SetActive(false);
		TopBar.instance.Clear();
	}

	// Opening files
	void OpenFileDialog() {
		CameraDrag.instance.blocked = true;
		var extensions = new[] {
			new ExtensionFilter("Arc app GPX or Moves json", "gpx", "json")
		};

		string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);
		Debug.Log("Starting loading...");
		StartCoroutine(LoadFilesAsync(paths));
	}
	IEnumerator LoadFilesAsync(string[] paths) {
		loadingDialogGo.SetActive(true);
		UpdateLoadingText("Starting");
		yield return new WaitForSeconds(0.5f);
		int fileNumer = 1;
		List<string> exceptionsWhileLoading = new List<string>();
		foreach (var item in paths) {
			string tempItem = item.Replace("file://", "").Replace("%20", " ");

			if (item.ToLower().EndsWith("gpx")) {
				UpdateLoadingText(string.Format("Opening {0} GPX file", fileNumer + "/" + paths.Length));
				yield return new WaitForSeconds(0.1f);
				string jsonData = File.ReadAllText(tempItem);
				UpdateLoadingText(string.Format("Importing {0} GPX file", fileNumer + "/" + paths.Length));
				yield return new WaitForSeconds(0.1f);
				try {
					LoadFiles(ConverterLibrary.ArcExportConverter.ConvertGpxToJson(jsonData, 1), true);
					uploadedFiles.Add(GetFileName(tempItem));
				} catch (Exception ex) {
					exceptionsWhileLoading.Add("Couldn't open file #" + fileNumer + " : " + tempItem);
				}

			} else if (item.ToLower().EndsWith("json")) {
				UpdateLoadingText(string.Format("Opening {0} JSON file", fileNumer + "/" + paths.Length));
				yield return new WaitForSeconds(0.1f);
				string jsonData = File.ReadAllText(tempItem);
				UpdateLoadingText(string.Format("Importing {0} JSON file", fileNumer + "/" + paths.Length));
				yield return new WaitForSeconds(0.1f);
				try {
					LoadFiles(jsonData, false);
					uploadedFiles.Add(GetFileName(tempItem));
				} catch (Exception ex) {
					exceptionsWhileLoading.Add("Couldn't open file #" + fileNumer + " : " + tempItem);
				}
			}
			fileNumer++;
		}
		UpdateLoadingText("Finishing");
		yield return new WaitForSeconds(0.1f);
		if (daysToDraw.Count > 0) {
			CalculationAfterLoadedFiles();
			filesBox.SetupTexts(uploadedFiles);
			CheckIfCanDraw(); // Timeline draw
		}
		UpdateLoadingText("Done!", false);
		CameraDrag.instance.blocked = false;
		Debug.Log("UNBLOCKED");
		yield return new WaitForSeconds(0.5f);
		if (exceptionsWhileLoading.Count > 0) {
			loadingDialogText.text = "";
			foreach (var item in exceptionsWhileLoading) {
				loadingDialogText.text += item + "\n\n";
			}
			yield return new WaitForSeconds(5f);
		}
		loadingDialogGo.SetActive(false);
	}

	void UpdateLoadingText(string updatedText, bool addDots = true) {
		loadingDialogText.text = updatedText;
		if (addDots)
			loadingDialogText.text += "...";

	}

	string GetFileName(string path) {
		string output = "";
		for (int i = 0; i < path.Length; i++) {
			if (path[i] == '/') {
				output = "";
			} else {
				output += path[i];
			}
		}
		Debug.Log("File name: " + output);
		return output;
	}

	// Loading json files
	public int dayNumber;
	void LoadFiles(string jsonData, bool canChangeWeight) {
		string text = "{ day: " + jsonData + "}";
		FullStoryLine m = JsonConvert.DeserializeObject<FullStoryLine>(text);
		foreach (var item in m.day) {
			DateTime timelineDay = ReturnSimpleDate(item.date);
			if (!days.ContainsKey(timelineDay) && item.summary != null) {
				DayClass tempDay = new DayClass(timelineDay, item, dayNumber++, canChangeWeight);
				days.Add(timelineDay, tempDay);
				daysToDraw.Add(tempDay);
			}
		}
		//RenderMap.instance.ChangeDaysRangeFilter(new DateTime(2018, 03, 01), new DateTime(2018, 03, 10));
	}
	void CalculationAfterLoadedFiles(bool daysToDrawLoaded = true) {
		if (!daysToDrawLoaded) {
			foreach (var item in days.Values) {
				daysToDraw.Add(item);
			}
		}
		if (daysToDraw.Count == 0)
			return;
		long drawnPoints = 0;
		foreach (var item in daysToDraw) {
			PlacesRanking.instance.AnalyseDay(item.day);
			ChartUI.instance.CheckMaxCalories(item.day);
			drawnPoints += RenderMap.instance.RenderDay(item.day);
		}
		Debug.Log("Drawn points: " + drawnPoints.ToString());
		PlacesRanking.instance.SortAndDisplay();
		ChartUI.instance.SetupCharts();

		firstDate = days.Keys.Min();
		lastDate = days.Keys.Max();
		selectedDay = lastDate;
		daysToDraw.Clear();

		blankPlaceholder.SetActive(false);
		dateTimeArrows.SetActive(true);
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
					if (child.gameObject.GetComponent<ActivityUI>() != null)
						Destroy(child.gameObject);
				}
				foreach (Transform child in summariesGO.transform) {
					if (child.gameObject.GetComponent<SummaryItem>() != null)
						Destroy(child.gameObject);
				}
				activitiesList.Clear();
				summaries.Clear();
				DrawTimeline(timeline.day, timeline.canChangeWeight);
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
	void DrawTimeline(MovesJson m, bool canChangeWeight) {
		animator.SetTrigger("Refresh");
		foreach (var item in selectedDayDateText) {
			item.text = ReturnSimpleDate(m.date).ToString("dd MMMMM yyyy");
		}
		ChartUI.instance.CheckChartSelected();
		StartCoroutine(RenderAfterTime(m, canChangeWeight));
	}
	IEnumerator RenderAfterTime(MovesJson m, bool canChangeWeight) {
		yield return new WaitForSeconds(0.3f);

		// Summary
		foreach (var item in m.summary) {
			SpawnSummary(item, canChangeWeight);
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
	void SpawnSummary(MovesJson.SummaryInfo summary, bool canChangeWeight) {
		if (summary.group == "transport" || summary.duration < 60)
			return;
		GameObject summaryObject = Instantiate(summaryPrefab, historySpawn.transform.position, historySpawn.transform.rotation);
		RectTransform summaryObjectRect = summaryObject.GetComponent<RectTransform>();
		summaryObject.transform.SetParent(summariesGO.transform);
		summaryObjectRect.localScale = summaryObjectRect.lossyScale;
		SummaryItem summaryItem = summaryObject.GetComponent<SummaryItem>();
		summaryItem.Setup(summary, canChangeWeight);
		summaries.Add(summaryItem);
	}
	void SpawnActivity(ActivityType? type, double distance, float time, DateTime endTime, MovesJson.SegmentsInfo.PlaceInfo placeInfo = null) {
		if (time < 60)
			return;
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
	public void RefreshSummaries(bool weight, bool distance) {
		foreach (var item in summaries) {
			item.Refresh();
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
