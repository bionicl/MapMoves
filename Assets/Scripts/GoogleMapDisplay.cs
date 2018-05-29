using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoogleMapDisplay : MonoBehaviour {
	public static GoogleMapDisplay instance;

	public GameObject tilePrefab;
	float baseTileSize = 0.3826f;
	public Vector2[] zoomLevelTiers;
	public string style;
	class OneRequest {
		public Vector3 position;
		public int groupId;
		public int zoomLevel;
		public float tileSize;

		public OneRequest(Vector3 position, int groupId, int zoomLevel, float tileSize) {
			this.position = position;
			this.groupId = groupId;
			this.zoomLevel = zoomLevel;
			this.tileSize = tileSize;
		}

	}

	GameObject[] mapTilesGroups;

	public float timeMarginAfterMove = 0.2f;
	float timeSinceLastRefresh;
	bool checkForLastRefresh = true;

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
		CheckIfRenderMap();
	}

	// Update map after mouse move
	void CheckIfRenderMap() {
		timeSinceLastRefresh += Time.deltaTime;
		if (timeSinceLastRefresh > timeMarginAfterMove && checkForLastRefresh) {
			RenderMap();
			checkForLastRefresh = false;
		}
	}
	public void MapMoved() {
		timeSinceLastRefresh = 0;
		checkForLastRefresh = true;
	}

	// Disable smaller maps
	public void ChangeMapZoom() {
		float userMapScale = Camera.main.orthographicSize;
		for (int i = 0; i < zoomLevelTiers.Length; i++) {
			//Debug.Log(zoomLevelTiers.Length - i - 1);
			mapTilesGroups[i].SetActive(userMapScale < zoomLevelTiers[i].x);
		}
		MapMoved();
	}

	// Create new tile
	void RenderMap() {
		OneRequest request = CalculateMapZoom();
		StartCoroutine(DownloadMap(CreateNewTile, request));
	}
	IEnumerator DownloadMap(Action<Sprite, OneRequest> action, OneRequest request) {
		string url = ReturnApiUrl(request);

		using (WWW www = new WWW(url)) {
			yield return www;

			Sprite sprite = Sprite.Create(www.texture,
										  new Rect(0, 0, 600, 600),
										  new Vector2(0.5f, 0.5f));
			action.Invoke(sprite, request);
		}
	}
	void CreateNewTile(Sprite sprite, OneRequest request) {
		Vector3 tempPos = request.position;
		tempPos.z = request.zoomLevel * -1 + 40;
		GameObject tempGo = Instantiate(tilePrefab, tempPos, transform.rotation);
		tempGo.GetComponent<SpriteRenderer>().sprite = sprite;
		tempGo.transform.SetParent(mapTilesGroups[request.groupId].transform);
		tempGo.transform.SetAsLastSibling();
		tempGo.transform.localScale = new Vector3(request.tileSize, request.tileSize, 1);
	}
	OneRequest CalculateMapZoom() {
		int groupId = 0;
		int zoomLevel = 0;

		float userMapScale = Camera.main.orthographicSize;
		for (int i = 0; i < zoomLevelTiers.Length; i++) {
			if (userMapScale <= zoomLevelTiers[i].x) {
				zoomLevel = (int)zoomLevelTiers[i].y;
				groupId = i;
				break;
			}
		}
		return new OneRequest(Camera.main.transform.position,
		                      groupId,
							  zoomLevel,
							  baseTileSize * Mathf.Pow(2, 12 - zoomLevel));
	}
	string ReturnApiUrl(OneRequest request) {
		Vector3 tempPos = request.position;
		Vector2 metersPos = Conversion.MetersToLatLon(new Vector2(tempPos.x, tempPos.y));
		string output = string.Format("http://maps.googleapis.com/maps/api/staticmap?center={0},{1}&zoom={2}&size=600x600&key={3}{4}", metersPos.x, metersPos.y, request.zoomLevel, GoogleLocationApi.instance.apiKey, style);
		Debug.Log(output);
		return output;
	}
}
