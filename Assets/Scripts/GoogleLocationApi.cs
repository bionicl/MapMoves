using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleLocationApiResponse {
	public class Results {
		public class AddressComponent {
			public string long_name;
			public string short_name;
			public string[] types;
		}
		public class Geometry {
			public class Location {
				public string lat;
				public string lng;
			}
			public class Viewport {
				public class Northeast {
					public string lat;
					public string lng;
				}
				public class Southwest {
					public string lat;
					public string lng;
				}

				public Northeast northeast;
				public Southwest southwest;
			}

			public Location location;
			public string location_type;
			public Viewport viewport;
		}

		public AddressComponent[] address_components;
		public string formatted_address;
		public Geometry geometry;
		public string place_id;
		public string[] types;
	}

	public Results[] results;
	public string status;
}

public class GoogleLocationApi : MonoBehaviour {
	public static GoogleLocationApi instance;

	void Awake() {
		instance = this;
	}

	public string apiKey;

	public void GetPlaceAddress(PlaceGroup place, Action<string> action) {
		StopAllCoroutines();
		StartCoroutine(GetAddress(place, action));
	}

	IEnumerator GetAddress(PlaceGroup place, Action<string> action) {
		int number = 0;
		string address = string.Format("https://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&location_type=ROOFTOP&result_type=street_address&key={2}",
		                               place.placeInfo.location.lat,
		                               place.placeInfo.location.lon,
		                               apiKey);
		using (UnityWebRequest www = UnityWebRequest.Get(address)) {
			yield return www.Send();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log(www.error);
			} else {
				GoogleLocationApiResponse m = JsonConvert.DeserializeObject<GoogleLocationApiResponse>(www.downloadHandler.text);
				if (m.results.Length > 0)
					action.Invoke(m.results[0].formatted_address);
			}
		}
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
