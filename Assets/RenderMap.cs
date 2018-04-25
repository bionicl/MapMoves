using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderMap : MonoBehaviour {
	public static RenderMap instance;

	public GameObject LinePrefab;
	public GameObject PlacePrefab;

	void Awake() {
		instance = this;
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
						List<Vector3> positions = new List<Vector3>();
						foreach (var item3 in item2.trackPoints) {
							positions.Add(new Vector3(item3.lon * 10, item3.lat * 10));
						}
						Vector3[] positionsArray = positions.ToArray();
						lineTemp.positionCount = positionsArray.Length;
						lineTemp.SetPositions(positions.ToArray());
					}
				}
			}
		}
		Debug.Log("RenderDay End");
	}

	void Clear() {
		foreach (Transform child in gameObject.transform) {
			Destroy(child.gameObject);
		}
	}
}
