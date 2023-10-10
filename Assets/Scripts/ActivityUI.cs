using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ActivityUI : MonoBehaviour {

	public GameObject place;
	public Image placeIcon;
	public Image move;
	public Text Header;
	public Text Subheader;
	public Text endTimeText;
	public int[] moveHeights = {30, 40, 60};
	public int[] placeHeights = {60, 70, 80};

	public Text MoveType;
	public Text MoveTime;
	public Text MoveMinText;

	public ActivityType? type;
	public double distance;
	public float time;
	public TimeSpan timeSpan;
	public string placename;
	public DateTime endTime;
	public PlaceSourceType? placeType;
	public string placeFbId;
	public PlaceGroup placeGroup;
	public Image placeBoxColor;

	[Header("Photos")]
	public Image[] images;
	public GameObject imagesGo;

	public GameObject activityGo;
	public GameObject placeGo;

	string[] imageURL = new string[3];

	// Images
//List<MediaItem> mediaItems;

	string[] activityTypeText = {
		"Walk",
		"Transport",
		"Cycling",
		"Train",
		"Dancing",
		"Bus",
		"Tram",
		"Running",
		"Car",
		"Underground",
		"Airplane",
		"Boat",
		"Escalator",
		"Ferry",
		"Funicular",
		"Motorcycle",
		"Sailing",
		"Ccooter",
		"Cross country skiing",
		"Downhill skiing",
		"Golfing",
		"Kayaking",
		"Paddling",
		"Paintball",
		"Riding",
		"Roller skiing",
		"Rollerblading",
		"Rollerskating",
		"Rowing",
		"Skateboarding",
		"Skating",
		"Snowboarding",
		"Snowshoeing",
		"Wheel chair"
	};

	public void Setup(ActivityType? type, double distance, float time, DateTime endTime, MovesJson.SegmentsInfo.PlaceInfo placeInfo) {
		this.type = type;
		this.distance = distance;
		this.time = time;
		this.endTime = endTime;
		if (placeInfo != null) {
			placeGroup = PlacesRanking.instance.FindPlace(placeInfo, this);
			this.placename = placeInfo.name;
			this.placeType = placeInfo.type;
			if (placeType == PlaceSourceType.facebook)
				placeFbId = placeInfo.facebookPlaceId;
		}

		TimeSpan t = TimeSpan.FromSeconds(this.time);
		placeBoxColor.gameObject.SetActive(false);
		SetSize(t);
		endTimeText.text = endTime.ToString("HH:mm");

		string timeShort = string.Format("{0}", t.Minutes);
		if (type == null) {
			place.SetActive(true);
			activityGo.SetActive(false);
			placeGo.SetActive(true);

			//place.GetComponent<Image>().color = placeGroup.Category.Category.color;
			//place.GetComponent<Image>().color = Color.white;
			//placeIcon.color = new Color(50, 50, 50);
			move.gameObject.SetActive(false);
			Header.text = placename;
			Subheader.gameObject.SetActive(true);
			Subheader.text = placeGroup.Category.placeTypeCategory.displayName;
			Subheader.text += " - " + String.Format("{0}:{1}", Mathf.Round(Convert.ToSingle(t.TotalHours)), Convert.ToSingle(t.Minutes).ToString().PadLeft(2, '0'));
			Subheader.text += "<size=7> min</size>";
			MoveType.gameObject.SetActive(false);
			MoveTime.gameObject.SetActive(false);
			MoveMinText.gameObject.SetActive(false);
			//if (distance >= 100)
			//	Subheader.text += distance.ToString() + "m";
			if (placeGroup != null && placeGroup.placeInfo != null)
				placeIcon.sprite = placeGroup.IconSprite;
		} else {
			place.SetActive(false);
			activityGo.SetActive(true);
			placeGo.SetActive(false);

			move.gameObject.SetActive(true);
			move.color = ReadJson.colors[(int)type];
			Subheader.text = timeShort + distance.ToString() + "m";
			Header.gameObject.SetActive(false);
			Subheader.gameObject.SetActive(false);
			MoveType.text = activityTypeText[(int)type.Value] + "  ";
			MoveType.color = ReadJson.colors[(int)type];
			MoveTime.text = timeShort;
		}
	}

	public void AddToExisting(double distance, float time, DateTime endTime) {
		this.distance += distance;
		this.time += time;
		this.endTime = endTime;

		TimeSpan t = TimeSpan.FromSeconds(this.time);
		SetSize(t);
		endTimeText.text = string.Format("{0}:{1}", this.endTime.Hour.ToString().PadLeft(2, '0'), this.endTime.Minute.ToString().PadLeft(2, '0'));

		string timeShort = string.Format("{0}min ", t.Minutes);
		if (type == null) {
			Subheader.text = timeShort;
			if (distance >= 100)
				Subheader.text += this.distance.ToString() + "m";
		} else {
			Subheader.text = timeShort + this.distance.ToString() + "m";
		}
	}

	void SetSize(TimeSpan t, bool hasImages = false) {
		this.timeSpan = t;
		int addidtionalHeight = 57;
		if (!hasImages)
			addidtionalHeight = 0;

		int[] height = moveHeights;
		if (this.type == null) {
			height = placeHeights;
			//Debug
			if (t.TotalMinutes >= 75) {
				placeBoxColor.gameObject.SetActive(true);
				placeBoxColor.color = placeGroup.Category.placeTypeCategory.ColorConverted;
			}
		}

		if (t.TotalMinutes < 10)
			GetComponent<RectTransform>().sizeDelta = new Vector2(0, height[0] + addidtionalHeight);
		else if (t.TotalMinutes < 30)
			GetComponent<RectTransform>().sizeDelta = new Vector2(0, height[1] + addidtionalHeight);
		else if (t.TotalMinutes < 60)
			GetComponent<RectTransform>().sizeDelta = new Vector2(0, height[2] + addidtionalHeight);
		else
			GetComponent<RectTransform>().sizeDelta = new Vector2(0, height[3] + addidtionalHeight);
	}

	public void ClickOnPlace() {
		if (placename != null && placeGroup != null)
			RightListUI.instance.NewPlace(placeGroup);
	}

	public void DestroyActivity() {
		ReadJson.instance.activitiesList.Remove(this);
		Destroy(gameObject);
	}

	//public void AddPhotos(List<MediaItem> mediaItems2) {
	//	Debug.Log($"Adding {mediaItems.Count} images...");
	//	mediaItems = mediaItems2;
	//	Debug.Log($"Added {this.mediaItems.Count} images!");
	//}

	public void DownloadPhotos(List<MediaItem> mediaItems) {
		//Debug.Log($"This endtime: {endTime}, Photo time: {mediaItems[0].creationDate}, Photo link: {mediaItems[0].productUrl}");
		imagesGo.SetActive(true);
		SetSize(timeSpan, true);
		int maxRange = 3;
		if (mediaItems.Count < 3) {
			maxRange = mediaItems.Count;
			for (int i = 2; i > 0; i--) {
				if (mediaItems.Count - 1 < i)
					images[i].gameObject.SetActive(false);
			}
		}
		for (int i = 0; i < maxRange; i++) {
			StartCoroutine(GooglePhotosApi.instance.DownloadImage(mediaItems[i].baseUrl + "=w100-h100-c", images[i]));
			imageURL[i] = mediaItems[i].productUrl;
		}
	}

	public void OpenProductLink(int imageId) {
		Application.OpenURL(imageURL[imageId]);
	}

}
