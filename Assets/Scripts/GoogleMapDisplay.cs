using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoogleMapDisplay : MonoBehaviour {
	public static GoogleMapDisplay instance;

	public GameObject tilePrefab;
	public float baseTileSize = 0.3826f;
	public Vector2[] zoomLevelTiers;
	public string style;

	int groupId;
	int zoomLevel;
	float tileSize;

	GameObject[] mapTilesGroups;
	private void Awake() {
		instance = this;

		mapTilesGroups = new GameObject[zoomLevelTiers.Length];
		GameObject mapTilesGroup = Instantiate(new GameObject(), new Vector3(0, 0, 0), transform.rotation);
		mapTilesGroup.name = "MapTiles";
		mapTilesGroup.transform.SetSiblingIndex(0);
		for (int i = 0; i < zoomLevelTiers.Length; i++) {
			GameObject tile = Instantiate(new GameObject(), new Vector3(0, 0, 0), transform.rotation);
			tile.name = zoomLevelTiers[i].y.ToString();
			tile.transform.SetParent(mapTilesGroup.transform);
			tile.transform.SetSiblingIndex(0);
			mapTilesGroups[i] = tile;
		}
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			RenderMap();
		}
	}

	// Disable smaller maps;
	public void ChangeMapZoom() {
		float userMapScale = Camera.main.orthographicSize;
		for (int i = 0; i < zoomLevelTiers.Length; i++) {
			//Debug.Log(zoomLevelTiers.Length - i - 1);
			mapTilesGroups[i].SetActive(userMapScale < zoomLevelTiers[i].x);
		}
	}

	// Create new tile
	void RenderMap() {
		CalculateMapZoom();
		StartCoroutine(DownloadMap(ReturnApiUrl(), CreateNewTile));
	}
	IEnumerator DownloadMap(string url, Action<Sprite> action) {
		// Start a download of the given URL
		using (WWW www = new WWW(url)) {
			// Wait for download to complete
			yield return www;

			// assign texture
			Sprite sprite = Sprite.Create(www.texture,
										  new Rect(0, 0, 600, 600),
										  new Vector2(0.5f, 0.5f));
			action.Invoke(sprite);
		}
	}
	void CreateNewTile(Sprite sprite) {
		Vector3 tempPos = Camera.main.transform.position;
		tempPos.z = zoomLevel * -1 + 40;
		GameObject tempGo = Instantiate(tilePrefab, tempPos, transform.rotation);
		tempGo.GetComponent<SpriteRenderer>().sprite = sprite;
		tempGo.transform.SetParent(mapTilesGroups[groupId].transform);
		tempGo.transform.localScale = new Vector3(tileSize, tileSize, 1);
	}
	void CalculateMapZoom() {
		float userMapScale = Camera.main.orthographicSize;
		for (int i = 0; i < zoomLevelTiers.Length; i++) {
			if (userMapScale <= zoomLevelTiers[i].x) {
				zoomLevel = (int)zoomLevelTiers[i].y;
				groupId = i;
				break;
			}
		}
		tileSize = baseTileSize * Mathf.Pow(2, 12 - zoomLevel);
	}
	string ReturnApiUrl() {
		Vector3 tempPos = Camera.main.transform.position;
		Vector2 metersPos = Conversion.MetersToLatLon(new Vector2(tempPos.x, tempPos.y));
		string output = string.Format("http://maps.googleapis.com/maps/api/staticmap?center={0},{1}&zoom={2}&size=600x600&key={3}{4}", metersPos.x, metersPos.y, zoomLevel, GoogleLocationApi.instance.apiKey, style);
		return output;
	}
}
