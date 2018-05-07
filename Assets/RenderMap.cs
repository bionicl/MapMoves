using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FilterTypes {
	walking,
	cycling,
	running,
	transport
}

public class RenderMap : MonoBehaviour {
	public static RenderMap instance;

	public GameObject LinePrefab;
	public GameObject PlacePrefab;

	List<GameObject>[] filterLines = new List<GameObject>[4];
	Dictionary<DateTime, GameObject> filterDays = new Dictionary<DateTime, GameObject>();
	GameObject loactionsGO;

	void Awake() {
		instance = this;
		for (int i = 0; i < filterLines.Length; i++) {
			filterLines[i] = new List<GameObject>();
		}
		loactionsGO = Instantiate(new GameObject(), new Vector3(0, 0, 0), transform.rotation);
		//loactionsGO.transform.SetParent(gameObject.transform);
		loactionsGO.name = "Loactions";
		loactionsGO.transform.SetSiblingIndex(1);
	}

	List<long> alreadyRenderedPlaces = new List<long>();
	public void RenderDay(MovesJson day) {
		GameObject dateGO = Instantiate(new GameObject(), transform.position, transform.rotation);
		dateGO.transform.SetParent(gameObject.transform);
		dateGO.name = day.date;
		filterDays.Add(ReadJson.ReturnSimpleDate(day.date), dateGO);

		foreach (var item in day.segments) {
			if (item.place != null && !alreadyRenderedPlaces.Contains(item.place.id)) {
				Vector2 position = Conversion.LatLonToMeters(item.place.location.lat, item.place.location.lon);
				Vector3 finalPos = new Vector3(position.x, position.y, 0);
				GameObject placeTemp = Instantiate(PlacePrefab, finalPos, transform.rotation);
				placeTemp.transform.SetParent(loactionsGO.transform);
				finalPos = placeTemp.transform.position;
				finalPos.z = -3;
				placeTemp.transform.position = finalPos;
				placeTemp.name = item.place.name;
				alreadyRenderedPlaces.Add(item.place.id);
			} else if (item.place == null) {
				foreach (var item2 in item.activities) {
					if (item2.trackPoints.Length > 0) {
						GameObject lineTempGo = Instantiate(LinePrefab, transform.position, transform.rotation);
						lineTempGo.transform.SetParent(dateGO.transform);
						lineTempGo.name = item2.activity.ToString();
						LineRenderer lineTemp = lineTempGo.GetComponent<LineRenderer>();

						// Set Line points 
						List<Vector3> positions = new List<Vector3>();
						foreach (var item3 in item2.trackPoints)
							positions.Add(Conversion.LatLonToMeters(item3.lat, item3.lon));
						Vector3[] positionsArray = positions.ToArray();
						lineTemp.positionCount = positionsArray.Length;
						lineTemp.SetPositions(positions.ToArray());

						// Set lines color
						Material material = lineTempGo.GetComponent<Renderer>().material;
						Color color = ReadJson.colors[(int)item2.activity]; 
						Debug.Log(item2.activity == ActivityType.walking);
						color.a = 0.3f;
						material.SetColor("_Color", color);

						// Add line to filters
						AddToFilterList(item2.activity, lineTempGo);
					}
				}
			}
		}
	}

	void AddToFilterList(ActivityType activity, GameObject line) {
		if (activity == ActivityType.walking)
			filterLines[0].Add(line);
		else if (activity == ActivityType.cycling)
			filterLines[1].Add(line);
		else if (activity == ActivityType.running)
			filterLines[2].Add(line);
		else
			filterLines[3].Add(line);
	}

	void Clear() {
		foreach (Transform child in gameObject.transform) {
			Destroy(child.gameObject);
		}
	}

	public void ChangeFilter(FilterTypes filterType, bool state) {
		foreach (var item in filterLines[(int)filterType]) {
			item.SetActive(state);
		}
	}
}

public static class Conversion {
	private const int TileSize = 256;
	/// <summary>according to https://wiki.openstreetmap.org/wiki/Zoom_levels</summary>
	private const int EarthRadius = 6378137; //no seams with globe example
	private const float InitialResolution = 2 * (float)Math.PI * EarthRadius / TileSize;
	private const float OriginShift = 2 * (float)Math.PI * EarthRadius / 2;

	public static Vector2 LatLonToMeters(Vector2 latLon) {
		return LatLonToMeters(latLon.x, latLon.y);
	}

	public static Vector2 LatLonToMeters(float lat, float lon) {
		float posx = lon * OriginShift / 180;
		float posy = (float)Math.Log(Math.Tan((90 + lat) * (float)Math.PI / 360)) / ((float)Math.PI / 180);
		posy = posy * OriginShift / 180;
		return new Vector2(posx / 10000, posy / 10000);
	}
}
