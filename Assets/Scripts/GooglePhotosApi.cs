using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;

class GooglePhotosResponse {
	public MediaItem[] mediaItems;
}

class MediaItem {
	public string id;
	public string description;
	public string productUrl;
	public string baseUrl;
	public string filename;
}

public class GooglePhotosApi : MonoBehaviour {

	public void SendRequest() {
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
		Debug.Log(response.mediaItems[0].baseUrl);
	}
}
