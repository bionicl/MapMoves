using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FacebookApiResponse {
	public class CategoryList {
		public string id;
		public string name;
	}
	public CategoryList[] category_list;
	public string id;
}

[Serializable]
public class SpecialIcons {
	public string name;
	public Sprite icon;
}

public class FacebookPlaces : MonoBehaviour {
	public static FacebookPlaces instance;

	public SpecialIcons[] icons;
	Dictionary<string, int> placesMemory = new Dictionary<string, int>();

	// In future versions Facebook API login dialog will be implemented
	public string fbAccessToken;

	void Awake() {
		instance = this;
	}

	public void GetPlaceCategory(string placeId, Image image) {
		StartCoroutine(GetText(placeId, image));
	}

	IEnumerator GetText(string placeId, Image image) {
		int number = 0;
		if (placesMemory.TryGetValue(placeId, out number)) {
			image.sprite = icons[number].icon;
		} else {
			string address = "https://graph.facebook.com/v2.12/" + placeId + "?fields=category_list&access_token=" + fbAccessToken;
			using (UnityWebRequest www = UnityWebRequest.Get(address)) {
				yield return www.Send();

				if (www.isNetworkError || www.isHttpError) {
					Debug.Log(www.error);
				} else {
					FacebookApiResponse m = JsonConvert.DeserializeObject<FacebookApiResponse>(www.downloadHandler.text);
					if (!FindIcon(placeId, m, image))
						Debug.Log(m.category_list[0].name);
				}
			}
		}
	}

	bool FindIcon(string placeId, FacebookApiResponse m, Image image) {
		for (int i = 0; i < icons.Length; i++) {
			if (icons[i].name == m.category_list[0].name) {
				if (image != null) {
					image.sprite = icons[i].icon;
				}
				placesMemory.Add(placeId, i);
				return true;
			}
		}
		return false;
	}
}
