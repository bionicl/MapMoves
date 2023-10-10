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
	public float linesSizeMultiplyer = 0.006f;
	float timeSinceLastZoom;

	public bool[] filterState = new bool[10];

	List<GameObject>[] filterLines = new List<GameObject>[10];
	List<GameObject>[] filterLinesMoreDetailed = new List<GameObject>[10];
	Dictionary<DateTime, GameObject> filterDays = new Dictionary<DateTime, GameObject>();
	GameObject loactionsGO;
	List<GameObject> shortPaths = new List<GameObject>();
	bool cursorLocked = false;
	public Vector3 lastCursorPosition = new Vector2();

	public Vector3 targetPosition;

	void Awake() {
		instance = this;
		targetMapScale = GetComponent<Camera>().orthographicSize;
		for (int i = 0; i < filterLines.Length; i++) {
			filterLines[i] = new List<GameObject>();
			filterLinesMoreDetailed[i] = new List<GameObject>();
		}
		loactionsGO = Instantiate(new GameObject(), new Vector3(0, 0, 0), transform.rotation);
		loactionsGO.name = "Loactions";
		loactionsGO.transform.SetSiblingIndex(1);
		for (int i = 0; i < filterState.Length; i++) {
			filterState[i] = true;
		}
	}

	void Update()  {
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			EnableShortPaths();
		} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			DisableShortPaths();
		}

	//Debug.Log(Input.mousePosition.x / Screen.width - 0.5f);
		timeSinceLastZoom += Time.deltaTime;
		if (GlobalVariables.instance.mapControls && timeSinceLastZoom >= 0.03f) {
			if (Input.mouseScrollDelta.y > 0) {
				mapScale /= 1.5f;
				if (mapScale < 0.0625f)
					mapScale = 0.0625f;
				else {
					// Map moving
					Vector3 tempPostion = transform.position;
					if (targetPosition != transform.position && cursorLocked)
						tempPostion = targetPosition;
					tempPostion.x += (Input.mousePosition.x / Screen.width - 0.5f) * 3.555f /2 * mapScale;
					tempPostion.y += (Input.mousePosition.y / Screen.height - 0.5f) * 3.555f /2f / 1.8f * mapScale;
					targetPosition = tempPostion;
					cursorLocked = true;
					lastCursorPosition = Input.mousePosition;
					UpdateMapSize();
				}
			} else if (Input.mouseScrollDelta.y < 0) {
				mapScale *= 1.5f;
				if (mapScale > 64)
					mapScale = 127;
				else {
					// Map moving
					Vector3 tempPostion = transform.position;
					if (targetPosition != transform.position && cursorLocked)
						tempPostion = targetPosition;
					tempPostion.x -= (Input.mousePosition.x / Screen.width - 0.5f) * 3.555f / 4 * mapScale;
					tempPostion.y -= (Input.mousePosition.y / Screen.height - 0.5f) * 3.555f / 4f / 1.8f * mapScale;
					targetPosition = tempPostion;
					cursorLocked = true;
					lastCursorPosition = Input.mousePosition;
				}

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
		if (targetPosition != transform.position && cursorLocked == true) {
			Vector3 tempTransform = transform.position;
			if (targetPosition.x > transform.position.x) {
				tempTransform.x += (targetPosition.x - transform.position.x) / 3.5f;
			} else if (targetPosition.x < transform.position.x) {
				tempTransform.x -= (transform.position.x - targetPosition.x) / 3.5f;
			}
			
			if (targetPosition.y > transform.position.y) {
				tempTransform.y += (targetPosition.y - transform.position.y) / 3.5f;
			} else if (targetPosition.x < transform.position.y) {
				tempTransform.y -= (transform.position.y - targetPosition.y) / 3.5f;
			}
			transform.position = tempTransform;
		}
	}

	public void MapMoved() {
		if (cursorLocked) {
			transform.position = targetPosition;
			cursorLocked = false;
		}
	}

	public void UpdateMapSize(float? newMapScale = null) {
		timeSinceLastZoom = 0;
		if (newMapScale.HasValue) {
			mapScale = newMapScale.Value;
		}
		targetMapScale = mapScale;
		PlacesRanking.instance.ChangePlacesSize(mapScale);
		foreach (var item in renderedLines) {
			item.widthMultiplier = 0.006f + (mapScale - 1) * linesSizeMultiplyer;
		}
		GoogleMapDisplay.instance.ChangeMapZoom();

		// Show or hide more detailed lines
		ShowDetailedLines(mapScale <= 0.15f);
		

	}
	void ShowDetailedLines(bool show) {
		for (int i = 0; i < filterLinesMoreDetailed.Length; i++)
		{
			bool filterEnabled = filterState[i];
			foreach (var item in filterLinesMoreDetailed[i])
			{
				item.SetActive(filterEnabled && show);
			}
		}
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

						// 1. Set Line points 
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

						// 2. Check common path
						CommonPath commonPath = new CommonPath(positionsArray[0], positionsArray[positionsArray.Length - 1], item2.activity);
						bool canBeIgnored = CommonPathChecker.CheckIfPathCanBeIgnored(commonPath);
						
						// 3. Setup line renderer
						GameObject lineTempGo = Instantiate(LinePrefab, transform.position, transform.rotation);
						lineTempGo.transform.SetParent(dateGO.transform);
						lineTempGo.name = item2.activity.ToString();
						LineRenderer lineTemp = lineTempGo.GetComponent<LineRenderer>();

						// 4. Set lines color and positions
						lineTempGo.GetComponent<Renderer>().material = SetMaterial(item2.activity);
						lineTemp.positionCount = positionsArray.Length;
						lineTemp.SetPositions(positionsArray);

						// 5. Add line to filters
						AddToFilterList(item2.activity, lineTempGo, canBeIgnored);
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

	void AddToFilterList(ActivityType activity, GameObject line, bool canBeIgnored) {
		switch (activity) {
			case ActivityType.walking:
				if (canBeIgnored) {
					filterLines[0].Add(line);
				} else {
					filterLinesMoreDetailed[0].Add(line);
				}
				break;
			case ActivityType.cycling:
				if (canBeIgnored) {
					filterLines[1].Add(line);
				} else {
					filterLinesMoreDetailed[1].Add(line);
				}
				break;
			case ActivityType.running:
				if (canBeIgnored) {
					filterLines[2].Add(line);
				} else {
					filterLinesMoreDetailed[2].Add(line);
				}
				break;
			case ActivityType.car:
				if (canBeIgnored) {
					filterLines[5].Add(line);
				} else {
					filterLinesMoreDetailed[5].Add(line);
				}
				break;
			case ActivityType.bus:
				if (canBeIgnored) {
					filterLines[6].Add(line);
				} else {
					filterLinesMoreDetailed[6].Add(line);
				}
				break;
			case ActivityType.train:
				if (canBeIgnored) {
					filterLines[7].Add(line);
				} else {
					filterLinesMoreDetailed[7].Add(line);
				}
				break;
			case ActivityType.airplane:
				if (canBeIgnored) {
					filterLines[8].Add(line);
				} else {
					filterLinesMoreDetailed[8].Add(line);
				}
				break;
			default:
				if (canBeIgnored) {
					filterLines[9].Add(line);
				} else {
					filterLinesMoreDetailed[9].Add(line);
				}
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
