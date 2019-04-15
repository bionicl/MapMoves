using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using UnityEngine.UI;
using System.Linq;

public class GooglePhotosResponse {
	public MediaItem[] mediaItems;

	public void CalculateDates() {
		foreach (var item in mediaItems) {
			item.creationDate = DateTime.Parse(item.mediaMetadata.creationTime);
		}
	}
}

public class MediaItem {
	public string id;
	public string description;
	public string productUrl;
	public string baseUrl;
	public string filename;
	public MediaMetadata mediaMetadata;
	public DateTime creationDate;
}

public class MediaMetadata {
	public string creationTime;
}

public class GooglePhotosApi : MonoBehaviour {
	public static GooglePhotosApi instance;

	public Image imageToDisplay;

	private void Awake() {
		instance = this;
	}

	public void SendRequest() {
		//OutputTimeLineItems();
		StartCoroutine(Upload());
	}


	IEnumerator Upload() {
		DateTime day = ReadJson.instance.selectedDay;
		Debug.Log(day);
		string body = "{\"pageSize\":100,\"filters\":{\"dateFilter\":{\"dates\":[{\"day\":" + day.Day + ",\"month\":" + day.Month + ",\"year\":" + day.Year + "}]}}}";
		Debug.Log(body);

		var request = new UnityWebRequest("https://photoslibrary.googleapis.com/v1/mediaItems:search", "POST");
		byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
		request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
		request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");

		string accessToken = "???";
		request.SetRequestHeader("Authorization", "Bearer " + accessToken);

		yield return request.SendWebRequest();

		Debug.Log("Status Code: " + request.responseCode);

		if (request.isNetworkError || request.isHttpError) {
			Debug.Log(request.error);
		} else {
			Debug.Log("Form upload complete!");
			Debug.Log(request.downloadHandler.text);
			ReadJSONResponse(request.downloadHandler.text);
		}
	}

	void ReadJSONResponse(string text) {
		GooglePhotosResponse response = JsonConvert.DeserializeObject<GooglePhotosResponse>(text);
		response.CalculateDates();
		AddPhotosToTimeLineItems(response);
		//StartCoroutine(DownloadImage(response.mediaItems[0].baseUrl + "=w400-h400-c", imageToDisplay));

	}

	void AddPhotosToTimeLineItems(GooglePhotosResponse response) {
		List<MediaItem> images = response.mediaItems.ToList();

		List<ActivityUI> timeline = ReadJson.instance.activitiesList;
		List<MediaItem> tempImages = new List<MediaItem>();
		List<ActivityUI> timeLinesWithPhotos = new List<ActivityUI>();
		DateTime startTime = new DateTime();
		for (int i = timeline.Count - 1; i >= 0; i--) {
			tempImages.Clear();
			if (i - 1 >= 0)
				startTime = timeline[i - 1].endTime;
			else {
				DateTime tempD = timeline[i].endTime;
				startTime = new DateTime(tempD.Year, tempD.Month, tempD.Day, 0, 0, 0);
			}
			for (int j = 0; j < images.Count; j++) {
				//Debug.Log("TIME RANGE: " + timeline[i].endTime + " <> " + startTime);
				if (images[j].creationDate < timeline[i].endTime && images[j].creationDate >= startTime) {
					tempImages.Add(images[j]);
					//images.Remove(images[j]);
				}
			}
			if (tempImages.Count > 0) {
				
				timeLinesWithPhotos.Add(timeline[i]);
				Debug.Log($"Adding {tempImages.Count} images...");
				timeline[i].DownloadPhotos(tempImages);

			}
		}
		//foreach (var item in timeLinesWithPhotos) {
		//	item.DownloadPhotos();
		//}
	}

	public IEnumerator DownloadImage(string url, Image targetImage) {
		// Start a download of the given URL
		var www = new WWW(url);
		// wait until the download is done
		yield return www;
		// Create a texture in DXT1 format
		Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);

		// assign the downloaded image to sprite
		www.LoadImageIntoTexture(texture);
		Rect rec = new Rect(0, 0, texture.width, texture.height);
		Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);

		Debug.Log("Downloaded image!");
		targetImage.sprite = spriteToUse;

		www.Dispose();
		www = null;
	}
}
