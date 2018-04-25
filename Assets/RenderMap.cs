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

	void Awake() {
		instance = this;
		for (int i = 0; i < filterLines.Length; i++) {
			filterLines[i] = new List<GameObject>();
		}
	}

	public void RenderDay(MovesJson day) {
		//Clear();
		List<long> alreadyRenderedPlaces = new List<long>();
		foreach (var item in day.segments) {
			if (item.place != null && !alreadyRenderedPlaces.Contains(item.place.id)) {
				Vector2 position = new Vector2(item.place.location.lon * 10, item.place.location.lat * 10);
				GameObject placeTemp = Instantiate(PlacePrefab, position, transform.rotation);
				placeTemp.transform.SetParent(gameObject.transform);
				placeTemp.name = item.place.name;
				alreadyRenderedPlaces.Add(item.place.id);
			} else if (item.place == null) {
				foreach (var item2 in item.activities) {
					if (item2.trackPoints.Length > 0) {
						GameObject lineTempGo = Instantiate(LinePrefab, transform.position, transform.rotation);
						lineTempGo.transform.SetParent(gameObject.transform);
						lineTempGo.name = item2.activity.ToString();
						LineRenderer lineTemp = lineTempGo.GetComponent<LineRenderer>();

						// Set Line points 
						List<Vector3> positions = new List<Vector3>();
						foreach (var item3 in item2.trackPoints)
							positions.Add(new Vector3(item3.lon * 10, item3.lat * 10));
						Vector3[] positionsArray = positions.ToArray();
						lineTemp.positionCount = positionsArray.Length;
						lineTemp.SetPositions(positions.ToArray());

						// Set lines color
						Material material = lineTempGo.GetComponent<Renderer>().material;
						Color color = ReadJson.colors[(int)item2.activity];
						color.a = 0.3f;
						material.SetColor("_TintColor", color);

						// Add line to filters
						AddToFilterList(item2.activity, lineTempGo);
					}
				}
			}
		}
		Debug.Log("RenderDay End");
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
