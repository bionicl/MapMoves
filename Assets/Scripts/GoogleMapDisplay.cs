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
	class OneRequest : ICloneable {
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

		public object Clone() {
			return this.MemberwiseClone();
		}

		public void ChangePos(bool up, bool right, bool down, bool left) {
			float valueToChange = tileSize * 6.4f;
			if (up)
				position.y += valueToChange;
			if (right)
				position.x += valueToChange;
			if (down)
				position.y -= valueToChange;
			if (left)
				position.x -= valueToChange;
		}
	}

	GameObject[] mapTilesGroups;
	List<Vector2>[] donePositions = new List<Vector2>[17];

	public float timeMarginAfterMove = 0.2f;
	public bool useGoogleMaps = false;
	float timeSinceLastRefresh;
	bool checkForLastRefresh = true;

	private void Awake() {
		instance = this;
		for (int i = 0; i < donePositions.Length; i++) {
			donePositions[i] = new List<Vector2>();
		}

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

	void Update() {
		CheckIfRenderMap();
	}

	// Update map after mouse move
	void CheckIfRenderMap() {
		timeSinceLastRefresh += Time.deltaTime;
		if (timeSinceLastRefresh > timeMarginAfterMove && checkForLastRefresh) {
			RenderMapTiles();
			checkForLastRefresh = false;
		}
	}
	public void MapMoved() {
		timeSinceLastRefresh = 0;
		checkForLastRefresh = true;
	}
	void RenderMapTiles() {
		OneRequest request = CalculateMapZoom();
		CreateAdditionalRequests(request);
		if (ChangeToGridPosition(request)) {
			if (CheckUIScale.isRetina)
				StartCoroutine(DownloadMapRetina(CreateNewTile, request));
			else
				StartCoroutine(DownloadMap(CreateNewTile, request));
		}
	}
	void CreateAdditionalRequests(OneRequest request) {
		List<OneRequest> requests = new List<OneRequest>();
		for (int i = 0; i < 8; i++) {
			requests.Add((OneRequest)request.Clone());
		}
		requests[0].ChangePos(true, false, false, true);
		requests[1].ChangePos(true, false, false, false);
		requests[2].ChangePos(true, true, false, false);
		requests[3].ChangePos(false, false, false, true);
		requests[4].ChangePos(false, true, false, false);
		requests[5].ChangePos(false, false, true, true);
		requests[6].ChangePos(false, false, true, false);
		requests[7].ChangePos(false, true, true, false);

		foreach (var item in requests) {
			if (ChangeToGridPosition(item)) {
				if (CheckUIScale.isRetina)
					StartCoroutine(DownloadMapRetina(CreateNewTile, item));
				else
					StartCoroutine(DownloadMap(CreateNewTile, item));
			}
		}
	}

	// Grid system
	bool ChangeToGridPosition(OneRequest request) {
		float x = request.position.x;
		float y = request.position.y;

		int xGrid = (int)(x / request.tileSize / 6.4f) + 1;
		int yGrid = (int)(y / request.tileSize / 6.4f) + 1;
		Vector2 tempVector = new Vector2(xGrid, yGrid);
		if (donePositions[request.zoomLevel].Contains(tempVector)) {
			//Debug.Log("Already drawn, skipping");
			return false;
		} else {
			//Debug.Log(string.Format("x: {0}  y: {1}", xGrid, yGrid));
			request.position.x = (float)xGrid * request.tileSize * 6.4f;
			request.position.y = (float)yGrid * request.tileSize * 6.4f;
			donePositions[request.zoomLevel].Add(tempVector);
			return true;
		}
	}

	// Disable smaller maps
	public void ChangeMapZoom() {
		float userMapScale = RenderMap.instance.targetMapScale;
		for (int i = 0; i < zoomLevelTiers.Length; i++) {
			//Debug.Log(zoomLevelTiers.Length - i - 1);
			mapTilesGroups[i].SetActive(userMapScale < zoomLevelTiers[i].x);
		}
		MapMoved();
	}

	// Create new tile
	IEnumerator DownloadMap(Action<Sprite, OneRequest> action, OneRequest request) {
		string url = ReturnApiUrl(request);

		using (WWW www = new WWW(url)) {
			yield return www;
			Sprite sprite = Sprite.Create(www.texture,
										  new Rect(0, 0, 640, 640),
										  new Vector2(0.5f, 0.5f));

			action.Invoke(sprite, request);
		}
	}

	IEnumerator DownloadMapRetina(Action<Sprite, OneRequest> action, OneRequest request) {
		string url = ReturnApiUrl(request);

		using (WWW www = new WWW(url)) {
			yield return www;
			Sprite sprite = Sprite.Create(www.texture,
										  new Rect(0, 0, 1280, 1280),
										  new Vector2(0.5f, 0.5f), 200);

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

	// Helpers
	OneRequest CalculateMapZoom() {
		int groupId = 0;
		int zoomLevel = 0;

		float userMapScale = RenderMap.instance.targetMapScale;
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
		string output = "";
		string retinaMultiplayer = "";
		if (CheckUIScale.isRetina)
			retinaMultiplayer = "@2x";
		if (useGoogleMaps)
			output = string.Format("http://maps.googleapis.com/maps/api/staticmap?center={0},{1}&zoom={2}&size=640x640&key={3}{4}", metersPos.x, metersPos.y, request.zoomLevel, GoogleLocationApi.instance.apiKey, style);
		else
			output = string.Format("https://api.mapbox.com/styles/v1/bionicl/cjbfocd42b59q2rqasdw3ezwb/static/{1},{0},{2},0,0/640x640{3}?access_token={4}&logo=false&attribution=false", metersPos.x, metersPos.y, request.zoomLevel-1, retinaMultiplayer, GoogleLocationApi.instance.mapBoxApiKey);
		return output;
	}
}
