using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FilterTypes {
	walking,
	cycling,
	running,
	transport,
	place,
	car,
	bus,
	train,
	plane,
	otherTransport,
}

public class RenderMap : MonoBehaviour {
	public static RenderMap instance;

	public GameObject LinePrefab;
	public GameObject PlacePrefab;
	public Material[] materials;
	public float targetMapScale;

	public float mapScale = 1f;
	public int maxMetersForShortPath = 3000;
	public double simplifyMultiplayer = 1;
	public double simplifyMultiplayerDetailed;
	float timeSinceLastZoom;

	public bool[] filterState = new bool[10];

	List<GameObject>[] filterLines = new List<GameObject>[10];
	Dictionary<DateTime, GameObject> filterDays = new Dictionary<DateTime, GameObject>();
	GameObject loactionsGO;
	List<GameObject> shortPaths = new List<GameObject>();

	void Awake() {
		instance = this;
		targetMapScale = GetComponent<Camera>().orthographicSize;
		for (int i = 0; i < filterLines.Length; i++) {
			filterLines[i] = new List<GameObject>();
		}
		loactionsGO = Instantiate(new GameObject(), new Vector3(0, 0, 0), transform.rotation);
		loactionsGO.name = "Loactions";
		loactionsGO.transform.SetSiblingIndex(1);
		for (int i = 0; i < filterState.Length; i++) {
			filterState[i] = true;
		}
	}

	void Update()  {
		timeSinceLastZoom += Time.deltaTime;
		if (GlobalVariables.inst.mapControls && timeSinceLastZoom >= 0.03f) {
			if (Input.mouseScrollDelta.y > 0) {
				mapScale /= 1.5f;
				if (mapScale < 0.0625f)
					mapScale = 0.0625f;
				UpdateMapSize();
			} else if (Input.mouseScrollDelta.y < 0) {
				mapScale *= 1.5f;
				if (mapScale > 64)
					mapScale = 127;
				UpdateMapSize();
			}
		}
		if (targetMapScale != Camera.main.orthographicSize) {
			if (targetMapScale > Camera.main.orthographicSize) {
				Camera.main.orthographicSize += (targetMapScale - Camera.main.orthographicSize) / 3.5f;
			} else if (targetMapScale < Camera.main.orthographicSize) {
				Camera.main.orthographicSize -= (Camera.main.orthographicSize - targetMapScale) / 3.5f;
			}
		}
	}

	public void UpdateMapSize(float? newMapScale = null) {
		timeSinceLastZoom = 0;
		if (newMapScale.HasValue)
			mapScale = newMapScale.Value;
		targetMapScale = mapScale;
		PlacesRanking.instance.ChangePlacesSize(mapScale);
		foreach (var item in renderedLines) {
			item.widthMultiplier = 0.006f + (mapScale - 1) * 0.006f;
		}
		GoogleMapDisplay.instance.ChangeMapZoom();
	}

	List<long> alreadyRenderedPlaces = new List<long>();
	List<LineRenderer> renderedLines = new List<LineRenderer>();
	public int RenderDay(MovesJson day) {
		int renderedPoints = 0;
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
				if (item.place.name == null)
					placeTemp.name = "???";
				else
					placeTemp.name = item.place.name;
				placeTemp.GetComponent<Place>().SetupPlace(item.place);
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
						foreach (var item3 in item2.trackPoints) {
							positions.Add(Conversion.LatLonToMeters(item3.lat, item3.lon));
						}
						if (item2.activity == ActivityType.walking || item2.activity == ActivityType.cycling || item2.activity == ActivityType.running)
							positions = SimplifyPath.Simplify(positions, simplifyMultiplayerDetailed);
						else
							positions = SimplifyPath.Simplify(positions, simplifyMultiplayer);
						renderedPoints += positions.Count;
						Vector3[] positionsArray = positions.ToArray();
						lineTemp.positionCount = positionsArray.Length;
						lineTemp.SetPositions(positionsArray);

						// Set lines color
						lineTempGo.GetComponent<Renderer>().material = SetMaterial(item2.activity);

						// Add line to filters
						AddToFilterList(item2.activity, lineTempGo);
						renderedLines.Add(lineTemp);
						lineTempGo.SetActive(filterState[(int)TranslateActivityToFilter(item2.activity)]);
						if (item2.distance < maxMetersForShortPath)
							shortPaths.Add(lineTempGo);
					}
				}
			}
		}
		return renderedPoints;
	}

	Material SetMaterial(ActivityType activity) {
		switch (activity) {
			case ActivityType.walking:
				return materials[0];
			case ActivityType.cycling:
				return materials[1];
			case ActivityType.running:
				return materials[2];
			case ActivityType.car:
				return materials[3];
			case ActivityType.bus:
				return materials[4];
			case ActivityType.train:
				return materials[5];
			case ActivityType.airplane:
				return materials[6];
			default:
				return materials[7];
		}
	}

	Material SetMaterial(int filterLine) {
		switch (filterLine) {
			case 0:
				return materials[0];
			case 1:
				return materials[1];
			case 2:
				return materials[2];
			case 5:
				return materials[3];
			case 6:
				return materials[4];
			case 7:
				return materials[5];
			case 8:
				return materials[6];
			default:
				return materials[7];
		}
	}

	void AddToFilterList(ActivityType activity, GameObject line) {
		switch (activity) {
			case ActivityType.walking:
				filterLines[0].Add(line);
				break;
			case ActivityType.cycling:
				filterLines[1].Add(line);
				break;
			case ActivityType.running:
				filterLines[2].Add(line);
				break;
			case ActivityType.car:
				filterLines[5].Add(line);
				break;
			case ActivityType.bus:
				filterLines[6].Add(line);
				break;
			case ActivityType.train:
				filterLines[7].Add(line);
				break;
			case ActivityType.airplane:
				filterLines[8].Add(line);
				break;
			default:
				filterLines[9].Add(line);
				break;
		}
	}

	public void Clear() {
		foreach (Transform child in gameObject.transform) {
			Destroy(child.gameObject);
		}
		foreach (Transform child in loactionsGO.transform) {
			Destroy(child.gameObject);
		}
		foreach (var item in filterLines) {
			item.Clear();
		}
		filterDays.Clear();
		alreadyRenderedPlaces.Clear();
		renderedLines.Clear();
	}

	// Filters
	public void ChangeFilter(FilterTypes filterType, bool state) {
		if (filterType == FilterTypes.place) {
			loactionsGO.SetActive(state);
		} else {
			filterState[(int)filterType] = state;
			foreach (var item in filterLines[(int)filterType]) {
				item.SetActive(state);
			}
		}
	}
	public void ChangeFilterColor(bool isUnified) {
		if (isUnified) {
			for (int i = 5; i < 10; i++) {
				foreach (var item in filterLines[i]) {
					item.GetComponent<Renderer>().material = materials[3];
				}
			}
		} else {
			for (int i = 5; i < 10; i++) {
				foreach (var item in filterLines[i]) {
					item.GetComponent<Renderer>().material = SetMaterial(i);
				}
			}
			foreach (var item in filterLines) {
				
			}
		}
	}
	public void ChangeDaysRangeFilter(DateTime startDay, DateTime endDay) {
		foreach (var item in filterDays) {
			bool isNotActive = item.Key < startDay || item.Key > endDay;
			item.Value.SetActive(!isNotActive);
		}
	}

	public void EnableShortPaths() {
		foreach (var item in shortPaths) {
			item.SetActive(true);
		}
	}
	public void DisableShortPaths() {
		foreach (var item in shortPaths) {
			item.SetActive(false);
		}
	}

	public static FilterTypes TranslateActivityToFilter(ActivityType activity) {
		switch (activity) {
			case ActivityType.walking:
				return FilterTypes.walking;
			case ActivityType.cycling:
				return FilterTypes.cycling;
			case ActivityType.running:
				return FilterTypes.running;
			case ActivityType.car:
				return FilterTypes.car;
			case ActivityType.bus:
				return FilterTypes.bus;
			case ActivityType.train:
				return FilterTypes.train;
			case ActivityType.airplane:
				return FilterTypes.plane;
			default:
				return FilterTypes.otherTransport;
		}
	}
}

public static class Conversion {
	private const int TileSize = 256;
	/// <summary>according to https://wiki.openstreetmap.org/wiki/Zoom_levels</summary>
	private const int EarthRadius = 6378137; //no seams with globe example
	private const float InitialResolution = 2 * (float)Math.PI * EarthRadius / TileSize;
	private const float OriginShift = 2 * (float)Math.PI * EarthRadius / 2;
	private const float multiplayer = 10000;

	public static Vector2 LatLonToMeters(Vector2 latLon) {
		return LatLonToMeters(latLon.x, latLon.y);
	}

	public static Vector2 LatLonToMeters(float lat, float lon) {
		float posx = lon * OriginShift / 180;
		float posy = (float)Math.Log(Math.Tan((90 + lat) * (float)Math.PI / 360)) / ((float)Math.PI / 180);
		posy = posy * OriginShift / 180;
		return new Vector2(posx / multiplayer, posy / multiplayer);
	}

	public static Vector2 MetersToLatLon(Vector2 m) {
		var vx = (m.x * multiplayer / OriginShift) * 180;
		var vy = (m.y * multiplayer / OriginShift) * 180;
		vy = 180 / (float)Math.PI * (2 * (float)Math.Atan(Math.Exp(vy * (float)Math.PI / 180)) - (float)Math.PI / 2);
		return new Vector2(vy, vx);
	}
}
